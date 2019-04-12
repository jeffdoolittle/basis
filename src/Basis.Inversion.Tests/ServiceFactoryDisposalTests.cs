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
            disposable.Disposed.Should().BeTrue();
        }

        [Fact]
        public void should_dispose_from_scoped_factory()
        {
            var factory = new ServiceFactory(_ => _.Apply<ScopedRegistry>());
            var scope = factory.CreateScope();
            var disposable = scope.GetService<Disposable>();
            scope.Dispose();
            disposable.Disposed.Should().BeTrue();
        }

        [Fact]
        public void should_dispose_from_transient_factory()
        {
            var factory = new ServiceFactory(_ => _.Apply<TransientRegistry>());
            var disposable = factory.GetService<Disposable>();
            factory.Dispose();
            disposable.Disposed.Should().BeTrue();
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
                if (Disposed)
                {
                    throw new Exception("Already disposed!");
                }
                Disposed = true;
            }

            public bool Disposed { get; private set; }
        }
    }
}