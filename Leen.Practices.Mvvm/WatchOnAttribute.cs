using System;

namespace Leen.Practices.Mvvm
{
    /// <summary>
    /// 一种用于监听指定的属性变更并执行相应操作的特性。
    /// <para>
    /// 应用于方法时方法参数应为监听的属性或无参方法。
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
    public sealed class WatchOnAttribute : Attribute
    {
        /// <summary>
        /// 构造 <see cref="WatchOnAttribute"/> 类的实例。
        /// </summary>
        /// <param name="targetProperty">监听的属性名称。</param>
        public WatchOnAttribute(string targetProperty)
        {
            if (string.IsNullOrEmpty(targetProperty))
            {
                throw new ArgumentException("属性名不能为空", nameof(targetProperty));
            }
            TargetProperty = targetProperty;
        }

        /// <summary>
        /// 构造 <see cref="WatchOnAttribute"/> 类的实例。
        /// </summary>
        /// <param name="targetProperty">监听的属性名称。</param>
        /// <param name="propertyType">监听的属性的类型，当应用于方法时必须。</param>
        public WatchOnAttribute(string targetProperty, Type propertyType = null)
        {
            if (string.IsNullOrEmpty(targetProperty))
            {
                throw new ArgumentException("属性名不能为空", nameof(targetProperty));
            }
            TargetProperty = targetProperty;
            PropertyType = propertyType;
        }

        /// <summary>
        /// 获取或设置监听的属性名称。
        /// </summary>
        public string TargetProperty { get; set; }

        /// <summary>
        /// 监听的属性的类型。
        /// </summary>
        public Type PropertyType { get; }
    }
}
