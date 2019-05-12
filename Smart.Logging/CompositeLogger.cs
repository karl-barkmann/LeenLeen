using System.Collections.Generic;

namespace Leen.Logging
{
    /// <summary>
    /// 实现一个输出日志到现有日志接口集合的组合日志接口。
    /// </summary>
    public class CompositeLogger : ILogger
    {
        private readonly List<ILogger> _loggers = new List<ILogger>();

        /// <summary>
        /// 构造 <see cref="CompositeLogger"/> 类的实例。
        /// </summary>
        /// <param name="loggers">现有的日志接口集合。</param>
        public CompositeLogger(ILogger[] loggers)
        {
            _loggers = new List<ILogger>(loggers);
        }

        /// <summary>
        /// 记录日志到现有日志接口集合中的各日志接口。
        /// </summary>
        /// <param name="message">日志消息内容。</param>
        /// <param name="category">描述日志内容的级别。</param>
        /// <param name="priority">描述日志内容的优先级。</param>
        public void Log(string message, LogLevel category, LogPriority priority)
        {
            foreach (var logger in _loggers)
            {
                logger.Log(message, category, priority);
            }
        }
    }
}
