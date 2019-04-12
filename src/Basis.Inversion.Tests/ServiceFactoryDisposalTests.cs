using System;
using FluentAssertions;
using Xunit;

namespace Basis.Inversion.Tests
{
    public class ServiceFactoryDisposalTests
    {
        [Fact]
        public void should_dispose_from_singleton_factory()
        {
            var factory = new ServiceFactory(_ => _.Apply<SingletonRegistry>());
            var disposable = factory.GetService<Disposable>();
            factory.Dispose();
            disposable.DisposeCount.Should().Be(1);
        }

        [Fact]
        public void should_dispose_from_scoped_factory()
        {
            var factory = new ServiceFactory(_ => _.Apply<ScopedRegistry>());
            var scope = factory.CreateScope();
            var disposable = scope.GetService<Disposable>();
            scope.Dispose();
            disposable.DisposeCount.Should().Be(1);
        }

        [Fact]
        public void should_dispose_from_transient_factory()
        {
            var factory = new ServiceFactory(_ => _.Apply<TransientRegistry>());
            var disposable = factory.GetService<Disposable>();
            factory.Dispose();
            disposable.DisposeCount.Should().Be(1);
        }

        [Fact]
        public void should_not_perform_multiple_disposals_for_objects_in_factory()
        {
            var factory = new ServiceFactory(_ => _.Apply<TransientRegistry>());
            var disposable = factory.GetService<Disposable>();

            factory.Dispose();
            disposable.DisposeCount.Should().Be(1);

            factory.Dispose();
            disposable.DisposeCount.Should().Be(1);
        }

        public class SingletonRegistry : Registry
        {
            public SingletonRegistry()
            {
                Register(_ => _.AddSingleton<Disposable, Disposable>());
            }
        }

        public class ScopedRegistry : Registry
        {
            public ScopedRegistry()
            {
                Register(_ => _.AddScoped<Disposable, Disposable>());
            }
        }

        public class TransientRegistry : Registry
        {
            public TransientRegistry()
            {
                Register(_ => _.AddTransient<Disposable, Disposable>());
            }
        }

        public class Disposable : IDisposable
        {
            public void Dispose()
            {
                DisposeCount++;
            }

            public int DisposeCount { get; private set; }
        }
    }
}