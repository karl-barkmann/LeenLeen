using Leen.Common.Internal;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Leen.Common.Xml
{
    /// <summary>
    /// 表示XML元素节点 <see cref="XElement"/> 的特性键值或节点键值集合。
    /// <para>
    /// 允许通过索引器访问不存在的特性或节点名称，若获取则此时返回为空，若设置则添加一个新的特性或节点。
    /// </para>
    /// </summary>
    public class XValues : Dictionary<string, string>
    {
        private readonly static XValues _empty = new XValues(0);

        /// <summary>
        /// 构造 <see cref="XValues"/> 类的实例。
        /// </summary>
        internal XValues() : base()
        {

        }

        /// <summary>
        /// 构造 <see cref="XValues"/> 类的实例，并指定初始容量。
        /// </summary>
        /// <param name="capacity">初始容量。</param>
        internal XValues(int capacity) : base(capacity)
        {

        }

        /// <summary>
        /// 使用已有的属性键值对构造 <see cref="XValues"/> 类的实例。
        /// </summary>
        /// <param name="ditionary">填充此字典的现有字典。</param>
        internal XValues(IDictionary<string, string> ditionary) : base(ditionary)
        {

        }

        /// <summary>
        /// 获取此对象的空实例.
        /// </summary>
        public static XValues Empty
        {
            get
            {
                return _empty;
            }
        }

        /// <summary>
        /// 获取或设置指定特性或节点的值，不存在该属性返回空字符串("")。
        /// </summary>
        /// <param name="key">XML特性或节点名称。</param>
        /// <returns></returns>
        public new string this[string key]
        {
            get
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    throw new ArgumentException("Invalid element name or attribute name", nameof(key));
                }

                if (ContainsKey(key))
                {
                    return base[key];
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    throw new ArgumentException("Invalid element name or attribute name", nameof(key));
                }

                if (ContainsKey(key))
                {
                    base[key] = value;
                }
                else
                {
                    base.Add(key, value);
                }
            }
        }

        /// <summary>
        /// 获取与指定特性或节点名称关联的字符串值，并尝试将其转换为特定类型的值。
        /// </summary>
        /// <typeparam name="T">要将值转换到的特定类型。</typeparam>
        /// <param name="key">XML特性或节点名称。</param>
        /// <param name="value">存在指定特性或节点并成功转换则返回转换后值，否则返回类型<typeparamref name="T"/>的默认值。</param>
        /// <returns>如存在指定特性或节点并成功转换则返回 true，否则返回false。</returns>
        public bool TryGetValue<T>(string key, out T value) where T : struct
        {
            return TryGetValue<T>(key, out value, default);
        }

        /// <summary>
        /// 获取与指定特性或节点名称关联的字符串值，并尝试将其转换为特定类型的值。
        /// </summary>
        /// <typeparam name="T">要将值转换到的特定类型。</typeparam>
        /// <param name="key">XML特性或节点名称。</param>
        /// <param name="value">存在指定特性或节点并成功转换则返回转换后值，否则返回类型<typeparamref name="T"/>的默认值。</param>
        /// <param name="defaultValue">转换失败或不存在指定特性或元素时返回的默认值。</param>
        /// <returns>如存在指定特性或节点并成功转换则返回 true，否则返回false。</returns>
        public bool TryGetValue<T>(string key, out T value, T defaultValue) where T : struct
        {
            value = defaultValue;
            if (base.TryGetValue(key, out string attValue))
            {
                return ParserCache<T>.TryParse(attValue, out value);
            }

            return false;
        }
    }
}
