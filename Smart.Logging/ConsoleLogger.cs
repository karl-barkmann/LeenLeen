using System;
using System.Threading;

namespace Smart.Logging
{
    /// <summary>
    /// 控制台日志记录接口。
    /// </summary>
    public class ConsoleLogger : ILogger
    {
        /// <summary>
        /// 记录日志消息。
        /// </summary>
        /// <param name="message">格式化日志消息。</param>
        /// <param name="args">格式化参数</param>
        public void WriteMessage(string message, params object[] args)
        {
            string formattedMessage = String.Format("{0}[{1}]     {2}:{3}",
                Thread.CurrentThread.Name, Thread.CurrentThread.ManagedThreadId,
                DateTime.Now.ToString(), String.Format(message, args));
            Console.WriteLine(formattedMessage);
        }
    }
}
