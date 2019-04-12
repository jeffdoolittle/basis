using Basis.Resource;

namespace Basis.Db
{
    internal class OracleDialect : IDialect
    {
        public DbProviderTypes ProviderType => DbProviderTypes.Oracle;
        public string ParameterPrefix => ":";
    }

    internal class SqliteDialect : IDialect
    {
        public DbProviderTypes ProviderType => DbProviderTypes.SqLite;
        public string ParameterPrefix => "@";
    }

    internal class PostgresDialect : IDialect
    {
        public DbProviderTypes ProviderType => DbProviderTypes.Postgres;
        public string ParameterPrefix => "@";
    }
}