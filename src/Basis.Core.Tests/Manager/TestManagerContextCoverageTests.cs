using System.Collections.Generic;
using System.Security.Claims;
using Basis.Manager;
using FluentAssertions;
using Xunit;

namespace Basis.Tests.Manager
{
    public class TestManagerContextTests
    {
        [Fact]
        public void can_add_and_retrieve_headers()
        {
            var testContext = new TestManagerContext();
            testContext.Headers.Add(new KeyValuePair<string, object>("key", "value"));

            var context = (IManagerContext) testContext;

            context.Headers.Should().Contain(new KeyValuePair<string, object>("key", "value"));
        }

        [Fact]
        public void can_add_and_clear_claims()
        {
            var testContext = new TestManagerContext();
            testContext.AddClaims(new Claim("test", "test"));
            testContext.ClearClaims();

            var context = (IManagerContext)testContext;
            context.User.Claims.Should().BeEmpty();
        }
    }
}