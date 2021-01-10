using System.Windows.Controls;
using System.Windows.Interop;

namespace System.Windows.Interactivity
{
    /// <summary>
    /// Represents a declarative rule that invoke trigger actions based on owner window or screen szie.
    /// <para>
    /// Be aware of <see cref="StackPanel"/>, this trigger seems don't like him.
    /// Try avoid to use this trigger with <see cref="StackPanel"/>.
    /// </para>
    /// </summary>
    /// <remarks>
    /// 使用Storyboard或VisualState来控制自适应布局是最佳实践。
    /// </remarks>
    public class AdaptiveTrigger : TriggerBase<FrameworkElement>
    {
        Window _ownerWindow;
        HwndSource _ownerSource;
        UIElement _rootUE;
        FrameworkElement _rootFE;

        /// <summary>
        /// 构造 <see cref="AdaptiveTrigger"/> 类的实例。
        /// </summary>
        public AdaptiveTrigger()
        {

        }

        /// <summary>
        /// 获取或设置一个值表示此触发器触发规则是否基于屏幕分辨率，若值为 False 则基于该触发器所属元素所在的窗体大小。
        /// </summary>
        public bool AdaptiveToScreen
        {
            get { return (bool)GetValue(AdaptiveToScreenProperty); }
            set { SetValue(AdaptiveToScreenProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="AdaptiveToScreen"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AdaptiveToScreenProperty =
            DependencyProperty.Register(nameof(AdaptiveToScreen), typeof(bool), typeof(AdaptiveTrigger), new PropertyMetadata(false));

        /// <summary>
        /// 获取或设置一个值表示此触发器触发规则计算方式是方向计算。例如MinWidth:1200时，则当目标宽度小于1200时触发此触发器，否则大于等于时触发此触发器。
        /// </summary>
        public bool Reverse
        {
            get { return (bool)GetValue(ReverseProperty); }
            set { SetValue(ReverseProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="ReverseProperty"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ReverseProperty =
            DependencyProperty.Register("Reverse", typeof(bool), typeof(AdaptiveTrigger), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets the minimum elemnt width at which the trigger actions should be invoked.
        /// </summary>
        public double MinWidth
        {
            get { return (double)GetValue(MinWidthProperty); }
            set { SetValue(MinWidthProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="MinWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MinWidthProperty =
            DependencyProperty.Register(nameof(MinWidth), typeof(double), typeof(AdaptiveTrigger), new PropertyMetadata(0d));

        /// <summary>
        /// Gets or sets the minimum elemnt height at which the trigger actions should be invoked.
        /// </summary>
        public double MinHeight
        {
            get { return (double)GetValue(MinHeightProperty); }
            set { SetValue(MinHeightProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="MinHeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MinHeightProperty =
            DependencyProperty.Register(nameof(MinHeight), typeof(double), typeof(AdaptiveTrigger), new PropertyMetadata(0d));

        /// <summary>
        /// 当触发器附加到的对象时调用。
        /// </summary>
        protected override void OnAttached()
        {
            if (!AssociatedObject.IsLoaded)
            {
                AssociatedObject.Loaded += AssociatedObject_Loaded;
            }
            else
            {
                AttachPresentationSource();
            }
            base.OnAttached();
        }

        /// <summary>
        /// 当触发从对象分离时调用。
        /// </summary>
        protected override void OnDetaching()
        {
            AssociatedObject.Loaded -= AssociatedObject_Loaded;
            if (_ownerWindow != null)
            {
                _ownerWindow.SizeChanged -= AdaptiveTrigger_SizeChanged;
                _ownerWindow = null;
            }

            if (_rootFE != null)
            {
                _rootFE.SizeChanged -= AdaptiveTrigger_SizeChanged;
                _rootFE = null;
            }
            if (_rootUE != null)
            {
                _rootUE.LayoutUpdated -= Root_LayoutUpdated;
                _rootUE = null;
            }

            base.OnDetaching();
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            AttachPresentationSource();
        }

        private void AttachPresentationSource()
        {
            if (_ownerWindow != null)
            {
                _ownerWindow.SizeChanged -= AdaptiveTrigger_SizeChanged;
            }
            var currentSize = Size.Empty;
            _ownerWindow = Window.GetWindow(AssociatedObject);
            if (_ownerWindow != null)
            {
                currentSize = _ownerWindow.RenderSize;
                _ownerWindow.SizeChanged += AdaptiveTrigger_SizeChanged;
            }

            if (_ownerSource != null)
            {
                if (_rootFE != null)
                {
                    _rootFE.SizeChanged -= AdaptiveTrigger_SizeChanged;
                }
                else
                {
                    if (_rootUE != null)
                    {
                        _rootUE.LayoutUpdated -= Root_LayoutUpdated;
                    }
                }
            }
            //Cross process addin hosted
            if (_ownerWindow == null || !_ownerWindow.IsVisible || !_ownerWindow.IsLoaded)
            {
                var presentationSource = PresentationSource.FromVisual(AssociatedObject);
                _ownerSource = presentationSource as HwndSource;
                if (_ownerSource != null)
                {
                    _rootFE = _ownerSource.RootVisual as FrameworkElement;
                    if (_rootFE != null)
                    {
                        currentSize = _rootFE.RenderSize;
                        _rootFE.SizeChanged += AdaptiveTrigger_SizeChanged;
                    }
                    else
                    {
                        _rootUE = _ownerSource.RootVisual as UIElement;
                        if (_rootUE != null)
                        {
                            currentSize = _rootUE.RenderSize;
                            _rootUE.LayoutUpdated += Root_LayoutUpdated;
                        }
                    }
                }
            }

            Console.WriteLine($"Attached {AssociatedObject.GetHashCode()} for {AssociatedObject.GetType().Name}");

            //如果第一次已经完成加载，手动触发一次
            if (currentSize != Size.Empty)
            {
                TriggerAction(currentSize);
            }
        }

        private void AdaptiveTrigger_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var newSize = e.NewSize;
            TriggerAction(newSize);
        }

        private void Root_LayoutUpdated(object sender, EventArgs e)
        {
            var rootUE = _ownerSource.RootVisual as UIElement;
            var newSize = rootUE.RenderSize;
            TriggerAction(newSize);
        }

        private void TriggerAction(Size currentSize)
        {
            var includeWidth = false;
            if (MinWidth != (double)MinWidthProperty.DefaultMetadata.DefaultValue)
            {
                includeWidth = true;
            }
            var includeHeight = false;
            if (MinHeight != (double)MinHeightProperty.DefaultMetadata.DefaultValue)
            {
                includeHeight = true;
            }
            if (includeWidth && includeHeight)
            {
                if (Reverse)
                {
                    if (currentSize.Width < MinWidth && currentSize.Height < MinHeight)
                    {
                        InvokeActions(null);
                    }
                }
                else
                {
                    if (currentSize.Width >= MinWidth && currentSize.Height >= MinHeight)
                    {
                        InvokeActions(null);
                    }
                }
            }
            else if (includeWidth)
            {
                if (Reverse)
                {
                    if (currentSize.Width < MinWidth)
                    {
                        InvokeActions(null);
                    }
                }
                else
                {
                    if (currentSize.Width >= MinWidth)
                    {
                        InvokeActions(null);
                    }
                }
            }
            else if (includeHeight)
            {
                if (Reverse)
                {
                    if (currentSize.Height < MinHeight)
                    {
                        InvokeActions(null);
                    }
                }
                else
                {
                    if (currentSize.Height >= MinHeight)
                    {
                        InvokeActions(null);
                    }
                }
            }
        }
    }
}
