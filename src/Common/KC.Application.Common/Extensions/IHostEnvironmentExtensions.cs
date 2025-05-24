using Microsoft.Extensions.Hosting;

namespace KC.Application.Common.Extensions
{
    public static class IHostEnvironmentExtensions
    {
        public static bool IsTest(this IHostEnvironment env)
        {
            return env.IsEnvironment("Test");
        }
    }
}
