using System.Windows;

namespace System.Windows
{
    /// <summary>
    /// 提供一组用于在逻辑树上进行查找的扩展方法。
    /// </summary>
    public static class LogicalTreeExtensions
    {
        /// <summary>
        /// 获取指定依赖属性系统对象的逻辑父对象，并尝试将其转换为特定类型。
        /// </summary>
        /// <typeparam name="T">要转换到的特定类型。</typeparam>
        /// <param name="current">指定依赖属性系统对象。</param>
        /// <returns></returns>
        public static T GetLogicalParent<T>(this DependencyObject current) where T : DependencyObject
        {
            DependencyObject parentObject = LogicalTreeHelper.GetParent(current);

            if (parentObject == null)
            {
                return null;
            }

            if (parentObject is T parent)
            {
                return parent;
            }
            else
            {
                return GetLogicalParent<T>(parentObject);
            }
        }
    }
}
