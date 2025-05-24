using KC.Domain.Common.Files;

namespace KC.Application.Common.Files
{
    public interface IZipProvider : IDisposable
    {
        Task<byte[]> ZipAsync(IList<KC.Domain.Common.Files.File> files, bool preserveDirectoryStructure, CancellationToken cancellationToken = default);
    }
}
