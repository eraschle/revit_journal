using System;
using System.Globalization;

namespace RevitJournalUI.Helper
{
    public static class TimeSpanHelper
    {
        private const string FormatMinuteAndSeconds = "mm\\.ss";
        private const string FormatMinutes = "mm";

        public static string GetMinutes(TimeSpan timeSpan)
        {
            return timeSpan.ToString(FormatMinutes, CultureInfo.CurrentCulture.DateTimeFormat);
        }

        public static string GetMinuteAndSeconds(TimeSpan timeSpan)
        {
            return timeSpan.ToString(FormatMinuteAndSeconds, CultureInfo.CurrentCulture.DateTimeFormat);
        }
    }
}
