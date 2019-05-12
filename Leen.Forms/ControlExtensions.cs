using System;
using System.Windows.Forms;

namespace Leen.Forms
{
    /// <summary>
    /// 控件扩展方法。
    /// </summary>
    public static class ControlExtensions
    {
        /// <summary>
        /// 在主线程上执行操作。
        /// </summary>
        /// <param name="control"></param>
        /// <param name="action"></param>
        /// <param name="args"></param>
        public static void SafeInvoke(this Control control, Action action, params object[] args)
        {
            if (control != null && control.IsHandleCreated && !control.IsDisposed)
            {
                if (control.InvokeRequired)
                {
                    try
                    {
                        control.Invoke(action, args);
                    }
                    catch
                    {
                        //swallow error
                    }
                }
                else
                {
                    action();
                }
            }
        }

        /// <summary>
        /// 在主线程上异步执行操作。
        /// </summary>
        /// <param name="control"></param>
        /// <param name="action"></param>
        /// <param name="args"></param>
        public static void SafeBeginInvoke(this Control control, Action action, params object[] args)
        {
            if (control != null && control.IsHandleCreated && !control.IsDisposed)
            {
                if (control.InvokeRequired)
                {
                    try
                    {
                        control.BeginInvoke(action, args);
                    }
                    catch
                    {
                        //swallow error
                    }
                }
                else
                {
                    action();
                }
            }
        }

        /// <summary>
        /// 在需要时在拥有此句柄的线程上执行操作。
        /// </summary>
        /// <param name="control"></param>
        /// <param name="doit"></param>
        public static void InvokeIfNeeded(this Control control, Action doit)
        {
            if (!control.IsDisposed && control.IsHandleCreated)
            {
                if (control.InvokeRequired)
                {
                    try
                    {
                        control.Invoke(doit);
                    }
                    catch (InvalidOperationException)
                    {
                        //swallow wired error    
                    }
                }
                else
                {
                    doit();
                }
            }
        }

        /// <summary>
        /// 在需要时在拥有此句柄的线程上执行操作。
        /// </summary>
        /// <param name="control"></param>
        /// <param name="doit"></param>
        public static void BeginInvokeIfNeeded(this Control control, Action doit)
        {
            if (!control.IsDisposed && control.IsHandleCreated)
            {
                if (control.InvokeRequired)
                {
                    try
                    {
                        control.BeginInvoke(doit);
                    }
                    catch (InvalidOperationException)
                    {
                        //swallow wired error    
                    }
                }
                else
                {
                    doit();
                }
            }
        }

        /// <summary>
        /// 在需要时在拥有此句柄的线程上执行操作。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="control"></param>
        /// <param name="doit"></param>
        /// <param name="args"></param>
        public static void InvokeIfNeeded<T>(this Control control, Action<T> doit, T args)
        {
            if (!control.IsDisposed && control.IsHandleCreated)
            {
                if (control.InvokeRequired)
                {
                    try
                    {
                        control.Invoke(doit, args);
                    }
                    catch (InvalidOperationException)
                    {
                        //swallow wired error    
                    }
                }
                else
                {
                    doit(args);
                }
            }
        }

        /// <summary>
        /// 在需要时在拥有此句柄的线程上执行操作。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="control"></param>
        /// <param name="doit"></param>
        /// <param name="args"></param>
        public static void BeginInvokeIfNeeded<T>(this Control control, Action<T> doit, T args)
        {
            if (!control.IsDisposed && control.IsHandleCreated)
            {
                if (control.InvokeRequired)
                {
                    try
                    {
                        control.BeginInvoke(doit, args);
                    }
                    catch (InvalidOperationException)
                    {
                        //swallow wired error    
                    }
                }
                else
                {
                    doit(args);
                }
            }
        }
    }
}
