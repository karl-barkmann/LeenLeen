using System.Windows;
using System.Windows.Media;

namespace Leen.Windows.Controls
{
    /// <summary>
    /// 定义一组切换按钮控件附加行为。
    /// </summary>
    public static class ToggleButtonAttatched
    {
        /// <summary>
        /// 切换按钮开启文本附加属性。
        /// </summary>
        public static readonly DependencyProperty ActiveTextProperty =
            DependencyProperty.RegisterAttached("ActiveText", typeof(string), typeof(ToggleButtonAttatched), new PropertyMetadata(string.Empty));

        /// <summary>
        /// 获取切换按钮开启文本。
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetActiveText(DependencyObject obj)
        {
            return (string)obj.GetValue(ActiveTextProperty);
        }

        /// <summary>
        /// 设置切换按钮开启文本。
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetActiveText(DependencyObject obj, string value)
        {
            obj.SetValue(ActiveTextProperty, value);
        }

        /// <summary>
        /// 切换按钮关闭文本附加属性。
        /// </summary>
        public static readonly DependencyProperty InactiveTextProperty =
            DependencyProperty.RegisterAttached("InactiveText", typeof(string), typeof(ToggleButtonAttatched), new PropertyMetadata(string.Empty));

        /// <summary>
        /// 获取切换按钮关闭文本。
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetInactiveText(DependencyObject obj)
        {
            return (string)obj.GetValue(InactiveTextProperty);
        }

        /// <summary>
        /// 设置切换按钮关闭文本。
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetInactiveText(DependencyObject obj, string value)
        {
            obj.SetValue(InactiveTextProperty, value);
        }

        /// <summary>
        /// 切换按钮开启画刷附加属性。
        /// </summary>
        public static readonly DependencyProperty ActiveBrushProperty =
            DependencyProperty.RegisterAttached("ActiveBrush", typeof(Brush), typeof(ToggleButtonAttatched),
                new PropertyMetadata(null));

        /// <summary>
        /// 获取切换按钮开启画刷。
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Brush GetActiveBrush(DependencyObject obj)
        {
            return (Brush)obj.GetValue(ActiveBrushProperty);
        }

        /// <summary>
        /// 设置切换按钮开启画刷。
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetActiveBrush(DependencyObject obj, Brush value)
        {
            obj.SetValue(ActiveBrushProperty, value);
        }

        /// <summary>
        /// 切换按钮关闭画刷附加属性。
        /// </summary>
        public static readonly DependencyProperty InactiveBrushProperty =
            DependencyProperty.RegisterAttached("InactiveBrush", typeof(Brush), typeof(ToggleButtonAttatched),
                new PropertyMetadata(null));

        /// <summary>
        /// 获取切换按钮关闭画刷。
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Brush GetInactiveBrush(DependencyObject obj)
        {
            return (Brush)obj.GetValue(InactiveBrushProperty);
        }

        /// <summary>
        /// 设置切换按钮关闭画刷。
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetInactiveBrush(DependencyObject obj, Brush value)
        {
            obj.SetValue(InactiveBrushProperty, value);
        }
    }
}
