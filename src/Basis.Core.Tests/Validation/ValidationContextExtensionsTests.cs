using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Basis.Validation;
using FakeItEasy;
using FluentAssertions;
using Xunit;

namespace Basis.Tests.Validation
{
    public class ValidationContextExtensionsTests
    {
        [Fact]
        public void can_resolve_registered_service()
        {
            var value = Guid.NewGuid().ToString();
            var provider = A.Fake<IServiceProvider>();
            A.CallTo(() => provider.GetService(typeof(string)))
                .Returns(value);
            var context = new ValidationContext(new object(), provider, null);
            var service = context.GetService<string>();
            service.Should().Be(value);
        }

        [Fact]
        public void can_fallback_to_items_when_service_is_not_found()
        {
            var value = Guid.NewGuid().ToString();
            var items = new Dictionary<object, object>();
            items.Add(typeof(string), value);
            var context = new ValidationContext(new object(), null, items);
            var service = context.GetService<string>();
            service.Should().Be(value);
        }

        [Fact]
        public void exception_if_service_is_not_registered()
        {
            var items = new Dictionary<object, object>();
            var context = new ValidationContext(new object(), null, items);
            Action action = () => context.GetService<string>();

            action.Should().Throw<BasisException>();
        }
    }
}