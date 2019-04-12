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
            var msg = $"{value}{nl}{ex}";
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
            if (level < Level)
            {
                return;
            }

            // var threadId = Thread.CurrentThread.ManagedThreadId;
            // var processId = Process.GetCurrentProcess().Id;
            // var userName = Thread.CurrentPrincipal?.Identity?.Name;
            // WriteLine(level, $"[{level}] (t: {threadId}, p: {processId}, u: {userName}) {value}");
            WriteLine(level, $"[{level}] {value}");
        }

        protected abstract void WriteLine(Levels level, string value);

        protected virtual Levels Level { get; } = Levels.INF;
    }
}