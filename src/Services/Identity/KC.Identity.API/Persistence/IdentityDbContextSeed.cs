using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using KC.Domain.Common.Extensions;
using KC.Domain.Common.Identity;
using KC.Domain.Common.ValueObjects.Addresses;
using KC.Identity.API.Entities;
using KC.Identity.API.Repositories;
using KC.Utils.Common;
using Polly;
using Polly.Retry;

namespace KC.Identity.API.Persistence
{
    [ExcludeFromCodeCoverage]
    public class IdentityDbContextSeed
    {
        private List<Permission> _permissions = new();

        public async Task SeedAsync(IIdentityUnitOfWork data, ILogger<IdentityDbContextSeed> logger, string? rootSeedDataPath = null)
        {
            var policy = CreatePolicy(logger, nameof(IdentityDbContextSeed));

            await policy.ExecuteAsync(async () =>
            {
                await SeedOrgsAsync(data, rootSeedDataPath);
                await SeedPermissionsAsync(data, rootSeedDataPath);
                await SeedRolesAsync(data, rootSeedDataPath);
            });
        }

        private static AsyncRetryPolicy CreatePolicy(ILogger<IdentityDbContextSeed> logger, string prefix, int retries = 3)
        {
            return Policy.Handle<SqlException>().
                WaitAndRetryAsync(
                    retryCount: retries,
                    sleepDurationProvider: _ => TimeSpan.FromSeconds(5),
                    onRetry: (exception, _, retry, _) => logger.LogWarning(exception, "[{prefix}] Exception {ExceptionType} with message {Message} detected on attempt {retry} of {retries}", prefix, exception.GetType().Name, exception.Message, retry, retries)
                );
        }

        private static async Task SeedOrgsAsync(IIdentityUnitOfWork data, string? rootSeedDataPath = null)
        {
            bool saveChanges = false;
            var path = string.IsNullOrEmpty(rootSeedDataPath) ? Path.GetFullPath("Seed/orgs.json") : Path.Combine(rootSeedDataPath, "locations.json");
            var orgs = ParseOrgsFromJson(path);
            if (orgs != null)
            {
                foreach (var org in orgs)
                {
                    var exists = await data.Orgs.AnyAsync(o => o.Code == org.Code);
                    if (!exists)
                    {
                        saveChanges = true;
                        data.Orgs.Add(org);
                    }
                }
            }
            if (saveChanges)
                await data.SaveEntitiesAsync();
        }

        private async Task SeedPermissionsAsync(IIdentityUnitOfWork data, string? rootSeedDataPath = null)
        {
            bool saveChanges = false;
            var path = string.IsNullOrEmpty(rootSeedDataPath) ? Path.GetFullPath("Seed/permissions.json") : Path.Combine(rootSeedDataPath, "permissions.json");
            _permissions = await data.Permissions.GetAllAsync();
            var permissions = ParsePermissionsFromJson(path);
            if (permissions != null)
            {
                foreach (var permission in permissions)
                {
                    if (!_permissions.Exists(p => p.Name == permission.Name))
                    {
                        saveChanges = true;
                        _permissions.Add(permission);
                        data.Permissions.Add(permission);
                    }
                }
            }
            if (saveChanges)
                await data.SaveEntitiesAsync();
        }

        private async Task SeedRolesAsync(IIdentityUnitOfWork data, string? rootSeedDataPath = null)
        {
            bool saveChanges = false;
            var path = string.IsNullOrEmpty(rootSeedDataPath) ? Path.GetFullPath("Seed/roles.json") : Path.Combine(rootSeedDataPath, "roles.json");
            var roles = ParseRolesFromJson(path);
            var dbRoles = await data.Roles.Include(r => r.Permissions).GetAllAsync();
            _permissions = await data.Permissions.GetAllAsync();
            if (roles != null && dbRoles != null)
            {
                foreach (var role in roles)
                {
                    var dbRole = dbRoles.Find(r => r.Name == role.Name);
                    if (dbRole is null)
                    {
                        saveChanges = true;
                        data.Roles.Add(role);
                    }
                    else
                    {
                        saveChanges |= dbRole.AddPermissions(role.Permissions);
                    }
                }
            }
            if (saveChanges)
                await data.SaveEntitiesAsync();
        }

        private static List<Org> ParseOrgsFromJson(string path)
        {
            var orgs = new List<Org>();
            var orgElements = JsonUtil.ParseJsonFile<JsonElement>(path);
            foreach (var orgElement in orgElements.EnumerateArray())
            {
                var orgType = orgElement.GetPropertyValue<OrgType>("Type");
                var code = orgElement.GetPropertyValue<string>("Code");
                var name = orgElement.GetPropertyValue<string>("Name");
                var legalName = orgElement.GetPropertyValue<string>("LegalName");
                var phone = orgElement.GetPropertyValue<string>("Phone");
                var fax = orgElement.GetPropertyValue<string>("Fax");
                var website = orgElement.GetPropertyValue<string>("Website");
                var addressElement = orgElement.GetProperty("Address");
                var addressType = addressElement.GetPropertyValue<string>("Type");
                var address1 = addressElement.GetPropertyValue<string>("Address1");
                var address2 = addressElement.GetPropertyValue<string>("Address2");
                var city = addressElement.GetPropertyValue<string>("City");
                var state = addressElement.GetPropertyValue<string>("State");
                var zipCode = addressElement.GetPropertyValue<string>("ZipCode");
                var address = new Address(addressType!, address1!, address2, city!, state!, zipCode!);
                orgs.Add(new Org(orgType, code, name!, legalName, address, phone, fax, website, null, null, null, null));
            }
            return orgs;
        }

        private static List<Permission> ParsePermissionsFromJson(string path)
        {
            var permissions = new List<Permission>();
            var elements = JsonUtil.ParseJsonFile<JsonElement>(path);
            foreach (var element in elements.EnumerateArray())
            {
                var name = element.GetPropertyValue<string>("Name");
                var category = element.GetPropertyValue<string>("Category");
                var description = element.GetPropertyValue<string>("Description");
                permissions.Add(new Permission(category!, name!, description));
            }
            return permissions;
        }

        private List<Role> ParseRolesFromJson(string path)
        {
            var roles = new List<Role>();
            var roleElements = JsonUtil.ParseJsonFile<JsonElement>(path);

            foreach (var roleElement in roleElements.EnumerateArray())
            {
                var roleType = roleElement.GetPropertyValue<RoleType>("Type");
                var orgType = roleElement.GetPropertyValue<OrgType>("OrgType");
                var name = roleElement.GetPropertyValue<string>("Name");
                var description = roleElement.GetPropertyValue<string>("Description");

                if (name is not null)
                {
                    var rolePermissions = new List<Permission>();
                    var permissionsExist = roleElement.TryGetProperty("PermissionNames", out JsonElement permissionNames);

                    if (permissionsExist)
                    {
                        foreach (var permissionName in permissionNames.EnumerateArray())
                        {
                            var permission = _permissions.Find(p => p.Name == permissionName.ToString());
                            if (permission is not null)
                            {
                                rolePermissions.Add(permission);
                            }
                        }
                    }
                    var role = new Role(roleType, orgType, name, description, null, rolePermissions);
                    roles.Add(role);
                }
            }
            return roles;
        }
    }
}
