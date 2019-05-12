using System;
using System.Runtime.InteropServices;

namespace Leen.Native
{
    /// <summary>
    /// 封装 Kernel32 API.
    /// </summary>
    public class Kernel32
    {
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释

        public const int
            STD_INPUT_HANDLE = -10,
            STD_OUTPUT_HANDLE = -11,
            STD_ERROR_HANDLE = -12;

#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

        /// <summary>
        /// 获取当前线程标识。
        /// </summary>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        public static extern uint GetCurrentThreadId();

        /// <summary>
        /// Retrieves a module handle for the specified module. The module must have been loaded by the calling process.
        /// </summary>
        /// <param name="name">The name of the loaded module (either a .dll or .exe file).</param>
        /// <returns>If the function succeeds, the return value is a handle to the specified module.
        /// If the function fails, the return value is NULL.To get extended error information, call <see cref="Kernel32.GetLastError()"/>.</returns>
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string name);

        /// <summary>
        /// Retrieves a handle to the specified standard device (standard input, standard output, or standard error).
        /// </summary>
        /// <param name="nStdHandle"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll",
            EntryPoint = "GetStdHandle",
            SetLastError = true,
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr GetStdHandle(int nStdHandle);

        /// <summary>
        /// Allocates a new console for the calling process.
        /// <para>
        /// Check msdn documention for more details.
        /// </para>
        /// </summary>
        /// <returns></returns>
        [DllImport("kernel32.dll",
            EntryPoint = "AllocConsole",
            SetLastError = true,
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AllocConsole();

        /// <summary>
        /// Detaches the calling process from its console.
        /// </summary>
        /// <returns></returns>
        [DllImport("kernel32.dll",
            EntryPoint = "FreeConsole",
            SetLastError = true,
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FreeConsole();

        /// <summary>
        /// Attaches the calling process to the console of the specified process.
        /// </summary>
        /// <param name="processId">
        /// The identifier of the process whose console is to be used. This parameter can be one of the following values:
        ///     pid                         Use the console of the specified process;
        ///     ATTACH_PARENT_PROCESS = -1, Use the console of the parent of the current process.
        /// </param>
        /// <returns></returns>
        [DllImport("kernel32.dll",
            EntryPoint = "AttachConsole",
            SetLastError = true,
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AttachConsole(int processId);

        /// <summary>
        /// Retrieves the calling thread's last-error code value. The last-error code is maintained on a per-thread basis. Multiple threads do not overwrite each other's last-error code.
        /// </summary>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        public static extern uint GetLastError();

        [DllImport("kernel32.dll")]
        public static extern uint GetErrorMode();

        [DllImport("kernel32.dll")]
        public static extern int GetThreadErrorMode();

        [DllImport("kernel32.dll")]
        public static extern uint SetErrorMode(uint mode);

        [DllImport("kernel32.dll")]
        public static extern bool SetThreadErrorMode(int newMode, out int oldMode);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetDllDirectory(string lpPathName);
    }
}

