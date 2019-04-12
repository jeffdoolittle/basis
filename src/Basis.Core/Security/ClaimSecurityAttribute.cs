using System;
using System.Linq;
using System.Security.Claims;

namespace Basis.Security
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Interface, AllowMultiple = true)]
    public class RequireClaimsAttribute : Attribute
    {
        public RequireClaimsAttribute(string type, params string[] values)
        {
            Claims = values
                .Select(value => new Claim(type, value))
                .ToArray();
        }

        public Claim[] Claims { get; }
    }
}