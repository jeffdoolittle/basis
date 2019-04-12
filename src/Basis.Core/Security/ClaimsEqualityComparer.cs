using System.Collections.Generic;
using System.Security.Claims;

namespace Basis.Security
{
    public class ClaimsEqualityComparer : IEqualityComparer<Claim>
    {
        public bool Equals(Claim x, Claim y)
        {
            if (ReferenceEquals(null, x)) return false;
            if (ReferenceEquals(null, y)) return false;
            if (ReferenceEquals(x, y)) return true;

            return x.Type == y.Type
                   && x.Value == y.Value
                   && x.ValueType == y.ValueType
                   && x.Issuer == y.Issuer;
        }

        public int GetHashCode(Claim obj)
        {
            unchecked
            {
                var hashCode = 17;
                hashCode = (hashCode * 23) + (obj.Type == null ? 0 : obj.Type.GetHashCode());
                hashCode = (hashCode * 23) + (obj.Value == null ? 0 : obj.Value.GetHashCode());
                return hashCode;
            }
        }
    }
}