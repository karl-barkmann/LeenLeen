using Smart.Common;
using Smart.Native;
using System;

namespace Demo
{
    public static class Program
    {
        private static void Main()
        {
            Console.WriteLine(NativeMethodsUtil.IsAnyUserLogon() ? "有用户登陆" : "无用户登陆");

            ApplicationRuntime.TimelyInfoUpdated += ApplicationRuntime_TimelyInfoUpdated;
            Console.WriteLine("System start up time: {0}", ApplicationRuntime.StartUpTime);
            Console.WriteLine("Host name: {0}", ApplicationRuntime.LocalHostName);
            Console.WriteLine("Host address: {0}", ApplicationRuntime.LocalHostAddress);

            Console.ReadLine();

        }

        static void ApplicationRuntime_TimelyInfoUpdated(TimelyInfoUpdatedEventArgs e)
        {
            Console.WriteLine("CPU: {0}%", Math.Round(e.TimelyInfo.CpuUsage, 0));
            Console.WriteLine("Memory: {0} / {1} MB",
                              Math.Round(e.TimelyInfo.PrivatePhysicalMemory, 1),
                              Math.Round(e.TimelyInfo.UsedPhysicalMemory, 1));
        }
    }
}
