using System;
using System.Runtime.Serialization;

namespace Leen.Media
{
    /// <summary>
    /// 表示媒体播放设备不可用的错误。
    /// <para>
    /// 通常表示计算机系统设备不可用、设备驱动不支持或未检测到DirectX运行环境。
    /// </para>
    /// </summary>
    [Serializable]
    public class MediaDeviceUnavailableException : MediaException
    {
        /// <summary>
        /// 构造 <see cref="MediaDeviceUnavailableException"/> 类的实例。
        /// </summary>
        public MediaDeviceUnavailableException()
        {

        }

        /// <summary>
        /// 构造 <see cref="MediaDeviceUnavailableException"/> 类的实例。
        /// </summary>
        /// <param name="message">错误描述。</param>
        public MediaDeviceUnavailableException(string message) : base(message)
        {

        }

        /// <summary>
        /// 构造 <see cref="MediaDeviceUnavailableException"/> 类的实例。
        /// </summary>
        /// <param name="errorCode">错误码。</param>
        public MediaDeviceUnavailableException(int errorCode) : base(errorCode, string.Empty)
        {

        }

        /// <summary>
        /// 构造 <see cref="MediaDeviceUnavailableException"/> 类的实例。
        /// </summary>
        /// <param name="errorCode">错误码。</param>
        /// <param name="message">错误描述。</param>
        public MediaDeviceUnavailableException(int errorCode, string message) : base(message)
        {

        }

        /// <summary>
        /// 构造 <see cref="MediaDeviceUnavailableException"/> 类的实例。
        /// </summary>
        /// <param name="message">错误描述。</param>
        /// <param name="inner">与此错误关联的内部异常对象。</param>
        public MediaDeviceUnavailableException(string message, Exception inner) : base(message, inner)
        {

        }

        /// <summary>
        /// 构造 <see cref="MediaDeviceUnavailableException"/> 类的实例。
        /// </summary>
        /// <param name="errorCode">错误码。</param>
        /// <param name="message">错误描述。</param>
        /// <param name="inner">与此错误关联的内部异常对象。</param>
        public MediaDeviceUnavailableException(int errorCode, string message, Exception inner) : base(message, inner)
        {

        }

        /// <summary>
        /// 构造 <see cref="MediaDeviceUnavailableException"/> 类的实例。
        /// </summary>
        /// <param name="errorCode">错误码。</param>
        /// <param name="inner">与此错误关联的内部异常对象。</param>
        public MediaDeviceUnavailableException(int errorCode, Exception inner) : base(errorCode, string.Empty, inner)
        {

        }

        /// <summary>
        /// 用序列化数据初始化 <see cref="MediaDeviceUnavailableException"/> 的新实例。
        /// </summary>
        /// <param name="serializationInfo">包含有关所引发异常的序列化对象数据的 <see cref="SerializationInfo"/> </param>
        /// <param name="streamingContext">包含关于源或目标的上下文信息</param>
        protected MediaDeviceUnavailableException(SerializationInfo serializationInfo, StreamingContext streamingContext)
        {
        }
    }
}
