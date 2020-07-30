using System;
using System.Windows;

namespace Leen.Media.Controls.Primitives
{
    /// <summary>
    /// 为 <see cref="MediaRenderElement.MediaFailed"/> 事件提供事件参数。
    /// </summary>
    public class MediaFailedRoutedEventArgs : RoutedEventArgs
    {
        private Exception _errorException;
        private int _errorCode;

        /// <summary>
        /// 构造 <see cref="MediaFailedRoutedEventArgs"/> 类的实例。
        /// </summary>
        /// <param name="routedEvent"></param>
        /// <param name="sender"></param>
        /// <param name="errorException">The exception that describes the media failure.</param>
        public MediaFailedRoutedEventArgs(
            RoutedEvent routedEvent,
            object sender,
            Exception errorException) : base(routedEvent, sender)
        {
            _errorException = errorException ?? throw new ArgumentNullException(nameof(errorException));
        }

        /// <summary>
        /// 构造 <see cref="MediaFailedRoutedEventArgs"/> 类的实例。
        /// </summary>
        /// <param name="routedEvent"></param>
        /// <param name="sender"></param>
        /// <param name="errorCode"> The error code that describes the media failure.</param>
        public MediaFailedRoutedEventArgs(
            RoutedEvent routedEvent,
            object sender,
            int errorCode) : base(routedEvent, sender)
        {
            _errorCode = errorCode;
        }

        /// <summary>
        /// The error code that describes the media failure.
        /// </summary>
        public int ErrorCode
        {
            get
            {
                return _errorCode;
            }
        }

        /// <summary>
        /// The exception that describes the media failure.
        /// </summary>
        public Exception ErrorException
        {
            get
            {
                return _errorException;
            }
        }
    }
}
