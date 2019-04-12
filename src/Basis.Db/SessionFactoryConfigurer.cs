using System;
using System.Configuration;
using Basis.Logging;
using Basis.Resource;

namespace Basis.Db
{
    public class SessionFactoryConfigurer : ISessionFactoryConfigurer
    {
        private readonly SessionFactoryConfiguration _configuration;

        public SessionFactoryConfigurer()
        {
            _configuration = new SessionFactoryConfiguration();
        }

        public ISessionFactoryConfigurer UseConnectionStringFromConfigurationNamed(string connectionStringName)
        {
            var connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            _configuration.ConnectionString = connectionString;
            return this;
        }

        public ISessionFactoryConfigurer UseConnectionString(string connectionStringName)
        {
            _configuration.ConnectionString = connectionStringName;
            return this;
        }

        public ISessionFactoryConfigurer WithProviderType(DbProviderTypes providerType)
        {
            _configuration.ProviderType = providerType;
            return this;
        }

        public ISessionFactoryConfigurer UseLogger(ILogger logger)
        {
            _configuration.Logger = logger ?? new DebugLogger();
            return this;
        }

        public ISessionFactoryConfigurer SetElapsedTimeWarningThreshold(TimeSpan threshold)
        {
            _configuration.ElapsedTimeWarningThreshold = threshold;
            return this;
        }

        public ISessionFactoryConfigurer SetTotalDbCallsPerSessionWarningThreshold(int threshold)
        {
            _configuration.TotalDbCallsPerSessionWarningThreshold = threshold;
            return this;
        }

        internal ISessionFactoryConfiguration BuildConfiguration()
        {
            return _configuration;
        }

        private class SessionFactoryConfiguration : ISessionFactoryConfiguration
        {
            public string ConnectionString { get; set; }
            public DbProviderTypes ProviderType { get; set; }
            public ILogger Logger { get; set; } = new DebugLogger();
            public TimeSpan ElapsedTimeWarningThreshold { get; set; } = TimeSpan.FromMilliseconds(100);
            public int TotalDbCallsPerSessionWarningThreshold { get; set; } = 10;
        }
    }
}
