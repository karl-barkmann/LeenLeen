using System;
using System.Diagnostics;
using System.Windows.Interop;

namespace Leen.Windows.Interaction
{
    /// <summary>
    /// 实现一个跨进程的窗口交互协定。
    /// </summary>
    class CrossProcessInteropService : IInteropService
    {
        private readonly IWin32Window _shell;

        /// <summary>
        /// 构造 <see cref="CrossProcessInteropService"/> 类的实例。
        /// </summary>
        /// <param name="shellWndHandle">主进程主窗口句柄。</param>
        public CrossProcessInteropService(IntPtr shellWndHandle)
        {
            if (IntPtr.Zero == shellWndHandle)
            {
                throw new ArgumentException("Invalid shell window handle", nameof(shellWndHandle));
            }
            _shell = new NativeWindow(shellWndHandle);
        }

        /// <summary>
        /// 构造 <see cref="CrossProcessInteropService"/> 类的实例。
        /// </summary>
        /// <param name="shellProcess">主进程对象。</param>
        public CrossProcessInteropService(Process shellProcess)
        {
            if (shellProcess == null)
            {
                throw new ArgumentNullException(nameof(shellProcess));
            }

            if (shellProcess.HasExited)
            {
                throw new ArgumentException("Shell process is not valid (exited or disposed)", nameof(shellProcess));
            }

            shellProcess.WaitForInputIdle();
            var shellHandle = shellProcess.MainWindowHandle;
            if (IntPtr.Zero == shellHandle)
            {
                throw new ArgumentException("Shell process is does not have a valid main window handle", nameof(shellProcess));
            }
            _shell = new NativeWindow(shellHandle);
        }

        /// <summary>
        /// 获取主进程主窗口。
        /// </summary>
        public IWin32Window Shell
        {
            get
            {
                return _shell;
            }
        }
    }
}
