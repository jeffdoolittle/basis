using System;
using System.Collections.Generic;
using System.Data;

namespace Basis.Resource
{
    public interface ISession
    {
        Guid SessionId { get; }
        IDbConnection Connection { get; }

        IDataReader ExecuteReader(string commandText, Action<ICommandConfigurer> configure, params object[] parameters);
        object ExecuteScalar(string commandText, Action<ICommandConfigurer> configure, params object[] parameters);
        int ExecuteNonQuery(string commandText, Action<ICommandConfigurer> configure, params object[] parameters);

        IDataReader ExecuteReader(string commandText, params object[] parameters);
        object ExecuteScalar(string commandText, params object[] parameters);
        int ExecuteNonQuery(string commandText, params object[] parameters);

        T FirstOrDefault<T>(string commandText, params object[] parameters) where T : class, new();
        T First<T>(string commandText, params object[] parameters) where T : class, new();
        T SingleOrDefault<T>(string commandText, params object[] parameters) where T : class, new();
        T Single<T>(string commandText, params object[] parameters) where T : class, new();
        IReadOnlyList<T> Fetch<T>(string commandText, params object[] parameters) where T : class, new();

        IPagingHelper<T> Paged<T>(string commandText, string orderBy, params object[] parameters) where T : class, new();

        T GetScalar<T>(string commandText, params object[] parameters);
        string GetString(string commandText, params object[] parameters);

        long Insert(object source, string tableName, string primaryKeyPropertyName, string sequenceName = null);
        void Insert(object source, string tableName);
        void Update(object source, string tableName, string primaryKeyPropertyName = "ID");
        void Delete(long id, string tableName, string primaryKeyPropertyName = "ID");
        void Delete(string tableName, string where, params object[] parameters);
    }
}