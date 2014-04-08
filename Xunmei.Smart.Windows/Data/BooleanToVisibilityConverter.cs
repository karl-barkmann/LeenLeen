using System;
using System.Windows;
using System.Windows.Data;

namespace Xunmei.Smart.Windows.Data
{
    /// <summary>
    /// 自定义布尔值到显示状态的值转换器。
    /// <remarks>
    /// 支持多种转换形式：
    /// 当绑定对象目标值不是布尔类型时，目标值与期待值的字符串值一致则显示，否则为不显示;
    /// 当绑定对象目标值是布尔类型时且无期待值则目标值决定是否显示;
    /// 当绑定对象目标值是布尔类型时且有期待值则目标值与期待值一致则显示，否则为不显示。
    /// </remarks>
    /// </summary>
    public class BooleanToVisibilityConverter : IValueConverter
    {
        #region IValueConverter 成员

        /// <summary>
        /// 转换值。
        /// </summary>
        /// <param name="value">绑定源生成的值。</param>
        /// <param name="targetType">绑定目标属性的类型。</param>
        /// <param name="parameter">要使用的转换器参数。</param>
        /// <param name="culture">要用在转换器中的区域性。</param>
        /// <returns>转换后的值。如果该方法返回 null，则使用有效的 null 值。</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is bool))
            {
                //不是数字，如果 value 和 parameter 相等 则返回可见
                if (value != null && parameter != null && value.ToString() == parameter.ToString())
                    return Visibility.Visible;
                return Visibility.Collapsed;
            }

            bool bValue = (bool)value;
            bool bParameter = false;
            bool isOk = false;
            if (parameter != null)
                isOk = Boolean.TryParse(parameter.ToString(), out bParameter);
            if (isOk)
            {
                if (bParameter == bValue)
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }

            if (bValue)
                return Visibility.Visible;
            return Visibility.Collapsed;
        }

        /// <summary>
        /// 转换值。
        /// </summary>
        /// <param name="value">绑定目标生成的值。</param>
        /// <param name="targetType">要转换到的类型。</param>
        /// <param name="parameter">要使用的转换器参数。</param>
        /// <param name="culture">要用在转换器中的区域性。</param>
        /// <returns>转换后的值。如果该方法返回 null，则使用有效的 null 值。</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
