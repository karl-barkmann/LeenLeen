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
            return obj.GetValue(IconProperty);
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

        /// <summary>
        /// 获取此依赖对象附加的图标宽度属性。
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static double GetIconWidth(DependencyObject obj)
        {
            return (double)obj.GetValue(IconWidthProperty);
        }

        /// <summary>
        /// 设置此依赖对象附加的图标宽度。
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetIconWidth(DependencyObject obj, double value)
        {
            obj.SetValue(IconWidthProperty, value);
        }

        /// <summary>
        /// 图标宽度附加依赖属性。
        /// </summary>
        public static readonly DependencyProperty IconWidthProperty =
            DependencyProperty.RegisterAttached("IconWidth", typeof(double), typeof(ControlAttatched), new PropertyMetadata(double.NaN));


        /// <summary>
        /// 获取此依赖对象附加的图标高度属性。
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static double GetIconHeight(DependencyObject obj)
        {
            return (double)obj.GetValue(IconHeightProperty);
        }

        /// <summary>
        /// 设置此依赖对象附加的图标高度。
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetIconHeight(DependencyObject obj, double value)
        {
            obj.SetValue(IconHeightProperty, value);
        }

        /// <summary>
        /// 图标高度附加依赖属性。
        /// </summary>
        public static readonly DependencyProperty IconHeightProperty =
            DependencyProperty.RegisterAttached("IconHeight", typeof(double), typeof(ControlAttatched), new PropertyMetadata(double.NaN));
    }
}
