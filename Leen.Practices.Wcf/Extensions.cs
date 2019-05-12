using System.Linq;
using System.Net;
using System.ServiceModel.Channels;

namespace Leen.Practices.Wcf
{
    /// <summary>
    /// WCF相关扩展方法。
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// 获取当前消息请求的远程网络端点。
        /// </summary>
        /// <param name="messageProperties"></param>
        /// <returns></returns>
        public static IPEndPoint ResolveIncomingEndPoint(this MessageProperties messageProperties)
        {
            object property = messageProperties.Values.First(
                (messageProperty) => { return messageProperty is RemoteEndpointMessageProperty; });

            RemoteEndpointMessageProperty remoteEndPointMessage =
                property as RemoteEndpointMessageProperty;

            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(remoteEndPointMessage.Address),
                remoteEndPointMessage.Port);

            return ipEndPoint;
        }
    }
}
