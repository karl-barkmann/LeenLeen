using System;
using System.Runtime.InteropServices;

namespace Leen.Native
{
    /// <summary>
    /// Some windows hook api.
    /// </summary>
    public class NativeMethods
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate IntPtr HookProc(int code, IntPtr wParam, IntPtr lParam);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate void WinEventProc(IntPtr hWinEventHook, uint eventId, IntPtr hwnd,
            int idObject, int idChild, uint idEventThread, uint dwmsEventTime);

        [DllImport("user32.dll", EntryPoint = "IsWindow")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindow([In] IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "SetWinEventHook")]
        public static extern System.IntPtr SetWinEventHook(uint eventMin, uint eventMax,
            [In] IntPtr hmodWinEventProc, WinEventProc pfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

        [DllImport("user32.dll", EntryPoint = "SetWindowsHookEx")]
        public static extern IntPtr SetWindowsHookExW(int idHook, HookProc lpfn, [In] IntPtr hmod, uint dwThreadId);

        [DllImport("user32.dll", EntryPoint = "GetWindowThreadProcessId")]
        public static extern uint GetWindowThreadProcessId([In] IntPtr hWnd, IntPtr lpdwProcessId);

        [DllImport("user32.dll", EntryPoint = "CallNextHookEx")]
        [return: MarshalAs(UnmanagedType.SysInt)]
        public static extern int CallNextHookEx([In] System.IntPtr hhk, int nCode, [MarshalAs(UnmanagedType.SysUInt)] uint wParam,
            [MarshalAs(UnmanagedType.SysInt)] int lParam);

        [DllImport("kernel32.dll", EntryPoint = "GetModuleHandle")]
        public static extern IntPtr GetModuleHandleA([In] [MarshalAs(UnmanagedType.LPStr)] string lpModuleName);

        /// <summary>
        /// 获取当前进程命令行参数。
        /// </summary>
        /// <returns>命令行参数字符串的指针。</returns>
        [DllImport("kernel32.dll", EntryPoint = "GetCommandLine")]
        public static extern IntPtr GetCommandLine();

        /// <summary>
        /// 获取当前进程的系统唯一标识。
        /// </summary>
        /// <returns></returns>
        [DllImport("kernel32.dll", EntryPoint = "GetCurrentProcessId")]
        public static extern int GetCurrentProcessId();

        [DllImport("kernel32.dll", SetLastError = true,
            CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool CloseHandle(IntPtr handle);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int WTSGetActiveConsoleSessionId();

        [DllImport("userenv.dll", SetLastError = true)]
        public static extern bool CreateEnvironmentBlock(
            out IntPtr lpEnvironment,
            IntPtr hToken,
            bool bInherit);

        [DllImport("userenv.dll", SetLastError = true,
            EntryPoint = "DestroyEnvironmentBlock")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DestroyEnvironmentBlock(IntPtr pEnviroment);

        [DllImport("wtsapi32.dll", SetLastError = true)]
        public static extern bool WTSEnumerateSessions(
            IntPtr hServer,
            uint reserverd,
            uint version,
            out IntPtr sessionInfo,
            out uint count);

        [DllImport("wtsapi32.dll", SetLastError = true)]
        public static extern bool WTSQueryUserToken(
            Int32 sessionId,
            out IntPtr Token);

        [DllImport("wtsapi32.dll", SetLastError = true)]
        public static extern bool WTSSendMessage(
            IntPtr hServer,
            int SessionId,
            String pTitle,
            int TitleLength,
            String pMessage,
            int MessageLength,
            int Style,
            int Timeout,
            out int pResponse,
            bool bWait);

        [DllImport("wtsapi32.dll", SetLastError = true)]
        public static extern void WTSFreeMemory(IntPtr pMemory);

        [DllImport("advapi32.dll", SetLastError = true,
             CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern bool CreateProcessAsUser(
            IntPtr hToken,
            string lpApplicationName,
            string lpCommandLine,
            ref SECURITY_ATTRIBUTES lpProcessAttributes,
            ref SECURITY_ATTRIBUTES lpThreadAttributes,
            bool bInheritHandle,
            Int32 dwCreationFlags,
            IntPtr lpEnvrionment,
            string lpCurrentDirectory,
            ref STARTUPINFO lpStartupInfo,
            ref PROCESS_INFORMATION lpProcessInformation);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool DuplicateTokenEx(
            IntPtr hExistingToken,
            Int32 dwDesiredAccess,
            ref SECURITY_ATTRIBUTES lpThreadAttributes,
            Int32 ImpersonationLevel,
            Int32 dwTokenType,
            ref IntPtr phNewToken);

        [DllImport("advapi32.dll", SetLastError = true,
            EntryPoint = "OpenProcessToken")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool OpenProcessToken(
            [In] IntPtr ProcessHandle,
            uint DesiredAccess,
            out IntPtr TokenHandle);

        [DllImport("advapi32.dll", SetLastError = true,
            EntryPoint = "LookupPrivilegeValue")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool LookupPrivilegeValue(
            [In] [MarshalAs(UnmanagedType.LPStr)] string lpSystemName,
            [In] [MarshalAs(UnmanagedType.LPStr)] string lpName,
            [Out] out LUID lpLuid);

        [DllImport("advapi32.dll", SetLastError = true,
            EntryPoint = "SetTokenInformation")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetTokenInformation(
            [In] IntPtr TokenHandle,
            TOKEN_INFORMATION_CLASS TokenInformationClass,
            [In] IntPtr TokenInformation,
            uint TokenInformationLength);

        [DllImport("advapi32.dll", SetLastError = true,
            EntryPoint = "AdjustTokenPrivileges")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AdjustTokenPrivileges(
            [In] System.IntPtr TokenHandle,
            [MarshalAs(UnmanagedType.Bool)] bool DisableAllPrivileges,
            [In] IntPtr NewState,
            uint BufferLength,
            IntPtr PreviousState,
            IntPtr ReturnLength);

        /// <summary>
        /// 广播消息。
        /// </summary>
        public const int HWND_BROADCAST = 0xffff;

        /// <summary>
        /// WM显示窗体。
        /// </summary>
        public static readonly int WM_SHOWME = RegisterWindowMessage("WM_SHOWME");

        /// <summary>
        /// 广播消息。
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int PostMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        /// <summary>
        /// 查找桌面窗口。
        /// </summary>
        /// <param name="className">类名</param>
        /// <param name="windowName">窗口标题。</param>
        /// <returns>窗口句柄。</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindow(string className, string windowName);

        /// <summary>
        /// 注册消息。
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int RegisterWindowMessage(string message);

        [StructLayout(LayoutKind.Sequential)]
        public struct STARTUPINFO
        {
            public Int32 cb;
            public string lpReserved;
            public string lpDesktop;
            public string lpTitle;
            public Int32 dwX;
            public Int32 dwY;
            public Int32 dwXSize;
            public Int32 dwXCountChars;
            public Int32 dwYCountChars;
            public Int32 dwFillAttribute;
            public Int32 dwFlags;
            public Int16 wShowWindow;
            public Int16 cbReserved2;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WTS_SESSION_INFO
        {
            public uint SessionId;
            public string pWinStationName;
            public WTS_CONNECTSTATE_CLASS State;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PROCESS_INFORMATION
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public Int32 dwProcessID;
            public Int32 dwThreadID;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SECURITY_ATTRIBUTES
        {
            public Int32 Length;
            public IntPtr lpSecurityDescriptor;
            public bool bInheritHandle;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct LUID
        {
            public uint LowPart;

            public int HighPart;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct TOKEN_PRIVILEGES
        {
            public uint PrivilegeCount;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1, ArraySubType = UnmanagedType.Struct)]
            public LUID_AND_ATTRIBUTES[] Privileges;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct LUID_AND_ATTRIBUTES
        {
            public LUID Luid;

            public uint Attributes;
        }

        public const uint NO_ACTIVE_SESSION_FOUND = 0xFFFFFFFF;
        public const int GENERIC_ALL_ACCESS = 0x10000000;
        public const int MAXIMUM_ALLOWED = 0x02000000;
        public const int SE_PRIVILEGE_ENABLED = 2;
        public const int TOKEN_ADJUST_SESSIONID = 256;
        public const int TOKEN_ADJUST_DEFAULT = 128;
        public const int TOKEN_ADJUST_GROUPS = 64;
        public const int TOKEN_ADJUST_PRIVILEGES = 32;
        public const int TOKEN_QUERY = 8;
        public const int TOKEN_DUPLICATE = 2;
        public const int TOKEN_ASSIGN_PRIMARY = 1;
        public const int TOKEN_READ = READ_CONTROL | TOKEN_QUERY;
        public const int TOKEN_WRITE = READ_CONTROL | TOKEN_ADJUST_PRIVILEGES | TOKEN_ADJUST_GROUPS |
            TOKEN_ADJUST_DEFAULT;
        public const int READ_CONTROL = 0x00020000;
        public const string SE_DEBUG_NAME = "SeDebugPrivilege";
        public static readonly IntPtr WTS_CURRENT_SERVER_HANDLE = IntPtr.Zero;

        public enum TOKEN_INFORMATION_CLASS
        {
            TokenUser = 1,

            TokenGroups,

            TokenPrivileges,

            TokenOwner,

            TokenPrimaryGroup,

            TokenDefaultDacl,

            TokenSource,

            TokenType,

            TokenImpersonationLevel,

            TokenStatistics,

            TokenRestrictedSids,

            TokenSessionId,

            TokenGroupsAndPrivileges,

            TokenSessionReference,

            TokenSandBoxInert,

            TokenAuditPolicy,

            TokenOrigin,

            MaxTokenInfoClass,
        }

        public enum TOKEN_TYPE
        {
            TokenPrimary = 1,
            TokenImpersonation
        }

        public enum WTS_CONNECTSTATE_CLASS
        {
            WTSActive,
            WTSConnected,
            WTSConnectQuery,
            WTSShadow,
            WTSDisconnected,
            WTSIdle,
            WTSListen,
            WTSReset,
            WTSDown,
            WTSInit,
        }
    }
}
