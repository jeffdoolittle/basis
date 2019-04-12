namespace Basis.Logging
{
    public class DebugLogger : LoggerBase
    {
        protected override void WriteLine(Levels level, string value)
        {
            System.Diagnostics.Debug.WriteLine(value);
        }

        protected override Levels Level => Levels.DBG;
    }
}
