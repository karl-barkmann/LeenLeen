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
        /// <param name="src"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static object GetPropValue(object src, string propertyName)
        {
            var propertyInfo = src.GetType().GetProperty(propertyName);
            return propertyInfo.GetValue(src, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetProperty, null, null, null);
        }

        /// <summary>
        ///  反射获取对象是否有指定属性。
        /// </summary>
        /// <param name="src"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static bool HasProperty(object src, string propertyName)
        {
            return src.GetType().GetProperty(propertyName) != null;
        }
    }
}
