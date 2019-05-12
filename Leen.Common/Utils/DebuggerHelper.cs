using Microsoft.Win32.SafeHandles;
using Leen.Native;
using System;
using System.IO;
using System.Text;

namespace Leen.Common.Utils
{
    /// <summary>
    /// 提供一些使用托管调试器的帮助方法。
    /// </summary>
    public static class DebuggerHelper
    {
        /// <summary>
        /// 打开调试控制台。
        /// </summary>
        public static void AllocDebugConsole()
        {
            Kernel32.AllocConsole();
            IntPtr stdHandle = Kernel32.GetStdHandle(Kernel32.STD_OUTPUT_HANDLE);
            SafeFileHandle safeFileHandle = new SafeFileHandle(stdHandle, true);
            FileStream fileStream = new FileStream(safeFileHandle, FileAccess.Write);
            Encoding encoding = Encoding.Default;
            StreamWriter standardOutput = new StreamWriter(fileStream, encoding);
            standardOutput.AutoFlush = true;
            Console.SetOut(standardOutput);
        }
    }
}
