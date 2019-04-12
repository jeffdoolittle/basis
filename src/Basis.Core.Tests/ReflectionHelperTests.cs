using System;
using System.Reflection;
using System.Reflection.Emit;
using FluentAssertions;
using Xunit;

namespace Basis.Tests
{
    public class ReflectionHelperTests
    {
        [Fact]
        public void can_get_static_property()
        {
            var val1 = ReflectionHelper.GetStaticProperty(typeof(Target), "Value");
            var val2 = ReflectionHelper.GetStaticProperty("Basis.Tests.ReflectionHelperTests+Target", "Value");

            val1.Should().Be(val2);
        }

        [Fact]
        public void can_get_type_by_name()
        {
            var type = ReflectionHelper.GetTypeFromName("System.Int32");
            type.Should().Be(typeof(int));
        }

        [Fact]
        public void can_load_assembly_by_name()
        {
            var a = ReflectionHelper.LoadAssembly("xunit.runner.utility.netcoreapp10.dll");
            a.Should().NotBeNull();
        }

        [Fact]
        public void can_load_assembly_by_full_name()
        {
            var a = ReflectionHelper.LoadAssembly("xunit.runner.utility.netcoreapp10, Version=2.3.1.3858, Culture=neutral, PublicKeyToken=8d05b1bb7a6fdb6c");
            a.Should().NotBeNull();
        }

        [Fact]
        public void can_load_type_from_assembly()
        {
            var type = ReflectionHelper.GetTypeFromName("Guard", "xunit.runner.utility.netcoreapp10.dll");
            type.Should().NotBeNull();
        }

        [Fact]
        public void can_load_type_from_assembly_loading_assembly()
        {
            var type = ReflectionHelper.GetTypeFromName("Guard", "xunit.runner.utility.netcoreapp10.dll", true);
            type.Should().NotBeNull();
        }

        [Fact]
        public void exception_when_type_not_found()
        {
            Action action = () => ReflectionHelper.GetTypeFromName("foobar");
            action.Should().Throw<BasisException>();
        }

        [Fact]
        public void exception_when_assembly_not_found()
        {
            Action action = () => ReflectionHelper.GetTypeFromName("nope", "nada"); ;
            action.Should().Throw<BasisException>();
        }

        [Fact]
        public void exception_when_assembly_found_but_not_type()
        {
            Action action = () => ReflectionHelper.GetTypeFromName("nope", "xunit.runner.utility.netcoreapp10.dll", true);
            action.Should().Throw<BasisException>();
        }

        public static class Target
        {
            public static string Value => "static property";
        }


    }
}