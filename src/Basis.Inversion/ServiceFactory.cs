using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Basis.Inversion
{
    public class ServiceFactory : IServiceFactory
    {
        private readonly ServiceProvider _provider;
        private readonly List<IServiceDescriptor> _configuration;

        public ServiceFactory(Action<IServiceFactoryConfigurer> configure)
        {
            try
            {
                var configurer = new ServiceFactoryConfigurer();
                configure(configurer);
                var registryType = configurer.RegistryType;
                var registry = Factory.CreateInstanceAs<Registry>(registryType);

                var serviceCollection = new ServiceCollection();
                registry.Configure(serviceCollection);

                _provider = serviceCollection.BuildServiceProvider(new ServiceProviderOptions
                {
                    ValidateScopes = true
                });

                _configuration = serviceCollection
                    .Select(x => (IServiceDescriptor)new ServiceDescriptorAdapter(x))
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new InversionException(string.Format(Messages.unable_to_construct_x, nameof(ServiceFactory)), ex);
            }
        }

        public object GetService(Type type)
        {
            return _provider.GetService(type);
        }

        public IScopedServiceFactory CreateScope()
        {
            return new ScopedServiceProvider(_provider.CreateScope());
        }

        public IReadOnlyList<IServiceDescriptor> Configuration => _configuration;

        private class ServiceFactoryConfigurer : IServiceFactoryConfigurer
        {
            public void Apply<TRegistry>() where TRegistry : Registry, new()
            {
                RegistryType = typeof(TRegistry);
            }

            public Type RegistryType { get; private set; }
        }

        private class ScopedServiceProvider : IScopedServiceFactory
        {
            private readonly IServiceScope _scope;

            public ScopedServiceProvider(IServiceScope scope)
            {
                _scope = scope;
            }

            public object GetService(Type serviceType)
            {
                return _scope.ServiceProvider.GetService(serviceType);
            }

            public void Dispose()
            {
                _scope.Dispose();
            }
        }

        [DebuggerDisplay("Lifetime = {Lifetime}, ServiceType = {ServiceType}, ImplementationType = {ImplementationType}")]
        private class ServiceDescriptorAdapter : IServiceDescriptor
        {
            private readonly ServiceDescriptor _descriptor;

            public ServiceDescriptorAdapter(ServiceDescriptor descriptor)
            {
                _descriptor = descriptor;
            }

            public string Lifetime => _descriptor.Lifetime.ToString();
            public Type ServiceType => _descriptor.ServiceType;
            public Type ImplementationType => _descriptor.ImplementationType;
        }

        public interface IServiceFactoryConfigurer
        {
            void Apply<TRegistry>() where TRegistry : Registry, new();
        }

        public void Dispose()
        {
            _provider.Dispose();
        }
    }
}