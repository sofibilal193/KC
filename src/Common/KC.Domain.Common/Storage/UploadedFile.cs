using System.Collections.Generic;

namespace KC.Domain.Common.Storage
{
    public class UploadedFile
    {
        public string Path { get; set; } = "";

        public string? Name { get; set; }

        public byte[]? Data { get; set; }

        public int? TtlDays { get; set; }

        public IDictionary<string, string>? MetaData { get; set; }
    }
}
