using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using Basis.Logging;
using FluentAssertions;
using Xunit;

namespace Basis.Tests.Logging
{
    public class LoggerBaseTests
    {
        [Fact]
        public void exception_when_attempting_to_log_null_value_with_level_specific_methods()
        {
            var logger = new FakeLogger();

            Action dbg = () => logger.Debug(null);
            Action inf = () => logger.Info(null);
            Action wrn = () => logger.Warn(null);
            Action err = () => logger.Error(null);
            Action err2 = () => logger.Error(null, null);

            dbg.Should().Throw<ArgumentNullException>();
            inf.Should().Throw<ArgumentNullException>();
            wrn.Should().Throw<ArgumentNullException>();
            err.Should().Throw<ArgumentNullException>();
            err2.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [InlineData(Levels.DBG)]
        [InlineData(Levels.INF)]
        [InlineData(Levels.WRN)]
        [InlineData(Levels.ERR)]
        public void exception_when_attempting_to_log_null_value_with_general_log_method(Levels level)
        {
            var logger = new FakeLogger();

            Action log = () => logger.Log(level, null);

            log.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void logs_contain_thread_process_and_user_info()
        {
            var userName = Guid.NewGuid().ToString();
            Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity(userName), new string[0]);

            var logger = new FakeLogger();

            var msg = Guid.NewGuid().ToString();
            logger.Debug(msg);

            logger.Logs.Should().HaveCount(1);

            var log = logger.Logs.First();

            log.Should().Contain(Levels.DBG.ToString());
            log.Should().Contain($"user: {userName}");
            log.Should().Contain(msg);
        }

        [Fact]
        public void can_filter_logs_that_are_at_a_lower_than_configured_level()
        {
            var logger = new FakeLogger(Levels.INF);

            var shouldNotBeIncluded = Guid.NewGuid().ToString();
            var shouldBeIncluded = Guid.NewGuid().ToString();

            logger.Debug(shouldNotBeIncluded);
            logger.Info(shouldBeIncluded);

            logger.Logs.Count.Should().Be(1);
            logger.Logs.Should().NotContain(x => x.Contains(shouldNotBeIncluded));
            logger.Logs.Should().Contain(x => x.Contains(shouldBeIncluded));
        }

        public class FakeLogger : LoggerBase
        {
            private readonly List<string> _logs = new List<string>();

            public FakeLogger(Levels level = Levels.DBG)
            {
                Level = level;
            }

            protected override void WriteLine(Levels level, string value)
            {
                _logs.Add(value);
            }

            public IReadOnlyList<string> Logs => _logs;

            protected override Levels Level { get; }
        }
    }
}
