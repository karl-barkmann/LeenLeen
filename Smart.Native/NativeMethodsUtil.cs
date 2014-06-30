using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace Smart.Native
{
    /// <summary>
    /// 进一步封装非托管API方法的辅助方法。
    /// </summary>
    public class NativeMethodsUtil
    {
        /// <summary>
        /// 在当前用户桌面显示一条消息。
        /// </summary>
        /// <param name="message">消息内容。</param>
        /// <param name="title">消息标题。</param>
        public static void WTSShowMessageBox(string message, string title)
        {
            int resp = 0;
            NativeMethods.WTSSendMessage(
                NativeMethods.WTS_CURRENT_SERVER_HANDLE,
                NativeMethods.WTSGetActiveConsoleSessionId(),
                title, title.Length,
                message, message.Length,
                0, 0, out resp, false);
        }

        /// <summary>
        /// 在当前用户会话下创建进程。
        /// </summary>
        /// <param name="app">进程名(绝对路径)。</param>
        /// <param name="arguments">启动参数。</param>
        /// <param name="callback">创建失败后的回调方法，第一参数表示导致失败的非托管API方法名，第二个参数表示导致失败的错误码。</param>
        /// <returns>如果创建成功则返回进程Id，创建失败返回-1。</returns>
        public static int CreateProcess(string app, string arguments, Action<string, int> callback)
        {
            bool result;
            IntPtr hToken = WindowsIdentity.GetCurrent().Token;
            IntPtr hDupedToken = IntPtr.Zero;

            NativeMethods.PROCESS_INFORMATION pi = new NativeMethods.PROCESS_INFORMATION();
            NativeMethods.SECURITY_ATTRIBUTES sa = new NativeMethods.SECURITY_ATTRIBUTES();
            sa.Length = Marshal.SizeOf(sa);

            NativeMethods.STARTUPINFO si = new NativeMethods.STARTUPINFO();
            si.cb = Marshal.SizeOf(si);
            si.lpDesktop = "winsta0\\default";

            IntPtr winlogonProcessHandle = IntPtr.Zero;

            int dwSessionID = 0;

            IntPtr pSessionInfo = IntPtr.Zero;
            uint sessionCount = 0;
            NativeMethods.WTSEnumerateSessions(
                NativeMethods.WTS_CURRENT_SERVER_HANDLE,
                0,
                1,
                out pSessionInfo,
                out sessionCount);

            int dataSize = Marshal.SizeOf(typeof(NativeMethods.WTS_SESSION_INFO));
            int current = (int)pSessionInfo;
            for (int i = 0; i < sessionCount; i++)
            {
                NativeMethods.WTS_SESSION_INFO sessionInfo =
                    (NativeMethods.WTS_SESSION_INFO)Marshal.PtrToStructure(
                        (IntPtr)current, typeof(NativeMethods.WTS_SESSION_INFO));
                if (NativeMethods.WTS_CONNECTSTATE_CLASS.WTSActive == sessionInfo.State)
                {
                    dwSessionID = (int)sessionInfo.SessionId;
                    break;
                }
                current += dataSize;
            }

            NativeMethods.WTSFreeMemory(pSessionInfo);

            try
            {
                Process[] winlogonProcesses = Process.GetProcessesByName("winlogon");

                foreach (Process process in winlogonProcesses)
                {
                    if (process.SessionId == dwSessionID)
                    {
                        winlogonProcessHandle = process.Handle;
                        break;
                    }
                }
            }
            catch (InvalidOperationException)
            {
                return -1;
            }

            result = NativeMethods.WTSQueryUserToken(dwSessionID, out hToken);

            if (!result)
            {
                if (callback != null)
                {
                    callback("WTSQueryUserToken", Marshal.GetLastWin32Error());
                }
            }

            IntPtr hwinlogonToken;
            result = NativeMethods.OpenProcessToken(
                winlogonProcessHandle,
                NativeMethods.TOKEN_ADJUST_PRIVILEGES | NativeMethods.TOKEN_QUERY | NativeMethods.TOKEN_DUPLICATE | NativeMethods.TOKEN_ASSIGN_PRIMARY | NativeMethods.TOKEN_ADJUST_SESSIONID | NativeMethods.TOKEN_READ | NativeMethods.TOKEN_WRITE,
                out hwinlogonToken);

            if (!result)
            {
                if (callback != null)
                {
                    callback("OpenProcessToken", Marshal.GetLastWin32Error());
                }
            }

            NativeMethods.LUID luid;
            NativeMethods.TOKEN_PRIVILEGES tp = new NativeMethods.TOKEN_PRIVILEGES();
            result = NativeMethods.LookupPrivilegeValue(
                String.Empty,
                NativeMethods.SE_DEBUG_NAME,
                out luid);

            if (!result)
            {
                if (callback != null)
                {
                    callback("LookupPrivilegeValue", Marshal.GetLastWin32Error());
                }
            }
            tp.PrivilegeCount = 1;
            tp.Privileges = new NativeMethods.LUID_AND_ATTRIBUTES[1];
            tp.Privileges[0].Luid = luid;
            tp.Privileges[0].Attributes = NativeMethods.SE_PRIVILEGE_ENABLED;

            result = NativeMethods.DuplicateTokenEx(
                hwinlogonToken,
                NativeMethods.MAXIMUM_ALLOWED,
                ref sa,
                (int)TokenImpersonationLevel.Identification,
                (int)NativeMethods.TOKEN_TYPE.TokenPrimary,
                ref hDupedToken);

            if (!result)
            {
                if (callback != null)
                {
                    callback("DuplicateTokenEx", Marshal.GetLastWin32Error());
                }
            }

            IntPtr pSessionId = Marshal.AllocHGlobal(sizeof(Int32));
            Marshal.WriteInt32(pSessionId, dwSessionID);
            result = NativeMethods.SetTokenInformation(
                hDupedToken,
                NativeMethods.TOKEN_INFORMATION_CLASS.TokenSessionId,
                pSessionId,
                sizeof(uint));

            if (!result)
            {
                if (callback != null)
                {
                    callback("SetTokenInformation", Marshal.GetLastWin32Error());
                }
            }

            IntPtr pTokenPrivilege = Marshal.AllocHGlobal(Marshal.SizeOf(tp));
            Marshal.StructureToPtr(tp, pTokenPrivilege, true);
            result = NativeMethods.AdjustTokenPrivileges(
                hDupedToken,
                false,
                pTokenPrivilege,
                (uint)Marshal.SizeOf(tp),
                IntPtr.Zero,
                IntPtr.Zero);

            if (!result)
            {
                if (callback != null)
                {
                    callback("AdjustTokenPrivileges", Marshal.GetLastWin32Error());
                }
            }

            IntPtr lpEnvironment = IntPtr.Zero;
            result = NativeMethods.CreateEnvironmentBlock(out lpEnvironment, hDupedToken, false);

            if (!result)
            {
                if (callback != null)
                {
                    callback("CreateEnvironmentBlock", Marshal.GetLastWin32Error());
                }
            }

            result = NativeMethods.CreateProcessAsUser(
                                  hDupedToken,
                                  app,
                                  app + " " + arguments,
                                  ref sa, ref sa,
                                  false, 0, IntPtr.Zero,
                                  Path.GetDirectoryName(app), ref si, ref pi);

            if (!result)
            {
                if (callback != null)
                {
                    callback("CreateProcessAsUser", Marshal.GetLastWin32Error());
                }
            }

            if (pi.hProcess != IntPtr.Zero)
                NativeMethods.CloseHandle(pi.hProcess);
            if (pi.hThread != IntPtr.Zero)
                NativeMethods.CloseHandle(pi.hThread);
            if (hToken != IntPtr.Zero)
                NativeMethods.CloseHandle(hToken);
            if (hDupedToken != IntPtr.Zero)
                NativeMethods.CloseHandle(hDupedToken);
            if (hwinlogonToken != IntPtr.Zero)
                NativeMethods.CloseHandle(hwinlogonToken);
            if (lpEnvironment != IntPtr.Zero)
                NativeMethods.DestroyEnvironmentBlock(lpEnvironment);

            if (result)
            {
                return pi.dwProcessID;
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// 当前计算机是否有用户会话登陆。
        /// </summary>
        /// <returns></returns>
        public static bool IsAnyUserLogon()
        {
            IntPtr pSessionInfo = IntPtr.Zero;
            uint sessionCount = 0;
            NativeMethods.WTSEnumerateSessions(
                NativeMethods.WTS_CURRENT_SERVER_HANDLE,
                0,
                1,
                out pSessionInfo,
                out sessionCount);

            int dataSize = Marshal.SizeOf(typeof(NativeMethods.WTS_SESSION_INFO));
            long current = (long)pSessionInfo;
            for (int i = 0; i < sessionCount; i++)
            {
                NativeMethods.WTS_SESSION_INFO sessionInfo =
                    (NativeMethods.WTS_SESSION_INFO)Marshal.PtrToStructure(
                        (IntPtr)current, typeof(NativeMethods.WTS_SESSION_INFO));
                if (NativeMethods.WTS_CONNECTSTATE_CLASS.WTSActive == sessionInfo.State)
                {
                    return true;
                }
                current += dataSize;
            }

            return false;
        }
    }
}
