#pragma warning disable 1591

using Leen.Common.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Leen.Windows.Data
{
    /// <summary>
    /// Returns false when a bool is true。
    /// If the value is not a bool, return itself.
    /// </summary>
    public class NotConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == DependencyProperty.UnsetValue || value == null)
                return value;

            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == DependencyProperty.UnsetValue || value == null)
                return value;
            return !(bool)value;
        }

        #endregion
    }

    /// <summary>
    /// Retuns true when all bool values are true.
    /// </summary>
    public class AndConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values == null)
                return false;

            bool result = true;

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] == DependencyProperty.UnsetValue)
                    continue;
                result = result && (bool)values[i];
                if (!result)
                {
                    return false;
                }
            }
            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return new object[] { false, Binding.DoNothing };

            return new object[] { value, Binding.DoNothing };
        }
    }

    /// <summary>
    /// Retuns true when one of bool values is true.
    /// </summary>
    public class OrConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values == null)
                return false;

            bool result = false;

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] == DependencyProperty.UnsetValue)
                    continue;
                result = result || (bool)values[i];
                if (result)
                {
                    return true;
                }
            }

            return result;
        }


        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Returns true when all bool values are false. 
    /// </summary>
    public class NorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values == null)
                return false;

            bool result = true;

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] == DependencyProperty.UnsetValue)
                    continue;
                result = result && (!(bool)values[i]);
                if (!result)
                {
                    return false;
                }
            }
            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 比较 value 和 parameter 是否相等，并返回Boolean值。
    /// </summary>
    public class EqualityToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == parameter;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 比较两个值是否相等，并返回Boolean值。
    /// </summary>
    public class IntegerEqualityToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
            {
                return false;
            }

            if (!int.TryParse(parameter.ToString(), out int specificState))
            {
                return false;
            }

            var stateFilter = (int)value;

            return stateFilter == specificState;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return 0;

            if (!int.TryParse(parameter.ToString(), out int specificState))
            {
                return false;
            }

            bool isChecked = (bool)value;
            if (isChecked)
            {
                return specificState;
            }
            else
            {
                return 0;
            }
        }
    }

    /// <summary>
    /// Returns true if Visible.
    /// Returns Visible if true,returns Collapsed if not true.
    /// </summary>
    public class VisibilityToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return false;

            if (!Enum.TryParse<Visibility>(value.ToString(), out Visibility visibility))
            {
                return false;
            }
            if (visibility == Visibility.Visible)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Visibility.Collapsed;

            if (!Boolean.TryParse(value.ToString(), out bool result))
            {
                return Visibility.Collapsed;
            }

            if (result)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }
    }

    /// <summary>
    /// Returns Collapsed if false,  returns Visible if true.
    /// </summary>
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Visibility.Collapsed;

            Boolean.TryParse(value.ToString(), out bool result);

            return result ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return false;

            Enum.TryParse<Visibility>(value.ToString(), out Visibility result);

            return result == Visibility.Visible;
        }
    }

    /// <summary>
    /// Returns Collapsed if true,  returns Visible if false.
    /// </summary>
    public class ReverseBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Visibility.Visible;

            Boolean.TryParse(value.ToString(), out bool result);

            return result ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return true;

            Enum.TryParse<Visibility>(value.ToString(), out Visibility result);

            return result == Visibility.Collapsed;
        }
    }

    /// <summary>
    /// 用于TextBlock当显示内容溢出内容区域时要采取的文本修整行为。
    /// <para>
    /// 当文本被截取后自动显示控件提示。
    /// </para>
    /// </summary>
    public class TrimmedTextBlockVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return Visibility.Collapsed;

            FrameworkElement textBlock = (FrameworkElement)value;

            textBlock.Measure(new System.Windows.Size(Double.PositiveInfinity, Double.PositiveInfinity));

            if (((FrameworkElement)value).ActualWidth < ((FrameworkElement)value).DesiredSize.Width)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 调试用值转换器。
    /// </summary>
    public class DebugConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    /// <summary>
    /// 切换显示Grid的行或列。
    /// </summary>
    public class VisibilityToStarGridLengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((Visibility)value == Visibility.Collapsed)
            {
                return new GridLength(0, GridUnitType.Pixel);
            }
            else
            {
                if (parameter == null)
                {
                    throw new ArgumentNullException(nameof(parameter), "请指定Star宽度!");
                }

                return new GridLength(double.Parse(parameter.ToString(), culture), GridUnitType.Star);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 切换显示Grid的行或列。
    /// </summary>
    public class VisibilityToPixelOrAutoGridLengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((Visibility)value == Visibility.Collapsed)
            {
                return new GridLength(0, GridUnitType.Pixel);
            }
            else
            {
                return new GridLength(0, GridUnitType.Auto);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 将枚举值转换为其对应的Description特性中的字符串。
    /// </summary>
    public class EnumDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return String.Empty;

            return EnumHelper.GetDescription(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 将枚举值和参数做比较如果他们是同一个枚举值，则返回true否则返回false。
    /// <remarks>
    /// 使用者应清楚同一枚举值是指同一枚举类型定义下的两个值。
    /// </remarks>
    /// </summary>
    public class EnumEqualityToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return false;

            Type enumType = value.GetType();
            string targetValue = parameter.ToString();

            if (enumType != parameter.GetType())
                return false;

            if (!targetValue.Contains("|") && (!enumType.IsEnum || !enumType.IsEnumDefined(parameter)))
            {
                throw new InvalidOperationException("EnumEqualityToBooleanConverter can only used for comparing enum values!");
            }

            if (targetValue.Contains("|"))
            {
                string[] conditionalValues = targetValue.Split('|');
                if (conditionalValues != null)
                {
                    foreach (var item in conditionalValues)
                    {
                        if (item == null)
                            continue;
                        if (item == value.ToString() || value.GetHashCode() == item.GetHashCode())
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
            else
            {
                return value.GetHashCode() == parameter.GetHashCode() || value.ToString() == parameter.ToString();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            if ((bool)value)
            {
                int index = new List<string>(Enum.GetNames(targetType)).IndexOf(parameter.ToString());
                if (index >= 0)
                {
                    return Enum.GetValues(targetType).GetValue(index);
                }

                if (Enum.GetValues(targetType).OfType<int>().Contains(parameter.GetHashCode()))
                {
                    return Enum.Parse(targetType, parameter.ToString());
                }
            }

            return null;
        }
    }

    /// <summary>
    /// 将文件长度转换为界面友好的文件大小。如 KB、MB、 GB 等;
    /// </summary>
    public class FileLengthToReadableSizeConverter : IValueConverter
    {
        static readonly string[] SizeSuffixes = { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            long size = (long)value;
            string readableSize = SizeSuffix(size);

            return readableSize;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        static string SizeSuffix(Int64 value)
        {
            if (value < 0) { return "-" + SizeSuffix(-value); }
            if (value == 0) { return "0 B"; }

            int mag = (int)Math.Log(value, 1024);
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            return string.Format("{0:n1} {1}", adjustedSize, SizeSuffixes[mag]);
        }
    }

    /// <summary>
    /// 使BitmapImage可以支持Binding,并且根据相应性能指数创建；
    /// 同时在Binding失败时加载默认图片。
    /// </summary>
    public class BitmapImageBindingSourceConverter : IValueConverter
    {
        public int DecodeWidth { get; set; }

        public int DecodeHeight { get; set; }

        public BitmapCacheOption CacheOption { get; set; }

        public BitmapCreateOptions CreateOptions { get; set; }

        public Uri DefaultSource { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            Uri sourceUri = null;
            Stream sourceStream = null;
            switch (value)
            {
                case string sourceStr:
                    try
                    {
                        sourceUri = new Uri(sourceStr);
                    }
                    catch (UriFormatException)
                    {
                        sourceUri = DefaultSource;
                    }
                    break;
                case Uri uri:
                    sourceUri = uri;
                    break;
                case Stream stream:
                    sourceStream = stream;
                    break;
            }

            if (sourceUri != null || sourceStream != null)
            {
                var source = new BitmapImage();
                source.BeginInit();
                source.UriSource = sourceUri;
                source.StreamSource = sourceStream;
                if (DecodeWidth >= 0)
                    source.DecodePixelWidth = DecodeWidth;
                if (DecodeHeight >= 0)
                    source.DecodePixelHeight = DecodeHeight;
                source.CacheOption = CacheOption;
                source.CreateOptions = CreateOptions;
                source.EndInit();
                source.Freeze();
                return source;
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 将字符串与十进制数相互转换。
    /// </summary>
    public class StringToNumericConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return 0;
            }

            if (string.IsNullOrWhiteSpace(value.ToString()))
            {
                return 0;
            }

            if (decimal.TryParse(value.ToString(), out decimal result))
            {
                return result;
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return string.Empty;
            }

            if (parameter != null)
            {
                return string.Format(culture.NumberFormat, parameter.ToString(), value);
            }
            else
            {
                return value.ToString();
            }
        }
    }

    /// <summary>
    /// 判断value是否为空或 <see cref="DependencyProperty.UnsetValue"/> 的值转换器。
    /// </summary>
    public class NotNullConverter : MarkupAccessibleValueConverter<NotNullConverter>, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && value != DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 判断value是否为空或 <see cref="DependencyProperty.UnsetValue"/> 并转换为控件可见枚举的值转换器。
    /// </summary>
    public class NotNullToVisiblityConverter : MarkupAccessibleValueConverter<NotNullToVisiblityConverter>, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool result = value != null && value != DependencyProperty.UnsetValue;
            return result ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 在纳秒双精度值与时间间隔之间相互转换。
    /// </summary>
    public class DoubleToTimeSpanConverter : MarkupAccessibleValueConverter<DoubleToTimeSpanConverter>, IValueConverter
    {
        /// <summary>
        /// 将时间间隔转换为纳秒双精度值。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value == DependencyProperty.UnsetValue)
            {
                return 0;
            }

            var timeSpan = (TimeSpan)value;
            return (double)timeSpan.Ticks / 10;
        }

        /// <summary>
        /// 将纳秒双精度值转换为时间间隔。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value == DependencyProperty.UnsetValue)
            {
                return TimeSpan.Zero;
            }

            double nanoseconds = (double)value;
            return TimeSpan.FromTicks((long)nanoseconds * 10);

        }
    }
}
