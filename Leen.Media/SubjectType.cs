using System.ComponentModel;

namespace Leen.Media
{
    /// <summary>
    /// 定义客户端产品中视频智能分析过程中产生的目标类型。
    /// </summary>
    public enum SubjectType
    {
        /// <summary>
        /// 未知目标类型。
        /// </summary>
        [Description("未知")]
        Unknown,

        /// <summary>
        /// 人类。
        /// </summary>
        [Description("人类")]
        HumanBeing,

        /// <summary>
        /// 动物。
        /// </summary>
        [Description("动物")]
        Animal,

        /// <summary>
        /// 车辆等交通工具。
        /// </summary>
        [Description("车辆")]
        Vehicle,

        /// <summary>
        /// 其他已识别的杂物，例如树木、广告牌等。
        /// </summary>
        [Description("杂物")]
        Other = 404
    }
}
