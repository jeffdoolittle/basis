using System;
using Basis.Resource;

namespace Basis.Db
{
    internal static class DialectResolver
    {
        public static IDialect Resolve(DbProviderTypes type)
        {
            switch (type)
            {
                case DbProviderTypes.SqLite:
                    return new SqliteDialect();
                case DbProviderTypes.Postgres:
                    return new PostgresDialect();
                case DbProviderTypes.Oracle:
                    return new OracleDialect();
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}