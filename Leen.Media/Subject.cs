using System;

namespace Leen.Media
{
    /// <summary>
    /// 定义客户端产品中视频智能分析过程中产生的目标对象。
    /// </summary>
    public struct Subject
    {
        /// <summary>
        /// 获取目标标识。
        /// </summary>
        public int Id;

        /// <summary>
        /// 获取目标坐标参数。
        /// </summary>
        public SubjectCordinate Cordinate;

        /// <summary>
        /// 获取目标类型。
        /// </summary>
        public SubjectType Type;

        /// <summary>
        /// 是否已经请求追踪该目标。
        /// </summary>
        public bool IsTrackingRequested;

        /// <summary>
        /// 获取目标出现的相对时间。
        /// </summary>
        public TimeSpan AppearenceTime;
    }
}
