using Leen.Logging;
using System;

namespace Leen.Common
{
    /// <summary>
    /// 客户端日志记录接口扩展类。
    /// </summary>
    public static class LoggerExtensions
    {
        /// <summary>
        /// 使用格式化字符串记录日志。
        /// </summary>
        /// <param name="logger">客户端日志记录接口。</param>
        /// <param name="format">格式化的日志字符串。</param>
        /// <param name="level">日志级别。</param>
        /// <param name="priority">日志优先级。</param>
        /// <param name="parameters">日志字符串格式化参数。</param>
        public static void Log(this ILogger logger, string format, LogLevel level, LogPriority priority, params object[] parameters)
        {
            if (logger == null)
                throw new ArgumentNullException("logger");
            logger.Log(string.Format(format, parameters), level, priority);
        }

        /// <summary>
        /// 记录异常日志信息。
        /// </summary>
        /// <param name="logger">客户端日志记录接口。</param>
        /// <param name="message">描述发生异常时的业务流程或操作信息。</param>
        /// <param name="exception">异常信息。</param>
        /// <param name="level">日志级别。</param>
        /// <param name="priority">日志优先级。</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public static void Log(
                            this ILogger logger,
                            string message,
                            Exception exception,
                            LogLevel level = LogLevel.Error,
                            LogPriority priority = LogPriority.Medium)
        {
            if (logger == null)
                throw new ArgumentNullException("logger");
            logger.Log(exception.BuildExceptionLog(message), level, priority);
        }
    }
}
