namespace KC.Domain.Common.Config
{
    public static class OrgConfigExtensions
    {
        public static T? GetValue<T>(this List<OrgConfigDto>? configs, string name)
            where T : struct
        {
            return configs?.Find(c => c.Name == name)?.GetValue<T>();
        }

        public static T? GetValueOrDefault<T>(this List<OrgConfigDto>? configs, string name)
            where T : class
        {
            return configs?.Find(c => c.Name == name)?.GetValue<T>();
        }
    }
}
