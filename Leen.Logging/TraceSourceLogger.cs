using System;
using System.Diagnostics;

namespace Leen.Logging
{
    /// <summary>
    /// TraceSource日志记录接口。
    /// </summary>
    public class TraceSourceLogger : ILogger, IDisposable
    {
        private TraceSource _traceSource;

        /// <summary>
        /// 构造TraceSource日志记录接口的实例。
        /// </summary>
        /// <param name="traceSourceName">TraceSource配置名。</param>
        public TraceSourceLogger(string traceSourceName)
            : this(new TraceSource(traceSourceName, SourceLevels.All))
        {

        }

        /// <summary>
        /// 构造TraceSource日志记录接口的实例。
        /// </summary>
        /// <param name="traceSource"></param>
        public TraceSourceLogger(TraceSource traceSource)
        {
            _traceSource = traceSource ?? throw new ArgumentNullException("traceSource");
        }

        /// <summary>
        /// 记录日志消息。
        /// </summary>
        /// <param name="message">日志消息内容。</param>
        /// <param name="category">描述日志内容的级别。</param>
        /// <param name="priority">描述日志内容的优先级。</param>
        public void Log(string message, LogLevel level, LogPriority priority)
        {
            _traceSource.TraceInformation(message);
        }

        /// <summary>
        /// 释放资源。
        /// </summary>
        public void Dispose()
        {
            if (_traceSource != null)
            {
                _traceSource.Flush();
                _traceSource.Close();
                _traceSource = null;
            }
        }
    }
}
