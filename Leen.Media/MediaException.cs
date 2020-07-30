using System;
using System.Runtime.Serialization;

namespace Leen.Media
{
    /// <summary>
    /// 表示在媒体播放间发生的错误。
    /// </summary>
    [Serializable]
    public class MediaException : Exception
    {
        /// <summary>
        /// 构造 <see cref="MediaException"/> 类的实例。
        /// </summary>
        public MediaException()
        {

        }

        /// <summary>
        /// 构造 <see cref="MediaException"/> 类的实例。
        /// </summary>
        /// <param name="message">错误描述。</param>
        public MediaException(string message) : base(message)
        {

        }

        /// <summary>
        /// 构造 <see cref="MediaException"/> 类的实例。
        /// </summary>
        /// <param name="errorCode">错误码。</param>
        public MediaException(int errorCode) : this(errorCode, string.Empty)
        {

        }

        /// <summary>
        /// 构造 <see cref="MediaException"/> 类的实例。
        /// </summary>
        /// <param name="errorCode">错误码。</param>
        /// <param name="message">错误描述。</param>
        public MediaException(int errorCode, string message) : base(message)
        {
            ErrorCode = errorCode;
        }

        /// <summary>
        /// 构造 <see cref="MediaException"/> 类的实例。
        /// </summary>
        /// <param name="message">错误描述。</param>
        /// <param name="inner">与此错误关联的内部异常对象。</param>
        public MediaException(string message, Exception inner) : base(message, inner)
        {

        }

        /// <summary>
        /// 构造 <see cref="MediaException"/> 类的实例。
        /// </summary>
        /// <param name="errorCode">错误码。</param>
        /// <param name="message">错误描述。</param>
        /// <param name="inner">与此错误关联的内部异常对象。</param>
        public MediaException(int errorCode, string message, Exception inner) : base(message, inner)
        {
            ErrorCode = errorCode;
        }

        /// <summary>
        /// 构造 <see cref="MediaException"/> 类的实例。
        /// </summary>
        /// <param name="errorCode">错误码。</param>
        /// <param name="inner">与此错误关联的内部异常对象。</param>
        public MediaException(int errorCode, Exception inner) : this(errorCode, string.Empty, inner)
        {

        }

        /// <summary>
        /// 用序列化数据初始化 <see cref="MediaException"/> 的新实例。
        /// </summary>
        /// <param name="serializationInfo">包含有关所引发异常的序列化对象数据的 <see cref="SerializationInfo"/> </param>
        /// <param name="streamingContext">包含关于源或目标的上下文信息</param>
        protected MediaException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
        {
        }

        /// <summary>
        /// 获取错误对象的错误码。
        /// </summary>
        public int ErrorCode { get; }
    }
}
