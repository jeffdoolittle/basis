using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Basis.Security
{
    public static class Check
    {
        public static bool HasClaims(IReadOnlyList<Claim> requiredClaims, IReadOnlyList<Claim> actualClaims)
        {
            return requiredClaims.All(claim => actualClaims.Contains(claim, new ClaimsEqualityComparer()));
        }

        public static void ThrowIfMissingRequiredClaim(IReadOnlyList<Claim> requiredClaims, IReadOnlyList<Claim> actualClaims)
        {
            var hasClaims = HasClaims(requiredClaims, actualClaims);
            if (!hasClaims)
            {
                var message = "";
                throw new SecurityException(message);
            }
        }
    }
}