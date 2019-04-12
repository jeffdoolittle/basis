using System;
using System.IO;
using Basis.Logging;
using Basis.Resource;
using FluentAssertions;
using Xunit;

namespace Basis.Db.Tests
{
    public class SqliteSessionTests : IDisposable
    {
        private readonly string _dbFile;
        private readonly TestLogger _logger;
        private readonly ISessionFactory _sessionFactory;

        public SqliteSessionTests()
        {
            _dbFile = $"./{Guid.NewGuid().ToString()}.db";

            _logger = new TestLogger();

            _sessionFactory = new SessionFactory(cfg =>
            {
                cfg.UseConnectionString($"Filename={_dbFile}");
                cfg.SetElapsedTimeWarningThreshold(TimeSpan.FromSeconds(5));
                cfg.SetTotalDbCallsPerSessionWarningThreshold(10);
                cfg.WithProviderType(DbProviderTypes.SqLite);
                cfg.UseLogger(_logger);
            });
        }

        [Fact]
        public void exception_when_query_exceeds_timeout()
        {

        }

        [Fact]
        public void can_create_table_and_insert_with_sequence()
        {
            using (var session = _sessionFactory.OpenSession())
            {
                session.ExecuteNonQuery("create table if not exists foo_bar (id integer primary key autoincrement, value text not null)");

                session.Insert(new FooBar{ Value = "froboz"}, "foo_bar", "Id", "seq_foo_bar");
                session.Insert(new FooBar{ Value = "frabaz"}, "foo_bar", "Id", "seq_foo_bar");

                session.Commit();
            }

            using (var session = _sessionFactory.OpenSession())
            {
                var items = session.Fetch<FooBar>("select * from foo_bar");

                items.Count.Should().Be(2);

                items[0].Id.Should().Be(1);
                items[1].Id.Should().Be(2);
            }

            _logger.Logs.Should().HaveCountGreaterThan(0);
        }

        private class FooBar
        {
            public long Id { get; set; }
            public string Value { get; set; }
        }

        public void Dispose()
        {
            File.Delete(_dbFile);
        }
    }
}
