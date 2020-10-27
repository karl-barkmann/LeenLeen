using System.Windows.Controls;
using System.Windows.Input;

namespace System.Windows.Interactivity
{
    /// <summary>
    /// 使TextBox支持Enter键的附加行为。
    /// </summary>
    public class TextBoxEnterBehavior : CommandBehaviorBase<TextBox>
    {
        /// <summary>
        /// 
        /// </summary>
        public TextBoxEnterBehavior()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewKeyDown += AssociatedObject_PreviewKeyDown;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PreviewKeyDown -= AssociatedObject_PreviewKeyDown;
        }

        private void AssociatedObject_PreviewKeyDown(object sender, Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ExecuteCommand(CommandParameter);
            }
        }
    }
}
