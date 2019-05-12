using System;
using System.Windows.Interop;

namespace Leen.Windows.Interaction
{
    /// <summary>
    /// 表示一个Win32本地窗口。
    /// </summary>
    public class NativeWindow : IWin32Window
    {
        private readonly IntPtr _handle;

        /// <summary>
        /// 构造 <see cref="NativeWindow"/> 类的实例。
        /// </summary>
        /// <param name="hwnd"></param>
        public NativeWindow(IntPtr hwnd)
        {
            _handle = hwnd;
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
    }
}
