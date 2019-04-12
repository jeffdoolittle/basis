using System;
using FluentAssertions;
using Xunit;

namespace Basis.Tests
{
    public class FactoryTests
    {
        [Fact]
        public void can_create_class_using_generic_factory()
        {
            var dto = Factory<Widget>.CreateInstance();
            dto.Should().NotBeNull();
        }

        [Fact]
        public void can_create_class_using_non_generic_factory()
        {
            var dto = Factory.CreateInstance(typeof(Widget));
            dto.Should().NotBeNull();
            dto.Should().BeOfType<Widget>();

            var dto2 = Factory.CreateInstance(typeof(Widget));
            dto2.Should().NotBeNull();
            dto2.Should().BeOfType<Widget>();
        }

        [Fact]
        public void exception_when_attempting_to_create_interface_type()
        {
            Func<object> func = () => Factory.CreateInstance(typeof(IWidget));

            func.Should().Throw<BasisException>();
        }

        [Fact]
        public void exception_when_attempting_to_create_type_without_public_parameterless_constructor()
        {
            Func<object> func = () => Factory.CreateInstance(typeof(AppDomain));

            func.Should().Throw<BasisException>();
        }

        public class Widget
        {
        }

        public interface IWidget
        {
        }        
    }
}