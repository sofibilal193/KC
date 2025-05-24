using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using KC.Domain.Common.Files;
using File = KC.Domain.Common.Files.File;

namespace KC.Infrastructure.Common.Files
{
    public interface IPdfProvider : IDisposable
    {
        Task<byte[]> MergeAsync(IList<File> files, CancellationToken cancellationToken = default);

        byte[] SetFields(byte[] pdfContent, Dictionary<string, object?> fields);
    }
}
