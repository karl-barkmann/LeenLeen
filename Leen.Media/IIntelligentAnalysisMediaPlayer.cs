using System;

namespace Leen.Media
{
    /// <summary>
    /// 表示 <see cref="IIntelligentAnalysisVideoPlayer"/> 正在请求目标定位跟踪（用户点击目标标记、点击目标跟踪指示器等）时引发的事件的处理程序。
    /// </summary>
    /// <param name="sender"><see cref="IMediaPlayer"/> 的实例。</param>
    /// <param name="e">表示事件参数。</param>
    public delegate void SubjectTrackingRequestedEventHandler(object sender, SubjectTrackingRequestedEventArgs e);

    /// <summary>
    /// 定义客户端产品中支持智能分析的通用媒体播放接口。
    /// </summary>
    public interface IIntelligentAnalysisVideoPlayer : ISeekableMediaPlayer
    {
        /// <summary>
        /// 当 <see cref="IIntelligentAnalysisVideoPlayer"/> 正在请求目标定位跟踪（用户点击目标标记、点击目标跟踪指示器等）时发生。
        /// </summary>
        event SubjectTrackingRequestedEventHandler TrackingRequested;

        /// <summary>
        /// 获取或设置一个值指示是否在播放中显示目标跟踪指示器。
        /// </summary>
        bool IsTrackerVisible { get; set; }

        /// <summary>
        /// 获取或设置一个值指示是否在播放中显示目标标记指示器。
        /// </summary>
        bool IsMarkerVisible { get; set; }

        /// <summary>
        /// 请求尝试追踪视频中的目标。
        /// </summary>
        /// <param name="x">假定目标位于视频画面中的平面坐标。</param>
        /// <param name="y">假定目标位于视频画面中的平面坐标。</param>
        void RequestTracking(double x, double y);

        /// <summary>
        /// 尝试追踪指定的目标。
        /// </summary>
        /// <param name="trackingSubject">追踪的目标对象。</param>
        void TrackingSubject(object trackingSubject);

        /// <summary>
        /// 获取当前播放位置通过视频智能分析得到的目标数据。
        /// </summary>
        /// <returns></returns>
        Subject[] GetRecentSubjects();

        /// <summary>
        /// 配置视频播放的目标摘要分析参数。
        /// </summary>
        /// <param name="subjectDensity">目标摘要生成时的目标密度。</param>
        /// <param name="subjectSummarySource">视频智能分析生成目标摘要数据源。</param>
        void ConfigSubjectSummary(int subjectDensity, object subjectSummarySource);
    }
}
