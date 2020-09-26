using System;
using System.Globalization;

namespace Utilities
{
    public static class DateUtils
    {
        public const string YearShort = "yy";
        public const string YearLong = "yyyy";

        public const string MonthShort = "MM";
        public const string MonthLong = "MMM";

        public const string Day = "dd";

        public const string Hour = "HH";
        public const string Minute = "mm";
        public const string Seconds = "ss";
        public const string Milliseconds = "ffff";


        public static string GetDate(string separator, params string[] format)
        {
            if (format is null || format.Length == 0)
            {
                return DateTime.Now.ToString(CultureInfo.CurrentCulture);
            }
            string formatString = string.IsNullOrWhiteSpace(separator) 
                ? string.Concat(format) 
                : string.Join(separator, format);
            return DateTime.Now.ToString(formatString, CultureInfo.CurrentCulture);
        }
    }
}
