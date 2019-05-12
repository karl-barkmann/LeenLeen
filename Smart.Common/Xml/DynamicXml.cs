using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Leen.Common.Internal;

namespace Leen.Common.Xml
{
    /// <summary>
    /// 提供用于在运行时动态解析XML文档的方法。
    /// <para>
    /// 此类允许通过动态名称的方式递归访问文档节点直到获取树节点末梢，因此开发者应了解文档结构避免访问不存在的节点而发生错误。
    /// 提供方法 <see cref="GetValue()"/> 获取当前节点的值;
    /// 提供方法 <see cref="GetAttribute(string)"/> 获取当前节点指定特性名称的值;
    /// 提供方法 <see cref="GetValue{T}()"/>  获取当前节点的值并将其转换为特定类型;
    /// 提供方法 <see cref="GetAttribute{T}(string)"/> 通过特性名称获取当前节点的特性值并将其转换为特定类型;
    /// 提供方法 <see cref="TryGetValue{T}(out T)"/> 获取当前节点的值并将尝试其转换为特定类型;
    /// 提供方法 <see cref="TryGetAttribute{T}(string, out T)"/> 通过特性名称获取当前节点的特性值并尝试将其转换为特定类型。
    /// 提供方法 <see cref="Convert{T}"/> 将当前动态XML对象转换为指定类型的实例。
    /// </para>
    /// </summary>
    /// <remarks>
    /// 此类不是线程安全的。
    /// </remarks>
    public class DynamicXml : DynamicObject, IEnumerable<DynamicXml>
    {
        private readonly XElement _current;
        private readonly bool _throwOnNotExists;
        private List<DynamicXml> _children;

        private DynamicXml(XElement parent, bool throwOnNotExists)
        {
            _current = parent;
            _throwOnNotExists = throwOnNotExists;
        }

        #region Properties

        /// <summary>
        /// 获取当前元素节点对象。
        /// </summary>
        public XElement Raw
        {
            get { return _current; }
        }

        /// <summary>
        /// 获取当前节点值。
        /// </summary>
        public string Value
        {
            get { return GetValue(); }
        }

        /// <summary>
        /// 获取当前节点对象的子节点的值，并将其所有子节点名称和值作为字典集合。
        /// <para>
        /// 若存在重复名称的子节点，将采用最后一次出现的子节点的值。不包含子节点下的节点。
        /// </para>
        /// </summary>
        public XValues Values
        {
            get { return GetValues(); }
        }

        /// <summary>
        /// 获取当前元素节点对象的特性字典集合。
        /// </summary>
        public XValues Attributes
        {
            get { return GetAttributes(); }
        }

        /// <summary>
        /// 访问当前动态对象指定索引位置的子对象。
        /// </summary>
        /// <param name="index">子对象的索引。</param>
        /// <returns></returns>
        public DynamicXml this[int index]
        {
            get
            {
                if (index < 0)
                {
                    throw new ArgumentException("Invalid element index", nameof(index));
                }

                if (_current.HasElements)
                {
                    return Children[index];
                }

                if (_throwOnNotExists)
                {
                    throw new DynamicXmlException("No child element found", _current.Name.LocalName, _current.ToString());
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 获取当前动态XML对象的子对象集合。
        /// </summary>
        protected List<DynamicXml> Children
        {
            get
            {
                EnsureChildrenCreated();
                return _children;
            }
        }

        #endregion

        #region Public Methods

        #region DynamicXml load/parse 

        /// <summary>
        /// 将XML文档字符串格式化为动态XML对象。
        /// <para>通过创建的 <see cref="DynamicXml"/> 对象访问不存在的节点时将会引发 <see cref="DynamicXmlException"/> 异常。</para>
        /// </summary>
        /// <param name="xmlString">要解析的XML文档字符串。</param>
        /// <returns>成功格式化后返回创建的动态XML对象。</returns>
        public static DynamicXml Parse(string xmlString)
        {
            return Parse(xmlString, true);
        }

        /// <summary>
        /// 将XML文档字符串格式化为动态XML对象。
        /// </summary>
        /// <param name="xmlString">要解析的XML文档字符串。</param>
        /// <param name="throwOnNotExists">访问不存在的节点时是否引发 <see cref="DynamicXmlException"/> 异常 。</param>
        /// <returns>成功格式化后返回创建的动态XML对象。</returns>
        public static DynamicXml Parse(string xmlString, bool throwOnNotExists)
        {
            if (string.IsNullOrWhiteSpace(xmlString))
            {
                throw new ArgumentNullException(nameof(xmlString));
            }

            return new DynamicXml(XDocument.Parse(xmlString).Root, throwOnNotExists);
        }

        /// <summary>
        /// 将包含XML文档的字节流加载为动态XML对象。
        /// <para>通过创建的 <see cref="DynamicXml"/> 对象访问不存在的节点时将会引发 <see cref="DynamicXmlException"/> 异常。</para>
        /// </summary>
        /// <param name="stream">要加载的XML文档字节流。</param>
        /// <returns></returns>
        public static DynamicXml Load(Stream stream)
        {
            return Load(stream, true);
        }

        /// <summary>
        /// 将包含XML文档的字节流加载为动态XML对象。
        /// </summary>
        /// <param name="stream">要加载的XML文档字节流。</param>
        /// <param name="throwOnNotExists">访问不存在的节点时是否引发 <see cref="DynamicXmlException"/> 异常。</param>
        /// <returns></returns>
        public static DynamicXml Load(Stream stream, bool throwOnNotExists)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            return new DynamicXml(XDocument.Load(stream).Root, throwOnNotExists);
        }

        /// <summary>
        /// 将XML文档文件加载为动态XML对象。
        /// <para>通过创建的 <see cref="DynamicXml"/> 对象访问不存在的节点时将会引发 <see cref="DynamicXmlException"/> 异常。</para>
        /// </summary>
        /// <param name="xmlFile">要加载的XML文档文件。</param>
        /// <returns></returns>
        public static DynamicXml Load(string xmlFile)
        {
            return Load(xmlFile, true);
        }

        /// <summary>
        /// 将XML文档文件加载为动态XML对象。
        /// </summary>
        /// <param name="xmlFile">要加载的XML文档文件。</param>
        /// <param name="throwOnNotExists">访问不存在的节点时是否引发 <see cref="DynamicXmlException"/> 异常。</param>
        /// <returns></returns>
        public static DynamicXml Load(string xmlFile, bool throwOnNotExists)
        {
            if (string.IsNullOrWhiteSpace(xmlFile))
            {
                throw new ArgumentNullException(nameof(xmlFile));
            }

            return new DynamicXml(XDocument.Load(xmlFile).Root, throwOnNotExists);
        }

        /// <summary>
        /// 创建空的动态XML对象。
        /// </summary>
        /// <param name="name">此XML动态对象的根节点名称。</param>
        /// <returns></returns>
        public static DynamicXml Create(string name)
        {
            return new DynamicXml(new XElement(XName.Get(name, string.Empty)), true);
        }

        #endregion

        #region Dynamic runtime support

        /// <summary>
        /// 提供用于获取成员值的操作的实现。 
        /// </summary>
        /// <param name="binder">提供有关调用动态操作的对象信息。binder.Name 属性提供对其执行动态操作的成员的名称。</param>
        /// <param name="result">获取操作的结果。 例如，如果为属性调用方法，您可以将属性值赋给 result。</param>
        /// <returns></returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var xNodeName = binder.Name;

            var nodes = _current.Elements(xNodeName);
            if (nodes.Count() > 1)
            {
                result = nodes.Select(n => new DynamicXml(n, _throwOnNotExists)).ToList();
                return true;
            }

            var node = _current.Element(xNodeName);
            if (node != null)
            {
                result = new DynamicXml(node, _throwOnNotExists);
                return true;
            }

            var elments = _current.Descendants(xNodeName);
            if (elments.Any())
            {
                if (elments.Count() > 1)
                {
                    result = nodes.Select(n => new DynamicXml(n, _throwOnNotExists)).ToList();
                    return true;
                }

                result = new DynamicXml(elments.First(), _throwOnNotExists);

                return true;
            }

            if (_throwOnNotExists)
            {
                throw new DynamicXmlException($"Unknow xml element \"{xNodeName}\"", xNodeName, _current.ToString(SaveOptions.None));
            }
            else
            {
                result = null;
                return true;
            }
        }

        /// <summary>
        /// 提供用于设置成员值的操作的实现。 
        /// </summary>
        /// <param name="binder">提供有关调用动态操作的对象信息。binder.Name 属性提供对其执行动态操作的成员的名称。</param>
        /// <param name="value">要设置的成员的值。</param>
        /// <returns></returns>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            var xNodeName = binder.Name;

            var element = _current.Element(xNodeName);

            switch (value)
            {
                case DynamicXml dynamicXml:
                    if (element != null)
                    {
                        element.Remove();
                    }
                    _current.Add(dynamicXml.Raw);
                    break;
                case IDynamicXmlSerializable dynamicXmlSerializable:
                    {
                        if (element != null)
                        {
                            element.Remove();
                        }
                        var child = Create(xNodeName);
                        dynamicXmlSerializable.WriteXml(child);
                        _current.Add(child.Raw);
                    }
                    break;
                case IEnumerable<DynamicXml> dynamicXmls:
                    if (element == null)
                    {
                        element = new XElement(XName.Get(xNodeName, string.Empty));
                        _current.Add(element);
                    }
                    else
                    {
                        element.RemoveAll();
                    }
                    foreach (var dynamicXml in dynamicXmls)
                    {
                        element.Add(dynamicXml.Raw);
                    }
                    break;
                case IEnumerable<IDynamicXmlSerializable> dynamicXmlSerializables:
                    if (element == null)
                    {
                        element = new XElement(XName.Get(xNodeName, string.Empty));
                        _current.Add(element);
                    }
                    else
                    {
                        element.RemoveAll();
                    }
                    foreach (var dynamicXmlSerializable in dynamicXmlSerializables)
                    {
                        var child = Create(xNodeName);
                        dynamicXmlSerializable.WriteXml(child);
                        element.Add(child.Raw);
                    }
                    break;
                default:
                    if (element == null)
                    {
                        _current.Add(new XElement(xNodeName, value));
                    }
                    else
                    {
                        element.SetValue(value);
                    }
                    break;
            }

            return true;
        }

        #endregion

        #region Element value evaluating

        /// <summary>
        /// 获取当前节点值。
        /// </summary>
        /// <returns>返回当前节点的值。</returns>
        public string GetValue()
        {
            return _current.Value;
        }

        /// <summary>
        /// 获取当前节点对象的子节点的值，并将其所有子节点名称和值作为字典集合。
        /// <para>
        /// 若存在重复名称的子节点，将采用最后一次出现的子节点的值。不包含子节点下的节点。
        /// </para>
        /// </summary>
        /// <returns>返回所有子节点名称和值作为字典集合，如不存在子节点则返回空的字典集合。</returns>
        public XValues GetValues()
        {
            return _current.EvaluateAll();
        }

        /// <summary>
        /// 获取XML节点对象的值，并将其转换为特定类型。
        /// </summary>
        /// <typeparam name="T">要将其转换到的类型。</typeparam>
        /// <returns>返回XML节点对象值。根据节点类型的不同，对应节点值的格式可能不同，
        /// 因此调用者应决定是否通过此方法获取转换后的值，否则可能会引发格式化异常。</returns>
        public T GetValue<T>() where T : struct
        {
            return _current.Evaluate<T>();
        }

        /// <summary>
        /// 获取当前节点值并尝试将其转换为特定类型。
        /// </summary>
        /// <typeparam name="T">要将其值转换到的类型。</typeparam>
        /// <param name="value">转换成功则返回值，否则返回类型的默认值。</param>
        /// <returns>转换成功则返回true，否则返回false。</returns>
        public bool TryGetValue<T>(out T value) where T : struct
        {
            value = default;
            var nodeValue = _current.Value;
            if (string.IsNullOrWhiteSpace(nodeValue))
            {
                return false;
            }

            return ParserCache<T>.TryParse(nodeValue, out value);
        }

        /// <summary>
        /// 获取当前节点值并尝试将其转换为特定类型。
        /// </summary>
        /// <typeparam name="T">要将其值转换到的类型。</typeparam>
        /// <param name="value">转换成功则返回值，否则返回<paramref name="defaultValue"/>。</param>
        /// <param name="defaultValue">转换失败时返回的默认值。</param>
        /// <returns>转换成功则返回true，否则返回false。</returns>
        public bool TryGetValue<T>(out T value, T defaultValue) where T : struct
        {
            value = defaultValue;
            var nodeValue = _current.Value;
            if (string.IsNullOrWhiteSpace(nodeValue))
            {
                return false;
            }

            return ParserCache<T>.TryParse(nodeValue, out value);
        }

        #endregion

        #region Element attribute evaluating

        /// <summary>
        /// 获取一个值指示是否包含指定名称的特性。
        /// </summary>
        /// <param name="attName">特性名称。</param>
        /// <returns></returns>
        public bool ContainsAttribute(string attName)
        {
            if (string.IsNullOrWhiteSpace(attName))
            {
                throw new ArgumentException("Invalid attribute name", nameof(attName));
            }

            if (_current.HasAttributes)
            {
                var attribute = _current.Attribute(attName);
                return attribute != null;
            }

            return false;
        }

        /// <summary>
        /// 获取XML元素节点上指定属性的值，并尝试将其转换为具体类型。
        /// </summary>
        /// <typeparam name="T">需要将属性值转换到的类型。</typeparam>
        /// <param name="attName">属性名称。</param>
        /// <returns>若存在指定名称的特性则尝试转换该值为具体的类型并返回，转换失败或不存在该特性时返回类型默认值。</returns>
        public T GetAttribute<T>(string attName) where T : struct
        {
            return _current.EvaluateAttribute<T>(attName);
        }

        /// <summary>
        /// 获取元素节点上指定特性的值。
        /// </summary>
        /// <param name="attributeName"></param>
        /// <returns>若存在指定名称的特性则返回该特性的值，否则返回 <see cref="string.Empty"/>。</returns>
        public string GetAttribute(string attributeName)
        {
            return _current.EvaluateAttribute(attributeName);
        }

        /// <summary>
        /// 获取当前元素节点对象的特性字典集合。
        /// </summary>
        /// <returns>返回当前节点的特性字典集合，不存在任何特性将返回空字典集合。</returns>
        public XValues GetAttributes()
        {
            return _current.EvaluateAttributes();
        }

        /// <summary>
        /// 尝试获取指定特性名称的特性值。
        /// </summary>
        /// <param name="attName">要获取的特性名称。</param>
        /// <param name="value">若存在指定名称特性，则返回其特性值。</param>
        /// <returns>存在指定特性则返回true，否则返回false。</returns>
        public bool TryGetAttribute<T>(string attName, out T value) where T : struct
        {
            value = default;

            if (_current.HasAttributes)
            {
                return _current.TryEvaluateAttribute<T>(attName, out value);
            }

            return false;
        }

        /// <summary>
        /// 尝试获取指定特性名称的特性值并将其转换为特定类型。
        /// </summary>
        /// <typeparam name="T">要将其转换到的类型。</typeparam>
        /// <param name="attName">要获取的特性名称。</param>
        /// <param name="value">若存在指定名称特性，且转换成功则返回其特性值，否则返回类型的默认值。</param>
        /// <param name="defaultValue"></param>
        /// <returns>存在指定特性且转换成功则返回true，否则返回false。</returns>
        public bool TryGetAttribute<T>(string attName, out T value, T defaultValue) where T : struct
        {
            value = defaultValue;

            if (_current.HasAttributes)
            {
                return _current.TryEvaluateAttribute<T>(attName, out value, defaultValue);
            }

            return false;
        }

        #endregion

        /// <summary>
        /// 设置当前节点的值。
        /// </summary>
        /// <param name="value">要设置的节点的值。</param>
        public void SetValue(object value)
        {
            _current.SetValue(value);
        }

        /// <summary>
        /// 设置当前节点指定特性的值。
        /// </summary>
        /// <param name="attName">特性的名称。</param>
        /// <param name="value">特性的值。</param>
        public void SetAttribute(string attName, object value)
        {
            _current.SetAttributeValue(attName, value);
        }

        /// <summary>
        /// 将当前动态XML对象转换为指定类型的实例。
        /// <para>此对象类型应具有无参构造函数。</para>
        /// </summary>
        /// <typeparam name="T">要转换到的对象类型。</typeparam>
        /// <returns></returns>
        public T Convert<T>() where T : IDynamicXmlSerializable, new()
        {
            T result = new T();
            result.ReadXml(this);
            return result;
        }

        /// <summary>
        /// 将当前动态XML对象的子对象转换为指定类型的实例的集合。
        /// <para>此对象类型应具有无参构造函数。</para>
        /// </summary>
        /// <typeparam name="T">要转换到的对象类型。</typeparam>
        /// <returns></returns>
        public IEnumerable<T> ConvertAll<T>() where T : IDynamicXmlSerializable, new()
        {
            List<T> result = new List<T>();
            Children.ForEach(child =>
            {
                var model = child.Convert<T>();
                result.Add(model);
            });

            return result;
        }

        /// <summary>
        /// 输出此对象的格式化字符串。
        /// </summary>
        /// <param name="format">
        /// 格式化参数。
        /// <para>
        /// {iv}:   获取此XML动态节点的值。
        /// </para>
        /// </param>
        /// <returns></returns>
        public string ToString(string format = null)
        {
            if (format == "{iv}")
            {
                return _current != null ? _current.Value : String.Empty;
            }

            return _current != null ? _current.ToString() : base.ToString();
        }

        /// <summary>
        /// 获取一个当前节点的子节点的动态XML对象的集合迭代器。
        /// </summary>
        /// <returns></returns>
        public IEnumerator<DynamicXml> GetEnumerator()
        {
            return ((IEnumerable<DynamicXml>)Children).GetEnumerator();
        }

        /// <summary>
        /// 获取一个当前节点的子节点的动态XML对象的集合迭代器。
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<DynamicXml>)Children).GetEnumerator();
        }

        #endregion

        private void EnsureChildrenCreated()
        {
            if (_children == null)
            {
                _children = _current.Elements().Select(x => new DynamicXml(x, _throwOnNotExists)).ToList();
            }
        }
    }
}
