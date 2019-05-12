using System;
using System.Windows.Data;

namespace Leen.Windows.Data
{
    /// <summary>
    /// 整形数字比较结果转换为布尔类型。
    /// <para>目标值与期待值进行比较。</para>
    /// </summary>
    public class IntegerToBooleanConverter : IValueConverter
    {
        /// <summary>
        /// 转换值。
        /// </summary>
        /// <param name="value">目标值。</param>
        /// <param name="targetType"></param>
        /// <param name="parameter">转换参数（期待值）。</param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return false;

            try
            {
                int integer = System.Convert.ToInt32(value);
                if (parameter != null)
                {
                    int target = System.Convert.ToInt32(parameter);
                    return integer > target;
                }
                return integer > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 转换值。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }
}
