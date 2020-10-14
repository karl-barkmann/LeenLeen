using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;

namespace Leen.Windows.Interaction
{
    /// <summary>
    /// 界面交互接口，提供大部分涉及UI的交互接口。
    /// </summary>
    public interface IUIInteractionService
    {
        /// <summary>
        /// 获取或设置用于创建弹出窗口的窗口封装接口。
        /// </summary>
        IWindowActivationBehavior ActivationBehavior { get; set; }

        /// <summary>
        /// 获取或设置用于进行弹出窗口与其他窗口交互的窗口交互协定。
        /// </summary>
        IInteropService InteropService { get; set; }

        /// <summary>
        /// 获取或设置此界面交互接口的应用程序主窗体视图模型。
        /// </summary>
        object Shell { get; set; }

        /// <summary>
        /// 显示消息对话框，如果需要将委托到UI线程上执行。
        /// </summary>
        /// <param name="message">消息内容，使用@NewLine代表换行，实现自动替换。</param>
        /// <param name="title">消息对话框的标题。</param>
        /// <param name="buttons">消息对话框按钮。</param>
        /// <param name="icon">消息对话框图标。</param>
        /// <param name="ownerViewModel">父视图模型。</param>
        /// <returns>对话框返回值。</returns>
        MessageBoxResult ShowMessage(string message,
                                     string title,
                                     MessageBoxButton buttons = MessageBoxButton.OK,
                                     MessageBoxImage icon = MessageBoxImage.Information,
                                     object ownerViewModel = null);

        /// <summary>
        /// 在指定的父视图模型对应视图上显示指定的视图模型对应的视图，如果需要将委托到UI线程上执行。
        /// </summary>
        /// <param name="ownerViewModel">父视图模型。当该值为 null 时，降采用应用程序主窗体作为父视图。</param>
        /// <param name="viewModel">子视图模型。</param>
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        void Show(object viewModel, object ownerViewModel);

        /// <summary>
        /// 在指定的父视图模型对应视图上显示指定的视图模型对应的视图，如果需要将委托到UI线程上执行。
        /// </summary>
        /// <param name="viewModel">子视图模型。</param>
        /// <param name="viewAlias">子视图别名，与Xaml中定义的 ViewLocator.Alias 对应。</param>
        /// <param name="ownerViewModel">父视图模型。当该值为 null 时，降采用应用程序主窗体作为父视图。</param>
        void Show(object viewModel, string viewAlias, object ownerViewModel);

        /// <summary>
        ///  在指定的父视图模型对应视图上显示指定的视图模型对应的模式对话框视图，如果需要将委托到UI线程上执行。
        /// </summary>
        /// <param name="viewModel">子视图模型。</param>
        /// <param name="ownerViewModel">父视图模型。当该值为 null 时，降采用应用程序主窗体作为父视图。</param>
        /// <returns>对话框返回值。</returns>
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        bool? ShowDialog(object viewModel, object ownerViewModel);

        /// <summary>
        ///  在指定的父视图模型对应视图上显示指定的视图模型对应的模式对话框视图，如果需要将委托到UI线程上执行。
        /// </summary>
        /// <param name="viewModel">子视图模型。</param>
        /// <param name="viewAlias">子视图别名，与Xaml中定义的 ViewLocator.Alias 对应。</param>
        /// <param name="ownerViewModel">父视图模型。当该值为 null 时，降采用应用程序主窗体作为父视图。</param>
        /// <returns>对话框返回值。</returns>
        bool? ShowDialog(object viewModel, string viewAlias, object ownerViewModel);

        /// <summary>
        /// 查找指定视图模型对应的视图并尝试关闭它，如果需要将委托到UI线程上执行。
        /// </summary>
        /// <param name="viewModel">已显示的视图模型。</param>
        /// <param name="dialogResult">关闭时的返回值。</param>
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        void Close(object viewModel, bool? dialogResult = null);

        /// <summary>
        /// 查找指定视图模型对应的视图并尝试最小化它，如果需要将委托到UI线程上执行。
        /// </summary>
        /// <param name="viewModel">已显示的视图模型。</param>
        void Minimize(object viewModel);

        /// <summary>
        /// 查找指定视图模型对应的视图并尝试激活它，如果需要将委托到UI线程上执行。
        /// </summary>
        /// <param name="viewModel">已显示的视图模型。</param>
        void Activate(object viewModel);

        /// <summary>
        /// 如果需要，将操作调度到默认UI线程器上执行并获取某个值或对象，否则在当前线程上执行。
        /// </summary>
        /// <typeparam name="T">要获取的值或对象的类型。</typeparam>
        /// <param name="func">值或对象的获取操作。</param>
        /// <returns>获取到的值或对象。</returns>
        T InvokeIfNeeded<T>(Func<T> func);

        /// <summary>
        /// 如果需要，将操作调度到拥有指定视图模型关联的视图的线程上执行并获取某个值或对象，否则在当前线程上执行。
        /// </summary>
        /// <typeparam name="T">要获取的值或对象的类型。</typeparam>
        /// <param name="func">值或对象的获取操作。</param>
        /// <param name="viewModel">对应的视图模型。</param>
        /// <returns>获取到的值或对象。</returns>
        T InvokeIfNeeded<T>(Func<T> func, object viewModel);

        /// <summary>
        /// 如果需要，将操作调度到默认UI线程上执行，否则在当前线程上执行。
        /// </summary>
        /// <param name="action">要执行的操作。</param>
        void InvokeIfNeeded(Action action);

        /// <summary>
        /// 如果需要，将操作调度到拥有指定视图模型关联的视图的线程上执行，否则在当前线程上执行。
        /// </summary>
        /// <param name="action">要执行的操作。</param>
        /// <param name="viewModel">对应的视图模型。</param>
        void InvokeIfNeeded(Action action, object viewModel);

        /// <summary>
        /// 如果需要，将操作调度到默认UI线程上执行，否则在当前线程上执行。
        /// </summary>
        /// <typeparam name="T">要执行的操作需要的参数类型。</typeparam>
        /// <param name="action">要执行的操作。</param>
        /// <param name="args">要执行的操作的参数。</param>
        void InvokeIfNeeded<T>(Action<T> action, T args);

        /// <summary>
        /// 如果需要，将操作调度到拥有指定视图模型关联的视图的线程上执行，否则在当前线程上执行。
        /// </summary>
        /// <typeparam name="T">要执行的操作需要的参数类型。</typeparam>
        /// <param name="action">要执行的操作。</param>
        /// <param name="args">要执行的操作的参数。</param>
        /// <param name="viewModel">对应的视图模型。</param>
        void InvokeIfNeeded<T>(Action<T> action, T args, object viewModel);

        /// <summary>
        /// 如果需要，将操作调度到默认UI线程上异步执行，否则在当前线程上立即执行。
        /// </summary>
        /// <param name="action">要执行的操作。</param>
        void BeginInvokeIfNeeded(Action action);

        /// <summary>
        /// 如果需要，将操作调度到拥有指定视图模型关联的视图的线程上异步执行，否则在当前线程上立即执行。
        /// </summary>
        /// <param name="action">要执行的操作。</param>
        /// <param name="viewModel">对应的视图模型。</param>
        void BeginInvokeIfNeeded(Action action, object viewModel);

        /// <summary>
        /// 如果需要，将操作调度到默认UI线程上异步执行，否则在当前线程上立即执行。
        /// </summary>
        /// <typeparam name="T">要执行的操作需要的参数类型。</typeparam>
        /// <param name="action">要执行的操作。</param>
        /// <param name="args">要执行的操作的参数。</param>
        void BeginInvokeIfNeeded<T>(Action<T> action, T args);

        /// <summary>
        /// 如果需要，将操作调度到拥有指定视图模型关联的视图的线程上异步执行，否则在当前线程上立即执行。
        /// </summary>
        /// <typeparam name="T">要执行的操作需要的参数类型。</typeparam>
        /// <param name="action">要执行的操作。</param>
        /// <param name="args">要执行的操作的参数。</param>
        /// <param name="viewModel">对应的视图模型。</param>
        void BeginInvokeIfNeeded<T>(Action<T> action, T args, object viewModel);

        /// <summary>
        /// 打开文件保存对话框，如果需要将委托到UI线程上执行。
        /// </summary>
        /// <param name="fileName">默认文件名。</param>
        /// <param name="defaultExit">默认文件扩展。</param>
        /// <param name="filter">文件扩展过滤。</param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        [SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "0#")]
        bool? OpenSaveFileDialog(ref string fileName, string defaultExit = ".*", string filter = "*.*");

        /// <summary>
        /// 打开文件保存对话框，如果需要将委托到UI线程上执行。
        /// </summary>
        /// <param name="fileName">默认文件名。</param>
        /// <param name="title">对话框标题栏名称。</param>
        /// <param name="defaultExit">默认文件扩展。</param>
        /// <param name="filter">文件扩展过滤。</param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        [SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "0#")]
        bool? OpenSaveFileDialog(ref string fileName, string title, string defaultExit = ".*", string filter = "*.*");

        /// <summary>
        /// 打开文件保存对话框，如果需要将委托到UI线程上执行。
        /// </summary>
        /// <param name="fileName">默认文件名。</param>
        /// <param name="title">对话框标题栏名称。</param>
        /// <param name="initialDirectory">默认保存目录。</param>
        /// <param name="defaultExit">默认文件扩展。</param>
        /// <param name="filter">文件扩展过滤。</param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        [SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "0#")]
        bool? OpenSaveFileDialog(ref string fileName, string title, string initialDirectory, string defaultExit = ".*", string filter = "*.*");

        /// <summary>
        /// 打开文件浏览对话框，如果需要将委托到UI线程上执行。
        /// </summary>
        /// <param name="fileName">默认文件名。</param>
        /// <param name="defaultExit">默认文件扩展。</param>
        /// <param name="filter">文件扩展过滤。</param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        [SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "0#")]
        bool? OpenFileBrowserDialog(ref string fileName, string defaultExit = ".*", string filter = "*.*");

        /// <summary>
        /// 打开文件浏览对话框，如果需要将委托到UI线程上执行。
        /// </summary>
        /// <param name="fileName">默认文件名。</param>
        /// <param name="title">对话框标题栏名称。</param>
        /// <param name="defaultExit">默认文件扩展。</param>
        /// <param name="filter">文件扩展过滤。</param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        [SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "0#")]
        bool? OpenFileBrowserDialog(ref string fileName, string title, string defaultExit = ".*", string filter = "*.*");

        /// <summary>
        /// 打开文件浏览对话框，如果需要将委托到UI线程上执行。
        /// </summary>
        /// <param name="fileName">默认文件名。</param>
        /// <param name="title">对话框标题栏名称。</param>
        /// <param name="initialDirectory">默认保存目录。</param>
        /// <param name="defaultExit">默认文件扩展。</param>
        /// <param name="filter">文件扩展过滤。</param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        [SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "0#")]
        bool? OpenFileBrowserDialog(ref string fileName, string title, string initialDirectory, string defaultExit = ".*", string filter = "*.*");
    }
}
