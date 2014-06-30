using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xunmei.Smart.Logging
{
    /// <summary>
    /// NLog日志记录接口。
    /// </summary>
    public class NLogger : ILogger, ILogWriter
    {
        private readonly Logger logger = LogManager.GetLogger("default");

        /// <summary>
        /// 构造默认的NLog日志记录接口。
        /// </summary>
        public NLogger()
        {

        }

        /// <summary>
        /// 构造NLog日志记录接口的实例。
        /// <param name="logger">NLog日志记录基类。</param>
        /// </summary>
        public NLogger(Logger logger)
        {
            this.logger = logger;
        }

        #region ILogger 成员

        /// <summary>
        /// 记录日志消息。
        /// </summary>
        /// <param name="message">格式化日志消息。</param>
        /// <param name="args">格式化参数</param>
        public void WriteMessage(string message, params object[] args)
        {
            logger.Info(message, args);
        }

        #endregion

        #region ILogWriter 成员

        /// <summary>
        /// 记录调试信息。
        /// </summary>
        /// <param name="message">信息内容。</param>
        public void Debug(string message)
        {
            logger.Debug(message);
        }

        /// <summary>
        /// 记录调试信息。
        /// </summary>
        /// <param name="message">信息内容，包含格式化信息。</param>
        /// <param name="args">格式化参数。</param>
        public void Debug(string message, params object[] args)
        {
            logger.Debug(message, args);
        }

        /// <summary>
        /// 记录跟踪调试信息。
        /// </summary>
        /// <param name="message">信息内容。</param>
        public void Trace(string message)
        {
            logger.Trace(message);
        }

        /// <summary>
        /// 记录跟踪调试信息。
        /// </summary>
        /// <param name="message">信息内容，包含格式化信息。</param>
        /// <param name="args">格式化参数。</param>
        public void Trace(string message, params object[] args)
        {
            logger.Trace(message, args);
        }

        /// <summary>
        /// 记录常规信息。
        /// </summary>
        /// <param name="message">信息内容。</param>
        public void Info(string message)
        {
            logger.Info(message);
        }

        /// <summary>
        /// 记录常规信息。
        /// </summary>
        /// <param name="message">信息内容，包含格式化信息。</param>
        /// <param name="args">格式化参数。</param>
        public void Info(string message, params object[] args)
        {
            logger.Info(message, args);
        }

        /// <summary>
        /// 记录警告信息。
        /// </summary>
        /// <param name="message">信息内容。</param>
        public void Warn(string message)
        {
            logger.Warn(message);
        }

        /// <summary>
        /// 记录警告信息。
        /// </summary>
        /// <param name="message">信息内容，包含格式化信息。</param>
        /// <param name="args">格式化参数。</param>
        public void Warn(string message, params object[] args)
        {
            logger.Warn(message, args);
        }

        /// <summary>
        /// 记录错误信息。
        /// </summary>
        /// <param name="message">信息内容。</param>
        public void Error(string message)
        {
            logger.Error(message);
        }

        /// <summary>
        /// 记录错误信息。
        /// </summary>
        /// <param name="message">信息内容，包含格式化信息。</param>
        /// <param name="args">格式化参数。</param>
        public void Error(string message, params object[] args)
        {
            logger.Error(message, args);
        }

        /// <summary>
        /// 记录致命错误信息。
        /// </summary>
        /// <param name="message">信息内容。</param>
        public void Fatal(string message)
        {
            logger.Fatal(message);
        }

        /// <summary>
        /// 记录致命错误信息。
        /// </summary>
        /// <param name="message">信息内容，包含格式化信息。</param>
        /// <param name="args">格式化参数。</param>
        public void Fatal(string message, params object[] args)
        {
            logger.Fatal(message, args);
        }

        /// <summary>
        /// 记录调试异常。
        /// </summary>
        /// <param name="message">信息内容。</param>
        /// <param name="exception">需要记录的异常。</param>
        public void DebugException(string message, Exception exception)
        {
            logger.DebugException(message, exception);
        }

        /// <summary>
        /// 记录跟踪调试异常。
        /// </summary>
        /// <param name="message">信息内容。</param>
        /// <param name="exception">需要记录的异常。</param>
        public void TraceException(string message, Exception exception)
        {
            logger.TraceException(message, exception);
        }

        /// <summary>
        /// 记录常规异常。
        /// </summary>
        /// <param name="message">信息内容。</param>
        /// <param name="exception">需要记录的异常。</param>
        public void InfoException(string message, Exception exception)
        {
            logger.InfoException(message, exception);
        }

        /// <summary>
        /// 记录警告异常。
        /// </summary>
        /// <param name="message">信息内容。</param>
        /// <param name="exception">需要记录的异常。</param>
        public void WarnException(string message, Exception exception)
        {
            logger.WarnException(message, exception);
        }

        /// <summary>
        /// 记录错误异常。
        /// </summary>
        /// <param name="message">信息内容。</param>
        /// <param name="exception">需要记录的异常。</param>
        public void ErrorException(string message, Exception exception)
        {
            logger.ErrorException(message, exception);
        }

        /// <summary>
        /// 记录致命错误异常。
        /// </summary>
        /// <param name="message">信息内容。</param>
        /// <param name="exception">需要记录的异常。</param>
        public void FatalException(string message, Exception exception)
        {
            logger.FatalException(message, exception);
        }

        #endregion
    }
}
