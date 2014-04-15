using System;

namespace Smart.Logging
{
    /// <summary>
    /// 日志记录接口，提供更多的记录方法。
    /// </summary>
    public interface ILogWriter
    {
        /// <summary>
        /// 记录调试日志。
        /// </summary>
        /// <param name="message">日志信息。</param>
        void Debug(string message);

        /// <summary>
        /// 记录调试日志。
        /// </summary>
        /// <param name="message">日志格式化信息。</param>
        /// <param name="args">格式化参数。</param>
        void Debug(string message, params object[] args);

        /// <summary>
        /// 记录调试异常日志。
        /// </summary>
        /// <param name="message">日志信息。</param>
        /// <param name="exception">异常对象。</param>
        void DebugException(string message, Exception exception);

        /// <summary>
        /// 记录错误日志。
        /// </summary>
        /// <param name="message">日志信息。</param>
        void Error(string message);

        /// <summary>
        /// 记录错误日志。
        /// </summary>
        /// <param name="message">日志格式化信息。</param>
        /// <param name="args">格式化参数。</param>
        void Error(string message, params object[] args);

        /// <summary>
        /// 记录错误异常日志。
        /// </summary>
        /// <param name="message">日志信息。</param>
        /// <param name="exception">异常对象。</param>
        void ErrorException(string message, Exception exception);

        /// <summary>
        /// 记录严重错误日志。
        /// </summary>
        /// <param name="message">日志信息。</param>
        void Fatal(string message);

        /// <summary>
        /// 记录严重错误日志。
        /// </summary>
        /// <param name="message">日志格式化信息。</param>
        /// <param name="args">格式化参数。</param>
        void Fatal(string message, params object[] args);

        /// <summary>
        /// 记录严重错误异常日志。
        /// </summary>
        /// <param name="message">日志信息。</param>
        /// <param name="exception">异常对象。</param>
        void FatalException(string message, Exception exception);

        /// <summary>
        /// 记录一般日志。
        /// </summary>
        /// <param name="message">日志信息。</param>
        void Info(string message);

        /// <summary>
        /// 记录一般日志。
        /// </summary>
        /// <param name="message">日志格式化信息。</param>
        /// <param name="args">格式化参数。</param>
        void Info(string message, params object[] args);

        /// <summary>
        /// 记录一般异常日志。
        /// </summary>
        /// <param name="message">日志信息。</param>
        /// <param name="exception">异常对象。</param>
        void InfoException(string message, Exception exception);

        /// <summary>
        /// 记录跟踪日志。
        /// </summary>
        /// <param name="message">日志信息。</param>
        void Trace(string message);

        /// <summary>
        /// 记录跟踪日志。
        /// </summary>
        /// <param name="message">日志信息。</param>
        /// <param name="args">格式化参数。</param>
        void Trace(string message, params object[] args);

        /// <summary>
        /// 记录跟踪异常日志。
        /// </summary>
        /// <param name="message">日志格式化信息。</param>
        /// <param name="exception">异常对象。</param>
        void TraceException(string message, Exception exception);

        /// <summary>
        /// 记录警告日志。
        /// </summary>
        /// <param name="message">日志信息。</param>
        void Warn(string message);

        /// <summary>
        /// 记录警告日志。
        /// </summary>
        /// <param name="message">日志格式化信息。</param>
        /// <param name="args">异常对象。</param>
        void Warn(string message, params object[] args);

        /// <summary>
        /// 记录警告异常日志。
        /// </summary>
        /// <param name="message">日志信息。</param>
        /// <param name="exception">异常对象。</param>
        void WarnException(string message, Exception exception);
    }

    /// <summary>
    /// 默认日志记录。
    /// </summary>
    public class NullLogWriter : ILogWriter
    {
        /// <summary>
        /// 记录调试日志。
        /// </summary>
        /// <param name="message">日志信息。</param>
        public void Debug(string message)
        {
        }

        /// <summary>
        /// 记录调试日志。
        /// </summary>
        /// <param name="message">日志格式化信息。</param>
        /// <param name="args">格式化参数。</param>
        public void Debug(string message, params object[] args)
        {
        }

        /// <summary>
        /// 记录调试异常日志。
        /// </summary>
        /// <param name="message">日志信息。</param>
        /// <param name="exception">异常对象。</param>
        public void DebugException(string message, Exception exception)
        {
        }

        /// <summary>
        /// 记录错误日志。
        /// </summary>
        /// <param name="message">日志信息。</param>
        public void Error(string message)
        {
        }

        /// <summary>
        /// 记录错误日志。
        /// </summary>
        /// <param name="message">日志格式化信息。</param>
        /// <param name="args">格式化参数。</param>
        public void Error(string message, params object[] args)
        {
        }

        /// <summary>
        /// 记录错误异常日志。
        /// </summary>
        /// <param name="message">日志信息。</param>
        /// <param name="exception">异常对象。</param>
        public void ErrorException(string message, Exception exception)
        {
        }

        /// <summary>
        /// 记录严重错误日志。
        /// </summary>
        /// <param name="message">日志信息。</param>
        public void Fatal(string message)
        {
        }

        /// <summary>
        /// 记录严重错误日志。
        /// </summary>
        /// <param name="message">日志格式化信息。</param>
        /// <param name="args">格式化参数。</param>
        public void Fatal(string message, params object[] args)
        {
        }

        /// <summary>
        /// 记录严重错误异常日志。
        /// </summary>
        /// <param name="message">日志信息。</param>
        /// <param name="exception">异常对象。</param>
        public void FatalException(string message, Exception exception)
        {
        }

        /// <summary>
        /// 记录一般日志。
        /// </summary>
        /// <param name="message">日志信息。</param>
        public void Info(string message)
        {
        }

        /// <summary>
        /// 记录一般日志。
        /// </summary>
        /// <param name="message">日志格式化信息。</param>
        /// <param name="args">格式化参数。</param>
        public void Info(string message, params object[] args)
        {
        }

        /// <summary>
        /// 记录一般异常日志。
        /// </summary>
        /// <param name="message">日志信息。</param>
        /// <param name="exception">异常对象。</param>
        public void InfoException(string message, Exception exception)
        {
        }

        /// <summary>
        /// 记录跟踪日志。
        /// </summary>
        /// <param name="message">日志信息。</param>
        public void Trace(string message)
        {
        }

        /// <summary>
        /// 记录跟踪日志。
        /// </summary>
        /// <param name="message">日志信息。</param>
        /// <param name="args">格式化参数。</param>
        public void Trace(string message, params object[] args)
        {
        }

        /// <summary>
        /// 记录跟踪异常日志。
        /// </summary>
        /// <param name="message">日志格式化信息。</param>
        /// <param name="exception">异常对象。</param>
        public void TraceException(string message, Exception exception)
        {
        }

        /// <summary>
        /// 记录警告日志。
        /// </summary>
        /// <param name="message">日志信息。</param>
        public void Warn(string message)
        {
        }

        /// <summary>
        /// 记录警告日志。
        /// </summary>
        /// <param name="message">日志格式化信息。</param>
        /// <param name="args">异常对象。</param>
        public void Warn(string message, params object[] args)
        {
        }

        /// <summary>
        /// 记录警告异常日志。
        /// </summary>
        /// <param name="message">日志信息。</param>
        /// <param name="exception">异常对象。</param>
        public void WarnException(string message, Exception exception)
        {
        }
    }
}

