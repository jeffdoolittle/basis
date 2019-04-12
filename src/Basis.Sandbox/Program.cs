using Basis.Logging;

namespace Basis.Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = new CompositeLogger(new DebugLogger(), new ConsoleLogger());

            logger.Info("Hello World!");
        }
    }
}
