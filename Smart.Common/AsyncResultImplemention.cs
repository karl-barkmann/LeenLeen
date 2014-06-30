using System;
using System.Threading;

namespace Smart.Common
{
    /// <summary>
    /// 表示没有返回值的异步操作。
    /// </summary>
    public class AsyncResultNoResult : IAsyncResult
    {
        private readonly AsyncCallback asyncCallback;
        private readonly object asyncState;

        private const int StatePending = 0;
        private const int StateCompletedSynchronously = 1;
        private const int StateCompletedAsynchronously = 2;
        private int completedState = StatePending;

        private ManualResetEvent asyncWaitHandle;
        private Exception exception;

        /// <summary>
        /// 构造异步操作的实例。
        /// </summary>
        /// <param name="asyncCallback">异步操作完成后的回调。</param>
        /// <param name="state">用户状态对象。</param>
        public AsyncResultNoResult(AsyncCallback asyncCallback, object state)
        {
            this.asyncCallback = asyncCallback;
            this.asyncState = state;
        }

        /// <summary>
        /// 设置异步操作已经完成。
        /// </summary>
        /// <param name="exception">异步操作过程中发生的异常。</param>
        /// <param name="completedSynchronously">操作是否为同步完成。</param>
        public void SetAsCompleted(Exception exception, bool completedSynchronously)
        {
            this.exception = exception;

            int prevState = Interlocked.Exchange(ref completedState, completedSynchronously ?
                StateCompletedSynchronously : StateCompletedAsynchronously);
            if (prevState != StatePending)
            {
                throw new InvalidOperationException("Can't set a result twice.");
            }

            if (asyncWaitHandle != null)
            {
                asyncWaitHandle.Set();
            }

            if (asyncCallback != null)
            {
                asyncCallback(this);
            }
        }

        /// <summary>
        /// 结束异步操作，如果未完成时调用，将阻塞直到异步操作完成。
        /// </summary>
        public void EndInvoke()
        {
            if (!IsCompleted)
            {
                AsyncWaitHandle.WaitOne();
                AsyncWaitHandle.Close();
                asyncWaitHandle = null;
            }

            if (exception != null)
                throw exception;
        }

        #region IAsyncResult 接口成员

        /// <summary>
        /// 获取用户定义的对象，它限定或包含关于异步操作的信息
        /// </summary>
        public object AsyncState
        {
            get { return asyncState; }
        }

        /// <summary>
        /// 获取异步操作等待句柄。
        /// </summary>
        public WaitHandle AsyncWaitHandle
        {
            get
            {
                if (asyncWaitHandle == null)
                {
                    bool done = IsCompleted;
                    ManualResetEvent waitHandle = new ManualResetEvent(done);
                    if (Interlocked.CompareExchange(ref asyncWaitHandle, waitHandle, null) != null)
                    {
                        waitHandle.Close();
                    }
                    else
                    {
                        if (!done && IsCompleted)
                        {
                            asyncWaitHandle.Set();
                        }
                    }
                }

                return asyncWaitHandle;
            }
        }

        /// <summary>
        /// 获取一个值，指示操作是否同步完成。
        /// </summary>
        public bool CompletedSynchronously
        {
            get { return Thread.VolatileRead(ref completedState) == StateCompletedSynchronously; }
        }

        /// <summary>
        /// 获取一个值，指示操作是否完成。
        /// </summary>
        public bool IsCompleted
        {
            get { return Thread.VolatileRead(ref completedState) != StatePending; }
        }

        #endregion
    }

    /// <summary>
    /// 表示有返回值的异步操作。
    /// </summary>
    /// <typeparam name="TResult">异步操作返回值类型。</typeparam>
    public class AsyncResult<TResult> : AsyncResultNoResult
    {
        private TResult result = default(TResult);

        /// <summary>
        /// 构造异步操作的实例。
        /// </summary>
        /// <param name="asyncCallback">异步操作完成后的回调。</param>
        /// <param name="state">用户状态对象。</param>
        public AsyncResult(AsyncCallback asyncCallback, object state)
            : base(asyncCallback, state)
        {

        }

        /// <summary>
        /// 设置异步操作已经完成。
        /// </summary>
        /// <param name="result">异步操作后的返回值。</param>
        /// <param name="completedSynchronously">操作是否为同步完成。</param>
        public void SetAsCompleted(TResult result, bool completedSynchronously)
        {
            this.result = result;

            //tell the base class the operation completed
            //sucessfully (no exception)
            base.SetAsCompleted(null, completedSynchronously);
        }

        /// <summary>
        /// 结束异步操作，如果未完成时调用，将阻塞直到异步操作完成。
        /// </summary>
        public new TResult EndInvoke()
        {
            base.EndInvoke();
            return result;
        }
    }
}
