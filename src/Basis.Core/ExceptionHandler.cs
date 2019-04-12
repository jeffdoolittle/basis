using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Basis
{
    internal static class ExceptionHandler
    {
        [StackTraceHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerNonUserCode]
        internal static void Do(Action action)
        {
            bool Func()
            {
                action();
                return true;
            }

            Do(Func);
        }

        [StackTraceHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerNonUserCode]
        internal static T Do<T>(Func<T> func)
        {
            try
            {
                return func();
            }
            catch (BasisException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BasisException("An exception occurred", ex);
            }
        }
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    internal sealed class StackTraceHiddenAttribute : Attribute
    {
    }

    internal static class ExceptionExtensions
    {
        public static string GetStackTraceWithoutHiddenMethods(this Exception e)
        {
            return string.Concat(
                // ReSharper disable once AssignNullToNotNullAttribute
                new StackTrace(e, true)
                    .GetFrames()
                    .Where(frame => !frame.GetMethod().IsDefined(typeof(StackTraceHiddenAttribute), true))
                    .Select(frame => new StackTrace(frame).ToString())
                    .ToArray());
        }

        public static string GetToStringWithoutHiddenMethods(this Exception e)
        {
            string message = e.Message;
            string s;
            var className = e.GetType().FullName;

            if (message == null || message.Length <= 0)
            {
                s = className;
            }
            else
            {
                s = className + ": " + message;
            }

            if (e.InnerException != null)
            {
                s = s + " ---> " + e.InnerException.GetToStringWithoutHiddenMethods() + Environment.NewLine + "   " + "Exception_EndOfInnerExceptionStack";
            }

            if (e.StackTrace != null)
            {
                s += Environment.NewLine + e.GetStackTraceWithoutHiddenMethods();
            }

            return s;
        }
    }
}