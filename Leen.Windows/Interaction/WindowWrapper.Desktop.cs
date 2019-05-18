// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using Microsoft.Windows.Shell;

namespace Leen.Windows.Interaction
{
    /// <summary>
    /// Defines a wrapper for the <see cref="Window"/> class that implements the <see cref="IWindow"/> interface.
    /// </summary>
    public class WindowWrapper : IWindow
    {
        private readonly Window _window;
        private readonly WindowInteropHelper _interopHelper;

        /// <summary>
        /// Initializes a new instance of <see cref="WindowWrapper"/>.
        /// </summary>
        public WindowWrapper() : this(new Window())
        {

        }

        /// <summary>
        /// 使用具体的WPF窗体实例化。
        /// </summary>
        /// <param name="window">承载窗口。</param>
        public WindowWrapper(Window window)
        {
            _window = window ?? throw new ArgumentNullException(nameof(window));
            _interopHelper = new WindowInteropHelper(_window);
            _window.CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, CloseWindowExcuted, CanCloseWindowExcuted));
            _window.CommandBindings.Add(new CommandBinding(SystemCommands.MaximizeWindowCommand, MaximizeWindowExcuted, CanMaximizeWindowExcuted));
            _window.CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand, MinimizeWindowExcuted, CanMinimizeWindowExcuted));
        }

        /// <summary>
        /// Ocurrs when the <see cref="Window"/> is closed.
        /// </summary>
        public event EventHandler Closed
        {
            add { _window.Closed += value; }
            remove { _window.Closed -= value; }
        }

        /// <summary>
        /// Gets or sets the title for the <see cref="IWindow"/>.
        /// </summary>
        public string Title
        {
            get { return _window.Title; }
            set { _window.Title = value; }
        }

        /// <summary>
        /// Gets or Sets the content for the <see cref="Window"/>.
        /// </summary>
        public object Content
        {
            get { return _window.Content; }
            set { _window.Content = value; }
        }

        /// <summary>
        /// Gets or Sets the <see cref="Window.Owner"/> control of the <see cref="Window"/>.
        /// </summary>
        public object Owner
        {
            get { return _window.Owner; }
            set
            {
                if (value is IntPtr ownerHandle)
                {
                    _interopHelper.Owner = ownerHandle;
                }
                else if(value is IWin32Window ownerWindow)
                {
                    _interopHelper.Owner = ownerWindow.Handle;
                }
                else
                {
                    _window.Owner = value as Window;
                }
            }
        }

        /// <summary>
        /// Gets or sets the height of the <see cref="Window"/>.
        /// </summary>
        public double Height
        {
            get { return _window.Height; }
            set { _window.Height = value; }
        }

        /// <summary>
        /// Gets or sets the width of the <see cref="Window"/>.
        /// </summary>
        public double Width
        {
            get { return _window.Width; }
            set { _window.Width = value; }
        }

        /// <summary>
        /// Gets or Sets the <see cref="FrameworkElement.Style"/> to apply to the <see cref="Window"/>.
        /// </summary>
        public Style Style
        {
            get { return _window.Style; }
            set { _window.Style = value; }
        }

        /// <summary>
        /// Gets or Sets the <see cref="FrameworkElement.DataContext"/> to apply to the <see cref="Window"/>.
        /// </summary>
        public object DataContext
        {
            get { return _window.DataContext; }
            set { _window.DataContext = value; }
        }

        /// <summary>
        /// 获取窗口句柄。
        /// </summary>
        public IntPtr Handle
        {
            get
            {
                _interopHelper.EnsureHandle();
                return _interopHelper.Handle;
            }
        }

        /// <summary>
        /// 获取或设置窗体标题栏图标。
        /// </summary>
        public ImageSource Icon
        {
            get
            {
                return _window.Icon;
            }
            set
            {
                _window.Icon = value;
            }
        }

        /// <summary>
        /// Opens the <see cref="Window"/>.
        /// </summary>
        public bool? ShowDialog()
        {
            if (Owner == null)
            {
                _window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }
            else
            {
                _window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }

            return _window.ShowDialog();
        }

        /// <summary>
        /// Opens the <see cref="Window"/>.
        /// </summary>
        public void Show()
        {
            if (Owner == null)
            {
                _window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }
            else
            {
                _window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }

            _window.Show();
        }

        /// <summary>
        /// Closes the <see cref="Window"/>.
        /// </summary>
        public void Close()
        {
            _window.DialogResult = null;
            _window.Close();
        }

        #region SystemCommands处理

        private void CanMinimizeWindowExcuted(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _window.WindowState != WindowState.Minimized;
        }

        private void MinimizeWindowExcuted(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(_window);
        }

        private void CanMaximizeWindowExcuted(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _window.WindowState != WindowState.Maximized;
        }

        private void MaximizeWindowExcuted(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MaximizeWindow(_window);
        }

        private void CanCloseWindowExcuted(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _window.IsVisible;
        }

        private void CloseWindowExcuted(object sender, ExecutedRoutedEventArgs e)
        {
            _window.DialogResult = null;
            SystemCommands.CloseWindow(_window);
        }

        #endregion
    }
}