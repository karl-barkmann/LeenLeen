using Leen.Native;
using System.Windows;

namespace Leen.Windows.Utils
{
    /// <summary>
    /// 系统鼠标帮助类。
    /// </summary>
    public class MouseUtil
    { 
        /// <summary>
        /// 获取鼠标屏幕位置。
        /// </summary>
        /// <returns></returns>
        public static Point GetMousePosition()
        {
            User32.Point point = new User32.Point();
            User32.GetCursorPos(ref point);
            return new Point(point.x, point.y);
        }
    }
}
