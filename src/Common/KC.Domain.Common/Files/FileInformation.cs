namespace KC.Domain.Common.Files
{
    public static class FileInformation
    {
        public static Tuple<string, string> GetFileInfoFromBase64(string fileBase64Data)
        {
            var data = fileBase64Data[..5].ToUpper();
            string type = "", contentType = "";

            if (data == "IVBOR")
            {
                type = "png"; contentType = "image/png";
            }
            else if (data == "/9J/4")
            {
                type = "jpeg"; contentType = "image/jpeg";
            }
            else if (data == "R0LGO")
            {
                type = "gif"; contentType = "image/gif";
            }
            else if (data == "JVBER")
            {
                type = "pdf"; contentType = "application/pdf";
            }
            else if (data == "PD94B" || data == "PHN2Z")
            {
                type = "svg"; contentType = "image/svg+xml";
            }
            return Tuple.Create(type, contentType);
        }

        public static string? GetFileExtension(string contentType)
        {
            return contentType switch
            {
                "image/png" => ".png",
                "image/jpeg" => ".jpg",
                "image/gif" => ".gif",
                "image/svg+xml" => ".svg",
                "application/pdf" => ".pdf",
                _ => null
            };
        }
    }
}
