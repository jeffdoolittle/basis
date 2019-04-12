using System;
using Basis.Logging;

namespace Basis.Resource
{
    public interface ISessionFactoryConfigurer
    {
        /// <summary>
        /// Configures session factory to use connection string from ConfigurationManager with the supplied connectionStringName
        /// </summary>
        /// <param name="connectionStringName"></param>
        /// <returns></returns>
        ISessionFactoryConfigurer UseConnectionStringFromConfigurationNamed(string connectionStringName);

        /// <summary>
        /// Configures session factory to use the supplied connectionString
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        ISessionFactoryConfigurer UseConnectionString(string connectionString);

        /// <summary>
        /// Configures session factory to use the provider matching the supplied providerName
        /// </summary>
        /// <param name="providerType"></param>
        /// <returns></returns>
        ISessionFactoryConfigurer WithProviderType(DbProviderTypes providerType);

        /// <summary>
        /// Set the Logger to be used for this session factor and any sessions it creates
        /// </summary>
        /// <param name="logger"></param>
        /// <returns></returns>
        ISessionFactoryConfigurer UseLogger(ILogger logger);

        /// <summary>
        /// Sets the elapsed times after which logging will switch from INF to WRN level
        /// </summary>
        /// <returns></returns>
        ISessionFactoryConfigurer SetElapsedTimeWarningThreshold(TimeSpan threshold);

        /// <summary>
        /// Sets the maximum number of db calls per session after which logging will switch from INF to WRN level
        /// </summary>
        /// <param name="threshold"></param>
        /// <returns></returns>
        ISessionFactoryConfigurer SetTotalDbCallsPerSessionWarningThreshold(int threshold);
    }
}