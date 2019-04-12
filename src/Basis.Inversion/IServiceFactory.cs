using System;

namespace Basis.Inversion
{
    public interface IServiceFactory : IServiceProvider, IDisposable
    {
        IScopedServiceFactory CreateScope();
    }

    public interface IScopedServiceFactory : IServiceProvider, IDisposable
    {
    }
}
