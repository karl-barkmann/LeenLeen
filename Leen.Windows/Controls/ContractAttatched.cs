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
        /// 设置是否启用的属性。
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
    }
}
