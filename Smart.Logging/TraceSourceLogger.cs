using System;
using System.Diagnostics;

namespace Smart.Logging
{
    /// <summary>
    /// TraceSource日志记录接口。
    /// </summary>
    public class TraceSourceLogger : ILogger, IDisposable
    {
        private TraceSource traceSource;

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
            if (traceSource == null)
                throw new ArgumentNullException("traceSource");

            this.traceSource = traceSource;
        }

        /// <summary>
        /// 记录日志。
        /// </summary>
        /// <param name="message">日志消息。</param>
        /// <param name="args">格式化参数。</param>
        public void WriteMessage(string message, params object[] args)
        {
            this.traceSource.TraceInformation(message, args);
        }

        /// <summary>
        /// 释放资源。
        /// </summary>
        public void Dispose()
        {
            if (this.traceSource != null)
            {
                this.traceSource.Flush();
                this.traceSource.Close();
                this.traceSource = null;
            }
        }
    }
}
