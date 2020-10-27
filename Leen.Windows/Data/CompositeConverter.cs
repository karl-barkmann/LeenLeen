using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Leen.Windows.Data
{
    /// <summary>
    /// 组合值转换器，用于使用特定的值转换序列来转换值。
    /// </summary>
    [ContentProperty(nameof(Converters))]
    [ContentWrapper(typeof(ValueConverterCollection))]
    public class CompositeConverter : IValueConverter
    {
        /// <summary>
        /// 构造 <see cref="CompositeConverter"/> 类的实例。
        /// </summary>
        public CompositeConverter()
        {
            Converters = new ValueConverterCollection();
        }

        /// <summary>
        /// 按转换顺序排列的值转换器。
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ValueConverterCollection Converters { get; }

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

    /// <summary>
    /// 表示一组值转换器。
    /// </summary>
    public class ValueConverterCollection : Collection<IValueConverter> { }
}
