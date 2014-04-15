/* * * * * * * * * * * * * * * * * * * * * * * * * * * 
 * 作者：李平
 * 日期：2012/3/21 11:09:21
 * 描述：应用程序运行时信息。
 * * * * * * * * * * * * * * * * * * * * * * * * * * */

using Microsoft.VisualBasic.Devices;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Xunmei.Smart.Common.Utils;

namespace Xunmei.Smart.Common
{
    /// <summary>
    /// 应用程序运行时信息。
    /// </summary>
    public class ApplicationRuntime : IDisposable
    {
        #region 成员变量

        private bool disposed;
        private static Thread timelyInfoUpdateWorker;
        private static TimeSpan timelyInfoUpdateInterval = TimeSpan.FromSeconds(1);
        private static bool keepUpdating;
        private static RuntimeTimelyInfo timelyInfo;
        private static PerformanceCounter performanceCounter = new PerformanceCounter(
                "Processor", "% Processor Time", "_Total");

        #endregion

        #region 构造函数

        private ApplicationRuntime()
        { }

        static ApplicationRuntime()
        {
            Start();
        }

        #endregion

        #region 公共事件

        /// <summary>
        /// 运行时及时信息更新事件委托。
        /// </summary>
        /// <param name="e">运行时及时信息。</param>
        public delegate void TimelyInfoUpdatedEvent(TimelyInfoUpdatedEventArgs e);

        /// <summary>
        /// 当运行时及时信息更新时发生。
        /// </summary>
        public static event TimelyInfoUpdatedEvent TimelyInfoUpdated;

        #endregion

        #region 公共属性

        /// <summary>
        /// 获取客户端主机地址，不是Lookback地址。
        /// </summary>
        public static string LocalHostAddress
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取客户端本地主机名。
        /// </summary>
        public static string LocalHostName
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取客户端启动时的系统时间。
        /// </summary>
        public static DateTime StartUpTime
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取客户端是否正在退出，当正在退出客户端时该值为true，其他时间应始终为false。
        /// </summary>
        public static bool IsExiting
        {
            get;
            set;
        }

        /// <summary>
        /// 获取客户端主机的总物理内存值（单位为M）。
        /// </summary>
        public static float TotalPhysicalMemory
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取应用程序运行时及时信息。
        /// </summary>
        public static RuntimeTimelyInfo TimelyInfo
        {
            get
            {
                RuntimeTimelyInfo timelyInfo = new RuntimeTimelyInfo();
                float usage = GetCpuUsage();
                float privatePhysicalMemory =
                    (float)(Process.GetCurrentProcess().PrivateMemorySize64 / 1024.00 / 1024.00);
                timelyInfo.CpuUsage = usage;
                timelyInfo.PrivatePhysicalMemory = privatePhysicalMemory;
                return timelyInfo;
            }
        }

        #endregion

        #region 私有方法

        private static void Start()
        {
            ComputerInfo computerInfo = new ComputerInfo();
            TotalPhysicalMemory =
                (float)(computerInfo.TotalPhysicalMemory / 1024.00 / 1024.00);
            StartUpTime = Process.GetCurrentProcess().StartTime;
            LocalHostName = Dns.GetHostName();
            LocalHostAddress = NetUtils.GetHostAddress().ToString();

            keepUpdating = true;
            if (timelyInfoUpdateWorker == null)
            {
                timelyInfoUpdateWorker = new Thread(new ThreadStart(UpdateRuntimeTimelyInfo));
                timelyInfoUpdateWorker.IsBackground = true;
                timelyInfoUpdateWorker.Name = "运行时及时信息工作线程";
                timelyInfoUpdateWorker.Start();
            }
        }

        private static void UpdateRuntimeTimelyInfo()
        {
            while (keepUpdating)
            {
                float usage = GetCpuUsage();
                float privatePhysicalMemory =
                    (float)(Process.GetCurrentProcess().PrivateMemorySize64 / 1024.00 / 1024.00);
                ComputerInfo computerInfo = new ComputerInfo();
                float usedPhysicalMemory =
                    (float)((computerInfo.TotalPhysicalMemory - computerInfo.AvailablePhysicalMemory) / 1024.00 / 1024.00);
                timelyInfo = new RuntimeTimelyInfo();
                timelyInfo.CpuUsage = usage;
                timelyInfo.PrivatePhysicalMemory = privatePhysicalMemory;
                timelyInfo.UsedPhysicalMemory = usedPhysicalMemory;

                TimelyInfoUpdatedEventArgs e = new TimelyInfoUpdatedEventArgs(timelyInfo);
                OnTimelyInfoUpdated(e);

                Thread.Sleep(timelyInfoUpdateInterval);
            }
        }

        private static float GetUsedPhysicalMemory()
        {
            float usedPhysicalMemory = 0;
            foreach (Process process in Process.GetProcesses())
            {
                usedPhysicalMemory += (float)(process.PrivateMemorySize64 / 1024.00 / 1024.00);
                process.Dispose();
            }

            return usedPhysicalMemory;
        }

        private static float GetCpuUsage()
        {
            if (performanceCounter != null)
            {
                return performanceCounter.NextValue();
            }
            return 0;
        }

        private static void BeginGetLocalHostAddress()
        {
            IAsyncResult result = Dns.BeginGetHostAddresses(LocalHostName,
                new AsyncCallback(EndGetLocalHostAddress), null);
        }

        private static void EndGetLocalHostAddress(IAsyncResult ar)
        {
            IPAddress[] ipAddresses = Dns.EndGetHostAddresses(ar);

            var queryResult = from ipAddress in ipAddresses
                              where ipAddress.AddressFamily == AddressFamily.InterNetwork
                              select ipAddress;

            if (queryResult != null && queryResult.Count() > 0)
                LocalHostAddress = queryResult.FirstOrDefault().ToString();
        }

        private static void OnTimelyInfoUpdated(TimelyInfoUpdatedEventArgs args)
        {
            TimelyInfoUpdatedEvent handler = TimelyInfoUpdated;

            if (handler != null)
            {
                handler(args);
            }
        }

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    keepUpdating = false;
                    if (performanceCounter != null)
                    {
                        performanceCounter.Dispose();
                        performanceCounter = null;
                    }
                }
                disposed = true;
            }
        }

        #endregion

        /// <summary>
        /// 资源释放。
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

    /// <summary>
    /// 应用程序运行时的及时变更信息。
    /// </summary>
    public struct RuntimeTimelyInfo
    {
        /// <summary>
        /// 获取客户端主机CPU使用率。
        /// </summary>
        public float CpuUsage
        {
            get;
            set;
        }

        /// <summary>
        /// 获取客户端进程物理内存使用量（单位为M）。
        /// </summary>
        public float PrivatePhysicalMemory
        {
            get;
            set;
        }

        /// <summary>
        /// 获取客户端主机已使用的物理内存量（单位为M）。
        /// </summary>
        public float UsedPhysicalMemory
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 运行时及时信息更新事件参数。
    /// </summary>
    public class TimelyInfoUpdatedEventArgs : EventArgs
    {
        /// <summary>
        /// 构造运行时及时信息更新事件参数。
        /// </summary>
        /// <param name="timelyInfo"></param>
        public TimelyInfoUpdatedEventArgs(RuntimeTimelyInfo timelyInfo)
        {
            TimelyInfo = timelyInfo;
            UpdateTime = DateTime.Now;
        }

        /// <summary>
        /// 应用程序运行时及时信息的更新时间。
        /// </summary>
        public DateTime UpdateTime
        {
            get;
            set;
        }

        /// <summary>
        /// 应用程序运行时及时信息。
        /// </summary>
        public RuntimeTimelyInfo TimelyInfo
        {
            get;
            set;
        }
    }
}
