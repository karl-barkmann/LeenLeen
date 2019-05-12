using System.ComponentModel;

namespace System.Windows.Input
{
    /// <summary>
    /// 标识鼠标滚轮方向。
    /// </summary>
    public enum MouseWheelDirection
    {
        /// <summary>
        /// 默认值/初始值。
        /// </summary>
        [Description("None")]
        None,

        /// <summary>
        /// 鼠标滚轮向上。
        /// </summary>
        [Description("鼠标滚轮向上")]
        Up,

        /// <summary>
        /// 鼠标滚轮向下。
        /// </summary>
        [Description("鼠标滚轮向下")]
        Down,
    }
}
