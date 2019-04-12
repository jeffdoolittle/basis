using System;
using Basis.Logging;
using FakeItEasy;
using FluentAssertions;
using Xunit;

namespace Basis.Tests.Logging
{
    public class CompositeLoggerTests
    {
        [Fact]
        public void exception_creating_composite_logger_with_null_dependencies()
        {
            Func<ILogger> func = () => new CompositeLogger(null);

            func.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void exception_creating_composite_logger_with_empty_dependencies()
        {
            Func<ILogger> func = () => new CompositeLogger();

            func.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void all_loggers_are_called_when_logging()
        {
            var logger1 = A.Fake<ILogger>();
            var logger2 = A.Fake<ILogger>();

            var logger = new CompositeLogger(logger1, logger2, new DebugLogger(), new ConsoleLogger(Levels.DBG));

            logger.Log(Levels.DBG, "test");

            A.CallTo(() => logger1.Log(Levels.DBG, "test")).MustHaveHappenedOnceExactly();
            A.CallTo(() => logger2.Log(Levels.DBG, "test")).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void all_loggers_are_called_when_logging_by_level()
        {
            var logger1 = A.Fake<ILogger>();
            var logger2 = A.Fake<ILogger>();

            var logger = new CompositeLogger(logger1, logger2, new DebugLogger(), new ConsoleLogger(Levels.DBG));

            logger.Debug("test");
            logger.Info("test");
            logger.Warn("test");
            logger.Error("test");
            logger.Error(new Exception("test"));

            VerifyExpectations(logger1);
            VerifyExpectations(logger2);
        }

        private void VerifyExpectations(ILogger fakeLogger)
        {
            A.CallTo(() => fakeLogger.Debug("test")).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLogger.Info("test")).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLogger.Warn("test")).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLogger.Error("test", null)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLogger.Error(A<Exception>._)).MustHaveHappenedOnceExactly();
        }
    }
}