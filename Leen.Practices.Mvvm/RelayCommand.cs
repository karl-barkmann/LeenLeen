using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace Leen.Practices.Mvvm
{
    /// <summary>
    /// 使用无参数委托实现一个<see cref="IRelayCommand"/>。
    /// </summary>
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
    public class RelayCommand : BindableBase, IRelayCommand, ICommand
    {
        #region Fields

        private readonly Func<bool> _canExecute;
        private readonly Action _execute;
        private string _text;
        private Key _key;
        private ModifierKeys _keyModifiers;
        private ModifierKeys _mouseModifiers;
        private KeyGesture _keyGesture;
        private MouseGesture _mouseGesture;
        private MouseAction _mouseAction;
        private string _mouseGestureText;
        private string _keyGestureText;
        private List<WeakReference> _canExecuteChangedHandlers;

        #endregion

        #region Constructors

        /// <summary>
        /// 私有构造函数。
        /// </summary>
        protected RelayCommand()
        {

        }

        /// <summary>
        /// 使用 <see cref="Action"/> 构造 <see cref="RelayCommand"/> 的实例。
        /// </summary>
        /// <param name="execute">执行命令时调用方法。</param>
        public RelayCommand(Action execute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        }

        /// <summary>
        ///  使用 <see cref="KeyGesture"/> 构造 <see cref="RelayCommand"/> 的实例。
        /// </summary>
        /// <param name="keyGesture">命令对应的键盘快捷键。</param>
        /// <param name="execute">执行命令时调用方法。</param>
        public RelayCommand(KeyGesture keyGesture, Action execute)
            : this(execute)
        {
            KeyGesture = keyGesture;
        }

        /// <summary>
        ///  使用 <see cref="MouseGesture"/> 构造 <see cref="RelayCommand"/> 的实例。
        /// </summary>
        /// <param name="mouseGesture">命令对应的鼠标操作。</param>
        /// <param name="execute">执行命令时调用方法。</param>
        public RelayCommand(MouseGesture mouseGesture, Action execute)
            : this(execute)
        {
            MouseGesture = mouseGesture;
        }

        /// <summary>
        ///  使用 <see cref="Key"/> 和 <see cref="Action"/> 构造 <see cref="RelayCommand"/> 的实例。
        /// </summary>
        /// <param name="key">快捷键组合命令的键值。</param>
        /// <param name="execute">执行命令时调用方法。</param>
        public RelayCommand(Key key, Action execute)
            : this(execute)
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
        ///  使用 <see cref="MouseAction"/> 和 <see cref="Action"/> 构造 <see cref="RelayCommand"/> 的实例。
        /// </summary>
        /// <param name="mouseAction">执行命令的鼠标操作。</param>
        /// <param name="execute">执行命令时调用方法。</param>
        public RelayCommand(MouseAction mouseAction, Action execute)
            : this(execute)
        {
            MouseGesture = new MouseGesture(mouseAction);
        }

        /// <summary>
        ///  使用 <see cref="Key"/>、<see cref="ModifierKeys"/> 和 <see cref="Action"/> 构造 <see cref="RelayCommand"/> 的实例。
        /// </summary>
        /// <param name="key">快捷键组合命令的键值。</param>
        /// <param name="keyModifierKeys">快捷键组合命令的修改键。</param>
        /// <param name="execute">执行命令时调用方法。</param>
        public RelayCommand(Key key, ModifierKeys keyModifierKeys, Action execute)
            : this(execute)
        {
            try
            {
                KeyGesture = new KeyGesture(key, keyModifierKeys);
            }
            catch (Exception)
            {
                Key = key;
                KeyGestureText = key.ToString();
            }
        }

        /// <summary>
        ///  使用 <see cref="MouseAction"/> 和 <see cref="Action"/> 构造 <see cref="RelayCommand"/> 的实例。
        /// </summary>
        /// <param name="mouseAction">执行命令的鼠标操作。</param>
        /// <param name="mouseModifierKeys">快捷键组合命令的修改键。</param>
        /// <param name="execute">执行命令时调用方法。</param>
        public RelayCommand(MouseAction mouseAction, ModifierKeys mouseModifierKeys, Action execute)
            : this(execute)
        {
            MouseGesture = new MouseGesture(mouseAction, mouseModifierKeys);
        }

        /// <summary>
        ///  使用 <see cref="Key"/>、<see cref="MouseAction"/> 和 <see cref="Action"/> 构造 <see cref="RelayCommand"/> 的实例。
        /// </summary>
        /// <param name="key">快捷键组合命令的键值。</param>
        /// <param name="mouseAction">执行命令的鼠标操作。</param>
        /// <param name="execute">执行命令时调用方法。</param>
        public RelayCommand(Key key, MouseAction mouseAction, Action execute)
            : this(key, execute)
        {
            MouseGesture = new MouseGesture(mouseAction);
        }

        /// <summary>
        ///  使用 <see cref="Key"/>、<see cref="ModifierKeys"/> 、<see cref="MouseAction"/> 和 <see cref="Action"/> 构造 <see cref="RelayCommand"/> 的实例。
        /// </summary>
        /// <param name="key">快捷键组合命令的键值。</param>
        /// <param name="keyModifierKeys">键盘快捷键组合命令的修改键。</param>
        /// <param name="mouseAction">执行命令的鼠标操作。</param>
        /// <param name="mouseModifierKeys">鼠标快捷键组合命令的修改键。</param>
        /// <param name="execute">执行命令时调用方法。</param>
        public RelayCommand(Key key, ModifierKeys keyModifierKeys, MouseAction mouseAction, ModifierKeys mouseModifierKeys, Action execute)
            : this(key, keyModifierKeys, execute)
        {
            MouseGesture = new MouseGesture(mouseAction, mouseModifierKeys);
        }

        /// <summary>
        /// 使用 <see cref="Action"/> 和 <see langword="Func"/> 构造 <see cref="RelayCommand"/> 的实例。
        /// </summary>
        /// <param name="execute">执行命令时调用方法。</param>
        /// <param name="canExecute">确定命令是否可以执行时调用方法。</param>
        public RelayCommand(Action execute, Func<bool> canExecute)
            : this(execute)
        {
            _canExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
        }

        /// <summary>
        ///  使用 <see cref="Key"/>、<see cref="Action"/> 和 <see langword="Func"/>  构造 <see cref="RelayCommand"/> 的实例。
        /// </summary>
        /// <param name="key">快捷键组合命令的键值。</param>
        /// <param name="execute">执行命令时调用方法。</param>
        /// <param name="canExecute">确定命令是否可以执行时调用方法。</param>
        public RelayCommand(Key key, Action execute, Func<bool> canExecute)
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
        ///  使用 <see cref="KeyGesture"/> 、<see cref="Action"/> 和 <see langword="Func"/>  构造 <see cref="RelayCommand"/> 的实例。
        /// </summary>
        /// <param name="keyGesture">命令对应的键盘快捷键。</param>
        /// <param name="execute">执行命令时调用方法。</param>
        /// <param name="canExecute">确定命令是否可以执行时调用方法。</param>
        public RelayCommand(KeyGesture keyGesture, Action execute, Func<bool> canExecute)
            : this(execute, canExecute)
        {
            KeyGesture = keyGesture;
        }

        /// <summary>
        ///  使用 <see cref="MouseGesture"/> 、<see cref="Action"/> 和 <see langword="Func"/>  构造 <see cref="RelayCommand"/> 的实例。
        /// </summary>
        /// <param name="mouseGesture">命令对应的鼠标操作。</param>
        /// <param name="execute">执行命令时调用方法。</param>
        /// <param name="canExecute">确定命令是否可以执行时调用方法。</param>
        public RelayCommand(MouseGesture mouseGesture, Action execute, Func<bool> canExecute)
            : this(execute, canExecute)
        {
            MouseGesture = mouseGesture;
        }

        /// <summary>
        ///  使用 <see cref="Key"/>、<see cref="ModifierKeys"/> 、<see cref="Action"/> 和 <see langword="Func"/>  构造 <see cref="RelayCommand"/> 的实例。
        /// </summary>
        /// <param name="key">快捷键组合命令的键值。</param>
        /// <param name="keyModifierKeys">快捷键组合命令的修改键。</param>
        /// <param name="execute">执行命令时调用方法。</param>
        /// <param name="canExecute">确定命令是否可以执行时调用方法。</param>
        public RelayCommand(Key key, ModifierKeys keyModifierKeys, Action execute, Func<bool> canExecute)
            : this(execute, canExecute)
        {
            try
            {
                KeyGesture = new KeyGesture(key, keyModifierKeys);
            }
            catch (Exception)
            {
                Key = key;
                KeyGestureText = key.ToString();
            }
        }

        /// <summary>
        ///  使用 <see cref="MouseAction"/> 、<see cref="Action"/> 和 <see langword="Func"/> 构造 <see cref="RelayCommand"/> 的实例。
        /// </summary>
        /// <param name="mouseAction">执行命令的鼠标操作。</param>
        /// <param name="execute">执行命令时调用方法。</param>
        /// <param name="canExecute">确定命令是否可以执行时调用方法。</param>
        public RelayCommand(MouseAction mouseAction, Action execute, Func<bool> canExecute)
            : this(execute, canExecute)
        {
            MouseGesture = new MouseGesture(mouseAction);
        }

        /// <summary>
        ///  使用 <see cref="Key"/>、<see cref="MouseAction"/> 、 <see cref="Action"/> 和 <see langword="Func"/> 构造 <see cref="RelayCommand"/> 的实例。
        /// </summary>
        /// <param name="key">快捷键组合命令的键值。</param>
        /// <param name="mouseAction">执行命令的鼠标操作。</param>
        /// <param name="execute">执行命令时调用方法。</param>
        /// <param name="canExecute">确定命令是否可以执行时调用方法。</param>
        public RelayCommand(Key key, MouseAction mouseAction, Action execute, Func<bool> canExecute)
            : this(key, execute, canExecute)
        {
            MouseGesture = new MouseGesture(mouseAction);
        }


        /// <summary>
        ///  使用 <see cref="Key"/>、<see cref="ModifierKeys"/> 、<see cref="MouseAction"/> 、 <see cref="Action"/> 和 <see langword="Func"/> 构造 <see cref="RelayCommand"/> 的实例。
        /// </summary>
        /// <param name="key">快捷键组合命令的键值。</param>
        /// <param name="keyModifierKeys">快捷键组合命令的修改键。</param>
        /// <param name="mouseAction">执行命令的鼠标操作。</param>
        /// <param name="execute">执行命令时调用方法。</param>
        /// <param name="canExecute">确定命令是否可以执行时调用方法。</param>
        public RelayCommand(Key key, ModifierKeys keyModifierKeys, MouseAction mouseAction, Action execute, Func<bool> canExecute)
            : this(key, keyModifierKeys, execute, canExecute)
        {
            MouseGesture = new MouseGesture(mouseAction);
        }

        /// <summary>
        ///  使用 <see cref="Key"/>、<see cref="ModifierKeys"/> 、<see cref="MouseAction"/> 、 <see cref="Action"/> 和 <see langword="Func"/> 构造 <see cref="RelayCommand"/> 的实例。
        /// </summary>
        /// <param name="key">快捷键组合命令的键值。</param>
        /// <param name="keyModifierKeys">快捷键组合命令的修改键。</param>
        /// <param name="mouseAction">执行命令的鼠标操作。</param>
        /// <param name="mouseModifierKeys">鼠标快捷键组合命令的修改键。</param>
        /// <param name="execute">执行命令时调用方法。</param>
        /// <param name="canExecute">确定命令是否可以执行时调用方法。</param>
        public RelayCommand(Key key, ModifierKeys keyModifierKeys, MouseAction mouseAction, ModifierKeys mouseModifierKeys, Action execute, Func<bool> canExecute)
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

        #region KeyBinding Supports

        /// <summary>
        /// 获取键盘快捷键组合命令的修改键。
        /// </summary>
        public ModifierKeys KeyModifiers
        {
            get { return _keyModifiers; }
            protected set
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
            protected set
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
            protected set
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
            protected set
            {
                SetProperty(ref _keyGestureText, value, () => KeyGestureText);
            }
        }

        #endregion

        #region MouseBinding Supports

        /// <summary>
        /// 获取命令的鼠标组合键。
        /// </summary>
        public MouseGesture MouseGesture
        {
            get
            {
                return _mouseGesture;
            }

            protected set
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

        /// <summary>
        /// 获取鼠标快捷键组合命令的修改键。
        /// </summary>
        public ModifierKeys MouseModifiers
        {
            get { return _mouseModifiers; }
            protected set
            {
                SetProperty(ref _mouseModifiers, value, () => MouseModifiers);
            }
        }

        /// <summary>
        /// 获取执行命令的鼠标操作。
        /// </summary>
        public MouseAction MouseAction
        {
            get { return _mouseAction; }
            protected set
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
            protected set
            {
                SetProperty(ref _mouseGestureText, value, () => MouseGestureText);
            }
        }

        #endregion

        #endregion

        #region Virtual Functions

        /// <summary>
        /// 通知影响是否应执行该命令的更改。
        /// </summary>
        public virtual void RaiseCanExecuteChanged()
        {
            CanExecuteChangedCommandManager.CallWeakReferenceHandlers(_canExecuteChangedHandlers);
        }

        /// <summary>
        /// 调用此命令时调用的方法。
        /// </summary>
        public virtual void Execute()
        {
            if (CanExecute())
            {
                _execute();
            }
        }

        /// <summary>
        /// 确定此命令是否可以在其当前状态下执行的方法。
        /// </summary>
		/// <returns> 如果可执行此命令，则为 true；否则为 false。</returns>
        public virtual bool CanExecute()
        {
            return _canExecute == null || _canExecute();
        }

        #endregion

        #region ICommand实现

        /// <summary>
        /// 确定此命令是否可以在其当前状态下执行的方法。
        /// </summary>
        /// <param name="parameter">此命令使用的数据。如果此命令不需要传递数据，则该对象可以设置为 null。</param>
        /// <returns> 如果可执行此命令，则为 true；否则为 false。</returns>
        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute();
        }

        /// <summary>
        /// 调用此命令时调用的方法。
        /// </summary>
        /// <param name="parameter">此命令使用的数据。如果此命令不需要传递数据，则该对象可以设置为 null。</param>
        void ICommand.Execute(object parameter)
        {
            Execute();
        }

        #endregion
    }
}
