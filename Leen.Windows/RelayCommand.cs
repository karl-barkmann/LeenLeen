using System;
using System.Windows.Input;

namespace Leen.Windows
{
    /// <summary>
    /// 
    /// </summary>
    internal class RelayCommand : ICommand
    {
        #region 成员变量

        /// <summary>
        /// 
        /// </summary>
        protected Action _targetExecuteMethod;
        /// <summary>
        /// 
        /// </summary>
        protected Func<bool> _targetCanExcuteMethod;

        #endregion

        #region 构造函数

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateCommand&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="executeMethod">执行的命令</param>
        public RelayCommand(Action executeMethod)
        {
            _targetExecuteMethod = executeMethod;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateCommand&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="excuteMethod">执行的命令</param>
        /// <param name="canExcuteMethod">是否可执行</param>
        public RelayCommand(Action excuteMethod, Func<bool> canExcuteMethod)
        {
            _targetExecuteMethod = excuteMethod;
            _targetCanExcuteMethod = canExcuteMethod;
        }

        #endregion

        #region ICommand 成员

        /// <summary>
        /// Determines whether this instance can execute.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance can execute; otherwise, <c>false</c>.
        /// </returns>
        public void RaiseCanExecute()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, new EventArgs());
        }

        /// <summary>
        /// 定义用于确定此命令是否可以在其当前状态下执行的方法。
        /// </summary>
        /// <param name="parameter">此命令使用的数据。 如果此命令不需要传递数据，则该对象可以设置为 null。</param>
        /// <returns>如果可以执行此命令，则为 true；否则为 false。</returns>
        public bool CanExecute(object parameter)
        {
            if (_targetCanExcuteMethod != null)
            {
                return _targetCanExcuteMethod();
            }

            if (_targetExecuteMethod != null)
                return true;
            return false;
        }

        /// <summary>
        /// 当出现影响是否应执行该命令的更改时发生。
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// 定义在调用此命令时调用的方法。
        /// </summary>
        /// <param name="parameter">此命令使用的数据。 如果此命令不需要传递数据，则该对象可以设置为 null。</param>
        public void Execute(object parameter)
        {
            if (_targetExecuteMethod != null)
                _targetExecuteMethod();
        }

        /// <summary>
        /// 触发CanExcuteChanged事件
        /// </summary>
        public void OnCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, new EventArgs());
        }

        #endregion
    }
}
