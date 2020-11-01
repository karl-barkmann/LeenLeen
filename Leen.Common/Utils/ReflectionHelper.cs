using System;
using System.Linq;
using System.Reflection;

namespace Leen.Common.Utils
{
    /// <summary>
    /// 使用反射API的帮组类。
    /// </summary>
    public static class ReflectionHelper
    {
        /// <summary>
        /// 反射获取对象的属性值。
        /// </summary>
        /// <param name="src">属性所属对象示例。</param>
        /// <param name="propertyName">目标属性名称或包含目标属性的属性路径字符串。</param>
        /// <param name="propertyType">目标属性类型。</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">属性名或属性路径不正确，没有找到匹配的属性。</exception>
        public static object GetPropValue(object src, string propertyName, Type propertyType = null)
        {
            if (src == null)
                return null;
            object propertyVal = src;
            var propertyPathes = propertyName.Split('.');
            foreach (var property in propertyPathes)
            {
                PropertyInfo propertyInfo = null;
                if (propertyType == null || (propertyPathes.Length > 1 && property != propertyPathes.Last()))
                    propertyInfo = propertyVal.GetType().GetProperty(property);
                else
                    propertyInfo =propertyVal.GetType().GetProperty(property,
                                                      BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                                                      null,
                                                      propertyType,
                                                      new Type[0],
                                                      new ParameterModifier[0]);
                if (propertyInfo != null)
                {
                    propertyVal = propertyInfo.GetValue(
                        propertyVal,
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetProperty,
                        null,
                        null,
                        null);
                }
                else
                {
                    throw new ArgumentException("属性名或属性路径不正确，没有找到匹配的属性。", nameof(propertyName));
                }
            }

            return propertyVal;
        }

        /// <summary>
        /// 获取指定属性值。
        /// </summary>
        /// <typeparam name="T">属性值的类型。</typeparam>
        /// <param name="src">属性所属对象示例。</param>
        /// <param name="propertyName">属性名称。</param>
        /// <exception cref="ArgumentException">属性名或属性路径不正确，没有找到匹配的属性。</exception>
        /// <returns></returns>
        public static T GetPropValue<T>(object src, string propertyName)
        {
            object retval = GetPropValue(src, propertyName, typeof(T));
            if (retval == null) { return default; }
            return (T)retval;
        }

        /// <summary>
        ///  反射获取对象是否有指定属性。
        /// </summary>
        /// <param name="src">属性所属对象示例。</param>
        /// <param name="propertyName">属性名称。</param>
        /// <returns></returns>
        public static bool HasProperty(object src, string propertyName)
        {
            return src.GetType().GetProperty(propertyName) != null;
        }
    }
}
