using Leen.Common;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Leen.Practices.Mvvm
{
    /// <summary>
    /// 使用参数委托实现一个<see cref="IAsyncRelayCommand"/>。
    /// </summary>
    public class AsyncRelayCommand<T> : AsyncRelayCommand, IAsyncRelayCommand<T, T>, IAsyncRelayCommand<T>, INotifyCanExecuteChangedCommand, ICommand
    {
        private bool _isExecuting;
        private readonly Func<T, Task> _execute;
        private readonly Func<T, bool> _canExecute;
        private readonly Action<Exception> _errorHandler;

        /// <summary>
        /// 构造 <see cref="AsyncRelayCommand"/> 的实例。
        /// </summary>
        /// <param name="execute">调用命令时执行的异步委托。</param>
        /// <param name="canExecute">确定此命令是否可在其当前状态下执行的委托。</param>
        /// <param name="onException">调用命令执行时发生异常的委托。</param>
        public AsyncRelayCommand(
            Func<T, Task> execute,
            Func<T, bool> canExecute = null,
            Action<Exception> onException = null)
        {
            _execute = execute;
            _canExecute = canExecute;
            _errorHandler = onException;
        }

        /// <summary>
        /// 确定此命令是否可在其当前状态下执行的方法。
        /// </summary>
        /// <returns> 如果可执行此命令，则为 true；否则为 false。</returns>
		/// <param name="parameter">此命令使用的数据。如果此命令不需要传递数据，则该对象可以设置为 null。</param>
        public bool CanExecute(T parameter)
        {
            return !_isExecuting && (_canExecute?.Invoke(parameter) ?? true);
        }

        /// <summary>
		/// 在调用此命令时要调用的异步方法。
		/// </summary>
		/// <param name="parameter">此命令使用的数据。如果此命令不需要传递数据，则该对象可以设置为 null。</param>
		/// <returns>返回命令执行生成的 <see cref="Task"/> 对象。</returns>
        public async Task ExecuteAsync(T parameter)
        {
            if (CanExecute())
            {
                try
                {
                    _isExecuting = true;
                    await _execute(parameter);
                }
                finally
                {
                    _isExecuting = false;
                }
            }

            RaiseCanExecuteChanged();
        }

        #region Explicit implementations
        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute((T)parameter);
        }

        void ICommand.Execute(object parameter)
        {
            ExecuteAsync((T)parameter).SafeFireAndForget(_errorHandler);
        }
        #endregion
    }
}
