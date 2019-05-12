using System.Diagnostics;

namespace Leen.Logging
{
    /// <summary>
    /// 表示输出到调试器输出流的日志接口。
    /// </summary>
    public class DebugLogger : ILogger
    {
        /// <summary>
        /// 记录日志消息。
        /// </summary>
        /// <param name="message">日志消息内容。</param>
        /// <param name="category">描述日志内容的级别。</param>
        /// <param name="priority">描述日志内容的优先级。</param>
        public void Log(string message, LogLevel level, LogPriority priority)
        {
#if DEBUG
            Debug.WriteLine($"[{level.ToString().ToUpper()}] -[{priority.ToString().ToUpper()}]: {message}");
#else 
            Trace.WriteLine($"[{level.ToString().ToUpper()}] -[{priority.ToString().ToUpper()}]: {message}");
#endif
        }
    }
}
