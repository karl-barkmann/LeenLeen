using System.Windows.Input;

namespace Leen.Practices.Mvvm
{
	/// <summary>
	/// 定义一个可委托执行动作的命令。
	/// </summary>
	public interface IRelayCommand<in TExecute, in TCanExecute> : IRelayCommand<TExecute>, INotifyCanExecuteChangedCommand, ICommand
	{
		/// <summary>
		/// 确定此命令是否可在其当前状态下执行的方法。
		/// </summary>
		/// <returns> 如果可执行此命令，则为 true；否则为 false。</returns>
		/// <param name="parameter">此命令使用的数据。如果此命令不需要传递数据，则该对象可以设置为 null。</param>
		bool CanExecute(TCanExecute parameter);
	}

	/// <summary>
	/// 定义一个可委托执行动作的命令。
	/// </summary>
	public interface IRelayCommand<in T> : INotifyCanExecuteChangedCommand, ICommand
	{
		/// <summary>
		/// 在调用此命令时要调用的异步方法。
		/// </summary>
		/// <param name="parameter">此命令使用的数据。如果此命令不需要传递数据，则该对象可以设置为 null。</param>
		void Execute(T parameter);
	}

	/// <summary>
	/// 定义一个可委托执行动作的命令。
	/// </summary>
	public interface IRelayCommand : INotifyCanExecuteChangedCommand, ICommand
	{
		/// <summary>
		/// 定义在调用此命令时要调用的方法。
		/// </summary>
		void Execute();

		/// <summary>
		/// 确定此命令是否可在其当前状态下执行的方法。
		/// </summary>
		/// <returns> 如果可执行此命令，则为 true；否则为 false。</returns>
		bool CanExecute();
	}
}
