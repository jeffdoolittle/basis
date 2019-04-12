using System;

namespace Basis.Inversion
{
    public interface IRegistryConfigurer
    {
        IRegistryConfigurer AddSingleton<TService, TImplementation>() where TService : class where TImplementation : class, TService;
        IRegistryConfigurer AddSingleton<TService>(Func<IServiceProvider, TService> factory) where TService : class;

        IRegistryConfigurer AddScoped<TService, TImplementation>() where TService : class where TImplementation : class, TService;
        IRegistryConfigurer AddScoped<TService>(Func<IServiceProvider, TService> factory) where TService : class;

        IRegistryConfigurer AddTransient<TService, TImplementation>() where TService : class where TImplementation : class, TService;
        IRegistryConfigurer AddTransient<TService>(Func<IServiceProvider, TService> factory) where TService : class;
    }
}