using Leen.Common.Internal;
using System;
using System.Collections.Generic;

namespace Leen.Common
{
    /// <summary>
    /// 定义一组通用的扩展方法。
    /// </summary>
    public static class GenericExtensions
    {
        /// <summary>
        /// 将字符串表现形式转换为等效的可空值类型。
        /// <para>
        /// 若字符串为空或空格将返回类型的默认值。
        /// </para>
        /// </summary>
        /// <typeparam name="T">要转换到的可空值类型。</typeparam>
        /// <param name="valueStr">要转换的字符串。</param>
        /// <returns>返回转换后的值。</returns>
        public static T? ToNullable<T>(this string valueStr) where T : struct
        {
            T? result = default;
            if (!string.IsNullOrWhiteSpace(valueStr))
            {
                result = ParserCache<T>.Parse(valueStr);
            }
            return result;
        }

        /// <summary>
        /// 尝试将字符串表现形式转换为等效的可空值类型。
        /// <para>
        /// 若字符串为空或空格将返回类型的默认值。
        /// </para>
        /// </summary>
        /// <typeparam name="T">要转换到的可空值类型。</typeparam>
        /// <param name="valueStr">要转换的字符串。</param>
        /// <param name="value">返回成功转换后的值，转换失败则返回此类型默认值。</param>
        /// <returns>转换成功则返回true, 否则返回false。</returns>
        public static bool ToNullable<T>(this string valueStr, out T? value) where T : struct
        {
            value = default;

            bool result = false;

            if (!string.IsNullOrWhiteSpace(valueStr))
            {
                result = ParserCache<T>.TryParse(valueStr, out T parseValue);
                value = parseValue;
            }

            return result;
        }

        /// <summary>
        /// 将字符串表现形式转换为等效的值类型。
        /// </summary>
        /// <typeparam name="T">要转换到的值类型。</typeparam>
        /// <param name="valueStr">要转换的字符串。</param>
        /// <returns>返回转换后的值。</returns>
        public static T ToValue<T>(this string valueStr) where T : struct
        {
            T result = new T();

            if (!string.IsNullOrWhiteSpace(valueStr))
            {
                result = ParserCache<T>.Parse(valueStr);
            }

            return result;
        }

        /// <summary>
        /// 尝试将字符串表现形式转换为等效的值类型。
        /// </summary>
        /// <typeparam name="T">要转换到的值类型。</typeparam>
        /// <param name="valueStr">要转换的字符串。</param>
        /// <param name="value">返回成功转换后的值，转换失败则返回此类型默认值。</param>
        /// <returns>转换成功则返回true, 否则返回false。</returns>
        public static bool ToValue<T>(this string valueStr, out T value) where T : struct
        {
            value = default;

            bool result = false;

            if (!string.IsNullOrWhiteSpace(valueStr))
            {
                result = ParserCache<T>.TryParse(valueStr, out value);
            }

            return result;
        }

        /// <summary>
        /// 将一定格式的字符串表现形式转换为等效的值类型集合。
        /// <para>
        /// 例如: "1,2,3,4,5,6"
        /// </para>
        /// </summary>
        /// <typeparam name="T">要转换到的值类型。</typeparam>
        /// <param name="valueStr">要转换的字符串。</param>
        /// <param name="separator">各值的分隔符。</param>
        /// <returns>返回转换后的值的集合。</returns>
        public static IEnumerable<T> ToValues<T>(this string valueStr, string separator) where T : struct
        {
            if (string.IsNullOrWhiteSpace(valueStr))
            {
                throw new ArgumentNullException(nameof(valueStr));
            }

            List<T> values = new List<T>();
            var valueArray = valueStr.Split(new string[] { separator }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var value in valueArray)
            {
                values.Add(value.ToValue<T>());
            }

            return values;
        }

        /// <summary>
        /// 尝试将一定格式的字符串表现形式转换为等效的值类型集合。
        /// <para>
        /// 例如: "1,2,3,4,5,6"
        /// </para>
        /// </summary>
        /// <typeparam name="T">要转换到的值类型。</typeparam>
        /// <param name="valueStr">要转换的字符串。</param>
        /// <param name="separator">各值的分隔符。</param>
        /// <param name="values">返回成功转换后的值的集合，转换失败则返回null。</param>
        /// <returns>转换成功则返回true, 否则返回false。</returns>
        public static bool ToValues<T>(this string valueStr, string separator, out IEnumerable<T> values) where T : struct
        {
            if (string.IsNullOrWhiteSpace(valueStr))
            {
                throw new ArgumentNullException(nameof(valueStr));
            }

            values = null;
            var converterdValues = new List<T>();
            var valueArray = valueStr.Split(new string[] { separator }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var value in valueArray)
            {
                if (value.ToValue<T>(out T converterVal))
                {
                    converterdValues.Add(converterVal);
                }
                else
                {
                    return false;
                }
            }

            values = converterdValues;
            return true;
        }

        /// <summary>
        /// 将一定格式的字符串表现形式转换为等效的值类型集合。
        /// <para>
        /// 例如: "1,2,3,4,5,6"
        /// </para>
        /// </summary>
        /// <typeparam name="T">要转换到的值类型。</typeparam>
        /// <param name="valueStr">要转换的字符串。</param>
        /// <param name="separator">各值的分隔符。</param>
        /// <returns>返回转换后的值的集合。</returns>
        public static IEnumerable<T> ToValues<T>(this string valueStr, char separator) where T : struct
        {
            if (string.IsNullOrWhiteSpace(valueStr))
            {
                throw new ArgumentNullException(nameof(valueStr));
            }

            List<T> values = new List<T>();
            var valueArray = valueStr.Split(new char[] { separator }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var value in valueArray)
            {
                values.Add(value.ToValue<T>());
            }

            return values;
        }

        /// <summary>
        /// 尝试将一定格式的字符串表现形式转换为等效的值类型集合。
        /// <para>
        /// 例如: "1,2,3,4,5,6"
        /// </para>
        /// </summary>
        /// <typeparam name="T">要转换到的值类型。</typeparam>
        /// <param name="valueStr">要转换的字符串。</param>
        /// <param name="separator">各值的分隔符。</param>
        /// <param name="values">返回成功转换后的值的集合，转换失败则返回null。</param>
        /// <returns>转换成功则返回true, 否则返回false。</returns>
        public static bool ToValues<T>(this string valueStr, char separator, out IEnumerable<T> values) where T : struct
        {
            if (string.IsNullOrWhiteSpace(valueStr))
            {
                throw new ArgumentNullException(nameof(valueStr));
            }

            values = null;
            var converterdValues = new List<T>();
            var valueArray = valueStr.Split(new char[] { separator }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var value in valueArray)
            {
                if (value.ToValue<T>(out T converterVal))
                {
                    converterdValues.Add(converterVal);
                }
                else
                {
                    return false;
                }
            }

            values = converterdValues;
            return true;
        }
    }
}
