using Leen.Native;
using Leen.Windows.Interaction;
using Microsoft.Win32;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

namespace Leen.Practices.Mvvm
{
    /// <summary>
    /// 提供界面交互接口的实现。
    /// <para>
    /// 若要实现自定义的界面交互接口应当继承该类。
    /// </para>
    /// </summary>
    public class UIInteractionService : IUIInteractionService
    {
        private object _pushedModal;

        /// <summary>
        /// 构造 <see cref="UIInteractionService"/> 的实例。
        /// </summary>
        public UIInteractionService()
        {
            ActivationBehavior = new WindowActivationBehavior();
            InteropService = Mvvm.InteropService.Default;
        }

        /// <summary>
        /// 获取或设置用户弹出窗口的窗口封装接口。
        /// </summary>
        public virtual IWindowActivationBehavior ActivationBehavior { get; set; }

        /// <summary>
        /// 获取或设置用于弹出窗口与其他窗口进行交互的窗口交互协定。
        /// </summary>
        public virtual IInteropService InteropService { get; set; }

        /// <summary>
        /// 获取或设置此界面交互接口的应用程序主窗体视图模型。
        /// </summary>
        public virtual object Shell { get; set; }

        /// <summary>
        /// 显示消息对话框，如果需要将委托到UI线程上执行。
        /// </summary>
        /// <param name="message">消息内容，使用@NewLine代表换行，实现自动替换。</param>
        /// <param name="title">消息对话框的标题。</param>
        /// <param name="buttons">消息对话框按钮。</param>
        /// <param name="icon">消息对话框图标。</param>
        /// <param name="ownerViewModel">父视图模型。</param>
        /// <returns>对话框返回值。</returns>
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public MessageBoxResult ShowMessage(string message,
                                            string title,
                                            MessageBoxButton buttons = MessageBoxButton.OK,
                                            MessageBoxImage icon = MessageBoxImage.Information,
                                            object ownerViewModel = null)
        {
            StringBuilder content = new StringBuilder(message);
            content = content.Replace("@NewLine", Environment.NewLine);
            content = content.Replace("@newline", Environment.NewLine);
            content = content.Replace("@NEWLINE", Environment.NewLine);

            if (ownerViewModel == null)
                ownerViewModel = _pushedModal ?? Shell;
            MessageBoxResult result = InvokeIfNeeded(() => ShowMessageImpl(content.ToString(), title, buttons, icon, null));
            return result;
        }

        /// <summary>
        /// 如果需要，在默认UI调度线程上执行操作并获取某个值或对象，否则在当前线程上执行。
        /// </summary>
        /// <typeparam name="T">要获取的值或对象的类型。</typeparam>
        /// <param name="func">值或对象的获取操作。</param>
        /// <returns>值或对象的获取操作的返回值。</returns>
        public T InvokeIfNeeded<T>(Func<T> func)
        {
            if (func == null)
            {
                throw new ArgumentNullException(nameof(func));
            }

            var application = Application.Current;
            if (application == null)
                application = new Application();
            var dispatcher = application.Dispatcher;
            return Invoke(func, dispatcher);
        }

        /// <summary>
        /// 如果需要，将操作调度到拥有指定视图模型关联的视图的线程上执行并获取某个值或对象，否则在当前线程上执行。
        /// </summary>
        /// <typeparam name="T">要获取的值或对象的类型。</typeparam>
        /// <param name="func">值或对象的获取操作。</param>
        /// <param name="viewModel">对应的视图模型。</param>
        /// <returns>获取到的值或对象。</returns>
        public T InvokeIfNeeded<T>(Func<T> func, object viewModel)
        {
            if (func == null)
            {
                throw new ArgumentNullException(nameof(func));
            }

            if (viewModel == null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            var viewForViewModel = ViewLocationProvider.FindViewForViewModel(viewModel);
            var dispatcher = viewForViewModel.ActualView.Dispatcher;
            return Invoke(func, dispatcher);
        }

        /// <summary>
        /// 如果需要，将操作调度到默认UI线程上执行，否则在当前线程上执行。
        /// </summary>
        /// <param name="action">要执行的操作。</param>
        public void InvokeIfNeeded(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            var application = Application.Current;
            if (application == null)
                application = new Application();
            var dispatcher = application.Dispatcher;
            Invoke(action, dispatcher);
        }

        /// <summary>
        /// 如果需要，将操作调度到拥有指定视图模型关联的视图的线程上执行，否则在当前线程上执行。
        /// </summary>
        /// <param name="action">要执行的操作。</param>
        /// <param name="viewModel">对应的视图模型。</param>
        public void InvokeIfNeeded(Action action, object viewModel)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            if (viewModel == null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            var viewForViewModel = ViewLocationProvider.FindViewForViewModel(viewModel);
            var dispatcher = viewForViewModel.ActualView.Dispatcher;
            Invoke(action, dispatcher);
        }

        /// <summary>
        /// 如果需要，将操作调度到默认UI线程上执行，否则在当前线程上执行。
        /// </summary>
        /// <typeparam name="T">要执行的操作需要的参数类型。</typeparam>
        /// <param name="action">要执行的操作。</param>
        /// <param name="args">要执行的操作的参数。</param>
        public void InvokeIfNeeded<T>(Action<T> action, T args)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            var application = Application.Current;
            if (application == null)
                application = new Application();
            var dispatcher = application.Dispatcher;
            Invoke(action, args, dispatcher);
        }

        /// <summary>
        /// 如果需要，将操作调度到拥有指定视图模型关联的视图的线程上执行，否则在当前线程上执行。
        /// </summary>
        /// <typeparam name="T">要执行的操作需要的参数类型。</typeparam>
        /// <param name="action">要执行的操作。</param>
        /// <param name="args">要执行的操作的参数。</param>
        /// <param name="viewModel">对应的视图模型。</param>
        public void InvokeIfNeeded<T>(Action<T> action, T args, object viewModel)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            if (viewModel == null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            var viewForViewModel = ViewLocationProvider.FindViewForViewModel(viewModel);
            var dispatcher = viewForViewModel.ActualView.Dispatcher;
            Invoke(action, args, dispatcher);
        }

        /// <summary>
        /// 如果需要，将操作调度到默认UI线程上异步执行，否则在当前线程上立即执行。
        /// </summary>
        /// <param name="action">要执行的操作。</param>
        public void BeginInvokeIfNeeded(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            var application = Application.Current;
            if (application == null)
                application = new Application();
            var dispatcher = application.Dispatcher;
            BeginInvoke(action, dispatcher);
        }

        /// <summary>
        /// 如果需要，将操作调度到拥有指定视图模型关联的视图的线程上异步执行，否则在当前线程上立即执行。
        /// </summary>
        /// <param name="action">要执行的操作。</param>
        /// <param name="viewModel">对应的视图模型。</param>
        public void BeginInvokeIfNeeded(Action action, object viewModel)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            if (viewModel == null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            var viewForViewModel = ViewLocationProvider.FindViewForViewModel(viewModel);
            var dispatcher = viewForViewModel.ActualView.Dispatcher;
            BeginInvoke(action, dispatcher);
        }

        /// <summary>
        /// 如果需要，将操作调度到默认UI线程上异步执行，否则在当前线程上立即执行。
        /// </summary>
        /// <typeparam name="T">要执行的操作需要的参数类型。</typeparam>
        /// <param name="action">要执行的操作。</param>
        /// <param name="args">要执行的操作的参数。</param>
        public void BeginInvokeIfNeeded<T>(Action<T> action, T args)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            var application = Application.Current;
            if (application == null)
                application = new Application();
            var dispatcher = application.Dispatcher;
            BeginInvoke(action, args, dispatcher);
        }

        /// <summary>
        /// 如果需要，将操作调度到拥有指定视图模型关联的视图的线程上异步执行，否则在当前线程上立即执行。
        /// </summary>
        /// <typeparam name="T">要执行的操作需要的参数类型。</typeparam>
        /// <param name="action">要执行的操作。</param>
        /// <param name="args">要执行的操作的参数。</param>
        /// <param name="viewModel">对应的视图模型。</param>
        public void BeginInvokeIfNeeded<T>(Action<T> action, T args, object viewModel)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            if (viewModel == null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            var viewForViewModel = ViewLocationProvider.FindViewForViewModel(viewModel);
            var dispatcher = viewForViewModel.ActualView.Dispatcher;
            BeginInvoke(action, args, dispatcher);
        }

        /// <summary>
        /// 在指定的父视图模型对应视图上显示指定的视图模型对应的视图，如果需要将委托到UI线程上执行。
        /// </summary>
        /// <param name="viewModel">子窗体的视图模型。</param>
        /// <param name="ownerViewModel">父窗体的视图模型。当该值为 null 时，将采用应用程序主窗体作为父视图。</param>
        public void Show(object viewModel, object ownerViewModel)
        {
            if (viewModel == null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            void showCallback()
            {
                IView view = ViewLocationProvider.GetViewForViewModel(viewModel);
                Show(viewModel, view, ownerViewModel);
            }

            if (ownerViewModel != null)
            {
                InvokeIfNeeded(() =>
                {
                    showCallback();
                }, ownerViewModel);
            }
            else
            {
                InvokeIfNeeded(() =>
                {
                    showCallback();
                });
            }
        }

        /// <summary>
        /// 在指定的父视图模型对应视图上显示指定的视图模型对应的视图，如果需要将委托到UI线程上执行。
        /// </summary>
        /// <param name="viewModel">子视图模型。</param>
        /// <param name="viewAlias">子视图别名，与Xaml中定义的 ViewLocator.Alias 对应。</param>
        /// <param name="ownerViewModel">父视图模型。当该值为 null 时，将采用应用程序主窗体作为父视图。</param>
        public void Show(object viewModel, string viewAlias, object ownerViewModel)
        {
            if (viewModel == null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            if (string.IsNullOrEmpty(viewAlias))
            {
                throw new ArgumentException($"{nameof(viewAlias)} can not be null or empty", nameof(viewAlias));
            }

            void showCallback()
            {
                IView view = ViewLocationProvider.GetViewForViewModel(viewModel);
                Show(viewModel, view, ownerViewModel);
            }

            if (ownerViewModel != null)
            {
                InvokeIfNeeded(() =>
                {
                    showCallback();
                }, ownerViewModel);
            }
            else
            {
                InvokeIfNeeded(() =>
                {
                    showCallback();
                });
            }
        }

        /// <summary>
        /// 在指定的父视图模型对应视图上显示指定的视图模型对应的模式对话框视图，如果需要将委托到UI线程上执行。
        /// </summary>
        /// <param name="viewModel">子窗体的视图模型。</param>
        /// <param name="ownerViewModel">父窗体的视图模型。当该值为 null 时，将采用应用程序主窗体作为父视图。</param>
        /// <returns>窗体对话框返回值。</returns>
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public bool? ShowDialog(object viewModel, object ownerViewModel)
        {
            if (viewModel == null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            bool? showCallback()
            {
                IView view = ViewLocationProvider.GetViewForViewModel(viewModel);
                return ShowDialog(viewModel, view, ownerViewModel);
            }

            if (ownerViewModel != null)
            {
                return InvokeIfNeeded(() =>
                {
                    return showCallback();
                }, ownerViewModel);
            }
            else
            {
                return InvokeIfNeeded(() =>
                {
                    return showCallback();
                });
            }
        }

        /// <summary>
        ///  在指定的父视图模型对应视图上显示指定的视图模型对应的模式对话框视图，如果需要将委托到UI线程上执行。
        /// </summary>
        /// <param name="viewModel">子视图模型。</param>
        /// <param name="viewAlias">子视图别名，与Xaml中定义的 ViewLocator.Alias 对应。</param>
        /// <param name="ownerViewModel">父视图模型。当该值为 null 时，将采用应用程序主窗体作为父视图。</param>
        /// <returns>对话框返回值。</returns>
        public bool? ShowDialog(object viewModel, string viewAlias, object ownerViewModel)
        {
            if (viewModel == null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            if (string.IsNullOrEmpty(viewAlias))
            {
                throw new ArgumentException($"{nameof(viewAlias)} can not be null or empty", nameof(viewAlias));
            }

            bool? showCallback()
            {
                IView view = ViewLocationProvider.GetViewForViewModel(viewModel, viewAlias);
                return ShowDialog(viewModel, view, ownerViewModel);
            }

            if (ownerViewModel != null)
            {
                return InvokeIfNeeded(() =>
                {
                    return showCallback();
                }, ownerViewModel);
            }
            else
            {
                return InvokeIfNeeded(() =>
                {
                    return showCallback();
                });
            }
        }

        /// <summary>
        /// 查找指定视图模型对应的视图并尝试关闭它，如果需要将委托到UI线程上执行。
        /// </summary>
        /// <param name="viewModel">窗体的视图模型。</param>
        /// <param name="dialogResult">窗体关闭时的对话框返回值。</param>
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public void Close(object viewModel, bool? dialogResult = null)
        {
            if (viewModel == null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            InvokeIfNeeded(() =>
            {
                IView view = ViewLocationProvider.FindViewForViewModel(viewModel);

                if (view is IDisposable disposableView)
                {
                    disposableView.Dispose();
                }

                ViewLocator.SetIsRegistered(view.ActualView, false);
                Close(viewModel, view, dialogResult);

                if (view.DataContext is ViewModelBase viewModelBase)
                {
                    viewModelBase.CleanUp();
                }

                if (view.DataContext is IDisposable disposable)
                {
                    disposable.Dispose();
                }

            }, viewModel);
        }


        /// <summary>
        /// 查找指定视图模型对应的视图并尝试最小化它，如果需要将委托到UI线程上执行。
        /// </summary>
        /// <param name="viewModel">子窗体的视图模型。</param>
        public void Minimize(object viewModel)
        {
            if (viewModel == null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            InvokeIfNeeded(() =>
            {
                IView view = ViewLocationProvider.FindViewForViewModel(viewModel);
                Minimize(viewModel, view);
            }, viewModel);
        }

        /// <summary>
        /// 查找指定视图模型对应的视图并尝试激活它，如果需要将委托到UI线程上执行。
        /// </summary>
        /// <param name="viewModel">窗体的视图模型。</param>
        public void Activate(object viewModel)
        {
            if (viewModel == null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            InvokeIfNeeded(() =>
            {
                IView view = ViewLocationProvider.FindViewForViewModel(viewModel);
                Activate(viewModel, view);
            }, viewModel);
        }

        /// <summary>
        /// 打开文件保存对话框，如果需要将委托到UI线程上执行。
        /// </summary>
        /// <param name="fileName">默认文件名。</param>
        /// <param name="defaultExit">默认文件扩展。</param>
        /// <param name="filter">文件扩展过滤。</param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public bool? OpenSaveFileDialog(ref string fileName, string defaultExit = ".*", string filter = "*.*")
        {
            var dlg = new SaveFileDialog
            {
                FileName = fileName,
                DefaultExt = defaultExit,
                Filter = filter,
                CheckPathExists = true,
                AddExtension = true,
                ValidateNames = true
            };

            var result = dlg.ShowDialog();
            if (result == true)
                fileName = dlg.FileName;
            return result;
        }

        /// <summary>
        /// 打开文件保存对话框，如果需要将委托到UI线程上执行。
        /// </summary>
        /// <param name="fileName">默认文件名。</param>
        /// <param name="title">对话框标题栏名称。</param>
        /// <param name="defaultExit">默认文件扩展。</param>
        /// <param name="filter">文件扩展过滤。</param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public bool? OpenSaveFileDialog(ref string fileName, string title, string defaultExit = ".*", string filter = "*.*")
        {
            var dlg = new SaveFileDialog
            {
                FileName = fileName,
                DefaultExt = defaultExit,
                Filter = filter,
                CheckPathExists = true,
                AddExtension = true,
                ValidateNames = true,
                Title = title,
            };

            var result = dlg.ShowDialog();
            if (result == true)
                fileName = dlg.FileName;
            return result;
        }

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
        public bool? OpenSaveFileDialog(ref string fileName, string title, string initialDirectory, string defaultExit = ".*", string filter = "*.*")
        {
            var dlg = new SaveFileDialog
            {
                FileName = fileName,
                DefaultExt = defaultExit,
                Filter = filter,
                CheckPathExists = true,
                AddExtension = true,
                ValidateNames = true,
                Title = title,
                InitialDirectory = initialDirectory
            };

            var result = dlg.ShowDialog();
            if (result == true)
                fileName = dlg.FileName;
            return result;
        }

        /// <summary>
        /// 打开文件浏览对话框，如果需要将委托到UI线程上执行。
        /// </summary>
        /// <param name="fileName">默认文件名。</param>
        /// <param name="defaultExit">默认文件扩展。</param>
        /// <param name="filter">文件扩展过滤。</param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public bool? OpenFileBrowserDialog(ref string fileName, string defaultExit = ".*", string filter = "*.*")
        {
            var dlg = new OpenFileDialog
            {
                FileName = fileName,
                DefaultExt = defaultExit,
                Filter = filter,
                CheckPathExists = true,
                AddExtension = true,
                ValidateNames = true
            };

            var result = dlg.ShowDialog();
            if (result == true)
                fileName = dlg.FileName;
            return result;
        }

        /// <summary>
        /// 打开文件浏览对话框，如果需要将委托到UI线程上执行。
        /// </summary>
        /// <param name="fileName">默认文件名。</param>
        /// <param name="title">对话框标题栏名称。</param>
        /// <param name="defaultExit">默认文件扩展。</param>
        /// <param name="filter">文件扩展过滤。</param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public bool? OpenFileBrowserDialog(ref string fileName, string title, string defaultExit = ".*", string filter = "*.*")
        {
            var dlg = new OpenFileDialog
            {
                FileName = fileName,
                DefaultExt = defaultExit,
                Filter = filter,
                CheckPathExists = true,
                AddExtension = true,
                ValidateNames = true,
                Title = title,
            };

            var result = dlg.ShowDialog();
            if (result == true)
                fileName = dlg.FileName;
            return result;
        }

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
        public bool? OpenFileBrowserDialog(ref string fileName, string title, string initialDirectory, string defaultExit = ".*", string filter = "*.*")
        {
            var dlg = new OpenFileDialog
            {
                FileName = fileName,
                DefaultExt = defaultExit,
                Filter = filter,
                CheckPathExists = true,
                AddExtension = true,
                ValidateNames = true,
                Title = title,
                InitialDirectory = initialDirectory
            };

            var result = dlg.ShowDialog();
            if (result == true)
                fileName = dlg.FileName;
            return result;
        }

        /// <summary>
        /// 在指定的父视图模型对应视图上显示指定的视图模型对应的非模式对话框视图，如果需要将委托到UI线程上执行。
        /// </summary>
        /// <param name="viewModel">子窗体的视图模型。</param>
        /// <param name="view">子视图。</param>
        /// <param name="ownerViewModel">父窗体的视图模型。</param>
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        protected virtual void Show(object viewModel, IView view, object ownerViewModel = null)
        {
            IWindow dialog;
            if (view is Window dialogView)
            {
                dialog = new WindowWrapper(dialogView);
            }
            else
            {
                dialog = ActivationBehavior.CreateWindow();
                dialog.Content = view.ActualView;
            }
            RetrieveWindowProperties(view, dialog);

            if (ownerViewModel == null)
                ownerViewModel = _pushedModal ?? Shell;

            if (ownerViewModel != null)
                dialog.Owner = ViewLocationProvider.FindOwnerWindow(ownerViewModel);

            if (dialog.Owner == null && InteropService != null)
            {
                var ownnerHwnd = InteropService.Shell;
                dialog.Owner = ownnerHwnd;
            }

            dialog.DataContext = viewModel;
            dialog.Show();
        }

        /// <summary>
        /// 查找指定视图模型对应的视图并尝试关闭它，如果需要将委托到UI线程上执行。
        /// </summary>
        /// <param name="viewModel">子窗体视图模型。</param>
        /// <param name="view">子视图。</param>
        /// <param name="dialogResult">窗体对话框返回值。</param>
        protected virtual void Close(object viewModel, IView view, bool? dialogResult)
        {
            if (ViewLocationProvider.IsRootView(view) && !(view is Window))
            {
                //Root view can not be closed.
                //此时的视图可能是跨进程的插件界面根视图
                throw new InvalidOperationException("Can not close a root visual view.");
            }

            Window dialog = ViewLocationProvider.FindOwnerWindow(view);
            if (dialog == null)
            {
                throw new InvalidOperationException("In most cases, this is because you are trying to close the same view twice.");
            }
            var interopHelper = new WindowInteropHelper(dialog);
            if (interopHelper.Handle != InteropService.Shell.Handle)
            {
                if (ComponentDispatcher.IsThreadModal)
                {
                    dialog.DialogResult = dialogResult;
                }
                else
                {
                    throw new InvalidOperationException(
                        string.Format($"Only modal window can set dialog result. If you need a dialog result , try {nameof(ShowDialog)} function instead of {nameof(Show)}."));
                }
            }

            dialog.Close();
        }

        /// <summary>
        /// 查找指定视图模型对应的视图并尝试最小化它，如果需要将委托到UI线程上执行。
        /// </summary>
        /// <param name="viewModel">子窗体的视图模型。</param>
        /// <param name="view">子视图。</param>
        protected virtual void Minimize(object viewModel, IView view)
        {
            if (ViewLocationProvider.IsRootView(view) && !(view is Window))
            {
                //Root view can not be closed.
                //此时的视图可能是跨进程的插件界面根视图
                throw new InvalidOperationException("Can not minimize a root visual view.");
            }

            Window dialog = ViewLocationProvider.FindOwnerWindow(view);
            if (dialog == null)
            {
                throw new InvalidOperationException($"Call {nameof(Show)} or {nameof(ShowDialog)} before trying to minimize it.");
            }
            dialog.WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// 查找指定视图模型对应的视图并尝试激活它，如果需要将委托到UI线程上执行。
        /// </summary>
        /// <param name="viewModel">子窗体的视图模型。</param>
        /// <param name="view">子视图。</param>
        protected virtual void Activate(object viewModel, IView view)
        {
            if (ViewLocationProvider.IsRootView(view) && !(view is Window))
            {
                //Root view can not be closed.
                //此时的视图可能是跨进程的插件界面根视图
                User32.SetActiveWindow(InteropService.Shell.Handle);
                return;
            }

            Window dialog = ViewLocationProvider.FindOwnerWindow(view);
            if (dialog == null)
            {
                throw new InvalidOperationException($"Call {nameof(Show)} or {nameof(ShowDialog)} before trying to activate it.");
            }
            dialog.Activate();
        }

        /// <summary>
        /// 在指定的父视图模型对应视图上显示指定的视图模型对应的模式对话框视图，如果需要将委托到UI线程上执行。
        /// </summary>
        /// <param name="viewModel">子窗体的视图模型。</param>
        /// <param name="view">子视图。</param>
        /// <param name="ownerViewModel">父窗体的视图模型。</param>
        /// <returns>窗体对话框的返回值。</returns>
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        protected virtual bool? ShowDialog(object viewModel, IView view, object ownerViewModel = null)
        {
            IWindow dialog;
            if (view is Window dialogView)
            {
                dialog = new WindowWrapper(dialogView);
            }
            else
            {
                dialog = ActivationBehavior.CreateWindow();
                dialog.Content = view.ActualView;
            }
            RetrieveWindowProperties(view, dialog);

            if (ownerViewModel == null)
                ownerViewModel = _pushedModal ?? Shell;

            if (ownerViewModel != null)
                dialog.Owner = ViewLocationProvider.FindOwnerWindow(ownerViewModel);

            if (dialog.Owner == null && InteropService != null)
            {
                var ownnerHwnd = InteropService.Shell;
                dialog.Owner = ownnerHwnd;
            }

            dialog.DataContext = viewModel;

            User32.EnableWindow(InteropService.Shell.Handle, false);
            _pushedModal = viewModel;
            try
            {
                return dialog.ShowDialog();
            }
            finally
            {
                _pushedModal = null;
                User32.EnableWindow(InteropService.Shell.Handle, true);
            }
        }

        /// <summary>
        /// 显示消息对话框。
        /// </summary>
        /// <param name="message">消息内容。</param>
        /// <param name="title">消息对话框的标题。</param>
        /// <param name="buttons">消息对话框按钮。</param>
        /// <param name="icon">消息对话框图标。</param>
        /// <param name="ownerViewModel">父视图模型。</param>
        /// <returns>对话框返回值。</returns>
        protected virtual MessageBoxResult ShowMessageImpl(string message,
                                                           string title,
                                                           MessageBoxButton buttons,
                                                           MessageBoxImage icon,
                                                           object ownerViewModel)
        {
            Window owner = null;
            if (ownerViewModel != null)
                owner = ViewLocationProvider.FindOwnerWindow(ownerViewModel);

            if (owner == null)
                return MessageBox.Show(message, title, buttons, icon);
            else
                return MessageBox.Show(owner, message, title, buttons, icon);
        }

        private static T Invoke<T>(Func<T> func, Dispatcher dispatcher)
        {
            if (!dispatcher.CheckAccess())
            {
                T result = default;

                dispatcher.Invoke(new Action(() =>
                {
                    result = func();
                }));

                return result;
            }
            else
            {
                return func();
            }
        }

        private void Invoke(Action action, Dispatcher dispatcher)
        {
            if (!dispatcher.CheckAccess())
            {
                dispatcher.Invoke(new Action(() =>
                {
                    action();
                }));
            }
            else
            {
                action();
            }
        }

        private void Invoke<T>(Action<T> action, T args, Dispatcher dispatcher)
        {
            if (!dispatcher.CheckAccess())
            {
                dispatcher.Invoke(new Action(() =>
                {
                    action(args);
                }));
            }
            else
            {
                action(args);
            }
        }

        private void BeginInvoke(Action action, Dispatcher dispatcher)
        {
            if (!dispatcher.CheckAccess())
            {
                dispatcher.BeginInvoke(new Action(() =>
                {
                    action();
                }));
            }
            else
            {
                action();
            }
        }

        private void BeginInvoke<T>(Action<T> action, T args, Dispatcher dispatcher)
        {
            if (!dispatcher.CheckAccess())
            {
                dispatcher.Invoke(new Action(() =>
                {
                    action(args);
                }));
            }
            else
            {
                action.Invoke(args);
            }
        }

        private void RetrieveWindowProperties(IView view, IWindow dialog)
        {
            //首先应用窗体样式，特定的窗体属性将能够覆写样式中的属性设置
            RetrieveWindowStyle(view, dialog);
            RetrieveWindowTitle(view, dialog);
            RetrieveWindowIcon(view, dialog);
            RetrieveWindowHeight(view, dialog);
            RetrieveWindowWidth(view, dialog);
        }

        private static void RetrieveWindowStyle(IView view, IWindow dialog)
        {
            var style = view.ActualView.GetValue(WindowPopupBehavior.ContainerWindowStyleProperty) as Style;
            if (style != WindowPopupBehavior.ContainerWindowStyleProperty.DefaultMetadata.DefaultValue)
            {
                dialog.Style = style;
            }
        }

        private static void RetrieveWindowWidth(IView view, IWindow dialog)
        {
            var width = view.ActualView.GetValue(WindowPopupBehavior.ContainerWindowWidthProperty);
            if (width != WindowPopupBehavior.ContainerWindowWidthProperty.DefaultMetadata.DefaultValue)
            {
                dialog.Width = (double)width;
            }
        }

        private static void RetrieveWindowHeight(IView view, IWindow dialog)
        {
            var height = view.ActualView.GetValue(WindowPopupBehavior.ContainerWindowHeightProperty);
            if (height != WindowPopupBehavior.ContainerWindowHeightProperty.DefaultMetadata.DefaultValue)
            {
                dialog.Height = (double)height;
            }
        }

        private static void RetrieveWindowIcon(IView view, IWindow dialog)
        {
            var icon = view.ActualView.GetValue(WindowPopupBehavior.ContainerWindowIconProperty);
            if (icon != WindowPopupBehavior.ContainerWindowIconProperty.DefaultMetadata.DefaultValue)
            {
                dialog.Icon = icon as ImageSource;
            }
        }

        private static void RetrieveWindowTitle(IView view, IWindow dialog)
        {
            var title = view.ActualView.GetValue(WindowPopupBehavior.ContainerWindowTitleProperty);
            if (title != WindowPopupBehavior.ContainerWindowTitleProperty.DefaultMetadata.DefaultValue)
            {
                dialog.Title = title as string;
            }

            if (title == null || (title is string strTile) && string.IsNullOrEmpty(strTile) && string.IsNullOrWhiteSpace(strTile))
            {
                void ViewLoaded(object s, EventArgs e)
                {
                    RetrieveWindowTitle(view, dialog);
                    view.ActualView.Loaded -= ViewLoaded;
                }
                view.ActualView.Loaded += ViewLoaded;
            }
        }
    }
}
