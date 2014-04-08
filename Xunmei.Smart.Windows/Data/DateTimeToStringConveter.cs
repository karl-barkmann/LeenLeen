using System;
using System.Windows.Data;

namespace Xunmei.Smart.Windows.Data
{
    /// <summary>
    /// 日期对象转换为指定格式，转换参数为格式化格式。
    /// </summary>
    public class DateTimeToStringConveter : IValueConverter
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
            if (!(value is DateTime))
                return value;

            if (!(parameter is string))
                return value.ToString();

            string result = null;
            try
            {
                result = ((DateTime)value).ToString(parameter.ToString());
            }
            catch
            {
                result = value.ToString();
            }

            return result;
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
