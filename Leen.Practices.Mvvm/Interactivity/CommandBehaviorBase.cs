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
        /// ����Ϊ���ӵ� AssociatedObject ����á�
        /// </summary>
        protected override void OnAttached()
        {
            this.commandCanExecuteChangedHandler = new EventHandler(this.CommandCanExecuteChanged);
            base.OnAttached();
        }

        #region CommandProperty

        /// <summary>
        /// ��ȡ�����ø���Ϊ���������
        /// </summary>
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        /// <summary>
        /// �ṩ�� <see cref="Command"/> ����������������֧�֡�
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
        /// ��ȡ�����������ִ�в�����
        /// </summary>
        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        /// <summary>
        /// �ṩ�� <see cref="CommandParameter"/> ����������������֧�֡�
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
        /// ������������ı�ʱ��
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