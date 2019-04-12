using System;
using FluentAssertions;
using Xunit;

namespace Basis.Inversion.Tests
{
    public class ScopedServiceFactoryTests
    {
        [Fact]
        public void can_retrieve_service_configuration()
        {
            using (var root = new ServiceFactory(_ => _.Apply<OperationRegistry>()))
            {
                var cfg = root.Configuration;
                cfg.Count.Should().Be(5);
            }
        }

        [Fact]
        public void cannot_resolve_scoped_service_from_root_provider()
        {
            using (var root = new ServiceFactory(_ => _.Apply<OperationRegistry>()))
            {
                var provider = root;

                Action tryToResolveScopedServiceFromScopeRequiredRoot =
                    () => provider.GetService<OperationService>();

                var expectedMessage = "Cannot resolve '*' from root provider because it " +
                                      "requires scoped service '*'.";

                tryToResolveScopedServiceFromScopeRequiredRoot
                    .Should()
                    .Throw<InvalidOperationException>()
                    .WithMessage(expectedMessage);
            }
        }

        [Fact]
        public void can_resolve_scoped_service_in_scope()
        {
            using (var root = new ServiceFactory(_ => _.Apply<OperationRegistry>()))
            using (var scope = root.CreateScope())
            {
                var service1 = scope.GetService<OperationService>();
                var service2 = scope.GetService<OperationService>();

                service1.SingletonInstanceOperation.OperationId
                    .Should().Be(service2.SingletonInstanceOperation.OperationId);

                service1.SingletonOperation.OperationId
                    .Should().Be(service2.SingletonOperation.OperationId);

                // same scope resolution so scopes should match
                service1.ScopedOperation.OperationId
                    .Should().Be(service2.ScopedOperation.OperationId);

                service1.TransientOperation.OperationId
                    .Should().NotBe(service2.TransientOperation.OperationId);
            }
        }

        [Fact]
        public void can_resolve_distinct_scoped_services_from_different_scopes()
        {
            using (var root = new ServiceFactory(_ => _.Apply<OperationRegistry>()))
            using (var scope1 = root.CreateScope())
            using (var scope2 = root.CreateScope())
            {
                var service1 = scope1.GetService<OperationService>();
                var service2 = scope2.GetService<OperationService>();

                service1.SingletonInstanceOperation.OperationId
                    .Should().Be(service2.SingletonInstanceOperation.OperationId);

                service1.SingletonOperation.OperationId
                    .Should().Be(service2.SingletonOperation.OperationId);

                // different scopes so scopes should not match
                service1.ScopedOperation.OperationId
                    .Should().NotBe(service2.ScopedOperation.OperationId);

                service1.TransientOperation.OperationId
                    .Should().NotBe(service2.TransientOperation.OperationId);
            }
        }

        public class OperationRegistry : Registry
        {
            public OperationRegistry()
            {
                Register(services =>
                {
                    services.AddTransient<IOperationTransient, Operation>();
                    services.AddScoped<IOperationScoped, Operation>();
                    services.AddSingleton<IOperationSingleton, Operation>();
                    services.AddSingleton<IOperationSingletonInstance>(sp => new Operation(Guid.Empty));
                    services.AddTransient<OperationService, OperationService>();
                });
            }
        }
    }
}