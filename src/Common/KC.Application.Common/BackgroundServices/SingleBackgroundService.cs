using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using KC.Application.Common.Cacheing;
using KC.Application.Common.Settings;
using KC.Utils.Common;

namespace KC.Application.Common.BackgroundServices
{
    [ExcludeFromCodeCoverage(Justification = "Abstract background service does not need unit tests.")]
    [SuppressMessage("Maintainability", "S4055", Justification = "Abstract background service class has some false positives that do not apply.")]
    [SuppressMessage("Maintainability", "S2221", Justification = "Abstract background service class has some false positives that do not apply.")]
    public abstract class SingleBackgroundService : BackgroundService
    {
        protected ILogger Logger { get; init; }
        protected IServiceProvider? ServiceProvider { get; init; }
        protected string WorkerName { get; init; }

        private readonly ICache _cache;
        private string _leaseCacheKey = "";
        private bool _isLeaseActive;
        private bool _isOneTimeStartupCompleted;
        private readonly BackgroundServiceConfig _serviceConfig;
        private bool CanServiceStart => !_isLeaseActive && !_isOneTimeStartupCompleted;

        private readonly string _hostName;

        protected SingleBackgroundService(IServiceProvider serviceProvider, IHostApplicationLifetime lifetime, ILogger logger,
            BackgroundServiceConfig config)
        {
            Logger = logger;
            ServiceProvider = serviceProvider;
            using var scope = serviceProvider.CreateScope();
            _cache = scope.ServiceProvider.GetRequiredService<ICache>();
            WorkerName = config.WorkerName ?? GetType().FullName ?? GetType().Name;
            _hostName = OSUtil.GetHostName();
            _serviceConfig = config;
            lifetime.ApplicationStopped.Register(async () => await StopAsync(CancellationToken.None));
        }

        /// <summary>
        /// Exceptions thrown here are turned into alerts.
        /// </summary>
        public abstract Task DoWorkAsync();

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Awaiting Task.Yield() transitions to asyncronous operation immediately.
            // This allows startup to continue without waiting.
            await Task.Yield();

            Logger.LogInformation("{WorkerName} has {StartDelay} start delay configured.", WorkerName, _serviceConfig.StartDelay);
            Logger.LogInformation("{WorkerName} has {IntervalPeriod} interval period configured.", WorkerName, _serviceConfig.IntervalPeriod);

            try
            {
                await Task.Delay(_serviceConfig.StartDelay, stoppingToken).ConfigureAwait(false);
                await StartServiceAsync();
                while (_serviceConfig.IsEnabled && CanServiceStart && !stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        await DoWorkAsync().ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(
                            ex,
                            "Unhandled exception occurred in the {worker}. Sending an alert. Worker will retry after the normal interveral.",
                            WorkerName);
                    }

                    if (_serviceConfig.IntervalPeriod.HasValue)
                    {
                        await Task.Delay(_serviceConfig.IntervalPeriod.Value, stoppingToken).ConfigureAwait(false);
                    }
                    else
                    {
                        _isOneTimeStartupCompleted = true;
                    }
                }

                Logger.LogInformation(
                    "{WorkerName} execution ended on {hostName}. Cancellation token = {IsCancellationRequested}",
                        WorkerName, _hostName, stoppingToken.IsCancellationRequested);
            }
            catch (Exception ex) when (stoppingToken.IsCancellationRequested)
            {
                Logger.LogWarning(ex, "{WorkerName} execution cancelled on {hostName}.", WorkerName, _hostName);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Unhandled exception. {WorkerName} execution stopping on {hostName}.", WorkerName, _hostName);
            }
        }

        protected async Task StartServiceAsync()
        {
            Logger.LogInformation("{WorkerName} is starting on {HostName}.", WorkerName, _hostName);
            // check lease in redis
            _leaseCacheKey = $"{WorkerName}.Lease";
            var lease = await _cache.GetAsync<string>(_leaseCacheKey);
            Logger.LogInformation("Retrieved {lease} for {key} from cache.", lease, _leaseCacheKey);
            if (!string.IsNullOrEmpty(lease) && lease != _hostName)
            {
                Logger.LogInformation("{WorkerName} already has an active lease on {lease}.", WorkerName, lease);
                _isLeaseActive = true;
            }
            else
            {
                await _cache.SetAsync(_leaseCacheKey, _hostName);
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await StopServiceAsync();
            await base.StopAsync(cancellationToken: cancellationToken);
        }

        protected async Task StopServiceAsync()
        {
            Logger.LogInformation("{WorkerName} is stopping on {HostName}.", WorkerName, _hostName);
            if (!string.IsNullOrEmpty(_leaseCacheKey) && !_isLeaseActive)
            {
                await _cache.RemoveAsync(_leaseCacheKey);
                Logger.LogInformation("Removed lease: {Key} from cache.", _leaseCacheKey);
            }
        }

        public override void Dispose()
        {
            _cache.Dispose();
            base.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
