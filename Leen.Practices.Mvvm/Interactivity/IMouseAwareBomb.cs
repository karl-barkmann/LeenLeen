namespace System.Windows.Interactivity
{
    /// <summary>
    /// 定义需要感知鼠标位置的拖拽物接口。
    /// </summary>
    public interface IMouseAwareBomb
    {
        /// <summary>
        /// 设置拖放时的鼠标位置。
        /// </summary>
        /// <param name="mousePos"></param>
        void SetMousePosition(Point mousePos);
    }
}
