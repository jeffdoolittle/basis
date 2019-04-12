using System.Linq;
using System.Security.Claims;
using Basis.Security;

namespace Basis.Tests.Security
{
    public interface IServiceWithNoClaims
    {
        void When();
    }

    [RequireClaims(RoleClaims.Type, RoleClaims.IsAuthenticated)]
    public interface IServiceWithServiceLevelRequireClaim
    {
        void When();
    }

    public interface IServiceWithMethodLevelRequireClaim
    {
        [RequireClaims(RoleClaims.Type, RoleClaims.OtherClaim)]
        void When();
    }

    [RequireClaims(RoleClaims.Type, RoleClaims.IsAuthenticated)]
    public interface IServiceWithServiceAndMethodLevelRequireClaims
    {
        [RequireClaims(RoleClaims.Type, RoleClaims.OtherClaim)]
        void When();
    }

    public interface IServiceWithMethodLevelRequireClaims
    {
        [RequireClaims(RoleClaims.Type, RoleClaims.IsAuthenticated, RoleClaims.OtherClaim)]
        void When();
    }

    [RequireClaims(RoleClaims.Type, RoleClaims.IsAuthenticated, RoleClaims.OtherClaim)]
    public interface IServiceWithServiceLevelRequireClaims
    {
        void When();
    }

    public static class RoleClaims
    {
        public const string Type = "basis-security-role-claim";

        public const string IsAuthenticated = nameof(IsAuthenticated);
        public const string OtherClaim = nameof(OtherClaim);        
    }
}