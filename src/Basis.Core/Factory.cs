using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace Basis
{
    public static class Factory<T> where T : new()
    {
        private static readonly Func<T> CreateInstanceFunc =
            Expression.Lambda<Func<T>>(Expression.New(typeof(T))).Compile();

        public static T CreateInstance() => CreateInstanceFunc();
    }

    public static class Factory
    {
        private static readonly ConcurrentDictionary<Type, Func<object>> Cache = new ConcurrentDictionary<Type, Func<object>>();

        public static T CreateInstanceAs<T>(Type type)
        {
            return (T)CreateInstance(type);
        }

        public static object CreateInstance(Type type)
        {
            try
            {
                Func<object> factory;

                if (Cache.ContainsKey(type))
                {
                    factory = Cache[type];
                }
                else
                {
                    var factoryType = typeof(Factory<>).MakeGenericType(type);

                    var methodInfo = factoryType
                        .GetMethod(nameof(Factory<object>.CreateInstance), BindingFlags.Public | BindingFlags.Static);

                    // ReSharper disable once PossibleNullReferenceException
                    factory = () => methodInfo.Invoke(null, null);

                    Cache.TryAdd(type, factory);
                }

                return factory();
            }
            catch (Exception ex)
            {
                throw new BasisException(string.Format(Messages.unable_to_create_instance_of_type_x, type), ex);
            }
        }
    }
}