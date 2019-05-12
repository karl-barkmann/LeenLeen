using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace System.Windows.Interactivity
{
    /// <summary>
    /// 使 <see cref="ItemsControl"/> 支持 <see cref="ISupportIncrementalLoading"/> 增量加载接口集合。
    /// </summary>
    public class IncrementalLoadingBehavior : Behavior<ItemsControl>
    {
        #region Fileds

        private ScrollViewer _scrollViewer;

        #endregion

        #region InternalVerticalOffsetProperty

        private double InternalVerticalOffset
        {
            get { return (double)GetValue(InternalVerticalOffsetProperty); }
            set { SetValue(InternalVerticalOffsetProperty, value); }
        }

        private static readonly DependencyProperty InternalVerticalOffsetProperty =
            DependencyProperty.Register("InternalVerticalOffset", typeof(double), typeof(IncrementalLoadingBehavior), new PropertyMetadata(0D, InternalVerticalOffsetChanged));

        private static void InternalVerticalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IncrementalLoadingBehavior behavior = d as IncrementalLoadingBehavior;
            behavior.OnInternalVerticalOffsetChanged(e.OldValue, e.NewValue);
        }

        #endregion

        #region DirectionTypeProperty

        /// <summary>
        /// <see cref="Direction"/> 的 <see cref="DependencyProperty"/>。
        /// </summary>
        public static readonly DependencyProperty DirectionProperty = DependencyProperty.Register(
            "Direction",
            typeof(IncrementalDirection),
            typeof(IncrementalLoadingBehavior),
            new PropertyMetadata(IncrementalDirection.Bottom));

        /// <summary>
        /// 获取或设置增量展示方向。
        /// </summary>
        public IncrementalDirection Direction
        {
            get
            {
                return (IncrementalDirection)GetValue(DirectionProperty);
            }
            set
            {
                base.SetValue(DirectionProperty, value);
            }
        }

        #endregion

        #region ThresholdProperty

        /// <summary>
        /// 获取或设置增量加载的阈值。
        /// 当 <see cref="ScrollViewer"/> 的可滚动条区域的剩余长度小于视图大小乘以该阈值时则触发增量加载。
        /// </summary>
        public double Threshold
        {
            get { return (double)GetValue(ThresholdProperty); }
            set { SetValue(ThresholdProperty, value); }
        }

        /// <summary>
        /// <see cref="Threshold"/> 的 <see cref="DependencyProperty"/> 。
        /// </summary>
        public static readonly DependencyProperty ThresholdProperty =
            DependencyProperty.Register("Threshold", typeof(double), typeof(IncrementalLoadingBehavior), new PropertyMetadata(0.2d));

        #endregion

        /// <summary>
        /// Called after the behavior is attached to an AssociatedObject.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            if (AssociatedObject.IsLoaded)
            {
                _scrollViewer = AssociatedObject.GetVisualChild<ScrollViewer>();
                if (_scrollViewer != null)
                {
                    AttachScrollViewer(_scrollViewer);
                }
            }
            else
            {
                AssociatedObject.Loaded += AssociatedObject_Loaded;
            }
        }

        /// <summary>
        /// Called when the behavior is being detached from its AssociatedObject, but before
        ///    it has actually occurred.
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            if (_scrollViewer != null)
            {
                _scrollViewer.ScrollChanged += _scrollViewer_ScrollChanged;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        protected virtual void OnInternalVerticalOffsetChanged(object oldValue, object newValue)
        {
            //initializing monitor binding
            if (oldValue is int)
                return;

            double change = (double)newValue - (double)oldValue;
            switch (Direction)
            {
                case IncrementalDirection.Bottom:
                    if (change <= 0)
                        return;
                    break;
                case IncrementalDirection.Top:
                    break;
                case IncrementalDirection.Left:
                    break;
                case IncrementalDirection.Right:
                    break;
            }


            Debug.WriteLine("Binding Got you!");
            if (AssociatedObject.ItemsSource is ISupportIncrementalLoading incrementalLoading && incrementalLoading.HasMoreItems)
            {
                incrementalLoading.LoadMoreItemsAsync(1);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scrollViewer"></param>
        protected virtual void AttachScrollViewer(ScrollViewer scrollViewer)
        {
            BindToScrollViewer(scrollViewer);
            scrollViewer.ScrollChanged += _scrollViewer_ScrollChanged;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scrollViewer"></param>
        protected virtual void DetachScrollViewer(ScrollViewer scrollViewer)
        {
            scrollViewer.ScrollChanged -= _scrollViewer_ScrollChanged;
            BindingOperations.ClearBinding(this, InternalVerticalOffsetProperty);
        }

        #region Private Helpers

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            _scrollViewer = AssociatedObject.GetVisualChild<ScrollViewer>();
            if (_scrollViewer != null)
            {
                AttachScrollViewer(_scrollViewer);
            }
            AssociatedObject.Loaded -= AssociatedObject_Loaded;
        }

        private void _scrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {

        }

        private void BindToScrollViewer(ScrollViewer scrollViewer)
        {
            Binding binding = new Binding();
            binding.Source = scrollViewer;
            binding.Path = new PropertyPath("VerticalOffset");

            BindingOperations.SetBinding(this, InternalVerticalOffsetProperty, binding);
        }

        #endregion
    }

    /// <summary>
    /// 表示 <see cref="ISupportIncrementalLoading"/> 增量加载的界面增量展现方式。
    /// </summary>
    public enum IncrementalDirection
    {
        /// <summary>
        /// 向下增量显示；
        /// </summary>
        Bottom,

        /// <summary>
        /// 向上增量显示；
        /// </summary>
        Top,

        /// <summary>
        /// 向左增量显示；
        /// </summary>
        Left,

        /// <summary>
        /// 向右增量显示；
        /// </summary>
        Right,
    }

    /// <summary>
    /// 指定支持增量加载的集合视图的调用协定。
    /// </summary>
    public interface ISupportIncrementalLoading
    {
        /// <summary>
        /// Gets a sentinel value that supports incremental loading implementations.
        /// </summary>
        bool HasMoreItems { get; }

        /// <summary>
        /// Initializes incremental loading from the view.
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        Task<int> LoadMoreItemsAsync(uint count);
    }
}
