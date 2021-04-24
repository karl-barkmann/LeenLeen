﻿using System;
using System.Threading.Tasks;

namespace Leen.Common
{
    /// <summary>
    /// 为基于Task的异步编程模型提供的帮助方法。
    /// </summary>
    public static class TaskExtensions
    {
        private static bool _shouldAlwaysRethrowException;
        private static Action<Exception> _onException;

        /// <summary>
        /// Initialize SafeFireAndForget
        ///
        /// Warning: When <c>true</c>, there is no way to catch this exception and it will always result in a crash. Recommended only for debugging purposes.
        /// </summary>
        /// <param name="shouldAlwaysRethrowException">If set to <c>true</c>, after the exception has been caught and handled, the exception will always be rethrown.</param>
        public static void Initialize(in bool shouldAlwaysRethrowException = false) => _shouldAlwaysRethrowException = shouldAlwaysRethrowException;

        /// <summary>
        /// Remove the default action for SafeFireAndForget
        /// </summary>
        public static void RemoveDefaultExceptionHandling() => _onException = null;

        /// <summary>
        /// Set the default action for SafeFireAndForget to handle every exception
        /// </summary>
        /// <param name="onException">If an exception is thrown in the Task using SafeFireAndForget, <c>onException</c> will execute</param>
        public static void SetDefaultExceptionHandling(in Action<Exception> onException)
        {
            if (onException is null)
                throw new ArgumentNullException(nameof(onException));

            _onException = onException;
        }

        /// <summary>
        /// Safely execute the Task without waiting for it to complete before moving to the next line of code; commonly known as "Fire And Forget".
        /// </summary>
        /// <param name="task">Task.</param>
        /// <param name="onException">If an exception is thrown in the Task, <c>onException</c> will execute. If onException is null, the exception will be re-thrown</param>
        /// <param name="continueOnCapturedContext">If set to <c>true</c>, continue on captured context; this will ensure that the Synchronization Context returns to the calling thread. If set to <c>false</c>, continue on a different context; this will allow the Synchronization Context to continue on a different thread</param>
        public static void SafeFireAndForget(this Task task, in Action<Exception> onException = null, in bool continueOnCapturedContext = false)
        {
            FireAndForgotImpl(task, continueOnCapturedContext, onException);
        }

        static async void FireAndForgotImpl(Task task, bool continueOnCapturedContext, Action<Exception> onException)
        {
            try
            {
                await task.ConfigureAwait(continueOnCapturedContext);
            }
            catch (Exception ex) when (onException != null || _onException != null)
            {
                _onException?.Invoke(ex);
                onException?.Invoke(ex);
                if (_shouldAlwaysRethrowException)
                    throw;
            }
        }
    }
}
