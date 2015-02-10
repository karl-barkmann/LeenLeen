using System;
using System.Windows;

namespace Smart.Practices.Mvvm
{
    /// <summary>
    /// 界面交互接口，提供大部分涉及UI的交互接口。
    /// </summary>
    public interface IUIInteractionService
    {
        /// <summary>
        /// 显示消息对话框。
        /// </summary>
        /// <param name="message">消息内容，使用@NewLine代表换行，接口将自动替换。</param>
        /// <param name="title">消息对话框的标题。</param>
        /// <param name="buttons">消息对话框按钮。</param>
        /// <param name="icon">消息对话框图标。</param>
        /// <returns>对话框返回值。</returns>
        MessageBoxResult ShowMessage(string message, string title,
            MessageBoxButton buttons = MessageBoxButton.OK,
            MessageBoxImage icon = MessageBoxImage.Information);

        /// <summary>
        /// 在指定的父视图模型对应视图上显示指定的视图模型对应的视图。
        /// </summary>
        /// <param name="ownnerViewModel">父窗体的视图模型。</param>
        /// <param name="viewModel">子窗体的视图模型。</param>
        /// <param name="allowUserClose">是否允许用户直接关闭，而不经过程序判断确认。</param>
        /// <param name="allowsTransparency">是否启用窗体透明化。</param>
        /// <param name="title">窗口标题。</param>
        /// <param name="height">窗体高度。</param>
        /// <param name="width">窗体宽度。</param>
        void Show(object viewModel, 
            object ownnerViewModel = null, 
            string title = "", 
            double? width = Double.NaN, 
            double? height = Double.NaN, 
            bool allowUserClose = false, 
            bool allowsTransparency = true);

        /// <summary>
        ///  在指定的父视图模型对应视图上显示指定的视图模型对应的模式对话框视图。
        /// </summary>
        /// <param name="viewModel">子窗体的视图模型。</param>
        /// <param name="ownnerViewModel">父窗体的视图模型。</param>
        /// <param name="title">窗口标题。</param>
        /// <param name="height">窗体高度。</param>
        /// <param name="width">窗体宽度。</param>
        /// <param name="allowUserClose">是否允许用户直接关闭，而不经过程序判断确认。</param>
        /// <param name="allowsTransparency">是否启用窗体透明化。</param>
        /// <returns>窗体对话框返回值。</returns>
        bool? ShowDialog(object viewModel, 
            object ownnerViewModel = null,
            string title = "", 
            double? width = Double.NaN, 
            double? height = Double.NaN, 
            bool allowUserClose = false,
            bool allowsTransparency = true);

        /// <summary>
        /// 查找指定视图模型对应的视图并关闭它。
        /// </summary>
        /// <param name="viewModel">窗体的视图模型。</param>
        void Close(object viewModel);

        /// <summary>
        /// 如果需要的话在UI调度线程上执行操作并获取某个值或对象，否则在当前线程上执行。
        /// </summary>
        /// <typeparam name="T">要获取的值或对象的类型。</typeparam>
        /// <param name="func">值或对象的获取操作。</param>
        /// <returns></returns>
        T InvokeIfNeeded<T>(Func<T> func);

        /// <summary>
        /// 如果需要的话在UI调度线程上执行操作，否则在当前线程上执行。
        /// </summary>
        /// <param name="action">要执行的操作。</param>
        void InvokeIfNeeded(Action action);

        /// <summary>
        /// 如果需要的话在UI调度线程上执行操作，否则在当前线程上执行。
        /// </summary>
        /// <typeparam name="T">要执行的操作需要的参数类型。</typeparam>
        /// <param name="action">要执行的操作。</param>
        /// <param name="args">要执行的操作的参数。</param>
        void InvokeIfNeeded<T>(Action<T> action, T args);

        /// <summary>
        /// 如果需要的话在UI调度线程上执行异步操作，否则在当前线程上执行。
        /// </summary>
        /// <param name="action">要执行的操作。</param>
        /// <param name="action"></param>
        void BeginInvokeIfNeeded(Action action);

        /// <summary>
        /// 如果需要的话在UI调度线程上执行异步操作，否则在当前线程上执行。
        /// </summary>
        /// <typeparam name="T">要执行的操作需要的参数类型。</typeparam>
        /// <param name="action">要执行的操作。</param>
        /// <param name="args">要执行的操作的参数。</param>
        void BeginInvokeIfNeeded<T>(Action<T> action, T args);

        /// <summary>
        /// 打开文件保存对话框。
        /// </summary>
        /// <param name="fileName">默认文件名。</param>
        /// <param name="defaultExit">默认文件扩展。</param>
        /// <param name="filter">文件扩展过滤。</param>
        /// <returns></returns>
        bool? OpenSaveFileDialog(ref string fileName, string defaultExit = ".*", string filter = "*.*");
    }
}
