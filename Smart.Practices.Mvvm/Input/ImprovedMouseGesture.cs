
namespace System.Windows.Input
{
    /// <summary>
    /// 定义可用于调用命令的鼠标输入笔势，支持鼠标点击次数及滚动方向判定以准确的调用鼠标输入笔势命令。
    /// </summary>
    public class ImprovedMouseGesture : MouseGesture
    {
        /// <summary>
        /// 初始化<see cref="ImprovedMouseGesture"/>类的实例。
        /// </summary>
        /// <param name="mouseAction">定义鼠标输入笔势的鼠标操作。</param>
        public ImprovedMouseGesture(MouseAction mouseAction)
            : base(mouseAction)
        {

        }

        /// <summary>
        /// 初始化<see cref="ImprovedMouseGesture"/>类的实例。
        /// </summary>
        /// <param name="direction">鼠标滚动方向。</param>
        public ImprovedMouseGesture(MouseWheelDirection direction)
            : base(MouseAction.WheelClick)
        {
            WheelDirection = direction;
        }

        /// <summary>
        /// 初始化<see cref="ImprovedMouseGesture"/>类的实例。
        /// </summary>
        /// <param name="mouseAction">定义鼠标输入笔势的鼠标操作。</param>
        /// <param name="modifiers">定义鼠标输入笔势的鼠标操作修改键。</param>
        public ImprovedMouseGesture(MouseAction mouseAction, ModifierKeys modifiers)
            : base(mouseAction, modifiers)
        {

        }

        /// <summary>
        /// 初始化<see cref="ImprovedMouseGesture"/>类的实例。
        /// </summary>
        /// <param name="modifiers">定义鼠标滚轮输入笔势的鼠标操作修改键。</param>
        /// <param name="direction">鼠标滚动方向。</param>
        public ImprovedMouseGesture(ModifierKeys modifiers, MouseWheelDirection direction)
            : base(MouseAction.WheelClick, modifiers)
        {
            WheelDirection = direction;
        }

        /// <summary>
        /// 获取或设置用于判定鼠标输入笔势滚动方向的值。
        /// </summary>
        public MouseWheelDirection WheelDirection { get; set; }

        /// <summary>
        /// 确定 System.Windows.Input.MouseGesture 是否和与指定 System.Windows.Input.InputEventArgs
        ///     对象关联的输入匹配。
        /// </summary>
        /// <param name="targetElement"> 目标。</param>
        /// <param name="inputEventArgs">要与此笔势进行比较的输入事件数据。</param>
        /// <returns></returns>
        public override bool Matches(object targetElement, InputEventArgs inputEventArgs)
        {
            if (!base.Matches(targetElement, inputEventArgs)) return false;
            if (!(inputEventArgs is MouseEventArgs)) return false;

            if (WheelDirection == MouseWheelDirection.None)
            {
                var args = (MouseButtonEventArgs)inputEventArgs;
                switch (MouseAction)
                {
                    case MouseAction.LeftClick:
                        return args.ClickCount == 1 && args.ChangedButton == MouseButton.Left;
                    case MouseAction.LeftDoubleClick:
                        return args.ClickCount == 2 && args.ChangedButton == MouseButton.Left;
                    case MouseAction.MiddleClick:
                        return args.ClickCount == 1 && args.ChangedButton == MouseButton.Middle;
                    case MouseAction.MiddleDoubleClick:
                        return args.ClickCount == 2 && args.ChangedButton == MouseButton.Middle;
                    case MouseAction.RightClick:
                        return args.ClickCount == 1 && args.ChangedButton == MouseButton.Right;
                    case MouseAction.RightDoubleClick:
                        return args.ClickCount == 2 && args.ChangedButton == MouseButton.Right;
                    case MouseAction.WheelClick:
                        return true;
                    case MouseAction.None:
                    default:
                        return false;
                }
            }
            else
            {
                var args = (MouseWheelEventArgs)inputEventArgs;
                switch (WheelDirection)
                {
                    case MouseWheelDirection.None:
                        return args.Delta == 0;
                    case MouseWheelDirection.Up:
                        return args.Delta > 0;
                    case MouseWheelDirection.Down:
                        return args.Delta < 0;
                    default:
                        return false;
                }
            }
        }
    }
}
