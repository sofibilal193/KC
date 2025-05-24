using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using FluentValidation;
using HealthChecks.UI.Client;
using Hellang.Middleware.ProblemDetails;
using MediatR;
using NSwag;
using NSwag.Generation.Processors.Security;
using KC.Application.Common.Auth;
using KC.Application.Common.Behaviors;
using KC.Application.Common.Extensions;
using KC.Application.Common.Filters;
using KC.Application.Common.Identity;
using KC.Application.Common.IntegrationEvents;
using KC.Application.Common.Rules;
using KC.Application.Common.Validations;
using KC.Domain.Common.Constants;
using KC.Domain.Common.Converters;
using KC.Domain.Common.Identity;
using KC.Application.Common.Events;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace KC.Application.Common
{
    [ExcludeFromCodeCoverage]
    public static class DependencyInjection
    {
        private static readonly AppSettings _settings = new();

        public static IServiceCollection AddApplication(this IServiceCollection services,
            IConfiguration configuration, IHostEnvironment env, ILogger logger, bool useIntegrationEvents = true)
        {
            services.AddSingleton(configuration);
            services.Configure<AppSettings>(configuration.GetSection("AppSettings"));
            configuration.GetSection("AppSettings").Bind(_settings);

            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
            AppDomain.CurrentDomain.SetData("REGEX_DEFAULT_MATCH_TIMEOUT", TimeSpan.FromSeconds(_settings.RegexTimeoutSeconds));

            services.AddHttpContextAccessor();

            if (_settings.MaxRequestFileSizeInBytes.HasValue)
            {
                services.Configure<KestrelServerOptions>(options =>
                    {
                        options.Limits.MaxRequestBodySize = _settings.MaxRequestFileSizeInBytes;
                    });
            }

            services
                .AddControllers(options => options.Filters.Add(typeof(HttpGlobalExceptionFilter)))
                .ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true)
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                    options.JsonSerializerOptions.Converters.Add(new TrimStringJsonConverter());
                });

            services.AddScoped<ICurrentUser, HttpCurrentUser>();

            // https://andrewlock.net/handling-web-api-exceptions-with-problemdetails-middleware/
            services.AddProblemDetails(opts =>
            {
                // Control when an exception is included
                opts.IncludeExceptionDetails = (_, _) => env.IsDevelopment() || env.IsTest();
            });
            ValidatorOptions.Global.LanguageManager = new ValidationLanguageManager();

            // add cors policies
            services.AddCorsPolicies(_settings, logger);

            // add auth
            if (_settings.AuthSettings.Type != ApiAuthType.JWT && !string.IsNullOrEmpty(_settings.AuthSettings.ClientId))
            {
                services.AddAuthentication(options =>
                {
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                }).AddApiAuth(configuration);
            }
            else
            {
                services.AddAuthentication(options =>
                {
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                }).AddJwtAuth(logger);
            }

            // add basic auth
            services.AddAuthentication(AuthSchemes.Basic)
                .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>(AuthSchemes.Basic, null)
                .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>(AuthSchemes.ApiKey, null);

            services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<JwtBearerOptions>, ConfigureJwtBearerOptions>());

            services.AddAuthorization(options =>
            {
                options.AddPolicy(AuthPolicies.SysAdmin, policy =>
                {
                    policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole("sys.admin");
                });
                options.AddPolicy(AuthPolicies.Basic, policy =>
                {
                    policy.AuthenticationSchemes.Add(AuthSchemes.Basic);
                    policy.RequireAuthenticatedUser();
                });
                options.AddPolicy(AuthPolicies.OrgUser, policy =>
                {
                    policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
                    policy.RequireAuthenticatedUser();
                    policy.Requirements.Add(new OrgUserRequirement());
                });
                options.AddPolicy(AuthPolicies.ApiKey, policy =>
                {
                    policy.AddAuthenticationSchemes(AuthSchemes.ApiKey);
                    policy.RequireAuthenticatedUser();
                });
                options.AddPolicy(AuthPolicies.TokenOrApiKey, policy =>
                {
                    policy.AddAuthenticationSchemes(AuthSchemes.ApiKey, JwtBearerDefaults.AuthenticationScheme);
                    policy.RequireAuthenticatedUser();
                });
            });

            // add authorization handler to make sure user can only access orgs they belong too
            services.AddScoped<IAuthorizationHandler, OrgUserAuthorizationHandler>();

            // add Mediatr
            services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(LoggingBehavior<,>).Assembly));

            // add rules evaluator
            services.AddSingleton<IRulesEvaluator, RulesEvaluator>();

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CachePipelineBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));

            // configure cookie options
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = _ => true;
                options.MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.None;
            });

            // add integration service
            if (useIntegrationEvents)
            {
                services.AddScoped<IIntegrationEventService, IntegrationEventService>();
                services.AddHostedService<IntegrationEventBackgroundService>();
            }

            // add event log sink service
            services.AddHostedService<EventLogSinkBackgroundService>();

            // add HTTP logging
            services.AddHttpLogging(options =>
            {
                options.LoggingFields = HttpLoggingFields.All;
                // log header values needed to debug reverse proxy issues
                options.RequestHeaders.Add("X-Forwarded-Host");
                options.RequestHeaders.Add("X-ORIGINAL-HOST");
                options.RequestHeaders.Add("X-Forwarded-PathBase");
                options.RequestHeaders.Add("X-Original-URL");
                options.RequestHeaders.Add("X-Azure-ClientIP");
                options.RequestHeaders.Add("X-Azure-SocketIP");
                options.RequestHeaders.Add("X-Forwarded-For");
                options.RequestHeaders.Add("X-Original-For");
                options.RequestHeaders.Add("X-Client-IP");
                options.RequestHeaders.Add("Client-IP");
            });

            // api versioning - https://code-maze.com/aspnetcore-api-versioning/
            services.AddApiVersioning(o =>
            {
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(_settings.ApiVersionSettings.DefaultVersion.MajorVersion,
                    _settings.ApiVersionSettings.DefaultVersion.MinorVersion);
                o.ReportApiVersions = true;
                o.ApiVersionReader = ApiVersionReader.Combine(
                    new QueryStringApiVersionReader("api-version"),
                    new HeaderApiVersionReader("x-api-version"),
                    new MediaTypeApiVersionReader("ver"),
                    new UrlSegmentApiVersionReader());
            });
            services.AddVersionedApiExplorer(
                options =>
                {
                    options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(_settings.ApiVersionSettings.DefaultVersion.MajorVersion,
                    _settings.ApiVersionSettings.DefaultVersion.MinorVersion);
                    options.GroupNameFormat = "'v'VVV";
                    options.SubstituteApiVersionInUrl = true;
                });

            // add NSwag Open Api specification - https://blog.rsuter.com/versioned-aspnetcore-apis-with-openapi-generation-and-azure-api-management/
            if (_settings.ApiVersionSettings.Versions is not null)
            {
                foreach (var version in _settings.ApiVersionSettings.Versions)
                {
                    services.AddOpenApiDocument(document =>
                    {
                        document.Title = _settings.OpenApiSettings.Title;
                        document.Description = _settings.OpenApiSettings.Description ?? "";

                        document.DocumentName = "v" + version.MajorVersion;
                        document.ApiGroupNames = new string[] { "v" + version.MajorVersion };
                        document.Version = version.MajorVersion + "." + version.MinorVersion;
                        document.SchemaSettings.SchemaNameGenerator = new ApiSchemaNameGenerator();

                        document.AddSecurity("JWT", new OpenApiSecurityScheme
                        {
                            Type = OpenApiSecuritySchemeType.ApiKey,
                            Name = "Authorization",
                            In = OpenApiSecurityApiKeyLocation.Header,
                            Description = "Type into the textbox: Bearer {your JWT token}."
                        });
                        document.AddSecurity("ApiKey", new OpenApiSecurityScheme
                        {
                            Type = OpenApiSecuritySchemeType.ApiKey,
                            Name = AuthConstants.ApiKeyHeaderName,
                            In = OpenApiSecurityApiKeyLocation.Header,
                            Description = "Type into the textbox: {your API key}."
                        });

                        document.OperationProcessors.Add(
                            new AspNetCoreOperationSecurityScopeProcessor("JWT"));
                        document.OperationProcessors.Add(
                            new AspNetCoreOperationSecurityScopeProcessor("ApiKey"));

                        if (_settings.OpenApiSettings.ShowSessionIdHeader)
                        {
                            document.OperationProcessors.Add(new AddSessionIdHeaderProcessor());
                        }
                    });
                }
            }

            return services;
        }

        public static IServiceCollection AddAppHealthChecks(this IServiceCollection services)
        {
            var hcBuilder = services.AddHealthChecks();
            hcBuilder.AddCheck("self", () => HealthCheckResult.Healthy());

            var sb = services.BuildServiceProvider();

            var hcs = from t in Assembly.GetEntryAssembly()?.GetTypes()
                      where t.GetInterfaces().Contains(typeof(IHealthCheck))
                      select ActivatorUtilities.CreateInstance(sb, t) as IHealthCheck;
            if (hcs?.Count() > 0)
            {
                var tags = new[] { "custom" };
                foreach (var hc in hcs)
                {
                    hcBuilder.AddCheck(hc.GetType().Name.ToLowerInvariant().Replace("healthcheck", ""),
                        hc, HealthStatus.Degraded, tags);
                }
            }
            return services;
        }

        public static IServiceCollection AddIntegrationEventHandlers(this IServiceCollection services, params Assembly[] assemblies)
        {
            if (assemblies?.Length > 0)
            {
                foreach (var assembly in assemblies)
                {
                    var types = assembly.GetTypes().Where(t => t.Name.EndsWith("IntegrationEventHandler"));
                    if (types?.Count() > 0)
                    {
                        foreach (var type in types)
                        {
                            services.AddTransient(type);
                        }
                    }
                }
            }
            return services;
        }

        public static IApplicationBuilder RegisterIntegrationEventHandlers(this IApplicationBuilder app, params Assembly[] assemblies)
        {
            if (assemblies?.Length > 0)
            {
                var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
                foreach (var assembly in assemblies)
                {
                    var types = assembly.GetTypes().Where(t => t.Name.EndsWith("IntegrationEventHandler"));
                    if (types?.Count() > 0)
                    {
                        foreach (var type in types)
                        {
                            eventBus.SubscribeDynamicAsync(type.Name.Replace("Handler", ""), type).GetAwaiter().GetResult();
                        }
                    }
                }
            }
            return app;
        }

        public static IApplicationBuilder UseApplication(this IApplicationBuilder app, IHostEnvironment env, IServiceCollection services, bool enableHttpLogging)
        {
            if (env.IsDevelopment())
                IdentityModelEventSource.ShowPII = true;

            app.UseProblemDetails();

            if (!env.IsDevelopment() && !env.IsEnvironment("Test"))
            {
                // when in azure, ensure headers are forwarded to prevent too many multiple redirects issue
                // https://docs.microsoft.com/en-us/aspnet/core/security/enforcing-ssl?view=aspnetcore-2.2&tabs=visual-studio
                app.UseXForwardedHeaders();
            }

            if (enableHttpLogging)
            {
                app.UseHttpLogging();
            }

            if (!env.IsProduction())
            {
                app.UseDeveloperExceptionPage();
                RegisteredServicesPage(app, services);
            }

            // use NSwag OpenApi
            app.UseSwaggerWithReverseProxySupport();

            app.UseRouting();

            // UseCors with default policy.
            app.UseCors();

            // authentication & authorization
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapControllers();
                endpoints.MapAppHealthChecks();
            });

            return app;
        }

        private static IEndpointRouteBuilder MapAppHealthChecks(this IEndpointRouteBuilder app)
        {
            app.MapHealthChecks("/hc", new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
            app.MapHealthChecks("/liveness", new HealthCheckOptions
            {
                Predicate = r => r.Name.Contains("self")
            });
            app.MapHealthChecks("/hc-custom", new HealthCheckOptions
            {
                Predicate = r => r.Tags.Contains("custom"),
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            return app;
        }

        private static IApplicationBuilder UseXForwardedHeaders(this IApplicationBuilder app)
        {
            var options = new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.All
            };

            // Only loopback proxies are allowed by default.
            // Clear that restriction because forwarders are being enabled by explicit configuration.
            // https://stackoverflow.com/a/56469499/5358985
            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();

            app.UseForwardedHeaders(options);

            app.Use((context, next) =>
            {
                if (context.Request.Headers.TryGetValue("X-Forwarded-PathBase", out var pathBases))
                {
                    context.Request.PathBase = pathBases[0];
                }
                return next();
            });

            return app;
        }

        private static IApplicationBuilder UseSwaggerWithReverseProxySupport(this IApplicationBuilder app)
        {
            var logger = app.ApplicationServices.GetService<ILogger>();

            app.UseOpenApi(config =>
            {
                // Without this the document will be cached with wrong URLs
                // and not work if later accessed from another host/path/scheme
                config.CreateDocumentCacheKey = request =>
                    string.Concat(
                    !string.IsNullOrEmpty(request.Headers["X-Azure-FDID"].FirstOrDefault())
                        ? request.Headers["X-Azure-FDID"].FirstOrDefault()
                        : request.Headers["X-Forwarded-Host"].FirstOrDefault() ?? request.Headers["X-ORIGINAL-HOST"].FirstOrDefault()
                    , request.Headers["X-Forwarded-PathBase"].FirstOrDefault()
                    , request.IsHttps);

                // Change document host and base path from headers (if set)
                config.PostProcess = (document, request) =>
                {
                    bool isAzureFD = !string.IsNullOrEmpty(request.Headers["X-Azure-FDID"].FirstOrDefault());
                    var originalHost = isAzureFD ? string.Empty : request.Headers["X-Forwarded-Host"].FirstOrDefault() ?? request.Headers["X-ORIGINAL-HOST"].FirstOrDefault();
                    var pathBase = request.Headers["X-Forwarded-PathBase"].FirstOrDefault();

                    logger?.LogInformation("OriginalHost: {host}. PathBase: {path}", originalHost, pathBase);

                    if (!string.IsNullOrEmpty(pathBase))
                        document.BasePath = pathBase;

                    if (!string.IsNullOrEmpty(originalHost))
                        document.Host = originalHost;
                };
            });

            app.UseSwaggerUi(settings =>
            {
                settings.TransformToExternalPath = (route, request) =>
                {
                    var pathBase = request.Headers["X-Forwarded-PathBase"].FirstOrDefault();
                    return !string.IsNullOrEmpty(pathBase)
                        ? $"{pathBase}{route}"
                        : route;
                };
            });

            return app;
        }

        private static void RegisteredServicesPage(IApplicationBuilder app, IServiceCollection services)
        {
            app.Map("/services", builder => builder.Run(async context =>
            {
                var sb = new StringBuilder();
                sb.Append("<h1>Registered Services</h1>");
                sb.Append("<table><thead>");
                sb.Append("<tr><th>Type</th><th>Lifetime</th><th>Instance</th></tr>");
                sb.Append("</thead><tbody>");
                foreach (var svc in services)
                {
                    sb.Append("<tr>");
                    sb.Append("<td>").Append(svc.ServiceType.FullName).Append("</td>");
                    sb.Append("<td>").Append(svc.Lifetime).Append("</td>");
                    sb.Append("<td>").Append(svc.ImplementationType?.FullName).Append("</td>");
                    sb.Append("</tr>");
                }
                sb.Append("</tbody></table>");
                await context.Response.WriteAsync(sb.ToString());
            }));
        }
    }
}