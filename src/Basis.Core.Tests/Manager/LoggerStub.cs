using System.Collections.Generic;
using Basis.Logging;

namespace Basis.Tests.Manager
{
    public class LoggerStub : LoggerBase
    {
        protected override void WriteLine(Levels level, string value)
        {
            Logs.Add(value);
        }

        protected override Levels Level { get; } = Levels.DBG;

        public List<string> Logs { get; } = new List<string>();
    }
}