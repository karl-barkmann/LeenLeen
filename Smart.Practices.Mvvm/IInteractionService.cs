using System.Windows;

namespace Smart.Practices.Mvvm
{
    /// <summary>
    /// 用户界面交互接口。
    /// </summary>
    public interface IInteractionService
    {
        /// <summary>
        /// 注册交互界面。
        /// </summary>
        /// <param name="view">交互界面元素。</param>
        void Register(FrameworkElement view);

        /// <summary>
        /// 退订交互界面。
        /// </summary>
        /// <param name="view">交互界面元素。</param>
        void Unregister(FrameworkElement view);

        /// <summary>
        /// 显示交互界面窗口。
        /// </summary>
        /// <typeparam name="T">交换界面窗口的类型。</typeparam>
        /// <param name="viewModel">交换界面窗口的视图模型。</param>
        /// <param name="ownnerViewModel">交换界面窗口的父窗口的类型。</param>
        void Show<T>(object viewModel, object ownnerViewModel = null) where T : Window;

        /// <summary>
        /// 显示交互界面窗口。
        /// </summary>
        /// <param name="viewModel">交换界面窗口的视图模型。</param>
        /// <param name="ownnerViewModel">交换界面窗口的父窗口的类型。</param>
        void Show(object viewModel, object ownnerViewModel = null);

        /// <summary>
        /// 显示交互界面窗口。
        /// <para>模式对话框。</para>
        /// </summary>
        /// <typeparam name="T">交换界面窗口的类型。</typeparam>
        /// <param name="viewModel">交换界面窗口的视图模型。</param>
        /// <param name="ownnerViewModel">交换界面窗口的父窗口的类型。</param>
        /// <returns>一个值指示用户操作结果。</returns>
        bool? ShowDialog<T>(object viewModel, object ownnerViewModel = null) where T : Window;

        /// <summary>
        /// 显示交互界面窗口。
        /// </summary>
        /// <para>模式对话框。</para>
        /// <param name="viewModel">交换界面窗口的视图模型。</param>
        /// <param name="ownnerViewModel">交换界面窗口的父窗口的类型。</param>
        /// <returns>一个值指示用户操作结果。</returns>
        bool? ShowDialog(object viewModel, object ownnerViewModel = null);

        /// <summary>
        /// 显示消息对话框。
        /// </summary>
        /// <param name="ownerViewModel">消息对话框的父窗口的类型。</param>
        /// <param name="messageText">消息内容。</param>
        /// <param name="caption">消息对话框标题。</param>
        /// <param name="button">消息框上的按钮。</param>
        /// <param name="image">消息框的图标。</param>
        /// <param name="result">指定消息框的默认结果。</param>
        /// <param name="options">消息框的选项。</param>
        /// <returns>一个值指定用户单击了哪个消息框按钮。</returns>
        MessageBoxResult ShowMessageBox(object ownerViewModel, string messageText, string caption, MessageBoxButton button, MessageBoxImage image, MessageBoxResult result, MessageBoxOptions options);
    }
}
