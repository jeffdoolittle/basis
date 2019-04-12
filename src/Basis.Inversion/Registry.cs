using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Basis.Inversion
{
    public class Registry
    {
        private readonly List<Action<IServiceCollection>> _actions;
        private readonly RegistryConfigurer _configurer;

        public Registry()
        {
            _actions = new List<Action<IServiceCollection>>();
            _configurer = new RegistryConfigurer(_actions);
        }

        internal void Configure(IServiceCollection serviceCollection)
        {
            _actions.ForEach(action => action(serviceCollection));
        }

        public void IncludeRegistry<T>() where T : Registry, new()
        {
            IncludeRegistry(new T());
        }

        public void IncludeRegistry(Registry registry)
        {
            foreach (var action in registry._actions)
            {
                _actions.Add(action);
            }
        }

        public void Register(Action<IRegistryConfigurer> action)
        {
            action(_configurer);
        }

        private class RegistryConfigurer : IRegistryConfigurer
        {
            private readonly List<Action<IServiceCollection>> _actions;

            internal RegistryConfigurer(List<Action<IServiceCollection>> actions)
            {
                _actions = actions;
            }

            public IRegistryConfigurer AddSingleton<TService, TImplementation>() where TService : class where TImplementation : class, TService
            {
                _actions.Add(_ => _.AddSingleton<TService, TImplementation>());
                return this;
            }

            public IRegistryConfigurer AddSingleton<TService>(Func<IServiceProvider, TService> factory) where TService : class
            {
                _actions.Add(_ => _.AddSingleton(factory));
                return this;
            }

            public IRegistryConfigurer AddScoped<TService, TImplementation>() where TService : class where TImplementation : class, TService
            {
                _actions.Add(_ => _.AddScoped<TService, TImplementation>());
                return this;
            }

            public IRegistryConfigurer AddScoped<TService>(Func<IServiceProvider, TService> factory) where TService : class
            {
                _actions.Add(_ => _.AddScoped(factory));
                return this;
            }

            public IRegistryConfigurer AddTransient<TService, TImplementation>() where TService : class where TImplementation : class, TService
            {
                _actions.Add(_ => _.AddTransient<TService, TImplementation>());
                return this;
            }

            public IRegistryConfigurer AddTransient<TService>(Func<IServiceProvider, TService> factory) where TService : class
            {
                _actions.Add(_ => _.AddTransient(factory));
                return this;
            }
        }
    }
}