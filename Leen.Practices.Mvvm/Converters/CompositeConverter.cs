using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace Leen.Practices.Mvvm
{
    /// <summary>
    /// 组合值转换器，用于使用特定的值转换序列来转换值。
    /// </summary>
    public class CompositeConverter : IValueConverter
    {
        /// <summary>
        /// 构造 <see cref="CompositeConverter"/> 类的实例。
        /// </summary>
        public CompositeConverter()
        {
            Converters = new List<IValueConverter>();
        }

        /// <summary>
        /// 按转换顺序排列的值转换器。
        /// </summary>
        public List<IValueConverter> Converters { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (Converters == null || Converters.Count < 1)
                return value;

            object result = value;

            for (int i = 0; i < Converters.Count; i++)
            {
                result = Converters[i].Convert(result, targetType, parameter, culture);
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (Converters == null || Converters.Count < 1)
                return value;

            object result = value;

            for (int i = 0; i < Converters.Count; i++)
            {
                result = Converters[i].ConvertBack(result, targetType, parameter, culture);
            }

            return result;
        }
    }
}
