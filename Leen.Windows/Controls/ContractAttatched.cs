using System.Windows;

namespace Leen.Windows.Controls
{
    /// <summary>
    /// 定义一组基于通用协定的附加行为。
    /// </summary>
    public static class ContractAttatched
    {
        /// <summary>
        /// 获取是否启用的属性。
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool GetIsEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsEnabledProperty);
        }

        /// <summary>
        /// 设置依赖对象是否启用的属性。
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetIsEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsEnabledProperty, value);
        }

        /// <summary>
        /// 是否启用的附加依赖属性。
        /// </summary>
        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(ContractAttatched), new PropertyMetadata(true));

        /// <summary>
        /// 获取依赖对象是否为第一个元素的依赖属性。
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool GetIsFirstElement(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsFirstElementProperty);
        }

        /// <summary>
        /// 设置依赖对象是否为第一个元素的依赖属性。
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetIsFirstElement(DependencyObject obj, bool value)
        {
            obj.SetValue(IsFirstElementProperty, value);
        }

        /// <summary>
        /// 是否是第一个元素的附加依赖属性。
        /// </summary>
        public static readonly DependencyProperty IsFirstElementProperty =
            DependencyProperty.RegisterAttached("IsFirstElement", typeof(bool), typeof(ContractAttatched), new PropertyMetadata(false));

        /// <summary>
        /// 获取依赖对象是否为最后一个元素的依赖属性。
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool GetIsLastElement(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsLastElementProperty);
        }

        /// <summary>
        /// 设置依赖对象是否为最后一个元素的依赖属性。
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetIsLastElement(DependencyObject obj, bool value)
        {
            obj.SetValue(IsLastElementProperty, value);
        }

        /// <summary>
        /// 是否是最后一个元素的附加依赖属性。
        /// </summary>
        public static readonly DependencyProperty IsLastElementProperty =
            DependencyProperty.RegisterAttached("IsLastElement", typeof(bool), typeof(ContractAttatched), new PropertyMetadata(false));
    }
}
