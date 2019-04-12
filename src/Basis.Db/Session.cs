using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using Basis.Logging;
using Basis.Resource;
using FastMember;

namespace Basis.Db
{
    internal class Session : ITransactionalSession
    {
        private readonly DbConnection _connection;
        private readonly DbTransaction _transaction;
        private readonly IDialect _dialect;
        private readonly ILogger _logger;

        public Session(DbConnection connection, DbTransaction transaction, IDialect dialect, ILogger logger)
        {
            _connection = connection;
            _transaction = transaction;
            _dialect = dialect;
            _logger = logger;
        }

        public Guid SessionId { get; } = Guid.NewGuid();

        public IDbConnection Connection => _connection;

        public void Dispose()
        {
            _transaction?.Dispose();
            _connection?.Dispose();
        }

        public IDataReader ExecuteReader(string commandText, Action<ICommandConfigurer> configure, params object[] parameters)
        {
            var command = CreateCommand(commandText, GetConfiguration(configure), parameters);
            return new AdaptedDataReader(command, command.ExecuteReader());
        }

        public object ExecuteScalar(string commandText, Action<ICommandConfigurer> configure, params object[] parameters)
        {
            using (var command = CreateCommand(commandText, GetConfiguration(configure), parameters))
            {
                return command.ExecuteScalar();
            }
        }

        public int ExecuteNonQuery(string commandText, Action<ICommandConfigurer> configure, params object[] parameters)
        {
            using (var command = CreateCommand(commandText, GetConfiguration(configure), parameters))
            {
                return command.ExecuteNonQuery();
            }
        }

        public IDataReader ExecuteReader(string commandText, params object[] parameters)
        {
            return ExecuteReader(commandText, null, parameters);
        }

        public object ExecuteScalar(string commandText, params object[] parameters)
        {
            return ExecuteScalar(commandText, null, parameters);
        }

        public int ExecuteNonQuery(string commandText, params object[] parameters)
        {
            return ExecuteNonQuery(commandText, null, parameters);
        }

        public T FirstOrDefault<T>(string commandText, params object[] parameters) where T : class, new()
        {
            using (var reader = ExecuteReader(commandText, parameters))
            {
                while (reader.Read())
                {
                    var result = reader.MapDataTo<T>();
                    return result;
                }
            }

            return null;
        }

        public T First<T>(string commandText, params object[] parameters) where T : class, new()
        {
            var result = FirstOrDefault<T>(commandText, parameters);

            if (result == null)
            {
                throw new ResourceException("No matching result was found");
            }

            return result;
        }

        public T SingleOrDefault<T>(string commandText, params object[] parameters) where T : class, new()
        {
            using (var reader = ExecuteReader(commandText, parameters))
            {
                while (reader.Read())
                {
                    var result = reader.MapDataTo<T>();

                    if (reader.Read())
                    {
                        throw new ResourceException("More than one element found when attempting to retrieve and Single result");
                    }

                    return result;
                }
            }

            return null;
        }

        public T Single<T>(string commandText, params object[] parameters) where T : class, new()
        {
            var result = SingleOrDefault<T>(commandText, parameters);

            if (result == null)
            {
                throw new ResourceException("No matching result was found");
            }

            return result;
        }

        public IReadOnlyList<T> Fetch<T>(string commandText, params object[] parameters) where T : class, new()
        {
            var list = new List<T>();

            using (var reader = ExecuteReader(commandText, parameters))
            {
                while (reader.Read())
                {
                    list.Add(reader.MapDataTo<T>());
                }
            }

            return list;
        }

        public IPagingHelper<T> Paged<T>(string commandText, string orderBy, params object[] parameters) where T : class, new()
        {
            var helper = new PagingHelper<T>(this, _dialect, commandText, orderBy, parameters);
            return helper;
        }

        public T GetScalar<T>(string commandText, params object[] parameters)
        {
            var value = ExecuteScalar(commandText, parameters);
            return value.As<T>();
        }

        public string GetString(string commandText, params object[] parameters)
        {
            var value = ExecuteScalar(commandText, parameters);
            return value.ToString();
        }

        public long Insert(object source, string tableName, string primaryKeyPropertyName, string sequenceName = null)
        {
            var accessor = TypeAccessor.Create(source.GetType());

            if (sequenceName == null)
            {
                var columns = accessor
                    .GetMembers()
                    .Select(m => new
                    {
                        NameUpper = m.Name.ToUpperInvariant(),
                        m.Name
                    })
                    .Where(c => c.NameUpper != "ID" && c.NameUpper != primaryKeyPropertyName.ToUpperInvariant())
                    .ToList();

                var valuesParameters = columns
                    .Select(m => accessor[source, m.Name])
                    .Select(Convert)
                    .ToArray();

                var idColumn = accessor
                    .GetMembers()
                    .Select(m => new
                    {
                        NameUpper = m.Name.ToUpperInvariant(),
                        m.Name
                    })
                    .SingleOrDefault(c => c.NameUpper == "ID" || c.NameUpper == primaryKeyPropertyName.ToUpperInvariant());

                var idValue = Convert(accessor[source, idColumn.Name]);

                var columnsSql = string.Join(",", columns.Select(c => c.NameUpper));
                var valuesParametersSql =
                    string.Join(", ",
                        Enumerable
                            .Range(0, columns.Count)
                            .Select(_ => $"{_dialect.ParameterPrefix}{_}"));

                var insert = $"INSERT INTO {tableName} ({primaryKeyPropertyName}, {columnsSql}) VALUES ({idValue}, {valuesParametersSql})";

                var rowsAffected = ExecuteNonQuery(insert, valuesParameters);

                if (rowsAffected != 1)
                {
                    throw new ResourceException($"Expected 1 row to be affected but result was {rowsAffected}");
                }

                return (long)idValue;
            }
            else
            {
                var columns = accessor
                    .GetMembers()
                    .Select(m => new
                    {
                        NameUpper = m.Name.ToUpperInvariant(),
                        m.Name
                    })
                    .Where(c => c.NameUpper != "ID" && c.NameUpper != primaryKeyPropertyName.ToUpperInvariant())
                    .ToList();

                var valuesParameters = columns
                    .Select(m => accessor[source, m.Name])
                    .Select(Convert)
                    .ToArray();

                var columnsSql = string.Join(",", columns.Select(c => c.NameUpper));
                var valuesParametersSql =
                    string.Join(", ",
                        Enumerable
                            .Range(0, columns.Count)
                            .Select(_ => $"{_dialect.ParameterPrefix}{_}"));

                // todo: terrible! use the dialect to do this for you
                if (_dialect.ProviderType == DbProviderTypes.SqLite)
                {
                    var prefix = _dialect.ParameterPrefix;
                    var sequence = FirstOrDefault<SqliteSequence>($"select * from sqlite_sequence where name = {prefix}0", sequenceName);
                    if (sequence == null)
                    {
                        sequence = new SqliteSequence { Name = sequenceName, Seq = 1 };
                        Insert(sequence, "sqlite_sequence");
                    }

                    var insert = $"INSERT INTO {tableName} ({primaryKeyPropertyName}, {columnsSql}) VALUES ({sequence.Seq}, {valuesParametersSql})";

                    var rowsAffected = ExecuteNonQuery(insert, valuesParameters);

                    if (rowsAffected != 1)
                    {
                        throw new ResourceException($"Expected 1 row to be affected but result was {rowsAffected}");
                    }

                    ExecuteNonQuery($"update sqlite_sequence set seq = {prefix}1 where name = {prefix}0", sequenceName, sequence.Seq + 1);

                    return sequence.Seq;
                }
                else
                {
                    var insert = $"INSERT INTO {tableName} ({primaryKeyPropertyName}, {columnsSql}) VALUES ({sequenceName}.NEXTVAL, {valuesParametersSql})";

                    var rowsAffected = ExecuteNonQuery(insert, valuesParameters);

                    if (rowsAffected != 1)
                    {
                        throw new ResourceException($"Expected 1 row to be affected but result was {rowsAffected}");
                    }

                    var selectId = $"select {sequenceName}.CURRVAL from DUAL";

                    var id = (long)System.Convert.ChangeType(ExecuteScalar(selectId), TypeCode.Int64);

                    return id;
                }
            }
        }

        private class SqliteSequence
        {
            public string Name { get; set; }
            public long Seq { get; set; }
        }

        public void Insert(object source, string tableName)
        {
            var accessor = TypeAccessor.Create(source.GetType());

            var columns = accessor
                .GetMembers()
                .Select(m => new
                {
                    NameUpper = m.Name.ToUpperInvariant(),
                    m.Name
                })
                .ToList();

            var valuesParameters = columns
                .Select(m => accessor[source, m.Name])
                .Select(Convert)
                .ToArray();

            var columnsSql = string.Join(",", columns.Select(c => c.NameUpper));
            var valuesParametersSql =
                string.Join(", ",
                    Enumerable
                        .Range(0, columns.Count)
                        .Select(_ => $"{_dialect.ParameterPrefix}{_}"));

            var insert = $"INSERT INTO {tableName} ({columnsSql}) VALUES ({valuesParametersSql})";

            var rowsAffected = ExecuteNonQuery(insert, valuesParameters);

            if (rowsAffected != 1)
            {
                throw new ResourceException($"Expected 1 row to be affected but result was {rowsAffected}");
            }
        }

        public void Update(object source, string tableName, string primaryKeyPropertyName = "ID")
        {
            var accessor = TypeAccessor.Create(source.GetType());

            var columns = accessor
                .GetMembers()
                .Select(m => new
                {
                    NameUpper = m.Name.ToUpperInvariant(),
                    m.Name
                })
                .ToList();

            var idColumn = columns.SingleOrDefault(x => x.NameUpper == primaryKeyPropertyName || x.NameUpper == "ID");

            if (idColumn == null)
            {
                throw new ArgumentException($"Property named {primaryKeyPropertyName} not found");
            }

            columns.Remove(idColumn);

            var valuesParameters = columns
                .Select(m => accessor[source, m.Name])
                .Select(Convert)
                .ToList();

            var idValue = accessor[source, idColumn.Name];

            valuesParameters.Insert(0, idValue);

            var setStatements = new List<string>();

            for (var c = 0; c < columns.Count; c++)
            {
                setStatements.Add($"{columns[c].NameUpper} = {_dialect.ParameterPrefix}{c + 1}");
            }

            var insert = $"UPDATE {tableName} SET {string.Join(", ", setStatements)} WHERE {primaryKeyPropertyName} = {_dialect.ParameterPrefix}0";

            var rowsAffected = ExecuteNonQuery(insert, valuesParameters.ToArray());

            if (rowsAffected != 1)
            {
                throw new ResourceException($"Expected 1 row to be affected but result was {rowsAffected}");
            }
        }

        public void Delete(long id, string tableName, string primaryKeyPropertyName = "ID")
        {
            var sql = $"delete from {tableName} where {primaryKeyPropertyName} = {_dialect.ParameterPrefix}0";

            var rowsAffected = ExecuteNonQuery(sql, id);

            if (rowsAffected != 1)
            {
                _logger.Info($"Delete from {tableName} with id {id} affected 0 rows");
            }
        }

        public void Delete(string tableName, string where, params object[] parameters)
        {
            var sql = $"delete from {tableName} {where}";

            var rowsAffected = ExecuteNonQuery(sql, parameters);

            if (rowsAffected == 0)
            {
                _logger.Info($"Delete from {tableName} with where clause '{where}' affected 0 rows");
            }
        }

        public void Commit()
        {
            _transaction?.Commit();
        }

        public void Rollback()
        {
            _transaction?.Rollback();
        }

        private static ICommandConfiguration GetConfiguration(Action<ICommandConfigurer> configure = null)
        {
            var configurer = new CommandConfigurer();
            configure?.Invoke(configurer);
            var configuration = configurer.GetConfiguration();
            return configuration;
        }

        private DbCommand CreateCommand(string query, ICommandConfiguration configuration, params object[] parameters)
        {
            var command = _connection.CreateCommand();
            command.Transaction = _transaction;
            command.CommandText = CleanupQuery(query);
            command.CommandTimeout = configuration.CommandTimeoutSeconds;

            if (_dialect.ProviderType == DbProviderTypes.Oracle)
            {
                ((dynamic)command).BindByName = true;
            }

            if (parameters != null && parameters.Any())
            {
                for (var p = 0; p < parameters.Length; p++)
                {
                    var parameter = command.CreateParameter();
                    parameter.ParameterName = _dialect.ParameterPrefix + p;
                    parameter.Value = parameters[p];
                    command.Parameters.Add(parameter);
                }
            }

            command.Prepare();

            return command;
        }

        private string CleanupQuery(string query)
        {
            var q = query.Trim();
            if (q.Contains(';'))
            {
                q = q.Replace(";", "");
            }

            return q;
        }

        private object Convert(object value)
        {
            if (value == null)
            {
                return DBNull.Value;
            }

            if (value is bool b)
            {
                return b == false ? 0 : 1;
            }

            if (value is DateTimeOffset dto)
            {
                return dto.ToString("O");
            }

            return value;
        }
    }
}