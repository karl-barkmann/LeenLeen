using Leen.Windows.Controls;
using System.Windows.Input;

namespace System.Windows.Interactivity
{
    /// <summary>
    /// 使SearchBox支持Enter键的附加行为。
    /// </summary>
    public class SearchBoxEnterBehavior : CommandBehaviorBase<SearchBox>
    {
        public SearchBoxEnterBehavior()
        {
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewKeyDown += AssociatedObject_PreviewKeyDown;
        }

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
