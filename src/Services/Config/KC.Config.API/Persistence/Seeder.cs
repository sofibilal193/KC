using System.Text.Json;
using KC.Config.API.Entities;
using KC.Config.API.Repositories;
using KC.Domain.Common.Config;
using KC.Domain.Common.Extensions;
using KC.Domain.Common.Identity;
using KC.Domain.Common.ValueObjects;
using KC.Domain.Common.ValueObjects.Addresses;
using KC.Utils.Common;
using KC.Utils.Common.Crypto;

namespace KC.Config.API.Persistence
{
    public class Seeder : ISeeder
    {
        private readonly ILogger<Seeder> _logger;
        private readonly IConfigUnitOfWork _data;
        private readonly ICryptoProvider _cryptoProvider;

        public Seeder(ILogger<Seeder> logger, IConfigUnitOfWork data,
            ICryptoProvider cryptoProvider)
        {
            _logger = logger;
            _data = data;
            _cryptoProvider = cryptoProvider;
        }

        public async Task SeedItemsAsync(CancellationToken cancellationToken = default)
        {
            await SeedConfigItemsAsync(cancellationToken);
            await SeedOrgConfigAsync(cancellationToken);
            await SeedUserConfigAsync(cancellationToken);
        }

        #region ConfigItems

        private async Task SeedConfigItemsAsync(CancellationToken cancellationToken = default)
        {
            string path = Path.GetFullPath("Seed/Config");
            if (Directory.Exists(path))
            {
                _logger.LogInformation("Found Config Seed Data: {directory}", path);
                foreach (string filePath in Directory.GetFiles(path))
                {
                    var content = await File.ReadAllTextAsync(filePath, cancellationToken);
                    if (!string.IsNullOrEmpty(content))
                    {
                        _logger.LogInformation("Found content in: {filePath}", filePath);
                        var items = JsonSerializer.Deserialize<List<ConfigItem>>(content);
                        if (items != null)
                        {
                            var currentItems = await _data.Configs.GetAllAsync(cancellationToken);
                            var missingRecords = items.Where(i => !currentItems.Exists(ci => ci.Type.ToLower() == i.Type.ToLower()
                                && ci.Name.ToLower() == i.Name.ToLower())).ToList();
                            if (missingRecords?.Count > 0)
                            {
                                _logger.LogInformation("Found {i1} missing records in {i2} total records in file: {i3}.",
                                    missingRecords.Count, items.Count, filePath);
                                missingRecords.ToList().ForEach(i =>
                                {
                                    if (i.IsEncrypted && !string.IsNullOrEmpty(i.Value))
                                        i.SetValue(_cryptoProvider?.EncryptString(i.Value));
                                });
                                _data.Configs.AddRange(missingRecords);
                                await _data.SaveEntitiesAsync(cancellationToken);
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region OrgConfig

        private async Task SeedOrgConfigAsync(CancellationToken cancellationToken = default)
        {
            string path = Path.GetFullPath("Seed/OrgConfig");
            if (Directory.Exists(path))
            {
                _logger.LogInformation("Found OrgConfig Seed Data: {directory}", path);
                foreach (string filePath in Directory.GetFiles(path))
                {
                    var list = new List<OrgConfigField>();
                    var orgElements = JsonUtil.ParseJsonFile<JsonElement>(filePath);
                    foreach (var orgElement in orgElements.EnumerateArray())
                    {
                        var category = orgElement.GetPropertyValue<string>("Category");
                        var fieldType = orgElement.GetPropertyValue<FieldType>("FieldType");
                        var name = orgElement.GetPropertyValue<string>("Name");
                        var description = orgElement.GetPropertyValue<string>("Description");
                        var displayOrder = orgElement.GetPropertyValue<short>("DisplayOrder");
                        var defaultValue = orgElement.GetPropertyValue<string>("DefaultValue");
                        var isOrgVisible = orgElement.GetPropertyValue<bool>("IsOrgVisible");
                        var fieldValues = orgElement.GetPropertyValue<IList<FieldValue>>("FieldValues");
                        var regexValidator = orgElement.GetPropertyValue<string>("RegexValidator");
                        var minValue = orgElement.GetPropertyValue<decimal>("MinValue");
                        var maxValue = orgElement.GetPropertyValue<decimal>("MaxValue");
                        var orgType = orgElement.GetPropertyValue<OrgType?>("OrgType");

                        if (fieldType == FieldType.Json
                            && defaultValue?.StartsWith('[') == false
                            && defaultValue?.StartsWith('{') == false)
                        {
                            var jsonFilePath = Path.GetFullPath(defaultValue);
                            if (File.Exists(jsonFilePath))
                            {
                                defaultValue = await File.ReadAllTextAsync(jsonFilePath, cancellationToken);
                            }
                        }

                        list.Add(new OrgConfigField(orgType, category ?? "", fieldType, name ?? "", description, displayOrder,
                            defaultValue, minValue, maxValue, regexValidator, isOrgVisible,
                            fieldValues?.ToList() ?? new List<FieldValue>()));
                    }

                    if (list.Count > 0)
                    {
                        var currentItems = await _data.OrgConfigs.GetAllAsync(cancellationToken);
                        foreach (var item in list)
                        {
                            var currentItem = currentItems.Find(i =>
                                i.OrgType == item.OrgType &&
                                i.Category.ToLower() == item.Category.ToLower() &&
                                i.Name.ToLower() == item.Name.ToLower());
                            if (currentItem is null)
                            {
                                _data.OrgConfigs.Add(item);
                            }
                            else
                            {
                                currentItem.Update(item);
                            }
                        }
                        await _data.SaveEntitiesAsync(cancellationToken);
                    }
                }
            }
        }

        #endregion

        #region Providers

        #endregion

        #region UserConfig

        private async Task SeedUserConfigAsync(CancellationToken cancellationToken = default)
        {
            string path = Path.GetFullPath("Seed/UserConfig");
            if (Directory.Exists(path))
            {
                _logger.LogInformation("Found UserConfig Seed Data: {directory}", path);
                foreach (string filePath in Directory.GetFiles(path))
                {
                    var list = new List<UserConfigField>();
                    var userElements = JsonUtil.ParseJsonFile<JsonElement>(filePath);
                    foreach (var userElement in userElements.EnumerateArray())
                    {
                        var category = userElement.GetPropertyValue<string>("Category");
                        var fieldType = userElement.GetPropertyValue<FieldType>("FieldType");
                        var name = userElement.GetPropertyValue<string>("Name");
                        var description = userElement.GetPropertyValue<string>("Description");
                        var displayOrder = userElement.GetPropertyValue<short>("DisplayOrder");
                        var defaultValue = userElement.GetPropertyValue<string>("DefaultValue");
                        var isUserVisible = userElement.GetPropertyValue<bool>("IsUserVisible");
                        var fieldValues = userElement.GetPropertyValue<IList<FieldValue>>("FieldValues");
                        var regexValidator = userElement.GetPropertyValue<string>("RegexValidator");
                        var minValue = userElement.GetPropertyValue<decimal>("MinValue");
                        var maxValue = userElement.GetPropertyValue<decimal>("MaxValue");

                        if (fieldType == FieldType.Json
                            && defaultValue?.StartsWith('[') == false
                            && defaultValue?.StartsWith('{') == false)
                        {
                            var jsonFilePath = Path.GetFullPath(defaultValue);
                            if (File.Exists(jsonFilePath))
                            {
                                defaultValue = await File.ReadAllTextAsync(jsonFilePath, cancellationToken);
                            }
                        }

                        list.Add(new UserConfigField(category ?? "", fieldType, name ?? "", description, displayOrder,
                            defaultValue, minValue, maxValue, regexValidator, isUserVisible,
                            fieldValues?.ToList() ?? new List<FieldValue>()));
                    }

                    if (list.Count > 0)
                    {
                        var currentItems = await _data.UserConfigs.GetAllAsync(cancellationToken);
                        foreach (var item in list)
                        {
                            var currentItem = currentItems.Find(i =>
                                i.Category.ToLower() == item.Category.ToLower() &&
                                i.Name.ToLower() == item.Name.ToLower());
                            if (currentItem is null)
                            {
                                _data.UserConfigs.Add(item);
                            }
                            else
                            {
                                currentItem.Update(item);
                            }
                        }
                        await _data.SaveEntitiesAsync(cancellationToken);
                    }
                }
            }
        }

        #endregion

    }
}
