/* * * * * * * * * * * * * * * * * * * * * * * * * * * 
 * 作者：李平
 * 日期：2012/7/11 9:47:38
 * 描述：IP地址转换器。
 * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System;
using System.Net;
using System.Windows.Data;

namespace Leen.Windows.Data
{
    /// <summary>
    /// IP地址字符串与IPAddress的值转换器。
    /// </summary>
    public class IPAddressToStringConverter : IValueConverter
    {
        #region IValueConverter 成员

        /// <summary>
        /// IPAddress转换至字符串值。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            IPAddress address = value as IPAddress;
            if (address == null)
                return String.Empty;
            else
                return address.ToString();
        }

        /// <summary>
        /// IP地址字符串值转换至IPAddress值。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            String address = value as String;
            if (String.IsNullOrWhiteSpace(address))
                return null;
            else
            {
                bool valid = false;
                IPAddress netAddress;
                valid = IPAddress.TryParse(address, out netAddress);

                if (valid)
                    return netAddress;
                else
                    return null;
            }
        }

        #endregion
    }
}
