using System.Security.Claims;
using Basis.Security;
using FluentAssertions;
using Xunit;

namespace Basis.Tests.Security
{
    public class ClaimsEqualityComparerTests
    {
        [Fact]
        public void can_compare_claims_for_equality()
        {
            var comparer = new ClaimsEqualityComparer();

            var x = new Claim("", "");
            var y = x;
            var z = new Claim("", "");

            comparer.Equals(x, y).Should().BeTrue();

            comparer.Equals(null, null).Should().BeFalse();
            comparer.Equals(null, y).Should().BeFalse();
            comparer.Equals(x, null).Should().BeFalse();

            comparer.Equals(x, z).Should().BeTrue();
        }
    }
}