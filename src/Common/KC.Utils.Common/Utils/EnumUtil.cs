using System;

namespace KC.Utils.Common
{
    public static class EnumUtil
    {
        public static string GetName(this Enum e)
        {
            return e.GetType().Name;
        }

        public static string GetValue(this Enum e)
        {
            return e.ToString();
        }
    }
}
