using System;
using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Linq;

namespace Smart.Windows.Data
{
    /// <summary>
    /// Returns false when a bool is true.
    /// </summary>
    public class NotConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
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

            bool result = false;

            for (int i = 0; i < values.Length; i++)
            {
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
            throw new NotImplementedException();
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
    /// Returns true when both of the bool values is false. 
    /// </summary>
    public class NorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values == null || values.Length < 2)
                return false;

            if (((bool)values[0] == false) && ((bool)values[1] == false))
                return true;

            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
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
            int specificState;
            if (!Int32.TryParse(parameter.ToString(), out specificState))
            {
                return false;
            }

            if (value == null || parameter == null)
            {
                return false;
            }

            var stateFilter = (int)value;

            return stateFilter == specificState;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int specificState;
            if (!Int32.TryParse(parameter.ToString(), out specificState))
            {
                return false;
            }

            if (value == null || parameter == null)
                return 0;

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

            Visibility visibility;
            if (!Enum.TryParse<Visibility>(value.ToString(), out visibility))
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

            bool result;
            if (!Boolean.TryParse(value.ToString(), out result))
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

            bool result;
            Boolean.TryParse(value.ToString(), out result);

            return result ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return false;

            Visibility result;
            Enum.TryParse<Visibility>(value.ToString(), out result);

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

            bool result;
            Boolean.TryParse(value.ToString(), out result);

            return result ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return true;

            Visibility result;
            Enum.TryParse<Visibility>(value.ToString(), out result);

            return result == Visibility.Collapsed;
        }
    }

    public class TrimmedTextBlockVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
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

    public class VisibilityToStarGridLengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((Visibility)value == Visibility.Collapsed)
            {
                return new GridLength(0, GridUnitType.Star);
            }
            else
            {
                if (parameter == null)
                {
                    throw new ArgumentNullException("parameter");
                }

                return new GridLength(double.Parse(parameter.ToString(), culture), GridUnitType.Star);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

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
}
