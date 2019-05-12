using System.Windows.Input;

namespace System.Windows.Interactivity
{
    /// <summary>
    /// 表示一个支持选中状态的 <see cref="UIElement"/>。
    /// </summary>
    public class SupportSelectionBehavior : Behavior<UIElement>
    {
        /// <summary>
        /// 获取或设置一个值标识是否已经选中该 <see cref="UIElement"/> 。
        /// </summary>
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        /// <summary>
        /// <see cref="IsSelected"/> 的依赖属性。
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(SupportSelectionBehavior), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


        /// <summary>
        /// Called after the behavior is attached to an AssociatedObject.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.PreviewMouseLeftButtonDown += AssociatedObject_PreviewMouseLeftButtonDown;
        }

        /// <summary>
        /// Called when the behavior is being detached from its AssociatedObject, but before
        ///    it has actually occurred.
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PreviewMouseLeftButtonDown -= AssociatedObject_PreviewMouseLeftButtonDown;
        }

        private void AssociatedObject_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                SetCurrentValue(IsSelectedProperty, false);
            }
            else
            {
                SetCurrentValue(IsSelectedProperty, true);
            }
        }
    }
}
