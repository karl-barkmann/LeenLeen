using System;
using System.Collections.Generic;
using System.Threading;

namespace Smart.Common.Utils
{
    /// <summary>
    /// 缓存线程，使用队列执行保存的action， 队列执行完一段时间后会自动释放
    /// </summary>
    public class QueuedActionExecuter
    {
        #region 成员变量

        private static readonly Object sysRoot = new object();
        private static QueuedActionExecuter instance;
        private Queue<ActionObject> actions;
        private List<int> removeList;//需要移除的列表

        private string threadName;
        private Thread thread;
        private double threadLifeMilliseconds = 60000;//单位毫秒

        private bool isRunning = false;
        private DateTime threadRefreshTime;//线程刷新时间，每次添加方法都会刷新
        private int threadRunIntervalMilliseconds = 100;//线程执行休息间隔

        private bool isBusy;
        private int maxId;

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public QueuedActionExecuter()
        {

        }

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
        /// 获取或设置线程寿命，单位毫秒.(空闲指定时间后回收)
        /// </summary>
        public double ThreadLifeMilliseconds
        {
            get { return threadLifeMilliseconds; }
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException("不能小于1");
                threadLifeMilliseconds = value;
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

        /// <summary>
        /// 获取共享实例
        /// </summary>
        public static QueuedActionExecuter Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (sysRoot)
                    {
                        if (instance == null)
                            instance = new QueuedActionExecuter();
                    }
                }
                return instance;
            }
        }

        #endregion

        #region 公开事件

        /// <summary>
        /// Busy状态发生变化
        /// </summary>
        public event EventHandler BusyChanged;

        /// <summary>
        /// 动作执行完成事件
        /// </summary>
        public event EventHandler<ExecuteResultArgs> ExcutedActionEvent;

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
        public int ExecuteAction(Action executeAction)
        {
            if (executeAction == null) throw new ArgumentNullException();

            if (actions == null)
            {
                lock (this)
                {
                    if (actions == null)
                        actions = new Queue<ActionObject>();
                }
            }

            lock (actions)
            {
                ActionObject obj = new ActionObject();

                //最大值了，这种情况应该不会出现
                if (maxId == Int32.MaxValue)
                    maxId = 0;
                obj.Id = maxId++;
                obj.Action = executeAction;
                actions.Enqueue(obj);

                threadRefreshTime = DateTime.Now;

                RunThread();

                return obj.Id;
            }
        }

        /// <summary>
        /// 移除某动作，如果该动作还未执行
        /// </summary>
        /// <param name="actionId"></param>
        public void RemoveAction(int actionId)
        {
            lock (this)
            {
                if (removeList == null)
                    removeList = new List<int>();

                if (removeList.Contains(actionId) || actionId < 0)
                    return;

                removeList.Add(actionId);
            }
        }

        /// <summary>
        /// 异步停止执行
        /// </summary>
        public void StopRunAsyn()
        {
            isRunning = false;
        }

        /// <summary>
        /// 同步停止执行,如果正在执行界面操作，此方法可能导致死锁
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

            bool timeOut = DateTime.Now - threadRefreshTime > TimeSpan.FromMilliseconds(threadLifeMilliseconds);

            while (isRunning && !timeOut)
            {
                try
                {
                    ExecuteActions();

                    timeOut = DateTime.Now - threadRefreshTime > TimeSpan.FromMilliseconds(threadLifeMilliseconds);

                    Thread.Sleep(TimeSpan.FromMilliseconds(threadRunIntervalMilliseconds));
                }
                catch (ThreadAbortException ex)
                {
                    actions.Clear();
                    break;
                }
            }

        }

        private void ExecuteActions()
        {
            bool existAction = true;

            do
            {
                Action tempAction;
                if (actions.Count > 0)
                {
                    IsBusy = true;

                    existAction = true;
                    var obj = actions.Peek();
                    ExecuteResult result = ExecuteResult.Executed;

                    if (removeList != null && removeList.Contains(obj.Id))
                    {
                        result = ExecuteResult.Throw;
                        actions.Dequeue();
                    }
                    else
                    {
                        tempAction = obj.Action;
                        //方法不为空，时间为空
                        if (tempAction != null)
                        {
                            tempAction();
                            result = ExecuteResult.Executed;
                            actions.Dequeue();
                        }
                    }

                    if (removeList != null && removeList.Contains(obj.Id))
                        removeList.Remove(obj.Id);

                    if (ExcutedActionEvent != null)
                        ExcutedActionEvent(obj, new ExecuteResultArgs() { ActionId = obj.Id, Result = result });
                }
                else
                {
                    existAction = false;
                }
            }
            while (existAction && isRunning);
            IsBusy = false;


        }

        private void OnBusyChanged()
        {
            var handle = BusyChanged;
            if (handle != null)
                handle(this, new EventArgs());
        }

        #endregion
    }

    /// <summary>
    /// 表示一个动作
    /// </summary>
    internal class ActionObject
    {
        /// <summary>
        /// 动作id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 动作
        /// </summary>
        public Action Action { get; set; }
    }

    /// <summary>
    /// 表示action的执行结果
    /// </summary>
    public enum ExecuteResult
    {
        /// <summary>
        /// 已执行
        /// </summary>
        Executed,
        /// <summary>
        /// 已抛弃
        /// </summary>
        Throw,

        ///// <summary>
        ///// 异常
        ///// </summary>
        //Exception
    }

    /// <summary>
    /// 执行结果参数
    /// </summary>
    public class ExecuteResultArgs : EventArgs
    {
        /// <summary>
        /// 执行结果
        /// </summary>
        public ExecuteResult Result { get; set; }

        /// <summary>
        /// 事件id
        /// </summary>
        public int ActionId { get; set; }
    }
}
