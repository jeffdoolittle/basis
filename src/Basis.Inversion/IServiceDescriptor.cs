using System;
using System.Diagnostics;

namespace Basis.Inversion
{
    public interface IServiceDescriptor
    {
        string Lifetime { get; }
        Type ServiceType { get; }
        Type ImplementationType { get; }
    }
}