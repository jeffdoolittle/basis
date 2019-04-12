using System;

namespace Basis.Logging
{
    public class CompositeLogger : ILogger
    {
        private readonly ILogger[] _loggers;

        public CompositeLogger(params ILogger[] loggers)
        {
            _loggers = loggers;
        }

        public void Debug(string value)
        {
            ForEach(logger => logger.Debug(value));
        }

        public void Info(string value)
        {
            ForEach(logger => logger.Info(value));
        }

        public void Warn(string value)
        {
            ForEach(logger => logger.Warn(value));
        }

        public void Error(string value, Exception ex = null)
        {
            ForEach(logger => logger.Error(value, ex));
        }

        public void Error(Exception ex)
        {
            ForEach(logger => logger.Error(ex));
        }

        public void Log(Levels level, string value)
        {
            ForEach(logger => logger.Log(level, value));
        }

        private void ForEach(Action<ILogger> action)
        {
            foreach (var logger in _loggers)
            {
                action(logger);
            }
        }
    }
}