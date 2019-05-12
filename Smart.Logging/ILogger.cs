
namespace Leen.Logging
{
    /// <summary>
    /// 日志记录接口。
    /// </summary>
    public interface ILogger  
    {
        /// <summary>
        /// 记录日志消息。
        /// </summary>
        /// <param name="message">日志消息内容。</param>
        /// <param name="category">描述日志内容的级别。</param>
        /// <param name="priority">描述日志内容的优先级。</param>
        void Log(string message, LogLevel level, LogPriority priority);
    }

    /// <summary>
    /// 默认日志记录接口。
    /// </summary>
    public class EmptyLogger : ILogger
    {
        /// <summary>
        /// 记录日志消息。
        /// </summary>
        /// <param name="message">日志消息内容。</param>
        /// <param name="category">描述日志内容的级别。</param>
        /// <param name="priority">描述日志内容的优先级。</param>
        public void Log(string message, LogLevel level, LogPriority priority)
        {
            
        }
    }
}
