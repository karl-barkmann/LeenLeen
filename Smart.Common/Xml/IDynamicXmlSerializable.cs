namespace Leen.Common.Xml
{
    /// <summary>
    /// 表示一种支持动态运行时序列化的接口。
    /// <para>
    /// 允许从 <see cref="DynamicXml"/> 反序列化此接口的实例或通过此接口序列化 <see cref="DynamicXml"/> 对象。
    /// </para>
    /// </summary>
    public interface IDynamicXmlSerializable
    {
        /// <summary>
        /// 允许从对象的 <see cref="DynamicXml"/> 表现形式生成该对象。
        /// </summary>
        /// <param name="xml">从中对对象进行反序列化的动态XML对象。</param>
        /// <returns></returns>
        void ReadXml(DynamicXml xml);

        /// <summary>
        /// 将对象转换为其 <see cref="DynamicXml"/> 的表示形式。
        /// </summary>
        /// <returns>对象要序列化为的动态XML对象。</returns>
        void WriteXml(DynamicXml xml);
    }
}
