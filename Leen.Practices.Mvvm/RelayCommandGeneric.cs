using System;
using System.Windows.Input;

namespace Leen.Practices.Mvvm
{
    /// <summary>
    /// 使用参数委托实现一个<see cref="IRelayCommand"/>。
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
    public class RelayCommand<T> : RelayCommand, IRelayCommand<T>, IRelayCommand<T, T>, IRelayCommand, ICommand
    {
        #region Fields

        private readonly Func<T, bool> _canExecute;
        private readonly Action<T> _execute;

        #endregion

        #region Constructors

        /// <summary>
        /// 使用 <see cref="Action"/> 构造 <see cref="RelayCommand"/> 的实例。
        /// </summary>
        /// <param name="execute">执行命令时调用方法。</param>
        public RelayCommand(Action<T> execute) => _execute = execute ?? throw new ArgumentNullException(nameof(execute));

        /// <summary>
        /// 使用 <see cref="Action"/> 和 <see langword="Func"/> 构造 <see cref="RelayCommand"/> 的实例。
        /// </summary>
        /// <param name="execute">执行命令时调用方法。</param>
        /// <param name="canExecute">确定命令是否可以执行时调用方法。</param>
        public RelayCommand(Action<T> execute, Func<T, bool> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
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
            KeyGesture = keyGesture ?? throw new ArgumentNullException(nameof(keyGesture));
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

        /// <summary>
        /// 调用此命令时调用的方法。
        /// </summary>
        public virtual void Execute(T parameter)
        {
            if (CanExecute(parameter))
            {
                _execute(parameter);
            }
        }

        /// <summary>
        /// 确定此命令是否可以在其当前状态下执行的方法。
        /// </summary>
        /// <param name="parameter">此命令使用的数据。如果此命令不需要传递数据，则该对象可以设置为 null。</param>
        /// <returns> 如果可执行此命令，则为 true；否则为 false。</returns>
        public virtual bool CanExecute(T parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }


        #region ICommand实现

        /// <summary>
        /// 确定此命令是否可以在其当前状态下执行的方法。
        /// </summary>
        /// <param name="parameter">此命令使用的数据。如果此命令不需要传递数据，则该对象可以设置为 null。</param>
        /// <returns> 如果可执行此命令，则为 true；否则为 false。</returns>
        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute((T)parameter);
        }

        /// <summary>
        /// 调用此命令时调用的方法。
        /// </summary>
        /// <param name="parameter">此命令使用的数据。如果此命令不需要传递数据，则该对象可以设置为 null。</param>
        void ICommand.Execute(object parameter)
        {
            Execute((T)parameter);
        }

        #endregion
    }
}
