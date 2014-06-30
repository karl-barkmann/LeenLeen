using System;
using System.ComponentModel;
using System.Reflection;

namespace Smart.Common
{
    /// <summary>
    /// 枚举扩展方法类。
    /// </summary>
    public static class EnumerationExtensions
    {
        /// <summary>
        /// 获取枚举值的描述特性中的描述字符串。
        /// </summary>
        /// <param name="value">描述字符串。</param>
        /// <returns></returns>
        public static string GetDescription(this Enum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());

            DescriptionAttribute attribute = Attribute.GetCustomAttribute(field,
                typeof(DescriptionAttribute)) as DescriptionAttribute;

            return attribute == null ? value.ToString() : attribute.Description;
        }
    }
}
