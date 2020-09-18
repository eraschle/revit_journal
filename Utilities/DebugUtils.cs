using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Utilities
{
    public static class DebugUtils
    {
        public static void DebugException<TSource>(Exception exception, string message = "", [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0)
            where TSource : class
        {
            if (string.IsNullOrWhiteSpace(message) == false)
            {
                message = $"Message: {message} Exception: ";
            }
            Debug.WriteLine($"[{lineNumber}] {typeof(TSource).Name}.{memberName}: {message}{exception.Message}");
        }

        public static void DebugStack<TSource>(Exception exception, [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0)
            where TSource : class
        {
            Debug.WriteLine($"[{lineNumber}] {typeof(TSource).Name}.{memberName}: {exception.StackTrace}");
        }
    }
}
