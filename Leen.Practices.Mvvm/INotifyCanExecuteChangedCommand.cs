using System.Windows.Input;

namespace Leen.Practices.Mvvm
{
    /// <summary>
    /// 定义一个可通知可执行状态更改的命令。
    /// </summary>
    public interface INotifyCanExecuteChangedCommand : ICommand
    {
        /// <summary>
        /// 引发 <seealso cref="ICommand.CanExecuteChanged"/> 事件。
        /// </summary>
        void RaiseCanExecuteChanged();
    }
}
