using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Utilities.System
{
    public static class DebugUtils
    {

        private static DateTime watch;
        private static string[] formatWatch = new string[] { DateUtils.Minute, DateUtils.Seconds, DateUtils.Milliseconds };

        public static void StartWatch<TSource>([CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0)
            where TSource : class
        {
            watch = DateTime.Now;
            Line<TSource>("Time watch started", memberName, lineNumber);
        }

        public static void StopWatch<TSource>([CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0)
            where TSource : class
        {
            var time = DateTime.Now - watch;
            var message = $"Total time: {DateUtils.AsString(time, Constant.Point, formatWatch)}";
            Line<TSource>(message, memberName, lineNumber);
        }

        public static void Line<TSource>(string message, [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0)
            where TSource : class
        {
            if (string.IsNullOrWhiteSpace(message)) { throw new ArgumentNullException(nameof(message)); }

            Debug.WriteLine($"[{lineNumber}] {typeof(TSource).Name}.{memberName}: {message}");
        }

        public static void Exception<TSource>(Exception exception, string message = "", [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0)
            where TSource : class
        {
            if (string.IsNullOrWhiteSpace(message) == false)
            {
                message = $"Message: {message} Exception: ";
            }
            Debug.WriteLine($"[{lineNumber}] {typeof(TSource).Name}.{memberName}: {message}{exception.Message}");
        }

        public static void Stack<TSource>(Exception exception, [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0)
            where TSource : class
        {
            Debug.WriteLine($"[{lineNumber}] {typeof(TSource).Name}.{memberName}: {exception.StackTrace}");
        }
    }
}
