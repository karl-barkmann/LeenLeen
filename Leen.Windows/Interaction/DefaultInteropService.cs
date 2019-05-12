using System;
using System.Windows;
using System.Windows.Interop;

namespace Leen.Windows.Interaction
{
    /// <summary>
    /// <see cref="IWin32Window"/> 窗口交互协定的默认实现。
    /// </summary>
    public class DefaultInteropService : IInteropService
    {
        private IWin32Window _window;

        /// <summary>
        /// 构造 <see cref="DefaultInteropService"/> 类的实例。
        /// </summary>
        /// <param name="shellWindow">应用程序的主窗体可以是模块化单一进程的主窗体，也可以是多进程插件架构的主进程的Shell窗体。</param>
        public DefaultInteropService(Window shellWindow)
        {
            if (shellWindow == null)
            {
                throw new ArgumentNullException(nameof(shellWindow));
            }
            var interopHelper = new WindowInteropHelper(shellWindow);
            interopHelper.EnsureHandle();
            _window = new NativeWindow(interopHelper.Handle);
        }

        /// <summary>
        /// 构造 <see cref="CrossProcessInteropService"/> 类的实例。
        /// </summary>
        /// <param name="shellWndHandle">主进程主窗口句柄。</param>
        public DefaultInteropService(IntPtr shellWndHandle)
        {
            if (IntPtr.Zero == shellWndHandle)
            {
                throw new ArgumentException("Invalid shell window handle", nameof(shellWndHandle));
            }
            _window = new NativeWindow(shellWndHandle);
        }


        /// <summary>
        /// 构造 <see cref="DefaultInteropService"/> 类的实例，采用 <see cref="Application.MainWindow"/> 作为主窗体如果有。
        /// </summary>
        public DefaultInteropService()
        {
            if (Application.Current != null && Application.Current.MainWindow != null)
            {
                var interopHelper = new WindowInteropHelper(Application.Current.MainWindow);
                interopHelper.EnsureHandle();
                _window = new NativeWindow(interopHelper.Handle);
            }
        }

        /// <summary>
        /// 获取当前应用程序的主窗体。
        /// </summary>
        public IWin32Window Shell
        {
            get
            {
                if (_window == null)
                {
                    if (Application.Current != null && Application.Current.MainWindow != null)
                    {
                        var interopHelper = new WindowInteropHelper(Application.Current.MainWindow);
                        interopHelper.EnsureHandle();
                        _window = new NativeWindow(interopHelper.Handle);
                    }
                }
                return _window;
            }
        }
    }
}
