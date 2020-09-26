using System;

namespace Utilities.System
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
            if (string.IsNullOrWhiteSpace(value)
                || string.IsNullOrWhiteSpace(starts))
            {
                return false;
            }
            return value.StartsWith(starts, StringComparison.CurrentCulture);
        }

        public static bool Ends(string value, string end)
        {
            if (string.IsNullOrWhiteSpace(value)
                 || string.IsNullOrWhiteSpace(end))
            {
                return false;
            }
            return value.EndsWith(end, StringComparison.CurrentCulture);
        }

        public static int Compare(string value, string other)
        {
            return string.Compare(value, other, StringComparison.CurrentCulture);
        }
    }
}
