using System.Collections.Generic;
using System.Security.Claims;

namespace Basis.Manager
{
    public interface IManagerContext
    {
        IEnumerable<KeyValuePair<string, object>> Headers { get; }
        ClaimsPrincipal User { get; }
    }

    public class TestManagerContext : IManagerContext
    {
        IEnumerable<KeyValuePair<string, object>> IManagerContext.Headers => Headers;
        ClaimsPrincipal IManagerContext.User => User;

        public List<KeyValuePair<string, object>> Headers { get; } = new List<KeyValuePair<string, object>>();
        public ClaimsPrincipal User => new ClaimsPrincipal(new ClaimsIdentity(_claims));

        private readonly List<Claim> _claims = new List<Claim>();

        public void AddClaims(params Claim[] claims)
        {
            _claims.AddRange(claims);
        }

        public void ClearClaims()
        {
            _claims.Clear();
        }
    }
}