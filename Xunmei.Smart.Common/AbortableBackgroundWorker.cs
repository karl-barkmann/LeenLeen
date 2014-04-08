/* * * * * * * * * * * * * * * * * * * * * * * * * * * 
 * 作者：李平
 * 日期：2012/6/28 8:28:54
 * 描述：可中止的后台工作者组件。
 * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System.Threading;

namespace System.ComponentModel
{
    /// <summary>
    /// 可中止的后台工作者组件。
    /// </summary>
    public class AbortableBackgroundWorker : BackgroundWorker
    {
        private Thread workThread;

        /// <summary>
        /// 可中止的后台工作者组件。
        /// </summary>
        public AbortableBackgroundWorker()
        {

        }

        /// <summary>
        /// 引发 System.ComponentModel.BackgroundWorker.DoWork 事件。
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDoWork(DoWorkEventArgs e)
        {
            workThread = Thread.CurrentThread;
            try
            {
                base.OnDoWork(e);
            }
            catch (ThreadAbortException)
            {
                e.Cancel = true;
                Thread.ResetAbort();
            }
        }

        /// <summary>
        /// 中止后台工作者组件。
        /// </summary>
        public void Abort()
        {
            if (workThread != null)
            {
                workThread.Abort();
                workThread = null;
            }
        }
    }
}
