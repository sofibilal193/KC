using System.Reflection;

namespace KC.Utils.Common
{
    public static class ReflectionUtil
    {
        public static string? GetAppName()
        {
            return Assembly.GetEntryAssembly()?.FullName;
        }
    }
}
