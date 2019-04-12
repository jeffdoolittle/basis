using System;

namespace Basis.Logging
{
    public class ConsoleLogger : LoggerBase
    {
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
    }
}