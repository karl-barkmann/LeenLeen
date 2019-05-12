//2012.9.6 zhongying 创建新的幻灯片控件
//2015.2.4 liping 添加ItemSource，ItemTemplate,ItemTemplateSelector使其支持数据binding.
//2015.2.5 liping 重命名为SlidablePanel,支持鼠标滚动滑动

using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Leen.Windows.Controls
{
    /// <summary>
    /// 全新的幻灯片面板控件，支持水平和垂直方向.代码重新整理更简洁，注释完整
    /// </summary>
    public class SlidablePanel : Panel
    {
        #region 成员变量

        Size _finalSize;
        bool mouseDown;
        Point mouseLast;//上一次的鼠标位置
        Point mouseFirst;//第一鼠标点下的坐标
        private static List<MouseWheelEventArgs> _reentrantList = new List<MouseWheelEventArgs>();

        #endregion

        #region 构造函数

        /// <summary>
        /// Initializes a new instance of the <see cref="SlidablePanel"/> class.
        /// </summary>
        public SlidablePanel()
        {
            RegisterEvent();
            ClipToBounds = true;
            Loaded += new RoutedEventHandler(NewSliderPanel_Loaded);
            SizeChanged += new SizeChangedEventHandler(NewSliderPanel_Loaded);
            MouseWheel += NewSliderPanel_MouseWheel;
            PreviewMouseWheel += NewSliderPanel_PreviewMouseWheel;
        }

        #endregion

        #region 公开属性

        #region 依赖属性

        #region SwitchPageWidth;

        /// <summary>
        /// The <see cref="SwitchPageLength" /> dependency property's name.
        /// </summary>
        public const string SwitchPageLengthPropertyName = "SwitchPageLength";

        /// <summary>
        /// 获取或设置拖放到达多少宽度时，切换面板的宽度。（如果小于1，则表示按呈现宽度的百分之比。大于1则按实际数据为单位）
        /// </summary>
        public double SwitchPageLength
        {
            get
            {
                return (double)GetValue(SwitchPageLengthProperty);
            }
            set
            {
                SetValue(SwitchPageLengthProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="SwitchPageLength" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty SwitchPageLengthProperty = DependencyProperty.Register(
            SwitchPageLengthPropertyName,
            typeof(double),
            typeof(SlidablePanel),
            new PropertyMetadata(0.2));

        #endregion

        #region EnableDrag

        /// <summary>
        /// The <see cref="EnableDrag" /> dependency property's name.
        /// </summary>
        public const string EnableDragPropertyName = "EnableDrag";

        /// <summary>
        /// 获取或设置 是否允许拖放
        /// </summary>
        public bool EnableDrag
        {
            get
            {
                return (bool)GetValue(EnableDragProperty);
            }
            set
            {
                SetValue(EnableDragProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="EnableDrag" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty EnableDragProperty = DependencyProperty.Register(
            EnableDragPropertyName,
            typeof(bool),
            typeof(SlidablePanel),
            new PropertyMetadata(true, new PropertyChangedCallback(EnableDragPropertyChangedCallback)));

        static void EnableDragPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SlidablePanel control = d as SlidablePanel;
            bool enable = (bool)e.NewValue;

            control.UnregisterEvent();
            if (enable)
                control.RegisterEvent();
        }

        #endregion

        #region Orientation

        /// <summary>
        /// The <see cref="Orientation" /> dependency property's name.
        /// </summary>
        public const string OrientationPropertyName = "Orientation";

        /// <summary>
        /// 获取或设置 控件布局的方向
        /// </summary>
        public Orientation Orientation
        {
            get
            {
                return (Orientation)GetValue(OrientationProperty);
            }
            set
            {
                SetValue(OrientationProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="Orientation" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
            OrientationPropertyName,
            typeof(Orientation),
            typeof(SlidablePanel),
            new PropertyMetadata(Orientation.Horizontal, new PropertyChangedCallback(OrientationPropertyChangedCallback)));

        static void OrientationPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SlidablePanel control = d as SlidablePanel;

            //control.InvalidateMeasure();
            control.InvalidateArrange();

            foreach (FrameworkElement child in control.InternalChildren)
            {
                TranslateTransform transform = transform = new TranslateTransform(0, 0);
                child.RenderTransform = transform;
            }
            control.Animation(control.SelectedIndex, 0);
        }

        #endregion

        #region 暂时注释，以后在加

        //#region ShowNeighbor

        ///// <summary>
        ///// The <see cref="ShowNeighbor" /> dependency property's name.
        ///// </summary>
        //public const string ShowNeighborPropertyName = "ShowNeighbor";

        ///// <summary>
        ///// 获取或设置 是否显示邻居
        ///// </summary>
        //public bool ShowNeighbor
        //{
        //    get
        //    {
        //        return (bool)GetValue(ShowNeighborProperty);
        //    }
        //    set
        //    {
        //        SetValue(ShowNeighborProperty, value);
        //    }
        //}

        ///// <summary>
        ///// Identifies the <see cref="ShowNeighbor" /> dependency property.
        ///// </summary>
        //public static readonly DependencyProperty ShowNeighborProperty = DependencyProperty.Register(
        //    ShowNeighborPropertyName,
        //    typeof(bool),
        //    typeof(CustomControl),
        //    new PropertyMetadata(false));

        //#endregion

        //#region ShowNeighborWidth

        ///// <summary>
        ///// The <see cref="ShowNeighborWidth" /> dependency property's name.
        ///// </summary>
        //public const string ShowNeighborWidthPropertyName = "ShowNeighborWidth";

        ///// <summary>
        ///// 获取或设置 显示邻居的宽度
        ///// </summary>
        //public double ShowNeighborWidth
        //{
        //    get
        //    {
        //        return (double)GetValue(ShowNeighborWidthProperty);
        //    }
        //    set
        //    {
        //        SetValue(ShowNeighborWidthProperty, value);
        //    }
        //}

        ///// <summary>
        ///// Identifies the <see cref="ShowNeighborWidth" /> dependency property.
        ///// </summary>
        //public static readonly DependencyProperty ShowNeighborWidthProperty = DependencyProperty.Register(
        //    ShowNeighborWidthPropertyName,
        //    typeof(double),
        //    typeof(CustomControl),
        //    new PropertyMetadata(0d));

        //#endregion

        #endregion

        #region SelectedIndex

        /// <summary>
        /// The <see cref="SelectedIndex" /> dependency property's name.
        /// </summary>
        public const string SelectedIndexPropertyName = "SelectedIndex";

        /// <summary>
        /// 获取或设置当前选中页面
        /// </summary>
        public int SelectedIndex
        {
            get
            {
                return (int)GetValue(SelectedIndexProperty);
            }
            set
            {
                SetValue(SelectedIndexProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="SelectedIndex" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedIndexProperty = DependencyProperty.Register(
            SelectedIndexPropertyName,
            typeof(int),
            typeof(SlidablePanel),
            new PropertyMetadata(0, new PropertyChangedCallback(SelectedIndexPropertyChangedCallback), new CoerceValueCallback(SelectedIndexCoerceValueCallback)));

        /// <summary>
        /// Selecteds the index coerce value callback.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="baseValue">The base value.</param>
        /// <returns></returns>
        static object SelectedIndexCoerceValueCallback(DependencyObject d, object baseValue)
        {
            SlidablePanel control = d as SlidablePanel;
            int count = (int)baseValue;
            if (count < 0) return 0;

            //else if (control.InternalChildren != null && count >= control.InternalChildren.Count)
            //    return control.InternalChildren.Count - 1;

            return count;
        }

        /// <summary>
        /// Selecteds the index property changed callback.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        static void SelectedIndexPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SlidablePanel control = d as SlidablePanel;
            control.Animation((int)e.NewValue, control.Duration);
        }

        #endregion

        #region Duration

        /// <summary>
        /// The <see cref="Duration" /> dependency property's name.
        /// </summary>
        public const string DurationPropertyName = "Duration";

        /// <summary>
        /// 获取或设置 动画时间
        /// </summary>
        public double Duration
        {
            get
            {
                return (double)GetValue(DurationProperty);
            }
            set
            {
                SetValue(DurationProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="Duration" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty DurationProperty = DependencyProperty.Register(
            DurationPropertyName,
            typeof(double),
            typeof(SlidablePanel),
            new PropertyMetadata(300d));

        #endregion

        #region ItemTempalte

        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(SlidablePanel),
            new PropertyMetadata(null, new PropertyChangedCallback(OnItemTemplatePropertyChanged)));

        private static void OnItemTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SlidablePanel sp = d as SlidablePanel;
            sp.OnItemsSourceChanged(null, sp.ItemsSource);
        }

        #endregion

        #region ItemsSource

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(SlidablePanel),
            new PropertyMetadata(null, new PropertyChangedCallback(OnItemsSourcePropertyChanged)));

        private static void OnItemsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IEnumerable oldValue = (IEnumerable)e.OldValue;
            IEnumerable newValue = (IEnumerable)e.NewValue;
            SlidablePanel sp = d as SlidablePanel;
            sp.OnItemsSourceChanged(oldValue, newValue);
        }

        #endregion

        #region ItemTemplateSelector

        public DataTemplateSelector ItemTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(ItemTemplateSelectorProperty); }
            set { SetValue(ItemTemplateSelectorProperty, value); }
        }

        public static readonly DependencyProperty ItemTemplateSelectorProperty =
            DependencyProperty.Register("ItemTemplateSelector", typeof(DataTemplateSelector), typeof(SlidablePanel),
            new PropertyMetadata(null, new PropertyChangedCallback(OnItemTemplateSelectorPropertyChanged)));

        private static void OnItemTemplateSelectorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SlidablePanel sp = d as SlidablePanel;
            sp.OnItemsSourceChanged(null, sp.ItemsSource);
        }

        #endregion

        #endregion

        #endregion

        #region 回调

        void NewSliderPanel_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (IsMouseOver)
            {
                if (e.Delta < 0)
                {
                    if (SelectedIndex < (Children.Count - 1))
                    {
                        SelectedIndex += 1;
                        e.Handled = true;
                    }
                }
                else
                {
                    if (SelectedIndex > 0)
                    {
                        SelectedIndex -= 1;
                        e.Handled = true;
                    }
                }
            }
        }

        void NewSliderPanel_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var sliderPanel = sender as SlidablePanel;
            if (sliderPanel != null && !e.Handled && _reentrantList.Contains(e))
            {
                var previewEventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
                {
                    RoutedEvent = UIElement.PreviewMouseWheelEvent,
                    Source = sender
                };
                var originalSource = e.OriginalSource as UIElement;
                _reentrantList.Add(previewEventArg);
                originalSource.RaiseEvent(previewEventArg);
                _reentrantList.Remove(previewEventArg);
                // at this point if no one else handled the event in our children, we do our job
                if (!previewEventArg.Handled && ((e.Delta <= 0 && (SelectedIndex == Children.Count - 1)) ||
                    (e.Delta > 0 && SelectedIndex == 0)))
                {
                    //This is it.
                    //Our work is done,let's our parent scroll it!
                    e.Handled = true;
                    var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
                    eventArg.RoutedEvent = UIElement.MouseWheelEvent;
                    eventArg.Source = sender;
                    var parent = sliderPanel.Parent as UIElement;
                    parent.RaiseEvent(eventArg);
                }
            }
        }

        void NewSliderPanel_Loaded(object sender, RoutedEventArgs e)
        {
            //第一次加载后 应用选中坐标
            Animation(SelectedIndex, 0);
        }

        void CustomControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            mouseDown = true;
            mouseFirst = mouseLast = e.GetPosition(this);
        }

        void CustomControl_MouseLeave(object sender, MouseEventArgs e)
        {
            //mouseDown = false;
        }

        void CustomControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ReleaseMouseCapture();

            var mouseFinal = e.GetPosition(this);

            int targetIndex = 0;

            bool isLeftOrDown = false;
            if (Orientation == Orientation.Horizontal)
                isLeftOrDown = mouseFinal.X - mouseFirst.X > 0;
            else
                isLeftOrDown = mouseFinal.Y - mouseFirst.Y > 0;

            double switchPageWidth = 0;
            double switchPageHeight = 0;
            if (SwitchPageLength < 1)
            {
                switchPageWidth = RenderSize.Width * SwitchPageLength;
                switchPageHeight = RenderSize.Height * SwitchPageLength;
            }
            else
                switchPageHeight = switchPageWidth = SwitchPageLength;

            bool isNextPage = (Math.Abs(mouseFinal.X - mouseFirst.X) > switchPageWidth) || (Math.Abs(mouseFinal.Y - mouseFirst.Y) > switchPageHeight);

            for (int i = 0; i < InternalChildren.Count; i++)
            {
                if (isNextPage)
                {
                    if (isLeftOrDown)
                        targetIndex = SelectedIndex - 1;
                    else
                        targetIndex = SelectedIndex + 1;
                }
                else
                {
                    targetIndex = SelectedIndex;
                }
                if (targetIndex < 0)
                    targetIndex = 0;
                else if (targetIndex >= InternalChildren.Count)
                    targetIndex = InternalChildren.Count - 1;
                Animation(targetIndex, Duration);
            }

            SelectedIndex = targetIndex;
            mouseDown = false;
        }

        void CustomControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (!mouseDown)
                return;

            var mouseNow = e.GetPosition(this);

            //移动所有子元素
            for (int i = 0; i < Children.Count; i++)
            {
                TranslateTransform transform = null;
                if (Orientation == Orientation.Horizontal)
                    transform = new TranslateTransform(Children[i].RenderTransform.Value.OffsetX + (mouseNow.X - mouseLast.X), 0);
                else
                    transform = new TranslateTransform(0, Children[i].RenderTransform.Value.OffsetY + (mouseNow.Y - mouseLast.Y));
                Children[i].RenderTransform = transform;
            }

            mouseLast = mouseNow;
        }

        #endregion

        #region 重写

        /// <summary>
        /// 在 <see cref="T:System.Windows.Controls.Panel"/> 元素的呈现处理过程中，绘制 <see cref="T:System.Windows.Media.DrawingContext"/> 对象的内容。
        /// </summary>
        /// <param name="dc">要绘制的 <see cref="T:System.Windows.Media.DrawingContext"/> 对象。</param>
        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
        }

        /// <summary>
        /// 当在派生类中重写时，请测量子元素在布局中所需的大小，然后确定 <see cref="T:System.Windows.FrameworkElement"/> 派生类的大小。
        /// </summary>
        /// <param name="availableSize">此元素可以赋给子元素的可用大小。可以指定无穷大值，这表示元素的大小将调整为内容的可用大小。</param>
        /// <returns>此元素在布局过程中所需的大小，这是由此元素根据对其子元素大小的计算而确定的。</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            Size resultSize = new Size(0, 0);

            foreach (UIElement child in Children)
            {
                //测量子元素
                child.Measure(availableSize);

                //返回子元素宽度、高度 或者 当前宽度中最大的那个
                resultSize.Width = Math.Max(resultSize.Width, child.DesiredSize.Width);
                resultSize.Height = Math.Max(resultSize.Height, child.DesiredSize.Height);
            }

            //如果为正无穷(就是默认值可能为0)，则返回默认可用大小
            resultSize.Width = double.IsPositiveInfinity(availableSize.Width) ? resultSize.Width : availableSize.Width;
            resultSize.Height = double.IsPositiveInfinity(availableSize.Height) ? resultSize.Height : availableSize.Height;

            _finalSize = resultSize;
            return resultSize;
        }

        /// <summary>
        /// 在派生类中重写时，请为 <see cref="T:System.Windows.FrameworkElement"/> 派生类定位子元素并确定大小。
        /// </summary>
        /// <param name="finalSize">父级中此元素应用来排列自身及其子元素的最终区域。</param>
        /// <returns>所用的实际大小。</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            //Clip = new RectangleGeometry(new Rect(0, 0, 20, DesiredSize.Height));

            for (int i = 0; i < InternalChildren.Count; i++)
            {
                var child = InternalChildren[i];

                Rect rect = new Rect();

                if (Orientation == System.Windows.Controls.Orientation.Horizontal)
                    rect = ArrangeHorizontal(child, i, finalSize);
                else
                    rect = ArrangeVertical(child, i, finalSize);

                child.Arrange(rect);
            }

            return base.ArrangeOverride(finalSize);
        }

        #endregion

        #region 私有方法

        protected virtual void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            Children.Clear();
            if (newValue != null)
            {
                foreach (var item in newValue)
                {
                    DataTemplate dataTemplate = null;

                    // get data template
                    if (this.ItemTemplateSelector != null)
                    {
                        dataTemplate = this.ItemTemplateSelector.SelectTemplate(item, this);
                    }
                    if (dataTemplate == null && this.ItemTemplate != null)
                    {
                        dataTemplate = this.ItemTemplate;
                    }

                    // load data template content
                    if (dataTemplate != null)
                    {
                        FrameworkElement child = dataTemplate.LoadContent() as FrameworkElement;
                        if (child != null)
                        {
                            // set data context
                            child.DataContext = item;
                            Children.Add(child);
                        }
                    }
                }
            }
        }

        private void RegisterEvent()
        {
            MouseMove += new MouseEventHandler(CustomControl_MouseMove);
            MouseUp += new MouseButtonEventHandler(CustomControl_MouseUp);
            MouseDown += new MouseButtonEventHandler(CustomControl_MouseDown);
            MouseLeave += new MouseEventHandler(CustomControl_MouseLeave);
        }

        private void UnregisterEvent()
        {
            MouseMove -= new MouseEventHandler(CustomControl_MouseMove);
            MouseUp -= new MouseButtonEventHandler(CustomControl_MouseUp);
            MouseDown -= new MouseButtonEventHandler(CustomControl_MouseDown);
            MouseLeave -= new MouseEventHandler(CustomControl_MouseLeave);
        }

        private void Animation(int index, double duration)
        {
            if (index < 0)
                index = 0;

            //double from = GetTargetX(fromIndex);
            double to = 0;

            if (Orientation == Orientation.Horizontal)
            {
                to = -GetTargetX(index);
            }
            else
            {
                to = -GetTargetY(index);
            }

            for (int i = 0; i < InternalChildren.Count; i++)
            {
                DoubleAnimation doubleAnimation = new DoubleAnimation(to, new Duration(TimeSpan.FromMilliseconds(duration)));

                if (Orientation == System.Windows.Controls.Orientation.Horizontal)
                {
                    TranslateTransform transform = new TranslateTransform(Children[i].RenderTransform.Value.OffsetX, 0);
                    Children[i].RenderTransform = transform;
                    ((TranslateTransform)Children[i].RenderTransform).BeginAnimation(TranslateTransform.XProperty, doubleAnimation);
                }
                else
                {
                    TranslateTransform transform = new TranslateTransform(0, Children[i].RenderTransform.Value.OffsetY);
                    Children[i].RenderTransform = transform;
                    ((TranslateTransform)Children[i].RenderTransform).BeginAnimation(TranslateTransform.YProperty, doubleAnimation);
                }
            }
        }

        private Rect ArrangeVertical(UIElement child, int childIndex, Size finalSize)
        {
            Rect result = new Rect();
            //if (ShowNeighbor)
            //{ }
            //else
            //{
            result = new Rect(new Point(0, GetTargetY(childIndex)), finalSize);
            //}

            return result;
        }

        private Rect ArrangeHorizontal(UIElement child, int childIndex, Size finalSize)
        {
            Rect result = new Rect();
            //if (ShowNeighbor)
            //{ }
            //else
            //{
            result = new Rect(new Point(GetTargetX(childIndex), 0), finalSize);
            //}
            return result;
        }

        //获取每一页的开始坐标
        private double GetTargetX(int index)
        {
            if (index <= 0) return 0;

            var result = index * _finalSize.Width;
            return result;
        }

        //获取每一页的开始坐标
        private double GetTargetY(int index)
        {
            if (index <= 0) return 0;

            var result = index * _finalSize.Height;
            return result;
        }

        #endregion
    }
}