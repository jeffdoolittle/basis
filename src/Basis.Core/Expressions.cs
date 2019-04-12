using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Basis
{
    public static class Expressions
    {
        public static string GetPropertyName<TSource, TProperty>(
            Expression<Func<TSource, TProperty>> propertyLambda)
        {
            var member = propertyLambda.Body as MemberExpression;
            if (member == null)
                throw new ArgumentException(string.Format(
                    $"Expression '{propertyLambda}' refers to a method, not a property."));

            var propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException(string.Format(
                    $"Expression '{propertyLambda}' refers to a field, not a property."));

            // unclear if this is necessary or not
            //var type = typeof(TSource);
            //if (type != propInfo.ReflectedType &&
            //    !type.IsSubclassOf(propInfo.ReflectedType))
            //    throw new ArgumentException(string.Format(
            //        $"Expression '{propertyLambda}' refers to a property that is not from type {type}."));

            return propInfo.Name;
        }
    }
}