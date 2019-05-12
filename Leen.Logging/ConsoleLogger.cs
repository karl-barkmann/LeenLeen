using System;
using System.Threading;

namespace Leen.Logging
{
    /// <summary>
    /// 控制台日志记录接口。
    /// </summary>
    public class ConsoleLogger : ILogger
    {
        private readonly object _logLocker = new object();
        private readonly LogLevel _lowestLevel;

        public ConsoleLogger() : this(LogLevel.Debug)
        {

        }

        public ConsoleLogger(LogLevel lowestLevel)
        {
            _lowestLevel = lowestLevel;
        }

        /// <summary>
        /// 记录日志消息。
        /// </summary>
        /// <param name="message">日志消息内容。</param>
        /// <param name="category">描述日志内容的级别。</param>
        /// <param name="priority">描述日志内容的优先级。</param>
        public void Log(string message, LogLevel level, LogPriority priority)
        {
            if (level < _lowestLevel)
                return;

            lock (_logLocker)
            {
                switch (level)
                {
                    case LogLevel.Debug:
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;
                    case LogLevel.Info:
                        Console.ForegroundColor = ConsoleColor.White;
                        break;
                    case LogLevel.Warning:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;
                    case LogLevel.Error:
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;
                    case LogLevel.Fatal:
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        break;
                    case LogLevel.None:
                    default:
                        Console.ForegroundColor = ConsoleColor.Blue;
                        break;
                }
                try
                {
                    var currentThread = Thread.CurrentThread;
                    var threadId = currentThread.ManagedThreadId;
                    var thread = $"{threadId}";
                    var time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss,fff");
                    Console.WriteLine($"{time}[{thread.PadRight(3, ' ')}] " +
                        $"{level.ToString().PadRight(10, ' ')}-{priority.ToString().PadRight(10, ' ')}: {message}");
                }
                finally
                {
                    Console.ResetColor();
                }
            }
        }
    }
}
