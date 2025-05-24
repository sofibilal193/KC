using System;
using System.Runtime.InteropServices;

namespace KC.Utils.Common
{
    /// <summary>
    /// A helper class to determine the operating system environment
    /// </summary>
    public static class OSUtil
    {
        public static bool IsWindows() =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        public static bool IsMacOS() =>
            RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

        public static bool IsLinux() =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

        public static bool IsAzure() =>
            Environment.GetEnvironmentVariable("WEBSITE_HOSTNAME")?.Contains("azurewebsites.net") == true;

        public static string GetHostName() => System.Net.Dns.GetHostName().ToLowerInvariant();
    }
}
