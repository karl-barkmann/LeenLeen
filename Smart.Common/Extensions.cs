using System;

namespace Smart.Common
{
    /// <summary>
    /// 扩展类。
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// 今天最后一秒时的时间。
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static DateTime TodayLastSecond(this DateTime time)
        {
            return time.Date.AddDays(1).AddMilliseconds(-1);
        }

        /// <summary>
        /// 今天第一秒时的时间。
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static DateTime TodayFirstSecond(this DateTime time)
        {
            return time.Date;
        }

        /// <summary>
        /// 本周第一秒时的时间。
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static DateTime WeekFirstSecond(this DateTime time)
        {
            return time.Date.AddDays(1-(int)time.DayOfWeek);
        }

        /// <summary>
        /// 本周最后一秒时的时间。
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static DateTime WeekLastSecond(this DateTime time)
        {
            return WeekFirstSecond(time).AddDays(7).AddMilliseconds(-1);
        }

        /// <summary>
        /// 本月第一秒时的时间。
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static DateTime MonthFirstSecond(this DateTime time)
        {
            return time.Date.AddDays(1 - time.Day);
        }

        /// <summary>
        /// 本月最后一秒时的时间。
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static DateTime MonthLastSecond(this DateTime time)
        {
            return MonthFirstSecond(time).AddDays(DateTime.DaysInMonth(time.Year, time.Month)).AddMilliseconds(-1);
        }

        /// <summary>
        /// 获取一个值，指示该异常是否必须抛出。
        /// </summary>
        /// <param name="exception">异常对象。</param>
        /// <returns></returns>
        public static bool MustBeThrown(this Exception exception)
        {
            return (exception is StackOverflowException || exception is OutOfMemoryException
                || exception is AccessViolationException);
        }

        /// <summary>
        /// 获取类型上指定的自定义特性。
        /// </summary>
        /// <typeparam name="T">要搜索的自定义特性类型。</typeparam>
        /// <param name="type">要搜索自定义特性的类型。</param>
        /// <returns></returns>
        public static T GetCustomAttribute<T>(this Type type) where T : Attribute
        {
            return Attribute.GetCustomAttribute(type, typeof(T)) as T;
        }    
    }
}
