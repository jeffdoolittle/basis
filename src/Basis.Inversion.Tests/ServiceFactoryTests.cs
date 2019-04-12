using System;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace Basis.Inversion.Tests
{
    public class ServiceFactoryTests
    {
        [Fact]
        public void can_resolve_disposable_service_that_is_disposed_when_factory_is_disposed()
        {
            var factory = new ServiceFactory(_ => _.Apply<ServiceRegistry>());
            var scope = factory.CreateScope();

            var service = factory.GetService<IService>();
            var scopedService = scope.GetService<IService>();

            service.Id.Should().NotBe(scopedService.Id);

            scope.Dispose();
            scopedService.IsDisposed.Should().BeTrue();

            factory.Dispose();
            service.IsDisposed.Should().BeTrue();
        }

        [Fact]
        public void can_retrieve_service_configuration()
        {
            var factory = new ServiceFactory(_ => _.Apply<ServiceRegistry>());
            var cfg = factory.Configuration.ToList();

            cfg.Should().Contain(x =>
                x.ServiceType == typeof(IService) &&
                x.ImplementationType == typeof(Service) &&
                x.Lifetime == "Transient");
        }
    }

    public class ServiceRegistry : Registry
    {
        public ServiceRegistry()
        {
            Register(_ => _.AddTransient<IService, Service>());
        }
    }

    public interface IService : IDisposable
    {
        Guid Id { get; }
        bool IsDisposed { get; }
    }

    public class Service : IService
    {
        public void Dispose()
        {
            if (IsDisposed)
            {
                throw new Exception("Already disposed!");
            }
            IsDisposed = true;
        }

        public Guid Id { get; } = Guid.NewGuid();
        public bool IsDisposed { get; private set; }
    }
}
