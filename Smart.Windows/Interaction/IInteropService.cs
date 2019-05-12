using System.Windows.Interop;

namespace Leen.Windows.Interaction
{
    /// <summary>
    /// 定义Win32窗口交互的协定。
    /// </summary>
    public interface IInteropService
    {
        /// <summary>
        /// 获取当前应用程序的主窗体。
        /// </summary>
        IWin32Window Shell { get; }
    }
}
