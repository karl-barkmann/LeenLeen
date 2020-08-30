/* * * * * * * * * * * * * * * * * * * * * * * * * * * 
 * 作者：李平
 * 日期：2012/2/24 14:02:52
 * 描述：使用netstat管道命令获取进程及端口信息。
 * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace Leen.Common.Utils
{
    /// <summary>
    /// 使用netstat管道命令获取进程及端口信息。
    /// </summary>
    public static class NetStatHelper
    {
        #region Members

        private const string FINDLISTENINGPROCESS = "netstat -nao |find /i \"listen\"";

        #endregion

        /// <summary>
        /// 获取端口是否可用。
        /// </summary>
        /// <param name="port">端口号</param>
        /// <returns>可用返回true，否则false。</returns>
        /// <exception cref="System.ArgumentException">端口号指定错误。</exception>
        public static bool PortCanUse(int port)
        {
            Process process = GetListeningProcess(port);
            if (process == null || process.HasExited)
                return true;

            return false;
        }

        /// <summary>
        /// 获取当前进程正在监听的端口。
        /// </summary>
        /// <returns>没有监听端口时，返回null。</returns>
        public static int[] GetListeningPort()
        {
            var ports = GetListeningPorts();
            using (Process process = Process.GetCurrentProcess())
            {
                foreach (var item in ports)
                {
                    if (item.Key == process.Id)
                    {
                        return item.Value;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 获取正在监听指定端口的进程信息。
        /// </summary>
        /// <param name="port">端口号</param>
        /// <returns>没有找到监听指定端口的进程时，将返回空(null)。</returns>
        /// <exception cref="System.ArgumentException">端口号指定错误。</exception>
        public static Process FindListeningProcess(int port)
        {
            if (port < 1 || port > 65536)
                throw new ArgumentException("port", "端口号指定错误。");
            Process process = GetListeningProcess(port);
            return process;
        }

        #region 私有方法

        private static List<string> DoNetSateCommand(string commandline)
        {
            Process netStateProcess = new Process();

            ProcessStartInfo startInfo = new ProcessStartInfo("cmd.exe");
            startInfo.Arguments = "/c " + commandline;
            startInfo.CreateNoWindow = true;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.RedirectStandardOutput = true;
            startInfo.UseShellExecute = false;

            netStateProcess.EnableRaisingEvents = true;
            netStateProcess.StartInfo = startInfo;
            netStateProcess.Start();

            //获取输出流
            List<String> netStateOutPuts = new List<string>();

            if (netStateProcess.StandardOutput != null)
            {
                string output = netStateProcess.StandardOutput.ReadToEnd();
                if (output != null && output.Length > 0)
                {
                    netStateOutPuts = new List<string>(output.Split(
                        Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
                }
            }

            //等待响应
            netStateProcess.WaitForExit();

            return netStateOutPuts;
        }

        private static IDictionary<int, int[]> GetListeningPorts()
        {
            List<string> netStateOutPuts = DoNetSateCommand(FINDLISTENINGPROCESS);

            IDictionary<int, int[]> ports = new Dictionary<int, int[]>();

            if (netStateOutPuts == null || netStateOutPuts.Count < 1)
                return null;

            foreach (string result in netStateOutPuts)
            {
                int processId = Analyse(result, out int port);
                if (ports.ContainsKey(port))
                {
                    List<int> existsPorts = new List<int>(ports[processId])
                    {
                        port
                    };

                    ports[processId] = existsPorts.ToArray();
                }
                else
                {
                    ports.Add(processId, new int[] { port });
                }
            }

            return ports;
        }

        private static Process GetListeningProcess(int port)
        {
            if (port < 1 || port > 65536)
                return null;

            int processId = -1;

            List<string> netStateOutPuts = DoNetSateCommand(FINDLISTENINGPROCESS);

            if (netStateOutPuts == null || netStateOutPuts.Count < 1)
                return null;

            foreach (string result in netStateOutPuts)
            {
                processId = Analyse(result, port);
                if (processId != -1)
                    break;
            }

            if (processId == -1)
                return null;

            try
            {
                return Process.GetProcessById(processId);
            }
            catch (ArgumentException argumentException)
            {
                Debug.WriteLine(
                    String.Format("已查找到{0}端口占用进程，但进程可能已退出。参考:"
                        + argumentException.Message, port));
                return null;
            }
        }

        private static int Analyse(string netStatResult, int targetPort)
        {
            int proccessId = -1;
            netStatResult = netStatResult.Trim();

            string[] results = netStatResult.Split(new char[] { ' ' },
                StringSplitOptions.RemoveEmptyEntries);

            if (results[0].ToUpper() != "TCP")
                return proccessId;
            IPEndPoint localEndPoint = GetIPEndPoint(results[1]);
            if (localEndPoint == null)
                return proccessId;
            if (targetPort == localEndPoint.Port)
                proccessId = Convert.ToInt32(results[4]);

            return proccessId;
        }

        private static int Analyse(string netStatResult, out int targetPort)
        {
            int proccessId = -1;
            targetPort = 0;
            netStatResult = netStatResult.Trim();

            string[] results = netStatResult.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (results[0].ToUpper() != "TCP")
                return proccessId;

            IPEndPoint localEndPoint = GetIPEndPoint(results[1]);
            if (localEndPoint == null)
                return proccessId;

            targetPort = localEndPoint.Port;
            proccessId = Convert.ToInt32(results[4]);

            return proccessId;
        }

        private static IPEndPoint GetIPEndPoint(string localEndPoint)
        {
            string[] endPoint = new string[2];
            int index = localEndPoint.LastIndexOf(':');
            endPoint[0] = localEndPoint.Substring(0, index);
            endPoint[1] = localEndPoint.Substring(index + 1);


            IPAddress localAddress = null;
            int localPort = Convert.ToInt32(endPoint[1]);

            string localHostName = endPoint[0];

            if (localHostName == "0.0.0.0" || localHostName == "[::]")
                localHostName = "127.0.0.1";

            IPAddress[] adAddresses = Dns.GetHostAddresses(localHostName);

            if (adAddresses != null || adAddresses.Length > 0)
                localAddress = adAddresses[0];

            return new IPEndPoint(localAddress, localPort);
        }

        #endregion

        /// <summary>
        /// netstat输出信息。
        /// </summary>
        private class NetStatResult
        {
            /// <summary>
            /// 获取一条netstat输出信息。
            /// </summary>
            /// <param name="netStatResult"></param>
            public NetStatResult(string netStatResult)
            {
                Analyse(netStatResult);
            }

            /// <summary>
            /// 协议类型
            /// </summary>
            public ProtocolType Protocol
            {
                get;
                set;
            }

            /// <summary>
            /// 本地网络端点
            /// </summary>
            public IPEndPoint LocalEndPoint
            {
                get;
                set;
            }

            /// <summary>
            /// 远程网络端点
            /// </summary>
            public IPEndPoint RemoteEndPoint
            {
                get;
                set;
            }

            /// <summary>
            /// netstat状态
            /// </summary>
            public string Status
            {
                get;
                set;
            }

            /// <summary>
            /// 进程ID
            /// </summary>
            public int PID
            {
                get;
                set;
            }

            private void Analyse(string netStatResult)
            {
                netStatResult = netStatResult.Trim();

                string[] results = netStatResult.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                switch (results[0].ToUpper())
                {
                    case "TCP":
                    default:
                        Protocol = ProtocolType.Tcp;
                        break;
                }

                LocalEndPoint = GetIPEndPoint(results[1]);
                RemoteEndPoint = GetIPEndPoint(results[2]);
                Status = results[3];
                PID = Convert.ToInt32(results[4]);
            }

            private IPEndPoint GetIPEndPoint(string localEndPoint)
            {
                string[] endPoint = new string[2];
                int index = localEndPoint.LastIndexOf(':');
                endPoint[0] = localEndPoint.Substring(0, index);
                endPoint[1] = localEndPoint.Substring(index + 1);


                IPAddress localAddress = null;
                int localPort = Convert.ToInt32(endPoint[1]);

                string localHostName = endPoint[0];

                if (localHostName == "0.0.0.0" || localHostName == "[::]")
                    localHostName = "127.0.0.1";

                IPAddress[] adAddresses = Dns.GetHostAddresses(localHostName);

                if (adAddresses != null || adAddresses.Length > 0)
                    localAddress = adAddresses[0];

                return new IPEndPoint(localAddress, localPort);
            }
        }
    }
}
