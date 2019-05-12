using Microsoft.Win32;
using System;

namespace Leen.Common.Utils
{
    /// <summary>
    /// 进程内存dump工具类。
    /// </summary>
    public static class DumpHelper
    {
        /// <summary>
        /// 判断相关注册表项是否存在
        /// </summary>
        /// <param name="appName">应用程序进程名称。</param>
        /// <param name="dumpPath"></param>
        /// <returns></returns>
        public static void EnsureRegistryExist(string appName, string dumpPath = "")
        {
            RegistryKey localKey;
            if (Environment.Is64BitOperatingSystem)
                localKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            else
                localKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
            RegistryKey wer = localKey.OpenSubKey(@"SOFTWARE\Microsoft\Windows\Windows Error Reporting", true);
            RegistryKey app = wer.CreateSubKey("LocalDumps\\" + appName, RegistryKeyPermissionCheck.ReadWriteSubTree);
            if (dumpPath == string.Empty) dumpPath = appName;
            app.SetValue("DumpFolder", @"%LOCALAPPDATA%\CrashDumps\" + dumpPath.Trim('\\'), RegistryValueKind.ExpandString);
            app.Close();
            wer.Close();
            localKey.Close();
        }
    }
}
