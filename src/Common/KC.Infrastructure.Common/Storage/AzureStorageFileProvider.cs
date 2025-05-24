using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using KC.Application.Common.Extensions;
using KC.Application.Common.Storage;
using KC.Domain.Common.Files;
using KC.Domain.Common.Storage;

namespace KC.Infrastructure.Common.Storage
{
    /// <summary>
    /// Azure Storage File provider class that implements IFileProvider
    /// </summary>
    public class AzureStorageFileProvider : IFileProvider
    {
        #region Private Properties

        private readonly AzureStorageFileOptions _options;

        private readonly string _invalidFileNameChars = @"[~#%&\*\{\}\(\)\\/:;<>\?\|""'\^]";
        private readonly string _invalidAsciiChars = @"[^\u0000-\u007F]+";
        private readonly IHostEnvironment _env;

        private readonly List<BlobContainerClient> _containers = new();

        #endregion

        #region Constructors

        public AzureStorageFileProvider(AzureStorageFileOptions options, IHostEnvironment env)
        {
            _options = options;
            _env = env;
        }

        #endregion

        #region IDisposable Methods

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // dispose resources
        }

        #endregion

        #region IFileProvider Methods

        public async Task<string?> UploadAsync(string containerName, string prefix,
            string contentType, byte[] data, CancellationToken cancellationToken = default)
        {
            string? filePath = null;

            List<UploadedFile> files = new()
            {
                new UploadedFile()
                {
                    Name = Guid.NewGuid().ToString() + FileInformation.GetFileExtension(contentType),
                    Data = data,
                    // MetaData = new Dictionary<string, string>() {
                    //     { "contenttype", Uri.EscapeDataString(contentType) },
                    //     { "uploaddateutc", Uri.EscapeDataString(DateTime.UtcNow.ToString()) },
                    // }
                }
            };

            if (await UploadAsync(containerName, prefix, files, cancellationToken))
            {
                filePath = files.FirstOrDefault()?.Path;
            }

            return filePath;
        }

        public async Task<bool> UploadAsync(string containerName, List<UploadedFile> files,
            CancellationToken cancellationToken = default)
        {
            return await UploadAsync(containerName, null, files, cancellationToken);
        }

        public async Task<bool> UploadAsync(string containerName, string? prefix,
            List<UploadedFile> files, CancellationToken cancellationToken = default)
        {
            // Get a reference to the container
            var container = await GetBlobContainerAsync(containerName, cancellationToken);

            foreach (var file in files)
            {
                // Retrieve reference to a blob
                string fileName = string.IsNullOrEmpty(file.Name) ? Guid.NewGuid().ToString() : CleanFileName(file.Name);
                string blobName = string.IsNullOrEmpty(prefix) ? fileName : prefix.ToLower() + "/" + fileName;
                BlobClient blob = container.GetBlobClient(blobName);
                if (blob != null)
                {
                    if (file.Data is not null)
                    {
                        using var ms = new MemoryStream(file.Data, false);
                        await blob.UploadAsync(ms, overwrite: true, cancellationToken);
                    }
                    file.Path = blob.Uri.ToString();

                    // set blob metadata
                    if (file.MetaData != null)
                    {
                        await blob.SetMetadataAsync(file.MetaData,
                            cancellationToken: cancellationToken);
                    }

                    if (file.TtlDays.HasValue)
                    {
                        await blob.SetTagsAsync(new Dictionary<string, string> { { "TTL", file.TtlDays.Value.ToString() } }, cancellationToken: cancellationToken);
                    }
                }
            }

            return true;
        }

        public async Task<UploadedFile?> GetAsync(string path,
            CancellationToken cancellationToken = default)
        {
            var files = await GetFilesAsync(new List<string>() { path }, cancellationToken);
            return files?.FirstOrDefault();
        }

        public async Task<List<UploadedFile>> GetFilesAsync(List<string> paths,
            CancellationToken cancellationToken = default)
        {
            List<UploadedFile> list = new();

            foreach (var path in paths)
            {
                var (containerName, blobName) = GetContainerNameFromUri(path);
                if (!string.IsNullOrEmpty(containerName) && !string.IsNullOrEmpty(blobName))
                {
                    // retrieve reference to container
                    var container = await GetBlobContainerAsync(containerName, cancellationToken);
                    var file = await GetFileFromBlobAsync(container, blobName, cancellationToken);
                    if (file is not null)
                        list.Add(file);
                }
            }

            return list;
        }

        public async Task<List<UploadedFile>> GetFilesAsync(string containerName,
            CancellationToken cancellationToken = default)
        {
            return await GetFilesAsync(containerName, null, cancellationToken);
        }

        public async Task<List<UploadedFile>> GetFilesAsync(string containerName, string? prefix,
            CancellationToken cancellationToken = default)
        {
            List<UploadedFile> list = new();

            // Retrieve a reference to a container.
            var container = await GetBlobContainerAsync(containerName, cancellationToken);

            // Call the listing operation and return pages of the specified size.
            var resultSegment = container.GetBlobsAsync(prefix: prefix,
                traits: BlobTraits.None,
                cancellationToken: cancellationToken)
                .AsPages(default);

            // Enumerate the blobs returned for each page.
            await foreach (Azure.Page<BlobItem> blobPage in resultSegment)
            {
                foreach (BlobItem blobItem in blobPage.Values)
                {
                    var file = await GetFileFromBlobAsync(container, blobItem.Name, cancellationToken);
                    if (file is not null)
                        list.Add(file);
                }
            }
            return list;
        }

        public async Task<List<UploadedFile>> GetListAsync(string containerName,
            CancellationToken cancellationToken = default)
        {
            return await GetListAsync(containerName, null, cancellationToken);
        }

        public async Task<List<UploadedFile>> GetListAsync(string containerName,
            string? prefix, CancellationToken cancellationToken = default)
        {
            List<UploadedFile> list = new();

            // Retrieve a reference to a container.
            var container = await GetBlobContainerAsync(containerName, cancellationToken);
            var containerUri = container.Uri.AbsoluteUri;

            // Call the listing operation and return pages of the specified size.
            var resultSegment = container.GetBlobsAsync(prefix: prefix,
                traits: BlobTraits.All,
                cancellationToken: cancellationToken)
                .AsPages(default);

            // Enumerate the blobs returned for each page.
            await foreach (Azure.Page<BlobItem> blobPage in resultSegment)
            {
                foreach (BlobItem blobItem in blobPage.Values)
                {
                    UploadedFile file = new()
                    {
                        Name = blobItem.Name,
                        Path = Flurl.Url.Combine(containerUri,
                            blobItem.Name),
                        MetaData = blobItem.Metadata
                    };
                    list.Add(file);
                }
            }
            return list;
        }

        public async Task<bool> SetMetaData(string path,
            IDictionary<string, string> metaData, CancellationToken cancellationToken = default)
        {
            // Retrieve a reference to a container.
            var (containerName, blobName) = GetContainerNameFromUri(path);
            if (!string.IsNullOrEmpty(containerName) && !string.IsNullOrEmpty(blobName))
            {
                var container = await GetBlobContainerAsync(containerName, cancellationToken);

                // Retrieve reference to a blob
                var blockBlob = container.GetBlobClient(blobName);
                if (blockBlob != null)
                {
                    await blockBlob.SetMetadataAsync(metaData, cancellationToken: cancellationToken);
                }
            }
            return true;
        }

        public async Task<bool> DeleteAsync(string fileName,
            CancellationToken cancellationToken = default)
        {
            // Get a reference to the container
            var (containerName, blobName) = GetContainerNameFromUri(fileName);
            var container = await GetBlobContainerAsync(containerName ?? "docs", cancellationToken);
            BlobClient blob = container.GetBlobClient(blobName);
            await blob.DeleteAsync(DeleteSnapshotsOption.None, null, cancellationToken);
            return true;
        }

        #endregion

        #region Private Methods

        private static async Task<UploadedFile?> GetFileFromBlobAsync(BlobContainerClient container, string blobName,
            CancellationToken cancellationToken = default)
        {
            UploadedFile? file = default;
            // Retrieve reference to a blob
            var blockBlob = container.GetBlobClient(blobName);
            if (await blockBlob.ExistsAsync(cancellationToken))
            {
                var containerUri = container.Uri.AbsoluteUri;
                var path = Flurl.Url.Combine(containerUri, blobName);

                file = new UploadedFile
                {
                    Path = path,
                    Name = blobName
                };
                using (var memoryStream = new MemoryStream())
                {
                    await blockBlob.DownloadToAsync(memoryStream, cancellationToken);
                    file.Data = memoryStream.ToArray();
                }

                // retrieve metadata
                var props = await blockBlob.GetPropertiesAsync(cancellationToken: cancellationToken);
                file.MetaData = props?.Value?.Metadata;
            }
            return file;
        }

        //[ExcludeFromCodeCoverage]
        private string CleanFileName(string fileName)
        {
            string name = Path.GetFileNameWithoutExtension(fileName);
            string ext = Path.GetExtension(fileName);
            // Replace invalid characters with empty strings.
            string returnstr = Regex.Replace(Regex.Replace(name, _invalidFileNameChars, "", RegexOptions.NonBacktracking),
                _invalidAsciiChars, "", RegexOptions.NonBacktracking);
            returnstr += ext;
            return returnstr;
        }

        // [ExcludeFromCodeCoverage]
        private BlobClientOptions? GetBlobClientOptions()
        {
            if (_options.RetryOptions != null)
            {
                return new BlobClientOptions
                {
                    Retry = {
                        Delay = _options.RetryOptions.Delay,
                        MaxDelay = _options.RetryOptions.MaxDelay,
                        MaxRetries = _options.RetryOptions.MaxRetries,
                        Mode = _options.RetryOptions.Mode,
                        NetworkTimeout = _options.RetryOptions.NetworkTimeout
                    },
                    GeoRedundantSecondaryUri = !string.IsNullOrEmpty(_options.GeoRedundantSecondaryUri) ?
                        new Uri(_options.GeoRedundantSecondaryUri) : null
                };
            }
            else
            {
                return null;
            }
        }

        // [ExcludeFromCodeCoverage]
        private BlobServiceClient GetBlobServiceClient()
        {
            // Create a BlobServiceClient that will authenticate through Active Directory
            if (_env.IsDevelopment() || _env.IsTest())
            {
                return new(_options.PrimaryUri, GetBlobClientOptions());
            }
            else
            {
                Uri accountUri = new(_options.PrimaryUri);
                return new(accountUri, new DefaultAzureCredential(), GetBlobClientOptions());
            }
        }

        // [ExcludeFromCodeCoverage]
        private static (string? containerName, string? blobName) GetContainerNameFromUri(string uri)
        {
            var blobClient = new BlobClient(new Uri(uri));
            return (blobClient.BlobContainerName, blobClient.Name);
        }

        // [ExcludeFromCodeCoverage]
        private async Task<BlobContainerClient> GetBlobContainerAsync(string containerName, CancellationToken cancellationToken)
        {
            var container = _containers.Find(c => c.Name == containerName);
            if (container is null)
            {
                var client = GetBlobServiceClient();
                container = client.GetBlobContainerClient(containerName);
                await container.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
                _containers.Add(container);
            }
            return container;
        }

        #endregion

    }
}
