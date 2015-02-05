using Microsoft.Win32;
using System;
using System.Text;
using System.Threading;
using System.Windows;

namespace Smart.Practices.Mvvm
{
    public class UIInteractionService : IUIInteractionService
    {
        public virtual MessageBoxResult ShowMessage(string message, string title, 
            MessageBoxButton buttons = MessageBoxButton.OK,
            MessageBoxImage icon = MessageBoxImage.Information)
        {
            StringBuilder content = new StringBuilder(message);
            content = content.Replace("@NewLine", Environment.NewLine);
            content = content.Replace("@newline", Environment.NewLine);
            content = content.Replace("@NEWLINE", Environment.NewLine);
            if (Thread.CurrentThread.ManagedThreadId != Application.Current.Dispatcher.Thread.ManagedThreadId)
            {
                MessageBoxResult result = MessageBoxResult.None;
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    result = MessageBox.Show(content.ToString(), title, buttons, icon);
                }));
                return result;
            }
            else
            {
                return MessageBox.Show(content.ToString(), title, buttons, icon);
            }
        }

        public T InvokeIfNeeded<T>(Func<T> func)
        {
            if (Thread.CurrentThread.ManagedThreadId != Application.Current.Dispatcher.Thread.ManagedThreadId)
            {
                T result = default(T);

                Application.Current.Dispatcher.Invoke(new Action(() =>
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

        public void InvokeIfNeeded(Action action)
        {
            if (Thread.CurrentThread.ManagedThreadId != Application.Current.Dispatcher.Thread.ManagedThreadId)
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    action();
                }));
            }
            else 
            {
                action();
            }
        }

        public void InvokeIfNeeded<T>(Action<T> action, T args)
        {
            if (Thread.CurrentThread.ManagedThreadId != Application.Current.Dispatcher.Thread.ManagedThreadId)
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    action(args);
                }));
            }
            else
            {
                action(args);
            }
        }

        public void BeginInvokeIfNeeded(Action action)
        {
            if (Thread.CurrentThread.ManagedThreadId != Application.Current.Dispatcher.Thread.ManagedThreadId)
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    action();
                }));
            }
            else
            {
                action.BeginInvoke(null, null);
            }
        }

        public void BeginInvokeIfNeeded<T>(Action<T> action, T args)
        {
            if (Thread.CurrentThread.ManagedThreadId != Application.Current.Dispatcher.Thread.ManagedThreadId)
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    action(args);
                }));
            }
            else
            {
                action.BeginInvoke(args, null, null);
            }
        }

        public void Show(object viewModel,
            object ownnerViewModel = null, 
            string title = "", 
            double? width = Double.NaN,
            double? height = Double.NaN,
            bool allowUserClose = false,
            bool allowsTransparency = true)
        {
            IView view = ViewLocationProvider.GetViewForViewModel(viewModel);
            Show(viewModel, view, ownnerViewModel, title, width, height, allowUserClose, allowsTransparency);
        }

        public bool? ShowDialog(object viewModel, 
            object ownnerViewModel = null, 
            string title = "", 
            double? width = Double.NaN,
            double? height = Double.NaN,
            bool allowUserClose = false,
            bool allowsTransparency = true)
        {
            IView view = ViewLocationProvider.GetViewForViewModel(viewModel);
            return ShowDialog(viewModel, view, ownnerViewModel, title, width, height, allowUserClose, allowsTransparency);
        }

        public void Close(object viewModel)
        {
            IView view = ViewLocationProvider.GetViewForViewModel(viewModel);
            Close(viewModel, view);
        }

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
            {
                fileName = dlg.FileName;
            }
            return result;
        }

        protected virtual void Show(object viewModel,
            IView view, 
            object ownnerViewModel = null, 
            string title = "", 
            double? width = Double.NaN,
            double? height = Double.NaN,
            bool allowUserClose = false,
            bool allowsTransparency = true)
        {
            Window dialog = (Window)view;

            if (ownnerViewModel != null)
            {
                dialog.Owner = ViewLocationProvider.FindOwnnerWindow(ownnerViewModel);
            }
            dialog.AllowsTransparency = allowsTransparency;
            dialog.DataContext = viewModel;
            if(width.HasValue)
                dialog.Width = width.Value;
            if(height.HasValue)
                dialog.Height = height.Value;
            dialog.Title = title;
            dialog.Show();
        }

        protected virtual void Close(object viewModel, IView view)
        {
            Window dialog = (Window)view;

            dialog.Close();
        }

        protected virtual bool? ShowDialog(object viewModel, 
            IView view, 
            object ownnerViewModel = null, 
            string title = "", 
            double? width = Double.NaN, 
            double? height = Double.NaN,
            bool allowUserClose = false,
            bool allowsTransparency = true)
        {
            Window dialog = (Window)view;

            if (ownnerViewModel != null)
            {
                dialog.Owner = ViewLocationProvider.FindOwnnerWindow(ownnerViewModel);
            }
            if (width.HasValue)
                dialog.Width = width.Value;
            if (height.HasValue)
                dialog.Height = height.Value;
            dialog.AllowsTransparency = allowsTransparency;
            dialog.Title = title;
            dialog.DataContext = viewModel;
            return dialog.ShowDialog();
        }
    }
}
