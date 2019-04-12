using System;

namespace Basis.Logging
{
    public class ConsoleLogger : LoggerBase
    {
        public ConsoleLogger(Levels level = Levels.INF)
        {
            Level = level;
        }

        protected override void WriteLine(Levels level, string value)
        {
            var original = Console.ForegroundColor;

            switch (level)
            {
                case Levels.DBG:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    break;
                case Levels.INF:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case Levels.WRN:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case Levels.ERR:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
            }

            try
            {

                Console.WriteLine(value);
            }
            finally
            {
                Console.ForegroundColor = original;
            }
        }

        protected override Levels Level { get; }
    }
}