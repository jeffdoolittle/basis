using System;
using System.Collections.Generic;
using System.Security.Claims;
using Basis.Security;
using FluentAssertions;
using Xunit;

namespace Basis.Tests.Security
{
    public class CheckTests
    {
        [Fact]
        public void can_validate_claims()
        {
            var claim = new Claim("test", "test");
            var required = new List<Claim> { claim };
            var allowed = new List<Claim> { claim };
            var notAllowed = new List<Claim> { };

            Check.HasClaims(required, allowed).Should().BeTrue();
            Check.HasClaims(required, notAllowed).Should().BeFalse();

            Action action = () => Check.ThrowIfMissingRequiredClaim(required, allowed);
            action.Should().NotThrow<SecurityException>();
        }

        [Fact]
        public void can_throw_if_required_claims_are_missing()
        {
            var claim = new Claim("test", "test");
            var required = new List<Claim> { claim };
            var notAllowed = new List<Claim> { };

            Action action = () => Check.ThrowIfMissingRequiredClaim(required, notAllowed);
            action.Should().Throw<SecurityException>();
        }
    }
}