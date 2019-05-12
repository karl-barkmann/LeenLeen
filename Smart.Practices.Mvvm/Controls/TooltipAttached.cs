namespace System.Windows.Controls
{
    /// <summary>
    ///  用于在资源文件中为元素的 <see cref="ToolTip"/> 定义 <see cref="Style"/> 的帮助类，使其支持Markup绑定。
    /// </summary>
    public class TooltipAttached
    {
        /// <summary>
        /// 获取当前元素的 <see cref="ToolTip"/> 样式。
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Style GetStyle(DependencyObject obj)
        {
            return (Style)obj.GetValue(StyleProperty);
        }

        /// <summary>
        /// 设置当前元素的 <see cref="ToolTip"/> 样式。
        /// </summary>
        /// <param name="obj">当前依赖属性关联的视图元素。</param>
        /// <param name="value">为元素设置的 <see cref="ToolTip"/> 样式。</param>
        public static void SetStyle(DependencyObject obj, Style value)
        {
            obj.SetValue(StyleProperty, value);
        }

        /// <summary>
        /// 定义 <see cref="ToolTip"/> 的样式，提供Markup绑定等支持。
        /// </summary>
        public static readonly DependencyProperty StyleProperty =
            DependencyProperty.RegisterAttached("Style", typeof(Style), typeof(TooltipAttached), new FrameworkPropertyMetadata(null, StylePropertyChanged));

        private static void StylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as FrameworkElement;
            if (element == null) return;
            if (e.OldValue != null)
            {
                element.Resources[typeof(ToolTip)] = null;
            }
            element.Resources.Add(typeof(ToolTip), e.NewValue as Style);
        }
    }
}
