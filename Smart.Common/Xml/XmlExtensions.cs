using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Leen.Common.Internal;

namespace Leen.Common.Xml
{
    /// <summary>
    /// 提供一组用于解析XML文档的扩展方法。
    /// </summary>
    public static class XmlExtensions
    {
        /// <summary>
        /// 使用XML文档字符串创建XML节点或文档对象。
        /// </summary>
        /// <param name="xmlDoc">要创建的XML节点或文档对象的XML字符串。</param>
        /// <returns>返回创建的XML根节点对象或文档对象。</returns>
        public static XContainer Build(this string xmlDoc)
        {
            if (string.IsNullOrWhiteSpace(xmlDoc))
            {
                throw new ArgumentNullException(nameof(xmlDoc));
            }
            return XDocument.Parse(xmlDoc, LoadOptions.None);
        }

        /// <summary>
        /// 获取XML节点对象的指定名称子节点。
        /// </summary>
        /// <param name="xObject">XML父节点对象。</param>
        /// <param name="elementName">子元素节点名称。</param>
        /// <returns>返回匹配名称的第一个子节点对象，不存在匹配名称将返回null。</returns>
        public static XElement Expand(this XContainer xObject, string elementName)
        {
            if (string.IsNullOrWhiteSpace(elementName))
            {
                throw new ArgumentNullException(nameof(elementName));
            }

            if (xObject == null)
            {
                return null;
            }

            return xObject.Element(elementName);
        }

        /// <summary>
        /// 获取XML节点对象的子节点集合。
        /// <para>不包含子节点下的节点。</para>
        /// </summary>
        /// <param name="xObject">XML父节点对象。</param>
        /// <returns>返回所有子节点对象，不存在任何子节点将返回null。</returns>
        public static IEnumerable<XElement> ExpandAll(this XContainer xObject)
        {
            if (xObject == null)
            {
                return null;
            }

            return xObject.Elements();
        }

        /// <summary>
        /// 获取XML节点对象的指定名称子节点集合。
        /// <para>不包含子节点下的节点。</para>
        /// </summary>
        /// <param name="xObject">XML父节点对象。</param>
        /// <param name="elementName">子元素节点名称。</param>
        /// <returns>返回匹配名称的所有子节点对象，不存在匹配名称将返回null。</returns>
        public static IEnumerable<XElement> ExpandAll(this XContainer xObject, string elementName)
        {
            if (string.IsNullOrWhiteSpace(elementName))
            {
                throw new ArgumentNullException(nameof(elementName));
            }

            if (xObject == null)
            {
                return null;
            }

            return xObject.Elements(elementName);
        }

        /// <summary>
        /// 获取XML节点对象的值。
        /// <para>
        /// 根据节点类型的不同，其返回值对应节点的值或节点的无格式序列化字符串。
        /// </para>
        /// </summary>
        /// <param name="xObject">XML父节点对象。</param>
        /// <returns>
        /// 返回XML节点对象值，根据节点类型的不同，其返回值对应节点的值或节点的无格式序列化字符串。
        /// </returns>
        public static string Evaluate(this XObject xObject)
        {
            if (xObject == null)
            {
                return string.Empty;
            }

            switch (xObject.NodeType)
            {
                case XmlNodeType.None:
                case XmlNodeType.EntityReference:
                case XmlNodeType.Entity:
                case XmlNodeType.DocumentFragment:
                case XmlNodeType.Notation:
                case XmlNodeType.Whitespace:
                case XmlNodeType.SignificantWhitespace:
                case XmlNodeType.EndElement:
                case XmlNodeType.EndEntity:
                case XmlNodeType.XmlDeclaration:
                    return null;
                case XmlNodeType.Element:
                    return ((XElement)xObject).Value;
                case XmlNodeType.Attribute:
                    return ((XAttribute)xObject).Value;
                case XmlNodeType.Text:
                    return ((XText)xObject).Value;
                case XmlNodeType.CDATA:
                    return ((XCData)xObject).Value;
                case XmlNodeType.Comment:
                    return ((XComment)xObject).Value;
                case XmlNodeType.Document:
                    return ((XDocument)xObject).ToString(SaveOptions.DisableFormatting);
                case XmlNodeType.DocumentType:
                    return ((XDocumentType)xObject).ToString(SaveOptions.DisableFormatting);
                case XmlNodeType.ProcessingInstruction:
                    return ((XProcessingInstruction)xObject).ToString(SaveOptions.DisableFormatting);
            }

            return string.Empty;
        }

        /// <summary>
        /// 获取XML节点对象的值，并将其转换为特定类型。
        /// </summary>
        /// <typeparam name="T">要将其转换到的类型。</typeparam>
        /// <param name="xObject">XML父节点对象。</param>
        /// <returns>返回XML节点对象值。根据节点类型的不同，对应节点值的格式可能不同，
        /// 因此调用者应决定是否通过此方法获取转换后的值，否则可能会引发格式化异常。</returns>
        public static T Evaluate<T>(this XObject xObject) where T : struct
        {
            if (xObject == null)
            {
                return default;
            }

            var valueStr = xObject.Evaluate();
            var value = valueStr.ToValue<T>();
            return value;
        }

        /// <summary>
        /// 获取XML节点对象的值，并尝试将其转换为特定类型。
        /// </summary>
        /// <typeparam name="T">要将其转换到的类型。</typeparam>
        /// <param name="xObject">XML父节点对象。</param>
        /// <param name="value">返回成功转换后XML节点对象的值，转换失败后返回类型的默认值。
        /// 根据节点类型的不同，对应节点值的格式可能不同。</param>
        /// <returns>转换成功时返回true,转换失败时返回false。</returns>
        public static bool TryEvaluate<T>(this XObject xObject, out T value) where T : struct
        {
            return TryEvaluate(xObject, out value, default);
        }

        /// <summary>
        /// 获取XML节点对象的值，并尝试将其转换为特定类型。
        /// </summary>
        /// <typeparam name="T">要将其转换到的类型。</typeparam>
        /// <param name="xObject">XML父节点对象。</param>
        /// <param name="defaultValue">转换失败时返回的默认值。</param>
        /// <param name="value">返回XML节点对象值。根据节点类型的不同，对应节点值的格式可能不同。</param>
        /// <returns>转换成功时返回true,转换失败时返回false。</returns>
        public static bool TryEvaluate<T>(this XObject xObject, out T value, T defaultValue) where T : struct
        {
            value = defaultValue;
            if (xObject == null)
            {
                return false;
            }

            var valueStr = xObject.Evaluate();
            return ParserCache<T>.TryParse(valueStr, out value);
        }

        /// <summary>
        /// 获取XML节点对象值的集合。
        /// </summary>
        /// <param name="elements">XML节点对象集合。</param>
        /// <returns> 返回XML节点对象值的集合,节点集合为null或不包含任何节点时将返回空的值的集合。</returns>
        public static IEnumerable<string> EvaluateAll(this IEnumerable<XElement> elements)
        {
            if (elements == null)
            {
                return Enumerable.Repeat(string.Empty, 0);
            }

            return elements.Select(x => x.Evaluate());
        }

        /// <summary>
        /// 获取XML节点对象值的集合。
        /// </summary>
        /// <typeparam name="T">要将节点值转换到的类型。</typeparam>
        /// <param name="elements">XML父节点对象集合。</param>
        /// <returns> 返回XML节点对象值的集合,节点集合为null或不包含任何节点时将返回空的值的集合。</returns>
        public static IEnumerable<T> EvaluateAll<T>(this IEnumerable<XElement> elements) where T : struct
        {
            if (elements == null)
            {
                return Enumerable.Repeat<T>(default, 0);
            }

            return elements.Select(x => x.Evaluate<T>());
        }

        /// <summary>
        /// 获取XML节点对象的子节点的值，并将其所有子节点名称和值作为字典集合。
        /// <para>
        /// 若存在重复名称的子节点，将采用最后一次出现的子节点的值。不包含子节点下的节点。
        /// </para>
        /// </summary>
        /// <param name="xObject">XML父节点对象。</param>
        /// <returns>返回所有子节点名称和值作为字典集合，如不存在子节点则返回空的字典集合。</returns>
        public static XValues EvaluateAll(this XContainer xObject)
        {
            if (xObject == null)
            {
                return XValues.Empty;
            }

            var elementValues = new Dictionary<string, string>();

            var children = xObject.Elements();
            if (children.Any())
            {
                foreach (var child in children)
                {
                    if (elementValues.ContainsKey(child.Name.LocalName))
                    {
                        elementValues[child.Name.LocalName] = child.Value;
                    }
                    else
                    {
                        elementValues.Add(child.Name.LocalName, child.Value);
                    }
                }
            }

            var xValues = new XValues(elementValues);
            return xValues;
        }

        /// <summary>
        /// 获取XML元素节点对象的特性字典集合。
        /// </summary>
        /// <param name="xObject">XML父元素节点对象。</param>
        /// <returns>返回XML特性字典集合，不存在任何特性将返回空字典集合。</returns>
        public static XValues EvaluateAttributes(this XElement xObject)
        {
            if (xObject == null)
            {
                return XValues.Empty;
            }

            Dictionary<string, string> attDic = new Dictionary<string, string>();
            if (xObject.HasAttributes)
            {
                IEnumerable<XAttribute> xAtts = xObject.Attributes();
                if (xAtts != null)
                {
                    attDic = xAtts.ToDictionary(att => att.Name.LocalName, att => att.Value);
                }
            }

            var atts = new XValues(attDic);

            return atts;
        }

        /// <summary>
        /// 获取元素节点上指定特性的值。
        /// </summary>
        /// <param name="xObject">元素节点。</param>
        /// <param name="attribtueName">特性名称。</param>
        /// <returns>若存在指定名称的特性则返回该特性的值，否则返回 <see cref="string.Empty"/>。</returns>
        public static string EvaluateAttribute(this XElement xObject, string attribtueName)
        {
            if (xObject == null || !xObject.HasAttributes)
            {
                return string.Empty;
            }

            var att = xObject.Attribute(attribtueName);
            if (att != null)
            {
                return att.Value;
            }

            return string.Empty;
        }

        /// <summary>
        /// 获取XML元素节点上指定属性的值，并将其转换为具体类型。
        /// </summary>
        /// <typeparam name="T">需要将属性值转换到的类型。</typeparam>
        /// <param name="xObject">XML元素节点。</param>
        /// <param name="attributeName">属性名称。</param>
        /// <returns>存在指定名称的特性且转换成功则返回其值，否则类型<typeparamref name="T"/>的默认值。</returns>
        public static T EvaluateAttribute<T>(this XElement xObject, string attributeName) where T : struct
        {
            T result = default;

            if (xObject == null || !xObject.HasAttributes)
            {
                return result;
            }

            var att = xObject.Attribute(attributeName);
            if (att != null && !string.IsNullOrWhiteSpace(att.Value))
            {
                result = att.Value.ToValue<T>();
                return result;
            }

            return result;
        }

        /// <summary>
        /// 获取XML元素节点上指定属性的值，并尝试将其转换为具体类型。
        /// </summary>
        /// <typeparam name="T">需要将属性值转换到的类型。</typeparam>
        /// <param name="xObject">XML元素节点。</param>
        /// <param name="attributeName">属性名称。</param>
        /// <param name="value">存在指定名称的特性且转换成功则返回其值，否则类型<typeparamref name="T"/>的默认值。</param>
        /// <returns>存在指定名称的特性且转换成功则返回true，否则返回false。</returns>
        public static bool TryEvaluateAttribute<T>(this XElement xObject, string attributeName, out T value) where T : struct
        {
            return TryEvaluateAttribute<T>(xObject, attributeName, out value, default);
        }

        /// <summary>
        /// 获取XML元素节点上指定属性的值，并尝试将其转换为具体类型。
        /// </summary>
        /// <typeparam name="T">需要将属性值转换到的类型。</typeparam>
        /// <param name="xObject">XML元素节点。</param>
        /// <param name="attributeName">属性名称。</param>
        /// <param name="value">存在指定名称的特性且转换成功则返回其值，否则返回 <paramref name="defaultValue"/>。</param>
        /// <param name="defaultValue">转换失败或不存在该特性时返回的默认值。</param>
        /// <returns>存在指定名称的特性且转换成功则返回true，否则返回false。</returns>
        public static bool TryEvaluateAttribute<T>(this XElement xObject, string attributeName, out T value, T defaultValue) where T : struct
        {
            value = defaultValue;
            if (xObject == null || !xObject.HasAttributes)
            {
                return false;
            }

            var att = xObject.Attribute(attributeName);
            if (att != null && !string.IsNullOrWhiteSpace(att.Value))
            {
                value = att.Value.ToValue<T>();
                return true;
            }

            return false;
        }

        public static Object Deserialize(this XmlNode xmlNode, Type type)
        {
            var xroot = type.GetCustomAttribute<XmlRootAttribute>();
            if (xroot == null)
            {
                var xtype = type.GetCustomAttribute<XmlTypeAttribute>();
                if (xtype != null && xmlNode.NodeType == XmlNodeType.Element)
                {
                    return Deserialize(xmlNode, type, new XmlQualifiedName(xmlNode.LocalName, xmlNode.NamespaceURI));
                }
            }

            using (var reader = new XmlNodeReader(xmlNode))
            {
                return Deserialize(reader, type);
            }
        }

        public static Object Deserialize(this XmlNode xmlNode, Type type, XmlQualifiedName root)
        {
            using (var reader = new XmlNodeReader(xmlNode))
            {
                return Deserialize(reader, type, root);
            }
        }

        public static T Deserialize<T>(this XmlNode xmlNode)
        {
            return (T)Deserialize(xmlNode, typeof(T));
        }

        public static T Deserialize<T>(this XmlNode xmlNode, XmlQualifiedName root)
        {
            using (var reader = new XmlNodeReader(xmlNode))
            {
                return Deserialize<T>(reader, root);
            }
        }

        public static Object Deserialize(this XNode xNode, Type type)
        {
            var xroot = type.GetCustomAttribute<XmlRootAttribute>();
            if (xroot == null)
            {
                var xtype = type.GetCustomAttribute<XmlTypeAttribute>();
                if (xtype != null && xNode.NodeType == XmlNodeType.Element)
                {
                    var xElement = (XElement)xNode;
                    return Deserialize(xNode, type, xElement.Name);
                }
            }
            using (var reader = xNode.CreateReader())
            {
                return Deserialize(reader, type);
            }
        }

        public static Object Deserialize(this XNode xNode, Type type, XName root)
        {
            using (var reader = xNode.CreateReader())
            {
                return Deserialize(reader, type, root);
            }
        }

        public static T Deserialize<T>(this XNode xNode)
        {
            return (T)Deserialize(xNode, typeof(T));
        }

        public static T Deserialize<T>(this XNode xNode, XName root)
        {
            return (T)Deserialize(xNode, typeof(T), root);
        }

        public static Object Deserialize(this XmlReader xmlReader, Type type)
        {
            var serializer = new XmlSerializer(type);
            return serializer.Deserialize(xmlReader);
        }

        public static Object Deserialize(this XmlReader xmlReader, Type type, string rootName, string rootNamespace)
        {
            var rootAttr = new XmlRootAttribute(rootName);
            rootAttr.Namespace = rootNamespace;
            var serializer = new XmlSerializer(type, rootAttr);
            return serializer.Deserialize(xmlReader);
        }

        public static Object Deserialize(this XmlReader xmlReader, Type type, XmlQualifiedName root)
        {
            if (root != null)
            {
                return Deserialize(xmlReader, type, root.Name, root.Namespace);
            }
            return Deserialize(xmlReader, type);
        }

        public static Object Deserialize(this XmlReader xmlReader, Type type, XName root)
        {
            if (root != null)
            {
                return Deserialize(xmlReader, type, root.LocalName, root.NamespaceName);
            }
            return Deserialize(xmlReader, type);
        }

        public static T Deserialize<T>(this XmlReader xmlReader)
        {
            return (T)Deserialize(xmlReader, typeof(T));
        }

        public static T Deserialize<T>(this XmlReader xmlReader, XmlQualifiedName root)
        {
            return (T)Deserialize(xmlReader, typeof(T), root);
        }

        public static T Deserialize<T>(this XmlReader xmlReader, XName root)
        {
            return (T)Deserialize(xmlReader, typeof(T), root.LocalName, root.NamespaceName);
        }

        public static XmlElement Serialize(this object obj)
        {
            var serializer = new XmlSerializer(obj.GetType());
            var xmlDoc = new XmlDocument();
            using (var writer = xmlDoc.CreateNavigator().AppendChild())
            {
                serializer.Serialize(writer, obj);
            }
            return xmlDoc.DocumentElement;
        }

        public static XmlElement Serialize<T>(this T obj)
        {
            var ns = new XmlSerializerNamespaces();
            var xroot = typeof(T).GetCustomAttribute<XmlRootAttribute>();
            if (xroot != null)
            {
                ns.Add("", xroot.Namespace);
            }
            else
            {
                ns.Add("", "");
            }
            return obj.Serialize(ns);
        }

        public static XElement SerializeAsXElement<T>(this T obj)
        {
            var ns = new XmlSerializerNamespaces();
            var xroot = typeof(T).GetCustomAttribute<XmlRootAttribute>();
            if (xroot != null)
            {
                ns.Add("", xroot.Namespace);
            }
            else
            {
                ns.Add("", "");
            }
            return obj.SerializeAsXElement(ns);
        }

        public static XElement SerializeAsXElement<T>(this T obj, XmlSerializerNamespaces ns)
        {
            var serializer = new XmlSerializer(typeof(T));
            var xd = new XDocument();
            using (var writer = xd.CreateWriter())
            {
                serializer.Serialize(writer, obj, ns);
            }
            var root = xd.Root;
            root.Remove();
            return root;
        }

        public static XmlElement Serialize<T>(this T obj, XmlSerializerNamespaces ns)
        {
            var serializer = new XmlSerializer(typeof(T));
            var xmlDoc = new XmlDocument();
            using (var writer = xmlDoc.CreateNavigator().AppendChild())
            {
                serializer.Serialize(writer, obj, ns);
            }
            return xmlDoc.DocumentElement;
        }

        public static XmlElement Serialize<T>(this T obj, XmlQualifiedName root)
        {
            var rootAttr = new XmlRootAttribute(root.Name);
            rootAttr.Namespace = root.Namespace;
            var serializer = new XmlSerializer(typeof(T), rootAttr);
            var xmlDoc = new XmlDocument();

            using (var writer = xmlDoc.CreateNavigator().AppendChild())
            {
                serializer.Serialize(writer, obj);
            }

            return xmlDoc.DocumentElement;
        }

        public static XmlElement ToXmlElement(this XElement xelement)
        {
            var xmlDoc = new XmlDocument();
            using (var reader = xelement.CreateReader())
            {
                xmlDoc.Load(reader);
            }
            return xmlDoc.DocumentElement;
        }

        public static XName ToXName(this XmlQualifiedName qname)
        {
            return XName.Get(qname.Name, qname.Namespace);
        }
    }
}
