using Leen.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Leen.Practices.Mvvm
{
    /// <summary>
    /// 使用无参数委托实现一个<see cref="IAsyncRelayCommand"/>。
    /// </summary>
    public class AsyncRelayCommand : BindableBase, IAsyncRelayCommand, INotifyCanExecuteChangedCommand, ICommand
    {
        private bool _isExecuting;
        private readonly Func<Task> _execute;
        private readonly Func<bool> _canExecute;
        private readonly Action<Exception> _errorHandler;
        private List<WeakReference> _canExecuteChangedHandlers;

        /// <summary>
        /// 私有构造函数。
        /// </summary>
        protected AsyncRelayCommand()
        {

        }

        /// <summary>
        /// 构造 <see cref="AsyncRelayCommand"/> 的实例。
        /// </summary>
        /// <param name="execute">调用命令时执行的异步委托。</param>
        /// <param name="canExecute">确定此命令是否可在其当前状态下执行的委托。</param>
        /// <param name="onException">调用命令执行时发生异常的委托。</param>
        public AsyncRelayCommand(
            Func<Task> execute,
            Func<bool> canExecute = null,
            Action<Exception> onException = null)
        {
            _execute = execute;
            _canExecute = canExecute;
            _errorHandler = onException;
        }

        /// <summary>
        /// 当出现影响是否应执行该命令的更改时发生。
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
                CanExecuteChangedCommandManager.AddWeakReferenceHandler(ref _canExecuteChangedHandlers, value, 2);
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
                CanExecuteChangedCommandManager.RemoveWeakReferenceHandler(_canExecuteChangedHandlers, value);
            }
        }

        /// <summary>
		/// 确定此命令是否可在其当前状态下执行的方法。
		/// </summary>
		/// <returns> 如果可执行此命令，则为 true；否则为 false。</returns>
        public virtual bool CanExecute()
        {
            return !_isExecuting && (_canExecute?.Invoke() ?? true);
        }

        /// <summary>
		/// 在调用此命令时要调用的异步方法。
		/// </summary>
		/// <returns>返回命令执行生成的 <see cref="Task"/> 对象。</returns>
        public virtual async Task ExecuteAsync()
        {
            if (CanExecute())
            {
                try
                {
                    _isExecuting = true;
                    await _execute();
                }
                finally
                {
                    _isExecuting = false;
                }
            }

            RaiseCanExecuteChanged();
        }

        /// <summary>
        /// 通知影响是否应执行该命令的更改。
        /// </summary>
        public virtual void RaiseCanExecuteChanged()
        {
            CanExecuteChangedCommandManager.CallWeakReferenceHandlers(_canExecuteChangedHandlers);
        }

        #region Explicit implementations
        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute();
        }

        void ICommand.Execute(object parameter)
        {
            ExecuteAsync().SafeFireAndForget(_errorHandler);
        }
        #endregion
    }
}
