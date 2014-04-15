using System;
using System.Windows.Data;

namespace Smart.Windows.Data
{
    /// <summary>
    /// 时间对象转换为中文描述。
    /// </summary>
    public class DateTimeToZhCNStringConverter : IValueConverter
    {
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
            var now = DateTime.Now;
            if (value is DateTime)
            {
                DateTime time = (DateTime)value;

                if (time == DateTime.MinValue)
                    return null;
                //是一天
                if (time.Year == now.Year && time.DayOfYear == now.DayOfYear)
                {
                    if (parameter == null)
                    {
                        var span = now - time;
                        if (span < TimeSpan.FromMinutes(1))
                        {
                            if (span.Seconds == 0)
                                return "刚刚";
                            return String.Format("{0}秒前", span.Seconds);
                        }
                        else if (span < TimeSpan.FromHours(1))
                            return String.Format("{0}分钟前", span.Minutes);
                        else
                            return time.ToString("今天 HH:mm");
                    }
                    return time.ToString("HH:mm:ss");
                }
                //不是一天，但是是一个月           //不是一天也不是一个月，但是是一年
                else if (time.Year == now.Year && time.Month == now.Month || time.Year == now.Year)
                {
                    //昨天
                    if (parameter == null && time.DayOfYear == time.DayOfYear - 1)
                    {
                        return time.ToString("昨天 HH:mm");
                    }
                    return time.ToString("MM月 d日 HH:mm");
                }
                //不是今年时间
                return time.ToString("yyyy年 MM月 d日 HH:mm");
            }
            else
                return value;
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
    }
}
