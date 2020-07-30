using System;

namespace Leen.Media
{
    /// <summary>
    /// 为 <see cref="IMediaPlayer.MediaFailed"/> 事件提供事件参数。
    /// </summary>
    public class MediaFailedEventArgs : EventArgs
    {
        private readonly Exception m_ErrorException;

        /// <summary>
        /// 构造 <see cref="MediaFailedEventArgs"/> 类的实例。
        /// </summary>
        /// <param name="error">详细描述失败原因的异常。</param>
        public MediaFailedEventArgs(Exception error) : base()
        {
            m_ErrorException = error;
        }

        /// <summary>
        ///  获取详细描述失败原因的异常。
        /// </summary>
        /// <value>用于详细描述错误情况的异常。</value>
        public Exception ErrorException
        {
            get
            {
                return m_ErrorException;
            }
        }
    }
}
