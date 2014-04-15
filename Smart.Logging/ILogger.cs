
namespace Smart.Logging
{
    /// <summary>
    /// 日志记录接口。
    /// </summary>
    public interface ILogger  
    {
        /// <summary>
        /// 记录日志消息。
        /// </summary>
        /// <param name="message">格式化日志消息。</param>
        /// <param name="args">格式化参数</param>
        void WriteMessage(string message, params object[] args);
    }

    /// <summary>
    /// 默认日志记录接口。
    /// </summary>
    public class NullLogger : ILogger
    {
        /// <summary>
        /// 记录日志消息。
        /// </summary>
        /// <param name="message">格式化日志消息。</param>
        /// <param name="args">格式化参数</param>
        public void WriteMessage(string message, params object[] args)
        {
            
        }
    }
}
