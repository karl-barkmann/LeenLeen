/* * * * * * * * * * * * * * * * * * * * * * * * * * * 
 * 作者：李平
 * 日期：2012/6/28 16:17:36
 * 描述：自动关闭的Popup。
 * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System;
using System.Windows.Threading;

namespace Smart.Windows.Controls
{
    /// <summary>
    /// 自动关闭的Popup。
    /// </summary>
    public class AutoClosePopup : NonTopmostPopup 
    {
        private DispatcherTimer closeTimer;

        /// <summary>
        /// 自动关闭的Popup。
        /// </summary>
        public AutoClosePopup()
        {
            IsTopmost = false;
            AutoCloseTimeOut = 1.5;
        }

        /// <summary>
        /// 自动关闭的超时时间(秒)。
        /// </summary>
        public double AutoCloseTimeOut
        {
            set;
            get;
        }

        /// <summary>
        /// 对 System.Windows.Controls.Primitives.Popup.IsOpen 属性的值从 false 更改为 true 的情况进行响应。
        /// </summary>
        /// <param name="e"></param>
        protected override void OnOpened(EventArgs e)
        {
            if (closeTimer == null)
            {
                closeTimer = new DispatcherTimer(TimeSpan.FromSeconds(AutoCloseTimeOut), DispatcherPriority.Normal, closeTimer_Tick, Dispatcher);
                closeTimer.Start();
            }
            base.OnOpened(e);
        }

        /// <summary>
        /// 对 System.Windows.Controls.Primitives.Popup.IsOpen 属性的值从 true 更改为 false 的情况进行响应。
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            if (closeTimer != null)
            {
                closeTimer.Stop();
                closeTimer = null;
            }
            base.OnClosed(e);
        }

        private void closeTimer_Tick(object sender, EventArgs e)
        {
            base.IsOpen = false;
            if (closeTimer != null)
            {
                closeTimer.Stop();
                closeTimer = null;
            }
        }
    }
}
