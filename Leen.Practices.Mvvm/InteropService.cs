using Leen.Windows.Interaction;
using System;
using System.Threading;
using System.Windows.Interop;

namespace Leen.Practices.Mvvm
{
    /// <summary>
    /// 实现一个默认的Win32 窗口交互协定。
    /// </summary>
    public class InteropService : IInteropService
    {
        private static readonly IInteropService _default = new DefaultInteropService();
        private static IInteropService _current;

        /// <summary>
        /// 获取当前应用程序的主窗体。
        /// <para>
        /// 返回应用程序进程的主窗体句柄。
        /// </para>
        /// </summary>
        public IWin32Window Shell
        {
            get
            {
                if (Current != null)
                {
                    return Current.Shell;
                }
                return _default.Shell;
            }
        }

        /// <summary>
        /// 返回当前自定义的Win32 窗口交互协定。如没有设置自定义协定，则返回默认实现。
        /// </summary>
        public static IInteropService Current
        {
            get
            {
                if (_current == null)
                {
                    return Default;
                }
                return _current;
            }
        }

        /// <summary>
        /// 返回其默认实现 <see cref="InteropService"/>。
        /// </summary>
        public static IInteropService Default
        {
            get
            {
                return _default;
            }
        }

        /// <summary>
        /// 设置一个自定义的Win32 窗口交互协定。
        /// </summary>
        /// <param name="interopService"></param>
        public static void SetInteropService(IInteropService interopService)
        {
            if (interopService == null)
            {
                throw new ArgumentException(nameof(interopService));
            }

            Interlocked.CompareExchange(ref _current, interopService, null);
        }
    }
}
