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
            DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(SupportSelectionBehavior), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// 获取或设置一个值标识是否不支持冒泡事件的点击选中/> 。
        /// </summary>
        public bool DirectlySelection
        {
            get { return (bool)GetValue(DirectlySelectionProperty); }
            set { SetValue(DirectlySelectionProperty, value); }
        }

        /// <summary>
        /// <see cref="DirectlySelection"/> 的依赖属性。
        /// </summary>
        public static readonly DependencyProperty DirectlySelectionProperty =
            DependencyProperty.Register(nameof(DirectlySelection), typeof(bool), typeof(SupportSelectionBehavior), new PropertyMetadata(false));

        /// <summary>
        /// Called after the behavior is attached to an AssociatedObject.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            if (!DirectlySelection)
                AssociatedObject.PreviewMouseLeftButtonDown += AssociatedObject_PreviewMouseLeftButtonDown;
            else
                AssociatedObject.MouseLeftButtonDown += AssociatedObject_MouseLeftButtonDown;
        }

        /// <summary>
        /// Called when the behavior is being detached from its AssociatedObject, but before
        ///    it has actually occurred.
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.MouseLeftButtonDown -= AssociatedObject_MouseLeftButtonDown;
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

        private void AssociatedObject_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
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
