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
        private readonly bool _multicast;
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
        /// <param name="multicast">确定此异步命令是否可以连续触发，而无需前一命令完成。</param>
        /// <param name="onException">调用命令执行时发生异常的委托。</param>
        public AsyncRelayCommand(
            Func<Task> execute,
            Func<bool> canExecute = null,
            bool multicast = false,
            Action<Exception> onException = null)
        {
            _execute = execute;
            _multicast = multicast;
            _canExecute = canExecute;
            _errorHandler = onException;
        }

        /// <summary>
        /// 获取命令是否正在执行中。
        /// </summary>
        public bool IsExecuting
        {
            get { return _isExecuting; }
            set
            {
                SetProperty(ref _isExecuting, value, () => IsExecuting);
            }
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
            if (!_multicast)
                return !IsExecuting && (_canExecute?.Invoke() ?? true);
            return _canExecute?.Invoke() ?? true;
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
                    IsExecuting = true;
                    RaiseCanExecuteChanged();
                    await _execute();
                }
                finally
                {
                    IsExecuting = false;
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
