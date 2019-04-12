using System;

namespace Basis
{
    public static class ObjectExtensions
    {
        public static void SafeDispose(this object value)
        {
            ExceptionHandler.Do(() =>
            {
                if (value != null && value is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            });
        }

        public static T As<T>(this object value)
        {
            return ExceptionHandler.Do(() => (T)As(value, typeof(T)));
        }

        public static object As(this object value, Type targetType)
        {
            return ExceptionHandler.Do(() =>
            {
                var t = targetType;

                if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    if (value == null)
                    {
                        return null;
                    }

                    t = Nullable.GetUnderlyingType(t);
                }

                return Convert.ChangeType(value, t);
            });
        }
    }
}