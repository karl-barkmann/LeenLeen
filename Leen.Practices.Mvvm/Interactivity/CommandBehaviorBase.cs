using System.Windows.Controls;
using System.Windows.Input;

namespace System.Windows.Interactivity
{
    /// <summary>
    /// Base behavior to handle connecting a <see cref="Control"/> to a Command.
    /// </summary>
    /// <typeparam name="T">The target object must derive from Control</typeparam>
    /// <remarks>
    /// CommandBehaviorBase can be used to provide new behaviors for commands.
    /// </remarks>
    public class CommandBehaviorBase<T> : Behavior<T> where T : UIElement
    {
        private EventHandler commandCanExecuteChangedHandler;

        /// <summary>
        /// 在行为附加到 AssociatedObject 后调用。
        /// </summary>
        protected override void OnAttached()
        {
            this.commandCanExecuteChangedHandler = new EventHandler(this.CommandCanExecuteChanged);
            base.OnAttached();
        }

        #region CommandProperty

        /// <summary>
        /// 获取或设置改行为关联的命令。
        /// </summary>
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        /// <summary>
        /// 提供对 <see cref="Command"/> 命令对象的依赖属性支持。
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(CommandBehaviorBase<T>), new PropertyMetadata(null, OnCommandPropertyChanged));

        private static void OnCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CommandBehaviorBase<T> behavior = d as CommandBehaviorBase<T>;
            behavior.OnCommandChanged(e.OldValue, e.NewValue);
        }

        #endregion

        #region CommandParameterProperty

        /// <summary>
        /// 获取或设置命令的执行参数。
        /// </summary>
        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        /// <summary>
        /// 提供对 <see cref="CommandParameter"/> 命令对象的依赖属性支持。
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(CommandBehaviorBase<T>), new PropertyMetadata(null));

        #endregion

        /// <summary>
        /// Updates the target object's IsEnabled property based on the commands ability to execute.
        /// </summary>
        protected virtual void UpdateEnabledState()
        {
            if (AssociatedObject == null)
            {
                //this.Command = null;
                //this.CommandParameter = null;
            }
            else if (this.Command != null)
            {
                AssociatedObject.IsEnabled = this.Command.CanExecute(this.CommandParameter);
            }
        }
        
        /// <summary>
        /// 当命令对象发生改变时。
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        protected virtual void OnCommandChanged(object oldValue, object newValue)
        {
            if (oldValue is ICommand oldCommand)
            {
                oldCommand.CanExecuteChanged -= this.commandCanExecuteChangedHandler;
            }
            if (newValue is ICommand newCommand)
            {
                newCommand.CanExecuteChanged += this.commandCanExecuteChangedHandler;
                UpdateEnabledState();
            }
        }

        /// <summary>
        /// Executes the command, if it's set, providing the <see cref="CommandParameter"/>
        /// </summary>
        protected virtual void ExecuteCommand(object parameter)
        {
            if (this.Command != null)
            {
                if (this.CommandParameter != null)
                {
                    this.Command.Execute(this.CommandParameter);
                }
                else
                {
                    this.Command.Execute(parameter);
                }
            }
        }

        private void CommandCanExecuteChanged(object sender, EventArgs e)
        {
            this.UpdateEnabledState();
        }
    }
}