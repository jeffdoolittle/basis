using System;
using System.Data;
using System.Linq;
using FastMember;

namespace Basis.Db
{
    // todo: needs to work for particular dialect, such as Oracle for converting strings to booleans
    internal static class DataReaderExtensions
    {
        public static T MapDataTo<T>(this IDataReader reader) where T : class, new()
        {
            var target = new T();
            MapDataTo(reader, target);
            return target;
        }

        public static void MapDataTo<T>(this IDataReader reader, T target)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            var objectMemberAccessor = TypeAccessor.Create(target.GetType());

            var propertiesHashSet =
                objectMemberAccessor
                    .GetMembers()
                    .Select(mp => new
                    {
                        mp.Name,
                        NameUpper = mp.Name.ToUpperInvariant(),
                        mp.Type
                    })
                    .ToHashSet();

            for (int i = 0; i < reader.FieldCount; i++)
            {
                var property = propertiesHashSet.SingleOrDefault(_ => _.NameUpper == reader.GetName(i).ToUpperInvariant());

                if (property == null) { continue; }

                var targetType = property.Type;

                var value = reader.GetValue(i);

                if (reader.IsDBNull(i))
                {
                    objectMemberAccessor[target, property.Name] = null;
                    continue;
                }

                if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    targetType = targetType.GetGenericArguments()[0];
                }
                else if (targetType == typeof(bool) && value is string)
                {
                    value = ConvertStringToBoolean(value.ToString());
                }

                objectMemberAccessor[target, property.Name] = Convert.ChangeType(value, targetType);
            }
        }

        private static bool ConvertStringToBoolean(string value)
        {
            switch (value.ToUpperInvariant())
            {
                case "1":
                case "Y":
                case "YES":
                case "T":
                case "TRUE":
                    return true;

                case "0":
                case "N":
                case "NO":
                case "F":
                case "FALSE":
                    return false;

                default:
                    throw new ArgumentException($"'{value}' is not a valid boolean string");
            }
        }
    }
}