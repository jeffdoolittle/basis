using System;
using System.Data.Common;

namespace Basis.Resource
{
    public interface IDbProviderFactoryResolver
    {
        DbProviderFactory GetDbProviderFactory(DbProviderTypes providerType);
    }

    public enum DbProviderTypes
    {
        SqLite,
        Postgres,
        Oracle
    }

    public class ReflectedDbProviderFactoryResolver : IDbProviderFactoryResolver
    {
        public DbProviderFactory GetDbProviderFactory(DbProviderTypes type)
        {
            if (type == DbProviderTypes.SqLite)
            {
#if NETFULL
        return GetDbProviderFactory("System.Data.SQLite.SQLiteFactory", "System.Data.SQLite");
#else
                return GetDbProviderFactory("Microsoft.Data.Sqlite.SqliteFactory", "Microsoft.Data.Sqlite");
#endif
            }
            if (type == DbProviderTypes.Postgres)
                return GetDbProviderFactory("Npgsql.NpgsqlFactory", "Npgsql");
            if (type == DbProviderTypes.Oracle)
                return GetDbProviderFactory("Oracle.ManagedDataAccess.Client", "Oracle.ManagedDataAccess");

            throw new NotSupportedException($"Unsupported provider factory {type.ToString()}");
        }

        private DbProviderFactory GetDbProviderFactory(string dbProviderFactoryTypename, string assemblyName)
        {
            var instance = ReflectionHelper.GetStaticProperty(dbProviderFactoryTypename, "Instance");
            if (instance == null)
            {
                var a = ReflectionHelper.LoadAssembly(assemblyName);
                if (a != null)
                    instance = ReflectionHelper.GetStaticProperty(dbProviderFactoryTypename, "Instance");
            }

            if (instance == null)
                throw new InvalidOperationException($"Unable to retrieve DbProviderFactory {dbProviderFactoryTypename}");

            return instance as DbProviderFactory;
        }
    }
}