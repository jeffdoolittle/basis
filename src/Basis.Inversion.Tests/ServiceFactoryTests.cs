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
            scopedService.DisposalCount.Should().Be(1);

            factory.Dispose();
            service.DisposalCount.Should().Be(1);
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

        [Fact]
        public void exception_when_configuration_fails()
        {
            var message = Guid.NewGuid().ToString();

            Action action = () => new ServiceFactory(_ => throw new Exception(message));

            action.Should().Throw<InversionException>()
                .WithInnerException<Exception>()
                .WithMessage(message);
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
        int DisposalCount { get; }
    }

    public class Service : IService
    {
        public void Dispose()
        {
            DisposalCount++;
        }

        public Guid Id { get; } = Guid.NewGuid();
        public int DisposalCount { get; private set; }
    }
}
