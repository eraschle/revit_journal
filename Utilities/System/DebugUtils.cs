﻿using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Utilities.System
{
    public static class DebugUtils
    {
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
