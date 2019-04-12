using System;
using System.Diagnostics;
using System.Threading;

namespace Basis.Logging
{
    public abstract class LoggerBase : ILogger
    {
        public void Debug(string value)
        {
            Log(Levels.DBG, value);
        }

        public void Info(string value)
        {
            Log(Levels.INF, value);
        }

        public void Warn(string value)
        {
            Log(Levels.WRN, value);
        }

        public void Error(string value, Exception ex = null)
        {
            var nl = Environment.NewLine;
            var msg = ex == null ? value : $"{value}{nl}{ex}";
            Log(Levels.ERR, msg);
        }

        public void Error(Exception ex)
        {
            Log(Levels.ERR, ex?.ToString());
        }

        public void Log(Levels level, string value)
        {
            WriteLog(level, value);
        }

        private void WriteLog(Levels level, string value)
        {
            Guard.NotNull(value, nameof(value));

            if (level < Level)
            {
                return;
            }

            var threadId = Thread.CurrentThread.ManagedThreadId;
            var processId = Process.GetCurrentProcess().Id;
            var userName = Thread.CurrentPrincipal?.Identity?.Name ?? "system";
            WriteLine(level, $"[{level}] - {DateTimeOffset.Now:O} - (thread: {threadId}, process: {processId}, user: {userName}) {value}");
        }

        protected abstract void WriteLine(Levels level, string value);

        protected abstract Levels Level { get; }
    }
}