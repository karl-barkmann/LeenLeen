using System;

namespace Leen.Common.Xml
{
    /// <summary>
    /// 描述动态运行时XML文档解析时发生的错误。
    /// </summary>
    public class DynamicXmlException : Exception
    {
        private readonly string _elementName;
        private readonly string _parentElement;

        /// <summary>
        /// 构造类型 <see cref="DynamicXmlException"/> 的实例。
        /// </summary>
        /// <param name="elementName">尝试访问的元素节点名称。</param>
        /// <param name="parentElement">尝试访问的元素节点的父级XML文档字符串。</param>
        public DynamicXmlException(string elementName, string parentElement) : base()
        {
            _elementName = elementName;
            _parentElement = parentElement;
        }

        /// <summary>
        /// 构造类型 <see cref="DynamicXmlException"/> 的实例。
        /// </summary>
        /// <param name="message">描述异常的错误信息。</param>
        /// <param name="elementName">尝试访问的元素节点名称。</param>
        /// <param name="parentElement">尝试访问的元素节点的父级XML文档字符串。</param>
        public DynamicXmlException(string message, string elementName, string parentElement) : base(message)
        {
            _elementName = elementName;
            _parentElement = parentElement;
        }

        /// <summary>
        /// 构造类型 <see cref="DynamicXmlException"/> 的实例。
        /// </summary>
        /// <param name="message">描述异常的错误信息。</param>
        /// <param name="innerException">此异常关联的内部异常。</param>
        /// <param name="elementName">尝试访问的元素节点名称。</param>
        /// <param name="parentElement">尝试访问的元素节点的父级XML文档字符串。</param>
        public DynamicXmlException(string message, Exception innerException, string elementName, string parentElement) : base(message, innerException)
        {
            _elementName = elementName;
            _parentElement = parentElement;
        }

        /// <summary>
        /// 获取尝试访问的元素节点名称。
        /// </summary>
        public string ElementName => _elementName;

        /// <summary>
        /// 获取尝试访问的元素节点的父级。
        /// </summary>
        public string ParentElement => _parentElement;
    }
}
