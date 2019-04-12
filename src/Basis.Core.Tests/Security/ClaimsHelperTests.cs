using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using Basis.Security;
using FluentAssertions;
using Xunit;

namespace Basis.Tests.Security
{
    public class ClaimsHelperTests
    {
        [Theory]
        [MemberData(nameof(GetConfigurations))]
        public void can_resolve_service_security_claims(Configuration configuration)
        {
            var claims = configuration.ClaimSource();
            claims.Count.Should().Be(configuration.ExpectedClaimsCount);
        }

        public static IEnumerable<object[]> GetConfigurations()
        {
            return Configurations().Select(x => new object[] { x });
        }

        public static IEnumerable<Configuration> Configurations()
        {
            yield return new Configuration<IServiceWithNoClaims>(_ => _.When(), 0);
            yield return new Configuration<IServiceWithServiceLevelRequireClaim>(_ => _.When(), 1);
            yield return new Configuration<IServiceWithMethodLevelRequireClaim>(_ => _.When(), 1);
            yield return new Configuration<IServiceWithServiceAndMethodLevelRequireClaims>(_ => _.When(), 2);
            yield return new Configuration<IServiceWithMethodLevelRequireClaims>(_ => _.When(), 2);
            yield return new Configuration<IServiceWithServiceLevelRequireClaims>(_ => _.When(), 2);
        }

        public class Configuration
        {
            public Configuration(Func<IReadOnlyList<Claim>> claimSource, int expectedClaimsCount)
            {
                ClaimSource = claimSource;
                ExpectedClaimsCount = expectedClaimsCount;
            }

            public Func<IReadOnlyList<Claim>> ClaimSource { get; set; }
            public int ExpectedClaimsCount { get; set; }
        }

        public class Configuration<T> : Configuration
        {
            public Configuration(Expression<Action<T>> expr, int expectedClaimsCount)
                : base(() => ClaimsHelper.GetRequiredClaims(expr), expectedClaimsCount)
            {
            }

            public override string ToString()
            {
                return $"{typeof(T).Name} expects {ExpectedClaimsCount} claims.";
            }
        }
    }
}
