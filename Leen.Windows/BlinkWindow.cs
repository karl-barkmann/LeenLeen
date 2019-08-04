using Leen.Native;
using System;
using System.Windows;
using System.Windows.Interop;

namespace Leen.Windows
{
    /// <summary>
    /// 支持在样式配置两种闪烁提示视图状态的窗体。
    /// </summary>
    [TemplateVisualState(GroupName = "ActivationStates", Name = "Blink")]
    [TemplateVisualState(GroupName = "ActivationStates", Name = "Normal")]
    public class BlinkWindow : Window
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            if (PresentationSource.FromVisual(this) is HwndSource hwndSource)
            {
                hwndSource.AddHook(WndProc);
            }
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            var retVal = IntPtr.Zero;

            switch (msg)
            {
                case User32.WM_NCACTIVATE:
                    retVal = User32.DefWindowProc(hwnd, User32.WM_NCACTIVATE, new IntPtr(1), new IntPtr(-1));
                    if ((int)wParam == 1)
                    {
                        VisualStateManager.GoToState(this, "Blink", false);
                    }
                    else
                    {
                        VisualStateManager.GoToState(this, "Normal", false);
                    }
                    handled = true;
                    break;
            }

            return retVal;
        }
    }
}
