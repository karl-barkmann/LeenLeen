using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Smart.Common.Utils
{
    /// <summary>
    /// 指定时间执行动作 执行完一段时间后会自动释放
    /// </summary>
    public class TimeActionExecuter
    {
        #region 成员变量

        private static readonly Object sysRoot = new object();
        private List<KeyValuePair<Action, DateTime>> actions;

        private string threadName;
        private Thread thread;

        private bool isRunning = false;
        private int threadRunIntervalMilliseconds = 100;//线程执行休息间隔

        private bool isBusy;

        #endregion

        #region 公开属性

        /// <summary>
        /// 获取或设置线程名字
        /// </summary>
        public string ThreadName
        {
            get { return threadName; }
            set
            {
                if (threadName == value) return;
                threadName = value;
                SetThreadName(value);
            }
        }

        /// <summary>
        /// 获取或设置线程执行间隔时间
        /// </summary>
        public int ThreadRunIntervalMilliseconds
        {
            get { return threadRunIntervalMilliseconds; }
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException("不能小于1");
                threadRunIntervalMilliseconds = value;
            }
        }

        /// <summary>
        /// 获取 是否正在忙
        /// </summary>
        public bool IsBusy
        {
            get { return isBusy; }
            private set
            {
                if (isBusy == value) return;
                isBusy = value;
                OnBusyChanged();
            }
        }

        #endregion

        #region 公开事件

        /// <summary>
        /// Busy状态发生变化
        /// </summary>
        public event EventHandler BusyChanged;

        #endregion

        #region 公开方法

        /// <summary>
        /// 设置线程名字
        /// </summary>
        /// <param name="threadName"></param>
        public void SetThreadName(string threadName)
        {
            if (thread != null)
                thread.Name = threadName;
            this.threadName = threadName;
        }

        /// <summary>
        /// 将要执行的队列添加进来
        /// </summary>
        /// <param name="executeAction">要加入队列的方法.</param>
        /// <param name="executeTime">执行此方法的时间,时间没到会阻塞后面的方法.</param>
        public void ExecuteAction(Action executeAction, DateTime executeTime)
        {
            if (executeAction == null) throw new ArgumentNullException();

            if (actions == null)
            {
                lock (this)
                {
                    if (actions == null)
                        actions = new List<KeyValuePair<Action, DateTime>>();
                }
            }

            lock (actions)
                actions.Add(new KeyValuePair<Action, DateTime>(executeAction, executeTime));

            RunThread();
        }

        /// <summary>
        /// 异步停止执行
        /// </summary>
        public void StopRunAsyn()
        {
            isRunning = false;
        }

        /// <summary>
        /// 同步停止执行
        /// </summary>
        public void StopRun()
        {
            isRunning = false;
            if (thread != null && thread.IsAlive)
            {
                thread.Abort();
                thread.Join();
            }
        }

        #endregion

        #region 私有方法

        private void RunThread()
        {
            isRunning = true;
            if (thread != null && thread.IsAlive)
                return;

            thread = new Thread(new ThreadStart(Execute));
            thread.Name = threadName;
            thread.IsBackground = true;
            thread.Start();
        }

        private void Execute()
        {
            List<KeyValuePair<Action, DateTime>> tempList = null;
 
            do
            {
                if (actions != null)
                    tempList = new List<KeyValuePair<Action, DateTime>>(actions);

                DateTime datetimeNow = DateTime.Now;

                var canExecuteActions = tempList.Where(m => m.Value <= datetimeNow);
                foreach (var item in canExecuteActions)
                {
                    item.Key();
                    lock (sysRoot)
                    {
                        if (actions.Contains(item))
                            actions.Remove(item);
                    }
                }

                Thread.Sleep(threadRunIntervalMilliseconds);

            }
            while (tempList != null && tempList.Count > 0 && isRunning);

            isRunning = false;
        }


        private void OnBusyChanged()
        {
            var handle = BusyChanged;
            if (handle != null)
                handle(this, new EventArgs());
        }

        #endregion
    }
}
