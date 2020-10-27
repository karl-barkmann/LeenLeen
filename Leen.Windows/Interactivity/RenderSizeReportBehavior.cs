namespace System.Windows.Interactivity
{
    /// <summary>
    /// 支持渲染大小绑定的UI元素行为。
    /// </summary>
    public class RenderSizeReportBehavior : Behavior<UIElement>
    {
        /// <summary>
        /// 获取元素渲染大小。
        /// </summary>
        public Size RenderSize
        {
            get { return (Size)GetValue(RenderSizeProperty); }
            set { SetValue(RenderSizeProperty, value); }
        }

        /// <summary>
        /// 元素渲染大小的依赖属性。
        /// </summary>
        public static readonly DependencyProperty RenderSizeProperty =
            DependencyProperty.Register(nameof(RenderSize),
                                        typeof(Size),
                                        typeof(RenderSizeReportBehavior),
                                        new PropertyMetadata(Size.Empty));

        /// <summary>
        /// 构造行为实例。
        /// </summary>
        public RenderSizeReportBehavior()
        {
        }

        /// <summary>
        /// 在行为附加到 AssociatedObject 后调用。
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.LayoutUpdated += AssociatedObject_LayoutUpdated;
        }

        /// <summary>
        /// 在行为与其 AssociatedObject 分离时（但在它实际发生之前）调用。
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.LayoutUpdated -= AssociatedObject_LayoutUpdated;
        }

        private void AssociatedObject_LayoutUpdated(object sender, System.EventArgs e)
        {
            SetCurrentValue(RenderSizeProperty, AssociatedObject.RenderSize);
        }
    }
}
