using System;
using System.Collections.Generic;
using System.Globalization;

namespace Utilities.System
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

        public static string GetPathDate(string separator = "", params string[] format)
        {
            if (string.IsNullOrWhiteSpace(separator))
            {
                separator = Constant.Minus;
            }
            if (format is null || format.Length == 0)
            {
                format = new string[] { YearLong, MonthShort, Day };
            }
            return AsString(separator, format);
        }

        public static string AsString(string separator = "", params string[] format)
        {
            return AsString(DateTime.Now, separator, format);
        }

        public static string AsString(DateTime dateTime, string separator = "", params string[] format)
        {
            if (format is null || format.Length == 0)
            {
                return dateTime.ToString(CultureInfo.CurrentCulture);
            }
            string formatString = GetFormatString(separator, format);
            return dateTime.ToString(formatString, CultureInfo.CurrentCulture);
        }

        private static string GetFormatString(string separator = "", params string[] format)
        {
            return string.IsNullOrWhiteSpace(separator)
                ? string.Concat(format)
                : string.Join(separator, format);
        }


        public static DateTime AsDate(string datetime)
        {
            return DateTime.TryParse(datetime, out var date) ? date : DateTime.MinValue;
        }

        public static string AsString(TimeSpan timeSpan, string separator = "", params string[] format)
        {
            if (separator.Contains(Constant.Point))
            {
                var separatorChars = new List<char>();
                for (int idx = 0; idx < separator.Length; idx++)
                {
                    var current = separator[idx];
                    if (current == Constant.PointChar)
                    {
                        separatorChars.Add(Constant.BackSlashChar);
                    }
                    separatorChars.Add(current);
                }
                separator = string.Concat(separatorChars);
            }
            string formatString = GetFormatString(separator, format);
            return timeSpan.ToString(formatString, CultureInfo.CurrentCulture.DateTimeFormat);
        }
    }
}
