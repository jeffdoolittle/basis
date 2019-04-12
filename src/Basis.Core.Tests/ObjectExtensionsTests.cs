using System;
using FluentAssertions;
using Xunit;

namespace Basis.Tests
{
    public class ObjectExtensionsTests
    {
        [Fact]
        public void can_safe_dispose_a_disposable_object()
        {
            var disposable = new Disposable();
            disposable.SafeDispose();
            disposable.IsDisposed.Should().BeTrue();
        }

        [Fact]
        public void can_safe_dispose_a_non_disposable_object()
        {
            var nonDisposable = new NonDisposable();
            Action action = () => nonDisposable.SafeDispose();
            action.Should().NotThrow();
        }

        [Theory]
        [InlineData("0", typeof(int), 0)]
        [InlineData(null, typeof(int?), null)]
        [InlineData(0, typeof(string), "0")]
        [InlineData(0, typeof(int?), 0)]
        public void can_convert_object_types_non_generically(object value, Type targetType, object expected)
        {
            var output = value.As(targetType);
            expected.Should().BeEquivalentTo(output);
        }

        [Fact]
        public void can_convert_object_types_generically()
        {
            var output = "0".As<int>();
            output.Should().Be(0);
        }

        [Fact]
        public void exception_when_types_do_not_convert()
        {
            Action action = () =>  DateTime.UtcNow.As<int>();
            var ex = action.Should().Throw<BasisException>()
                .And;

            var str = ex.ToString();
        }

        public class Disposable : IDisposable
        {
            public void Dispose()
            {
                IsDisposed = true;
            }

            public bool IsDisposed { get; private set; }
        }

        public class NonDisposable
        {
        }
    }
}