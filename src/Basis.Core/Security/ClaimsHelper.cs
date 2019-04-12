using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Claims;

namespace Basis.Security
{
    public static class ClaimsHelper
    {
        public static IReadOnlyList<Claim> GetRequiredClaims<TService>(Expression<Action<TService>> expr)
        {
            var methodInfo = ((MethodCallExpression)expr.Body).Method;

            return GetRequiredClaims(methodInfo);
        }

        public static IReadOnlyList<Claim> GetRequiredClaims(MethodInfo methodInfo)
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            var claims = methodInfo
                .DeclaringType
                .GetCustomAttributes<RequireClaimsAttribute>()
                .SelectMany(x => x.Claims)
                .Union(methodInfo.GetCustomAttributes<RequireClaimsAttribute>().SelectMany(x => x.Claims), new ClaimsEqualityComparer())
                .Distinct(new ClaimsEqualityComparer())
                .ToList();

            return claims;
        }
    }
}