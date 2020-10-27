using System;
using System.Windows.Data;
using System.Windows.Markup;

namespace Leen.Windows.Data
{
    /// <summary>
    /// 为值转换器提供扩展标记的抽象基类。
    /// <para>
    /// 允许我们之间在 Binding 中使用 <see cref="IValueConverter"/> 而不事先在资源中声明。
    /// </para>
    /// </summary>
    /// <example>
    /// <code>
    /// <TextBlock Text="{Binding SomePath, Converter={src:DummyConverter}}" />
    /// </code>
    /// </example>
    public abstract class MarkupAccessibleValueConverter<T> : MarkupExtension where T : IValueConverter, new()
    {
        //only one instance should initialize through markup extension.
        private static T _converter = default;

        /// <summary>
        /// 返回值转换器的唯一实例。
        /// </summary>
        /// <param name="serviceProvider">可以为标记扩展提供服务的对象。</param>
        /// <returns></returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (_converter == null)
            {
                _converter = new T();
            }

            return _converter;
        }
    }

    /// <summary>
    /// 为值转换器提供扩展标记的抽象基类。
    /// <para>
    /// 允许我们之间在Binding中使用 <see cref="IMultiValueConverter"/> 而不事先在资源中声明。
    /// </para>
    /// </summary>
    /// <example>
    /// <code>
    /// <TextBlock Text="{Binding SomePath, Converter={src:DummyConverter}}" />
    /// </code>
    /// </example>
    public abstract class MarkupAccessibleMultiValueConverter<T> : MarkupExtension where T : IMultiValueConverter, new()
    {
        //only one instance should initialize through markup extension.
        private static T _converter = default;

        /// <summary>
        /// 返回值转换器的唯一实例。
        /// </summary>
        /// <param name="serviceProvider">可以为标记扩展提供服务的对象。</param>
        /// <returns></returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (_converter == null)
            {
                _converter = new T();
            }

            return _converter;
        }
    }
}
