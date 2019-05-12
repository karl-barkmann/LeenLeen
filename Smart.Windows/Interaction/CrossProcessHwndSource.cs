using Leen.Native;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Interop;
using static Leen.Native.NativeMethods;
using static Leen.Native.User32;

namespace Leen.Windows.Interaction
{
    /// <summary>
    /// 表示在多进程架构中 Windows Presentation Foundation (WPF) 内容，提供对其本地句柄的访问及消息回调机制（但不可更改消息）。
    /// <para>
    /// 应使用 <see cref="FromHandle(IntPtr)"/> 方法获取此句柄的 <see cref="CrossProcessHwndSource"/> 封装。
    /// </para>
    /// </summary>
    [Obsolete("该类使用 SetWindowHookEx API，由于C#无法导出dll以进行dll注入所以无法实现跨进程消息挂载。但若尝试挂载的其他进程的句柄中包含当前进程的子句柄，则该类可以收到这个子句柄的WIN32消息。")]
    public class CrossProcessHwndSource : IDisposable, System.Windows.Interop.IWin32Window
    {
        private bool _isDisposed;
        private event HwndSourceHook _hooks;
        private readonly IntPtr _handle;
        private static HookProc hProc;
        private static IntPtr hHook;
        private const int WH_CALLWNDPROC = 4;
        private const int HC_ACTION = 0;
        private static Dictionary<IntPtr, CrossProcessHwndSource>
            _sources = new Dictionary<IntPtr, CrossProcessHwndSource>();

        /// <summary>
        /// 构造 <see cref="CrossProcessHwndSource"/> 类的实例。
        /// </summary>
        /// <param name="handle">其他WPF框架进程下的 <see cref="HwndSource"/> 句柄。</param>
        protected CrossProcessHwndSource(IntPtr handle)
        {
            _handle = handle;
        }

        /// <summary>
        /// 释放相关非托管资源。
        /// </summary>
        ~CrossProcessHwndSource()
        {
            Dispose(false);
        }

        /// <summary>
        /// 获取一个值指示 <see cref="CrossProcessHwndSource"/> 是否已经由调用<see cref="Dispose()"/>方法释放。
        /// </summary>
        public bool IsDisposed
        {
            get { return _isDisposed; }
        }

        /// <summary>
        /// 获取窗口句柄。
        /// </summary>
        public IntPtr Handle
        {
            get
            {
                return _handle;
            }
        }

        /// <summary>
        /// 使用特定句柄创建 <see cref="CrossProcessHwndSource"/>。
        /// <para>
        /// 调用者应维护此句柄关联的 <see cref="CrossProcessHwndSource"/> 生命周期。
        /// </para>
        /// </summary>
        /// <param name="handle">窗口句柄。</param>
        /// <returns></returns>
        public static CrossProcessHwndSource FromHandle(IntPtr handle)
        {
            if (handle == IntPtr.Zero)
            {
                throw new ArgumentException("无效的窗口句柄", nameof(handle));
            }

            if (_sources.ContainsKey(handle))
            {
                return _sources[handle];
            }

            var source = new CrossProcessHwndSource(handle);
            _sources.Add(handle, source);

            return source;
        }

        /// <summary>
        /// 添加接收所有窗口消息的事件处理程序。
        /// </summary>
        /// <param name="hook">要添加的事件处理程序。</param>
        public void AddHook(HwndSourceHook hook)
        {
            CheckDisposed();
            _hooks += hook;
            if (hHook == IntPtr.Zero)
            {
                hHook = SetHook();
            }
        }


        /// <summary>
        /// 移除经由调用 <see cref="AddHook(HwndSourceHook)"/> 添加的事件处理程序。
        /// </summary>
        /// <param name="hook">要移除的事件处理程序。</param>
        public void RemoveHook(HwndSourceHook hook)
        {
            CheckDisposed();
            _hooks -= hook;
        }

        /// <summary>
        /// 释放相关资源。
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 调用与此窗口关联的默认窗口过程。
        /// </summary>
        /// <param name="m">与当前 Windows 消息相关联的 <see cref="Message"/>。</param>
        protected virtual void WndProc(ref Message m)
        {
            if (_hooks != null)
            {
                // The default result for messages we handle is 0.
                IntPtr result = IntPtr.Zero;
                bool handled = false;
                Delegate[] handlers = _hooks.GetInvocationList();
                for (int i = handlers.Length - 1; i >= 0; --i)
                {
                    var hook = (HwndSourceHook)handlers[i];
                    result = hook(m.HWnd, m.Msg, m.WParam, m.LParam, ref handled);
                    if (handled)
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 释放相关资源。
        /// </summary>
        /// <param name="isDisposing">是否正在进行托管资源释放。</param>
        protected virtual void Dispose(bool isDisposing)
        {
            if (!_isDisposed)
            {
                if (isDisposing)
                {
                    if (_sources.ContainsKey(Handle))
                    {
                        _sources.Remove(Handle);
                        if (_sources.Count == 0)
                        {
                            UnHook();
                        }
                    }
                }

                UnHook();

                _isDisposed = true;
            }
        }

        private void CheckDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(null, "CrossProcessHwndSource Disposed");
            }
        }

        private static IntPtr SetHook()
        {
            hProc = new HookProc(CallWndProc);
            OperatingSystem osInfo = Environment.OSVersion;
            int versionMajor = osInfo.Version.Major;
            int versionMinor = osInfo.Version.Minor;
            var currentThreadId = Kernel32.GetCurrentThreadId();
            switch (versionMajor)
            {
                //XP主版本号为5，副版本号为1（目前没考虑wind2003和2002版本，所以没有用副版本号区分，将来要考虑的话再加判断，2002副版本号为0，2003为2）
                case 5:
                    hHook = SetWindowsHookEx(
                        WH_CALLWNDPROC,
                        hProc,
                        Kernel32.GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName),
                        (uint)currentThreadId);
                    break;
                //XP以后版本主版本号为6（副版本号WindowsVista：0，Windows7：1， Windows8：2）
                case 6:
                    hHook = SetWindowsHookEx(
                        WH_CALLWNDPROC,
                        hProc,
                        IntPtr.Zero,
                        (uint)currentThreadId);
                    break;
            }
            return hHook;
        }

        /// <summary>
        /// 卸载鼠标 Hook 回调。
        /// </summary>
        private static void UnHook()
        {
            if (hHook != IntPtr.Zero)
            {
                UnhookWindowsHookEx(hHook);
                hHook = IntPtr.Zero;
            }
        }

        private static IntPtr CallWndProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode < 0)
            {
                return CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
            }
            else
            {
                if (nCode == HC_ACTION)
                {
                    CWPStruct cwpStruct = (CWPStruct)Marshal.PtrToStructure(lParam, typeof(CWPStruct));

                    Message m = new Message()
                    {
                        HWnd = cwpStruct.hwnd,
                        LParam = cwpStruct.lParam,
                        WParam = cwpStruct.wParam,
                        Msg = (int)cwpStruct.message
                    };
                    var ancestorHwnd = GetAncestor(m.HWnd, 2);
                    var rootOwnnerHwnd = GetAncestor(m.HWnd, 3);

                    if (_sources.ContainsKey(m.HWnd))
                    {
                        _sources[m.HWnd].WndProc(ref m);
                    }
                    else if (_sources.ContainsKey(ancestorHwnd))
                    {
                        _sources[ancestorHwnd].WndProc(ref m);
                    }
                    else if (_sources.ContainsKey(rootOwnnerHwnd))
                    {
                        _sources[rootOwnnerHwnd].WndProc(ref m);
                    }
                }

                return CallNextHookEx(hHook, nCode, wParam, lParam);
            } 
        }
    }
}
