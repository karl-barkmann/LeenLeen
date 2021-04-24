using System.Threading.Tasks;
using System.Windows.Input;

namespace Leen.Practices.Mvvm
{
	/// <summary>
	/// 定义一个基于Task的异步命令接口。
	/// </summary>
	public interface IAsyncRelayCommand<in TExecute, in TCanExecute> : IAsyncRelayCommand<TExecute>, INotifyCanExecuteChangedCommand, ICommand
	{
		/// <summary>
		/// 确定此命令是否可在其当前状态下执行的方法。
		/// </summary>
		/// <returns> 如果可执行此命令，则为 true；否则为 false。</returns>
		/// <param name="parameter">此命令使用的数据。如果此命令不需要传递数据，则该对象可以设置为 null。</param>
		bool CanExecute(TCanExecute parameter);
	}

	/// <summary>
	/// 定义一个基于Task的异步命令接口。
	/// </summary>
	public interface IAsyncRelayCommand<in T> : INotifyCanExecuteChangedCommand, ICommand
    {
		/// <summary>
		/// 在调用此命令时要调用的异步方法。
		/// </summary>
		/// <returns>返回命令执行生成的 <see cref="Task"/> 对象。</returns>
		/// <param name="parameter">此命令使用的数据。如果此命令不需要传递数据，则该对象可以设置为 null。</param>
		Task ExecuteAsync(T parameter);
	}

	/// <summary>
	/// 定义一个基于Task的异步命令接口。
	/// </summary>
	public interface IAsyncRelayCommand : INotifyCanExecuteChangedCommand, ICommand
	{
		/// <summary>
		/// 在调用此命令时要调用的异步方法。
		/// </summary>
		/// <returns>返回命令执行生成的 <see cref="Task"/> 对象。</returns>
		Task ExecuteAsync();

		/// <summary>
		/// 确定此命令是否可在其当前状态下执行的方法。
		/// </summary>
		/// <returns> 如果可执行此命令，则为 true；否则为 false。</returns>
		bool CanExecute();
	}
}
