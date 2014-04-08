using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using Xunmei.Smart.Common.Utils;

namespace Xunmei.Smart.Common
{
    /// <summary>
    /// 多线程查询引擎。
    /// </summary>
    public class MTQEngine<T> : IDisposable
    {
        private bool asynchronus;
        private bool disposed;
        private AutoResetEvent waitHandle;
        private T[] results;
        private int queryedCount;
        private int queryCount;

        /// <summary>
        /// 构造一个多线程查询引擎的实例。
        /// </summary>
        public MTQEngine()
        {
            waitHandle = new AutoResetEvent(false);
        }

        /// <summary>
        /// 析构。
        /// </summary>
        ~MTQEngine()
        {
            Dispose(false);
        }

        /// <summary>
        /// 当查询完成时发生。
        /// </summary>
        public event EventHandler<AsyncQueryCompletedEventArgs<T>> Completed;

        /// <summary>
        /// 当查询被取消时发生。
        /// </summary>
        public event EventHandler<AsyncQueryCompletedEventArgs<T>> Canceld;

        /// <summary>
        /// 执行查询。
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> Execute(IEnumerable<IDbQuerySegement<T>> segements)
        {
            if (segements == null)
                throw new ArgumentNullException("segements");
            queryCount = segements.Count();
            results = new T[queryCount];
            if (queryCount > 0)
            {
                segements.OrderBy(segement => segement.Order).ForEach((segement, index) =>
                {
                    segement.Bulid().BeginInvoke(CallBack, index);
                });
                waitHandle.WaitOne();
            }
            return results;
        }

        /// <summary>
        /// 开始异步执行查询。
        /// </summary>
        public void ExecuteAsync(IEnumerable<IDbQuerySegement<T>> segements)
        {
            if (segements == null)
                throw new ArgumentNullException("segements");
            queryCount = segements.Count();
            results = new T[queryCount];
            segements = segements.OrderBy(segement => segement.Order);
            segements.ForEach((segement, index) =>
            {
                segement.Bulid().BeginInvoke(CallBack, index);
            });
            asynchronus = true;
        }

        /// <summary>
        /// 请求取消查询。
        /// </summary>
        public void Cancel()
        {
            AsyncQueryCompletedEventArgs<T> e = new AsyncQueryCompletedEventArgs<T>(
                        null, true, null, results);
            OnCanceld(e);
            asynchronus = false;
            results = null;
            Interlocked.Exchange(ref queryedCount, 0);
            waitHandle.Set();
        }

        /// <summary>
        /// 释放资源。
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 释放资源。
        /// </summary>
        /// <param name="disposing">是否正在主动释放。</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    results = null;
                    if (waitHandle != null)
                        waitHandle.Close();
                }

                disposed = true;
            }
        }

        /// <summary>
        /// 当查询被取完成发生。
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnCompleted(AsyncQueryCompletedEventArgs<T> e)
        {
            EventHandler<AsyncQueryCompletedEventArgs<T>> handler = Completed;
            if (handler != null)
            {
                handler.Invoke(this, e);
            }
        }

        /// <summary>
        /// 当查询被取消时发生。
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnCanceld(AsyncQueryCompletedEventArgs<T> e)
        {
            EventHandler<AsyncQueryCompletedEventArgs<T>> handler = Canceld;
            if (handler != null)
            {
                handler.Invoke(this, e);
            }
        }

        private void CallBack(IAsyncResult asyncResult)
        {
            int index = (int)asyncResult.AsyncState;
            AsyncResult result = (AsyncResult)asyncResult;
            Func<T> func = (Func<T>)result.AsyncDelegate;
            T t = func.EndInvoke(asyncResult);
            results[index] = t;

            Interlocked.Increment(ref queryedCount);

            if (queryedCount == queryCount)
            {
                Interlocked.Exchange(ref queryedCount, 0);
                waitHandle.Set();
                if (asynchronus)
                {
                    AsyncQueryCompletedEventArgs<T> e = new AsyncQueryCompletedEventArgs<T>(
                        null, false, null, results);
                    OnCompleted(e);
                    asynchronus = false;
                }
            }
        }
    }

    /// <summary>
    /// 异步查询完成参数。
    /// </summary>
    public class AsyncQueryCompletedEventArgs<T> : AsyncCompletedEventArgs
    {
        /// <summary>
        /// 构造异步查询完成参数的实例。
        /// </summary>
        /// <param name="error"></param>
        /// <param name="cancelled"></param>
        /// <param name="userState"></param>
        /// <param name="result"></param>
        public AsyncQueryCompletedEventArgs(Exception error, bool cancelled, object userState, IEnumerable<T> result) :
            base(error, cancelled, userState)
        { }

        /// <summary>
        /// 异步查询返回值。
        /// </summary>
        public IEnumerable<T> Result { get; set; }
    }

    /// <summary>
    /// 查询片段接口。
    /// </summary>
    public interface IDbQuerySegement<T>
    {
        /// <summary>
        /// 查询顺序。
        /// </summary>
        int Order { get; set; }

        /// <summary>
        /// 构造查询方法。
        /// </summary>
        /// <returns></returns>
        Func<T> Bulid();
    }
}
