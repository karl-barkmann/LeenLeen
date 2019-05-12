﻿using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace Leen.Practices.Mvvm
{
    /// <summary>
    /// 使用参数委托<see cref="Execute"/>及<see cref="CanExecute"/>定义一个<see cref="ICommand"/>。
    /// </summary>    
    /// <typeparam name="T">命令执行参数的类型。</typeparam>
    /// <remarks>
    /// 如果该命令需要支持快捷键操作或鼠标操作，则应指定对应的组合键和鼠标操作。
    /// <para/>
    /// <example>
    /// C#:
    ///  <code>
    ///     ICommand exitCommand = new DelegateCommand(Exit);
    ///     exitCommand.GestureKey = Key.X;
    ///     exitCommand.GestureModifier = ModifierKeys.Control;
    ///     exitCommand.MouseGesture = MouseAction.LeftDoubleClick;
    ///  </code>
    /// <para/>
    /// XAML:
    ///  <code>
    ///     <KeyBinding Key="{Binding ExitCommand.GestureKey}"
    ///                 Command="{Binding ExitCommand}"
    ///                 Modifiers="{Binding ExitCommand.GestureModifier}" />
    ///
    ///     <MouseBinding Command="{Binding ExitCommand}" 
    ///                 MouseAction="{Binding ExitCommand.MouseGesture}" />
    /// </code>
    /// </example>
    /// </remarks>
    public class RelayCommand<T> : BindableBase, ICommand
    {
        #region Fields

        private Func<T, bool> _canExecute;
        private Action<T> _execute;
        private string _text;
        private Key _key;
        private ModifierKeys _keyModifiers;
        private KeyGesture _keyGesture;
        private MouseAction _mouseAction;
        private string _keyGestureText;
        private List<WeakReference> _canExecuteChangedHandlers;
        private ModifierKeys _mouseModifiers;
        private MouseGesture _mouseGesture;
        private string _mouseGestureText;

        #endregion

        #region Constructors

        /// <summary>
        /// 使用 <see cref="Action"/> 和 <see langword="Func"/> 构造 <see cref="RelayCommand"/> 的实例。
        /// </summary>
        /// <param name="execute">执行命令时调用方法。</param>
        /// <param name="canExecute">确定命令是否可以执行时调用方法。</param>
        public RelayCommand(Action<T> execute, Func<T, bool> canExecute)
        {
            if (canExecute == null)
            {
                throw new ArgumentNullException("canExecute");
            }
            _execute = execute;
            _canExecute = canExecute;
        }

        /// <summary>
        ///  使用 <see cref="KeyGesture"/> 构造 <see cref="RelayCommand"/> 的实例。
        /// </summary>
        /// <param name="keyGesture">命令对应的键盘快捷键。</param>
        /// <param name="execute">执行命令时调用方法。</param>
        /// <param name="canExecute">确定命令是否可以执行时调用方法。</param>
        public RelayCommand(KeyGesture keyGesture, Action<T> execute, Func<T, bool> canExecute)
            : this(execute, canExecute)
        {
            KeyGesture = keyGesture;
        }

        /// <summary>
        ///  使用 <see cref="Key"/> 、<see cref="Action"/> 和 <see langword="Func"/>  构造 <see cref="RelayCommand"/> 的实例。
        /// </summary>
        /// <param name="key">快捷键组合命令的键值。</param>
        /// <param name="execute">执行命令时调用方法。</param>
        /// <param name="canExecute">确定命令是否可以执行时调用方法。</param>
        public RelayCommand(Key key, Action<T> execute, Func<T, bool> canExecute)
            : this(execute, canExecute)
        {
            try
            {
                KeyGesture = new KeyGesture(key);
            }
            catch (Exception)
            {
                Key = key;
                KeyGestureText = key.ToString();
            }
        }

        /// <summary>
        ///  使用 <see cref="Key"/>、<see cref="ModifierKeys"/> 、<see cref="Action"/> 和 <see langword="Func"/>  构造 <see cref="RelayCommand"/> 的实例。
        /// </summary>
        /// <param name="key">快捷键组合命令的键值。</param>
        /// <param name="modifierKeys">快捷键组合命令的修改键。</param>
        /// <param name="execute">执行命令时调用方法。</param>
        /// <param name="canExecute">确定命令是否可以执行时调用方法。</param>
        public RelayCommand(Key key, ModifierKeys modifierKeys, Action<T> execute, Func<T, bool> canExecute)
            : this(execute, canExecute)
        {
            try
            {
                KeyGesture = new KeyGesture(key, modifierKeys);
            }
            catch (Exception)
            {
                Key = key;
                KeyGestureText = key.ToString();
            }
        }

        /// <summary>
        ///  使用 <see cref="MouseGesture"/> 构造 <see cref="RelayCommand"/> 的实例。
        /// </summary>
        /// <param name="mouseGesture">命令对应的鼠标操作。</param>
        /// <param name="execute">执行命令时调用方法。</param>
        /// <param name="canExecute">确定命令是否可以执行时调用方法。</param>
        public RelayCommand(MouseGesture mouseGesture, Action<T> execute, Func<T, bool> canExecute)
            : this(execute, canExecute)
        {
            MouseGesture = mouseGesture;
        }

        /// <summary>
        ///  使用 <see cref="MouseAction"/> 、<see cref="Action"/> 和 <see langword="Func"/> 构造 <see cref="RelayCommand"/> 的实例。
        /// </summary>
        /// <param name="mouseAction">执行命令的鼠标操作。</param>
        /// <param name="execute">执行命令时调用方法。</param>
        /// <param name="canExecute">确定命令是否可以执行时调用方法。</param>
        public RelayCommand(MouseAction mouseAction, Action<T> execute, Func<T, bool> canExecute)
            : this(execute, canExecute)
        {
            MouseGesture = new MouseGesture(mouseAction);
        }

        /// <summary>
        ///  使用 <see cref="MouseAction"/> 、<see cref="ModifierKeys"/> 、<see cref="Action"/> 和 <see langword="Func"/> 构造 <see cref="RelayCommand"/> 的实例。
        /// </summary>
        /// <param name="mouseAction">执行命令的鼠标操作。</param>
        /// <param name="modifierKeys">快捷键组合命令的修改键。</param>
        /// <param name="execute">执行命令时调用方法。</param>
        /// <param name="canExecute">确定命令是否可以执行时调用方法。</param>
        public RelayCommand(MouseAction mouseAction, ModifierKeys modifierKeys, Action<T> execute, Func<T, bool> canExecute)
            : this(execute, canExecute)
        {
            MouseGesture = new MouseGesture(mouseAction, modifierKeys);
        }

        /// <summary>
        ///  使用 <see cref="Key"/>、<see cref="MouseAction"/> 、 <see cref="Action"/> 和 <see langword="Func"/> 构造 <see cref="RelayCommand"/> 的实例,
        ///  以同时支持键盘快捷键和鼠标操作。
        /// </summary>
        /// <param name="key">快捷键组合命令的键值。</param>
        /// <param name="mouseAction">执行命令的鼠标操作。</param>
        /// <param name="execute">执行命令时调用方法。</param>
        /// <param name="canExecute">确定命令是否可以执行时调用方法。</param>
        public RelayCommand(Key key, MouseAction mouseAction, Action<T> execute, Func<T, bool> canExecute)
            : this(key, execute, canExecute)
        {
            MouseGesture = new MouseGesture(mouseAction);
        }

        /// <summary>
        ///  使用 <see cref="Key"/>、<see cref="ModifierKeys"/> 、<see cref="MouseAction"/> 、 <see cref="Action"/> 和 <see langword="Func"/> 构造 <see cref="RelayCommand"/> 的实例,
        ///  以同时支持键盘快捷键和鼠标操作。
        /// </summary>
        /// <param name="key">快捷键组合命令的键值。</param>
        /// <param name="keyModifierKeys">键盘快捷键组合命令的修改键。</param>
        /// <param name="mouseAction">执行命令的鼠标操作。</param>
        /// <param name="execute">执行命令时调用方法。</param>
        /// <param name="canExecute">确定命令是否可以执行时调用方法。</param>
        public RelayCommand(Key key, ModifierKeys keyModifierKeys, MouseAction mouseAction, Action<T> execute, Func<T, bool> canExecute)
            : this(key, keyModifierKeys, execute, canExecute)
        {
            MouseGesture = new MouseGesture(mouseAction);
        }

        /// <summary>
        ///  使用 <see cref="Key"/>、<see cref="ModifierKeys"/> 、<see cref="MouseAction"/> 、 <see cref="Action"/> 和 <see langword="Func"/> 构造 <see cref="RelayCommand"/> 的实例,
        ///  以同时支持键盘快捷键和鼠标操作。
        /// </summary>
        /// <param name="key">快捷键组合命令的键值。</param>
        /// <param name="keyModifierKeys">键盘快捷键组合命令的修改键。</param>
        /// <param name="mouseAction">执行命令的鼠标操作。</param>
        /// <param name="mouseModifierKeys">鼠标快捷键组合命令的修改键。</param>
        /// <param name="execute">执行命令时调用方法。</param>
        /// <param name="canExecute">确定命令是否可以执行时调用方法。</param>
        public RelayCommand(Key key, ModifierKeys keyModifierKeys, MouseAction mouseAction, ModifierKeys mouseModifierKeys, Action<T> execute, Func<T, bool> canExecute)
            : this(key, keyModifierKeys, execute, canExecute)
        {
            MouseGesture = new MouseGesture(mouseAction, mouseModifierKeys);
        }

        #endregion

        #region Events

        /// <summary>
        /// 当出现影响是否应执行该命令的更改时发生。
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
                CanExecuteChangedCommandManager.AddWeakReferenceHandler(ref _canExecuteChangedHandlers, value, 2);
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
                CanExecuteChangedCommandManager.RemoveWeakReferenceHandler(_canExecuteChangedHandlers, value);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// 获取或设置此快捷的功能描述。
        /// </summary>
        public string Text
        {
            get { return _text; }
            set
            {
                SetProperty(ref _text, value, () => Text);
            }
        }

        #region KeyBinding Surpports

        /// <summary>
        /// 获取键盘快捷键组合命令的修改键。
        /// </summary>
        public ModifierKeys KeyModifiers
        {
            get { return _keyModifiers; }
            private set
            {
                SetProperty(ref _keyModifiers, value, () => KeyModifiers);
            }
        }

        /// <summary>
        /// 获取键盘快捷键组合命令的键值。
        /// </summary>
        public Key Key
        {
            get { return _key; }
            private set
            {
                SetProperty(ref _key, value, () => Key);
            }
        }

        /// <summary>
        /// 获取命令的键盘组合键。
        /// </summary>
        public KeyGesture KeyGesture
        {
            get { return _keyGesture; }
            private set
            {
                if (SetProperty(ref _keyGesture, value, () => KeyGesture))
                {
                    if (value != null)
                    {
                        Key = value.Key;
                        KeyModifiers = value.Modifiers;
                        KeyGestureText = value.GetDisplayString();
                    }
                    else
                    {
                        Key = Key.None;
                        KeyModifiers = ModifierKeys.None;
                        KeyGestureText = String.Empty;
                    }
                }
            }
        }

        /// <summary>
        /// 获取用于显示命令的键盘组合键的字符串。
        /// </summary>
        public string KeyGestureText
        {
            get { return _keyGestureText; }
            private set
            {
                SetProperty(ref _keyGestureText, value, () => KeyGestureText);
            }
        }

        #endregion

        #region MouseBinding Surpports

        /// <summary>
        /// 获取执行命令的鼠标操作。
        /// </summary>
        public MouseAction MouseAction
        {
            get { return _mouseAction; }
            private set
            {
                SetProperty(ref _mouseAction, value, () => MouseAction);
            }
        }

        /// <summary>
        /// 获取用于显示命令的鼠标组合键的字符串。
        /// </summary>
        public string MouseGestureText
        {
            get { return _mouseGestureText; }
            private set
            {
                SetProperty(ref _mouseGestureText, value, () => MouseGestureText);
            }
        }
        
        /// <summary>
        /// 获取命令的鼠标快捷键的修改键。
        /// </summary>
        public ModifierKeys MouseModifiers
        {
            get { return _mouseModifiers; }
            private set
            {
                SetProperty(ref _mouseModifiers, value, () => MouseModifiers);
            }
        }

        /// <summary>
        /// 获取命令的鼠标组合键。
        /// </summary>
        public MouseGesture MouseGesture
        {
            get { return _mouseGesture; }
            private set
            {
                if (SetProperty(ref _mouseGesture, value, () => MouseGesture))
                {
                    if (value != null)
                    {
                        MouseAction = value.MouseAction;
                        MouseModifiers = value.Modifiers;
                        MouseGestureText = value.GetDisplayString();
                    }
                    else
                    {
                        MouseAction = MouseAction.None;
                        MouseModifiers = ModifierKeys.None;
                        MouseGestureText = String.Empty;
                    }
                }
            }
        }

        #endregion

        #endregion

        #region Virtual Functions

        /// <summary>
        /// 定义用于确定此命令是否可以在其当前状态下执行的方法。
        /// </summary>
        /// <param name="parameter">此命令使用的数据。</param>
        /// <returns></returns>
        public virtual bool CanExecute(object parameter)
        {
            return _canExecute((T)parameter);
        }


        /// <summary>
        /// 定义在调用此命令时调用的方法。
        /// </summary>
        /// <param name="parameter">此命令使用的数据。</param>
        public virtual void Execute(object parameter)
        {
            _execute((T)parameter);
        }

        /// <summary>
        /// 通知影响是否应执行该命令的更改。
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
        public virtual void RaiseCanExecuteChanged()
        {
            CanExecuteChangedCommandManager.CallWeakReferenceHandlers(_canExecuteChangedHandlers);
        }

        #endregion
    }
}
