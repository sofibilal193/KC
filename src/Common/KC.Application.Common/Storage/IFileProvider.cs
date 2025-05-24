using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using KC.Domain.Common.Storage;

namespace KC.Application.Common.Storage
{
    public interface IFileProvider : IDisposable
    {
        Task<string?> UploadAsync(string containerName, string prefix,
            string contentType, byte[] data, CancellationToken cancellationToken = default);

        Task<bool> UploadAsync(string containerName, List<UploadedFile> files,
            CancellationToken cancellationToken = default);

        Task<bool> UploadAsync(string containerName, string prefix,
            List<UploadedFile> files, CancellationToken cancellationToken = default);

        Task<UploadedFile?> GetAsync(string path, CancellationToken cancellationToken = default);

        Task<List<UploadedFile>> GetFilesAsync(List<string> paths, CancellationToken cancellationToken = default);

        Task<List<UploadedFile>> GetFilesAsync(string containerName, CancellationToken cancellationToken = default);

        Task<List<UploadedFile>> GetFilesAsync(string containerName, string prefix,
            CancellationToken cancellationToken = default);

        Task<List<UploadedFile>> GetListAsync(string containerName, CancellationToken cancellationToken = default);

        Task<List<UploadedFile>> GetListAsync(string containerName, string prefix,
            CancellationToken cancellationToken = default);

        Task<bool> SetMetaData(string path, IDictionary<string, string> metaData,
            CancellationToken cancellationToken = default);

        Task<bool> DeleteAsync(string fileName,
            CancellationToken cancellationToken = default);
    }
}
