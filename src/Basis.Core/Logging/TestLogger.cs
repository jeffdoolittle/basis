using System.Collections.Generic;

namespace Basis.Logging
{
    public class TestLogger : LoggerBase
    {
        private readonly List<string> _logs = new List<string>();

        protected override void WriteLine(Levels level, string value)
        {
            _logs.Add(value);
        }

        protected override Levels Level => Levels.DBG;

        public IReadOnlyList<string> Logs => _logs;
    }
}