using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Basis.Logging;
using Basis.Resource;

namespace Basis.Db
{
    public class SessionFactory : ISessionFactory
    {
        private readonly string _connectionString;
        private readonly DbProviderFactory _dbProviderFactory;
        private readonly IDialect _dialect;
        private readonly ILogger _logger;
        private readonly TimeSpan _elapsedWarningThreshold;
        private readonly int _dbCallCountWarningThreshold;

        public SessionFactory(Action<ISessionFactoryConfigurer> configure)
        {
            try
            {
                var configurer = new SessionFactoryConfigurer();
                configure(configurer);
                var configuration = configurer.BuildConfiguration();
                _connectionString = configuration.ConnectionString;
                var resolver = new ReflectedDbProviderFactoryResolver();
                _dbProviderFactory = resolver.GetDbProviderFactory(configuration.ProviderType);
                _dialect = DialectResolver.Resolve(configuration.ProviderType);
                _logger = configuration.Logger;
                _elapsedWarningThreshold = configuration.ElapsedTimeWarningThreshold;
                _dbCallCountWarningThreshold = configuration.TotalDbCallsPerSessionWarningThreshold;
            }
            catch (Exception ex)
            {
                throw new ResourceException("Unable to create Session Factory", ex);
            }
        }

        public ITransactionalSession OpenSession(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            try
            {
                var connection = _dbProviderFactory.CreateConnection();

                if (connection == null)
                {
                    throw new ResourceException($"Provider '{_dbProviderFactory}' failed to create a connection");
                }

                connection.ConnectionString = _connectionString;
                connection.Open();
                var transaction = connection.BeginTransaction(isolationLevel);

                var session = new Session(connection, transaction, _dialect, _logger);
                var decorated = new SessionDecorator(session, _logger, _elapsedWarningThreshold, _dbCallCountWarningThreshold);
                return decorated;
            }
            catch (ResourceException ex)
            {
                _logger.Error(ex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                throw new ResourceException("Unable to Open Session", ex);
            }
        }

        private class SessionDecorator : ITransactionalSession
        {
            private readonly Stopwatch _sessionStopwatch;
            private readonly ITransactionalSession _inner;
            private readonly ILogger _logger;
            private readonly TimeSpan _warningThreshold;
            private readonly int _dbCallWarningThreshold;
            private int _dbCallCount;

            public SessionDecorator(ITransactionalSession inner, ILogger logger, TimeSpan warningThreshold, int dbCallWarningThreshold)
            {
                _sessionStopwatch = Stopwatch.StartNew();
                _inner = inner;
                _logger = logger;
                _warningThreshold = warningThreshold;
                _dbCallWarningThreshold = dbCallWarningThreshold;
            }

            public Guid SessionId => _inner.SessionId;
            public IDbConnection Connection => _inner.Connection;

            public IDataReader ExecuteReader(string commandText, Action<ICommandConfigurer> configure, params object[] parameters)
            {
                return Do(() => _inner.ExecuteReader(commandText, parameters), commandText);
            }

            public object ExecuteScalar(string commandText, Action<ICommandConfigurer> configure, params object[] parameters)
            {
                return Do(() => _inner.ExecuteScalar(commandText, parameters), commandText);
            }

            public int ExecuteNonQuery(string commandText, Action<ICommandConfigurer> configure, params object[] parameters)
            {
                return Do(() => _inner.ExecuteNonQuery(commandText, parameters), commandText);
            }

            public IDataReader ExecuteReader(string commandText, params object[] parameters)
            {
                return Do(() => _inner.ExecuteReader(commandText, parameters), commandText);
            }

            public object ExecuteScalar(string commandText, params object[] parameters)
            {
                return Do(() => _inner.ExecuteScalar(commandText, parameters), commandText);
            }

            public int ExecuteNonQuery(string commandText, params object[] parameters)
            {
                return Do(() => _inner.ExecuteNonQuery(commandText, parameters), commandText);
            }

            public T FirstOrDefault<T>(string commandText, params object[] parameters) where T : class, new()
            {
                return Do(() => _inner.FirstOrDefault<T>(commandText, parameters), commandText);
            }

            public T First<T>(string commandText, params object[] parameters) where T : class, new()
            {
                return Do(() => _inner.First<T>(commandText, parameters), commandText);
            }

            public T SingleOrDefault<T>(string commandText, params object[] parameters) where T : class, new()
            {
                return Do(() => _inner.SingleOrDefault<T>(commandText, parameters), commandText);
            }

            public T Single<T>(string commandText, params object[] parameters) where T : class, new()
            {
                return Do(() => _inner.Single<T>(commandText, parameters), commandText);
            }

            public IReadOnlyList<T> Fetch<T>(string commandText, params object[] parameters) where T : class, new()
            {
                return Do(() => _inner.Fetch<T>(commandText, parameters), commandText);
            }

            public IPagingHelper<T> Paged<T>(string commandText, string orderBy, params object[] parameters) where T : class, new()
            {
                return Do(() => _inner.Paged<T>(commandText, orderBy, parameters), commandText);
            }

            public T GetScalar<T>(string commandText, params object[] parameters)
            {
                return Do(() => _inner.GetScalar<T>(commandText, parameters), commandText);
            }

            public string GetString(string commandText, params object[] parameters)
            {
                return Do(() => _inner.GetString(commandText, parameters), commandText);
            }

            public long Insert(object source, string tableName, string primaryKeyPropertyName, string sequenceName)
            {
                return Do(() => _inner.Insert(source, tableName, primaryKeyPropertyName, sequenceName));
            }

            public void Insert(object source, string tableName)
            {
                Do(() => _inner.Insert(source, tableName));
            }

            public void Update(object source, string tableName, string primaryKeyPropertyName = "ID")
            {
                Do(() => _inner.Update(source, tableName, primaryKeyPropertyName));
            }

            public void Delete(long id, string tableName, string primaryKeyPropertyName = "ID")
            {
                Do(() => _inner.Delete(id, tableName, primaryKeyPropertyName));
            }

            public void Delete(string tableName, string where, params object[] parameters)
            {
                Do(() => _inner.Delete(tableName, where, parameters));
            }

            public void Dispose()
            {
                Do(() => _inner.Dispose(), Levels.DBG);
            }

            public void Commit()
            {
                Do(() => _inner.Commit(), Levels.INF);
            }

            public void Rollback()
            {
                Do(() => _inner.Rollback(), Levels.INF);
            }

            private T Do<T>(Func<T> action, string commandText = null, [CallerMemberName] string caller = "")
            {
                try
                {
                    var sw = Stopwatch.StartNew();

                    var result = action();

                    var elapsed = sw.Elapsed;

                    _dbCallCount++;

                    var level = elapsed < _warningThreshold && _dbCallCount < _dbCallWarningThreshold ? Levels.INF : Levels.WRN;

                    var commandInfo = string.IsNullOrWhiteSpace(commandText) ? "" : $"{Environment.NewLine}\t{commandText}";

                    _logger.Log(level, $"[Session {SessionId} {caller}] Elapsed: {sw.Elapsed} - Session Db Call Count: {_dbCallCount}{commandInfo}");

                    return result;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.ToString());
                    throw new ResourceException("A data access exception occurred", ex);
                }
            }

            private void Do(Action action, [CallerMemberName] string caller = "")
            {
                try
                {
                    var sw = Stopwatch.StartNew();

                    action();

                    var elapsed = sw.Elapsed;

                    _dbCallCount++;

                    var level = elapsed < _warningThreshold && _dbCallCount < _dbCallWarningThreshold ? Levels.INF : Levels.WRN;

                    _logger.Log(level, $"[Session {SessionId} {caller}] Elapsed: {sw.Elapsed} - Session Db Call Count: {_dbCallCount}");
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.ToString());
                    throw new ResourceException("A data access exception occurred", ex);
                }
            }

            private void Do(Action action, Levels level, [CallerMemberName] string caller = "")
            {
                try
                {
                    action();

                    var elapsed = _sessionStopwatch.Elapsed;

                    level = elapsed < _warningThreshold && _dbCallCount < _dbCallWarningThreshold ? level : Levels.WRN;

                    _logger.Log(level, $"[Session {SessionId} {caller}] Session Elapsed: {elapsed} - Session Db Call Count: {_dbCallCount}");
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.ToString());
                    throw new ResourceException("A data access exception occurred", ex);
                }
            }
        }
    }
}