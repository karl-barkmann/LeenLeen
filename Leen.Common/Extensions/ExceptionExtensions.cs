using System;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Leen.Common
{
    /// <summary>
    /// 异常扩展类。
    /// </summary>
    public static class ExceptionExtensions
    {
        private static readonly PropertyInfo _propertyRemoteStackTrace;

        static ExceptionExtensions()
        {
            _propertyRemoteStackTrace = typeof(Exception).GetProperty(
                "RemoteStackTrace",
                BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.NonPublic);
        }

        /// <summary>
        /// 判断异常是否为必须继续抛出的异常。
        /// <para>
        /// 通常指一些无法处理的异常，例如内存空间不足、堆栈溢出、内存地址访问冲突等。
        /// </para>
        /// </summary>
        /// <param name="exception">异常信息。</param>
        /// <returns></returns>
        public static bool MustBeThrown(this Exception exception)
        {
            if (exception is OutOfMemoryException || exception is StackOverflowException || exception is AccessViolationException)
                return true;
            return false;
        }

        /// <summary>
        /// 构造完整的格式化的未处理异常信息日志。
        /// </summary>
        /// <param name="exception">异常信息。</param>
        /// <param name="recursiveInnerException">是否递归输出内部异常。</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public static string BuildExceptionLog(this Exception exception, bool recursiveInnerException = true)
        {
            if (exception == null)
                throw new ArgumentNullException("exception");

            return exception.BuildExceptionLog("发生未处理异常，以下是异常详细信息：", recursiveInnerException);
        }

        /// <summary>
        /// 构造完整的格式化的异常信息日志。
        /// </summary>
        /// <param name="exception">异常信息。</param>
        /// <param name="logSummaryHeader">表示异常日志的汇总标题。</param>
        /// <param name="recursiveInnerException">是否递归输出内部异常。</param>
        /// <returns></returns>
        public static string BuildExceptionLog(this Exception exception, string logSummaryHeader, bool recursiveInnerException = true)
        {
            if (exception == null)
                throw new ArgumentNullException("exception");

            StringBuilder logBuilder = new StringBuilder(logSummaryHeader);

            logBuilder.AppendLine();
            logBuilder.AppendLine("************** Exception Text **************");

            if (exception.InnerException != null && recursiveInnerException)
            {
                int indent = 0;
                BuildExceptionLogRecursive(exception.InnerException, logBuilder, indent + 1);
            }

            logBuilder.AppendLine();
            logBuilder.Append("************** Outer Exception **************");
            logBuilder.AppendLine();

            BuildExceptionLog(exception, logBuilder);

            logBuilder.AppendLine();

            return logBuilder.ToString();
        }

        /// <summary>
        /// 如有远程异常堆栈，返回包含的远程异常堆栈。
        /// <para>对 Remoting / WCF 等 IPC 调用时发生的异常有效。</para>
        /// </summary>
        /// <param name="exception">异常信息。</param>
        /// <returns></returns>
        public static string GetRemoteStackTrace(this Exception exception)
        {
            if (exception == null)
                throw new ArgumentNullException("exception");
            exception.PreserveStackTrace();
            return (string)_propertyRemoteStackTrace.GetValue(exception, null);
        }

        /// <summary>
        /// 保留异常原始堆栈信息，此方法会帮助我们保留异常原始堆栈信息，即最开始引发异常的地方（行数、方法名等）。高于4.0版本此方法无效，请使用 ExceptionDispatchInfo。
        /// <para>
        /// 我们在处理异常时常在try-catch语句块内采用 throw 关键字抛出异常，通常这会保留异常堆栈信息。
        /// 但这有例外情况：有时我们会需要在try-catch语句块之外抛出异常;原始异常的引发发生在当前堆栈帧上（可以理解为异常时由当前方法内的代码，而不是其他方法内的代码引发）。
        /// 不管是以上哪种例外情况, 如果选择采用 throw exception / throw 的方式进行异常抛出，则此时我们应在try-catch语句块内将捕获到的异常进行堆栈信息保留；
        /// 如果选择构造新的异常并指定内部异常为捕获的异常进行异常抛出，则调用方法是不必要的。
        /// </para>
        /// <para>
        /// 我们建议避免不必要的捕获重抛出行为，若一定需要则应调用此方法进行堆栈信息保留再抛出。
        /// 并且我们认为构造新的异常并指定内部异常为捕获的异常，是最佳的异常处理方式。
        /// </para>
        /// </summary>
        /// <param name="exception">需要原始堆栈信息的异常对象。</param>
        public static void PreserveStackTrace(this Exception exception)
        {
#if NETFX_40
            _methodInternalPreserveStackTrace.Invoke(exception, null);
#elif NETFX_35
            _methodInternalPreserveStackTrace.Invoke(exception, null);
            
#elif NETFX_30
            _methodInternalPreserveStackTrace.Invoke(exception, null);

#elif NETFX_20
            _methodInternalPreserveStackTrace.Invoke(exception, null);
#endif
        }

        #region private helpers

        private static void BuildExceptionLogRecursive(Exception exception, StringBuilder logBuilder, int indent)
        {
            if (exception.InnerException != null)
            {
                BuildExceptionLogRecursive(exception.InnerException, logBuilder, indent + 1);
            }
            logBuilder.AppendLine();
            logBuilder.AppendFormat("************** {0}Exception **************",
                string.Concat(Enumerable.Repeat("Inner ", indent)));
            logBuilder.AppendLine();

            BuildExceptionLog(exception, logBuilder);
        }

        private static void BuildExceptionLog(Exception exception, StringBuilder logBuilder)
        {
            logBuilder.AppendFormat("异常类型:{0}", exception.GetType().FullName);
            logBuilder.AppendLine();
            logBuilder.AppendFormat("异常消息:{0}", exception.Message);
            logBuilder.AppendLine();
            logBuilder.AppendFormat("异常来源:{0}", exception.Source);
            logBuilder.AppendLine();
            logBuilder.Append(exception.TargetSite);
            logBuilder.AppendLine();
            logBuilder.Append(exception.StackTrace);
            logBuilder.AppendLine();
        }

        #endregion
    }
}
