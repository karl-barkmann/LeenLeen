using System;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Leen.Windows.Data
{
    /// <summary>
    /// 字节至图片的至转换器。
    /// <para>支持将图片解码为指定的解码宽度。</para>
    /// </summary>
    public class ByteToImageSourceConverter : IValueConverter, IMultiValueConverter
    {
        #region IValueConverter 成员

        /// <summary>
        /// 转换值，默认解码宽度为120像素。
        /// </summary>
        /// <param name="value">绑定源生成的值。</param>
        /// <param name="targetType">绑定目标属性的类型。</param>
        /// <param name="parameter">要使用的转换器参数。</param>
        /// <param name="culture">要用在转换器中的区域性。</param>
        /// <returns>转换后的值。如果该方法返回 null，则使用有效的 null 值。</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;

            byte[] buffer = value as byte[];
            if (buffer == null)
                return null;

            try
            {

                using (MemoryStream memortyStream = new MemoryStream(buffer))
                {
                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    int targetDecodeWidth = 0;
                    bool validDecodeWidth = false;
                    if (parameter != null)
                        validDecodeWidth = Int32.TryParse(parameter.ToString(), out targetDecodeWidth);
                    if (validDecodeWidth)
                        image.DecodePixelWidth = targetDecodeWidth;
                    image.StreamSource = memortyStream;
                    image.EndInit();
                    return image;
                }
            }
            catch
            {
                return null;
            }
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

        #region IMultiValueConverter 成员

        /// <summary>
        /// 使用指定的解码宽度转换内存数据至图片。
        /// </summary>
        /// <param name="values">转换参数（内存字节数组、解码宽度(int)）。</param>
        /// <param name="targetType">要转换到的类型。</param>
        /// <param name="parameter">要使用的转换器参数。</param>
        /// <param name="culture">要用在转换器中的区域性。</param>
        /// <returns>转换后的值。</returns>
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values == null || values.Length < 2)
                return null;

            byte[] imageData = values[0] as byte[];

            if (imageData == null)
                return null;

            int decodeWidth = 0;

            if (values[1] == null)
                decodeWidth = 120;

            if ((values[1].GetType() == typeof(Double) &&
                ((Double)values[1] != Double.NaN ||
                (Double)values[1] != Double.PositiveInfinity ||
                (Double)values[1] != Double.NegativeInfinity))
                || values[1].GetType() == typeof(Int32))
            {
                decodeWidth = System.Convert.ToInt32(values[1]);
            }

            bool validWidth = Int32.TryParse(values[1].ToString(), out decodeWidth);
            if (!validWidth)
                decodeWidth = 120;

            try
            {

                using (MemoryStream memortyStream = new MemoryStream(imageData))
                {
                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.DecodePixelWidth = decodeWidth;
                    image.StreamSource = memortyStream;
                    image.EndInit();
                    return image;
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetTypes"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
