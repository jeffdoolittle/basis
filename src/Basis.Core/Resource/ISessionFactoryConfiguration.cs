using System;
using Basis.Logging;

namespace Basis.Resource
{
    public interface ISessionFactoryConfiguration
    {
        string ConnectionString { get; }
        DbProviderTypes ProviderType { get; }
        ILogger Logger { get; }
        TimeSpan ElapsedTimeWarningThreshold { get; }
        int TotalDbCallsPerSessionWarningThreshold { get; }
    }
}