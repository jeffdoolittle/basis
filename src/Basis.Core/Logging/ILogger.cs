using System;

namespace Basis.Logging
{
    public interface ILogger
    {
        void Debug(string value);
        void Info(string value);
        void Warn(string value);
        void Error(string value, Exception ex = null);
        void Error(Exception ex);

        void Log(Levels level, string value);
    }
}