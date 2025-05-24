using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KC.Application.Common.Files;

namespace KC.Infrastructure.Common.Files
{
    /// <summary>
    /// Zip provider class that implements IZipProvider
    /// </summary>
    public class ZipProvider : IZipProvider
    {
        #region Private Properties

        #endregion

        #region Constructors

        public ZipProvider()
        {
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

        #region IZipProvider Methods

        public async Task<byte[]> ZipAsync(IList<Domain.Common.Files.File> files, bool preserveDirectoryStructure, CancellationToken cancellationToken = default)
        {
            using var outputStream = new MemoryStream();
            using (ZipArchive archive = new ZipArchive(outputStream, ZipArchiveMode.Create))
            {
                var entryNames = new List<string>();
                foreach (var file in files)
                {
                    if (file.Content?.Length > 0)
                    {
                        var entryName = preserveDirectoryStructure ? file.Name : Path.GetFileName(file.Name);
                        var count = entryNames.Count(name => name == entryName);
                        entryNames.Add(entryName);
                        if (count > 0)
                        {
                            var directory = preserveDirectoryStructure ? Path.GetDirectoryName(entryName) : null;
                            entryName = $"{Path.GetFileNameWithoutExtension(entryName)} ({count}){Path.GetExtension(entryName)}";
                            if (directory is not null)
                            {
                                entryName = Path.Combine(directory, entryName);
                            }
                        }
                        ZipArchiveEntry entry = archive.CreateEntry(entryName);
                        using var zipStream = entry.Open();
                        await zipStream.WriteAsync(file.Content.AsMemory(0, file.Content.Length), cancellationToken);
                    }
                }
            }

            return outputStream.ToArray();
        }

        #endregion

        #region Private Methods

        #endregion

    }
}
