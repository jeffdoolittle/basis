using System;
using FluentAssertions;
using Xunit;

namespace Basis.Tests
{
    public class ExpressionsTests
    {
        [Fact]
        public void can_get_property_name_from_valid_expressions()
        {
            var foo = new Foo();
            foo.Bar.Should().Be(0);

            var propertyName = Expressions.GetPropertyName<Foo, int>(_ => _.Bar);
            propertyName.Should().Be(nameof(Foo.Bar));
        }

        [Fact]
        public void exception_when_trying_to_get_method_name()
        {
            Action action = () => Expressions.GetPropertyName<Foo, string>(_ => _.ToString());
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void exception_when_trying_to_get_field_name()
        {
            Action action = () => Expressions.GetPropertyName<Foo, int>(_ => _.Baz);
            action.Should().Throw<ArgumentException>();
        }

        public class Foo
        {
            public int Bar { get; }
            public int Baz;
        }
    }
}