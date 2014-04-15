using System.Net;

namespace Smart.Common.Utils
{
    /// <summary>
    /// 网络帮助类。
    /// </summary>
    public class NetUtils
    {
        /// <summary>
        /// 获取非Loopback的本机IPV4地址。
        /// </summary>
        /// <returns></returns>
        public static IPAddress GetHostAddress()
        {
            var addresses = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (var item in addresses)
            {
                if (item == IPAddress.Loopback || item.IsIPv6LinkLocal ||
                    item.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork)
                    continue;
                return item;
            }
            return null;
        }
    }
}
