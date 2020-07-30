using System;

namespace Leen.Media
{
    /// <summary>
    /// 为 <see cref="IIntelligentAnalysisVideoPlayer.TrackingRequested"/> 事件提供数据。
    /// </summary>
    public class SubjectTrackingRequestedEventArgs : EventArgs
    {
        /// <summary>
        /// 构造 <see cref="SubjectTrackingRequestedEventArgs"/> 类的实例。
        /// </summary>
        /// <param name="trackingPosition">正在请求跟踪的目标自视频开始经过的时间间隔。</param>
        /// <param name="trackingSubject">正在请求跟踪的目标数据。</param>
        public SubjectTrackingRequestedEventArgs(TimeSpan trackingPosition, object trackingSubject)
        {
            TrackingPosition = trackingPosition;
            TrackingSubject = trackingSubject;
        }

        /// <summary>
        /// 获取正在请求跟踪的目标自视频开始经过的时间间隔。
        /// </summary>
        public TimeSpan TrackingPosition { get; private set; }

        /// <summary>
        /// 获取正在请求跟踪的目标数据。
        /// </summary>
        public object TrackingSubject { get; private set; }
    }
}
