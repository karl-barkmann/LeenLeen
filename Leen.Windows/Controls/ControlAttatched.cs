using System.Windows;

namespace Leen.Windows.Controls
{
    /// <summary>
    /// 定义一组控件附加属性。
    /// </summary>
    public static class ControlAttatched
    {
        /// <summary>
        /// 圆角附加依赖属性。
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty =
               DependencyProperty.RegisterAttached("CornerRadius",
                                                   typeof(CornerRadius),
                                                   typeof(ControlAttatched),
                                                   new FrameworkPropertyMetadata(new CornerRadius(0), FrameworkPropertyMetadataOptions.AffectsRender));
        /// <summary>
        /// 获取此依赖对象附加的圆角属性。
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static CornerRadius GetCornerRadius(DependencyObject d)
        {
            return (CornerRadius)d.GetValue(CornerRadiusProperty);
        }

        /// <summary>
        /// 设置此依赖对象附加的圆角属性。
        /// </summary>
        /// <param name="d"></param>
        /// <param name="value"></param>
        public static void SetCornerRadius(DependencyObject d, CornerRadius value)
        {
            d.SetValue(CornerRadiusProperty, value);
        }

        /// <summary>
        /// 获取此依赖对象附加的图标属性。
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static object GetIcon(DependencyObject obj)
        {
            return (object)obj.GetValue(IconProperty);
        }

        /// <summary>
        /// 设置此依赖对象附加的图标属性。
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetIcon(DependencyObject obj, object value)
        {
            obj.SetValue(IconProperty, value);
        }

        /// <summary>
        /// 图标附加依赖属性。
        /// </summary>
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.RegisterAttached("Icon", typeof(object), typeof(ControlAttatched), new PropertyMetadata(null));
    }
}
