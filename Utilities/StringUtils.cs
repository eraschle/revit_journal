using System;

namespace Utilities
{
    public static class StringUtils
    {
        public static bool Equals(string value, string other)
        {
            if (value is null && other is null) { return true; }

            if (value is null || other is null) { return false; }

            return value.Equals(other, StringComparison.CurrentCulture);
        }

        public static bool Starts(string value, string starts)
        {
            return value.StartsWith(starts, StringComparison.CurrentCulture);
        }

        public static int Compare(string value, string other)
        {
            return string.Compare(value, other, StringComparison.CurrentCulture);
        }
    }
}
