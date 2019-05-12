using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Leen.Windows.Controls
{
    /// <summary>
    /// 滑动面板,来自开源代码 加以修改
    /// </summary>
    public class SliderPanel : Panel, INotifyPropertyChanged
    {
        #region INotifyPropertyChanged 成员

        /// <summary>
        /// 在更改属性值时发生。
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void RaisePropertyChanged(String propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region 成员变量

        private Point mouseStart, mouseNow, mouseFirst, mouseFinal;
        int counter;
        //int selectedIndex;
        private bool canDrag;
        private double span = 0;
        private bool showNeighbor;
        private DateTime mouseWheelTime;
        private bool isMouseDown;
        double finalWidth = 0;
        private double duration;
        #endregion

        #region 公开属性

        /// <summary>
        /// 获取或设置动画过度时间
        /// </summary>
        public double Duration
        {
            get { return duration; }
            set
            {
                if (duration == value) return;
                if (value < 0)
                    value = 0;
                duration = value;
                RaisePropertyChanged("Duration");
            }
        }
        /// <summary>
        /// Gets or sets the counter.
        /// </summary>
        /// <value>The counter.</value>
        public int Counter
        {
            get
            {
                return counter;
            }
            set
            {
                counter = value;
                RaisePropertyChanged("Counter");
            }
        }

        /// <summary>
        /// 显示邻居
        /// </summary>
        public bool ShowNeighbor
        {
            get { return showNeighbor; }
            set { showNeighbor = value; }
        }

        /// <summary>
        /// 获取或设置 面板当前展示第几页
        /// </summary>
        /// <summary>
        /// The <see cref="SelectedIndex" /> dependency property's name.
        /// </summary>
        public const string SelectedIndexPropertyName = "SelectedIndex";

        /// <summary>
        /// Gets or sets the value of the <see cref="SelectedIndex" />
        /// property. This is a dependency property.
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
            typeof(SliderPanel),
            new PropertyMetadata(new PropertyChangedCallback((sender, e) =>
            {
                SliderPanel contro = sender as SliderPanel;
                contro.AnimateBySelectedIndex((int)e.NewValue);
            })));

        /// <summary>
        /// 获取或设置 是否可以通过鼠标拖动面板
        /// </summary>
        public bool CanDrag
        {
            get { return canDrag; }
            set
            {
                if (canDrag == value) return;
                canDrag = value;
                if (canDrag)
                {
                    MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(SliderPanel_MouseLeftButtonDown);
                    MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(SliderPanel_MouseLeftButtonUp);
                    MouseWheel += new System.Windows.Input.MouseWheelEventHandler(SliderPanel_MouseWheel);
                }
                else
                {
                    MouseLeftButtonDown -= new System.Windows.Input.MouseButtonEventHandler(SliderPanel_MouseLeftButtonDown);
                    MouseLeftButtonUp -= new System.Windows.Input.MouseButtonEventHandler(SliderPanel_MouseLeftButtonUp);
                    MouseWheel -= new System.Windows.Input.MouseWheelEventHandler(SliderPanel_MouseWheel);
                }

                RaisePropertyChanged("CanDrag");
            }
        }

        #endregion

        #region 公共事件

        /// <summary>
        /// 当选中页索引改变时引发。
        /// </summary>
        public static readonly RoutedEvent SelectedIndexChangedEvent;

        /// <summary>
        /// 当选中页索引改变时引发。
        /// </summary>
        public event EventHandler<RoutedEventArgs> SelectedIndexChanged
        {
            add
            {
                base.AddHandler(SelectedIndexChangedEvent, value);
            }
            remove
            {
                base.RemoveHandler(SelectedIndexChangedEvent, value);
            }
        }

        #endregion

        #region 构造函数

        static SliderPanel()
        {
            SelectedIndexChangedEvent = EventManager.RegisterRoutedEvent("SelectedIndexChanged", RoutingStrategy.Direct,
                typeof(EventHandler<RoutedEventArgs>), typeof(SliderPanel));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SliderPanel"/> class.
        /// </summary>
        public SliderPanel()
        {
            Duration = 0.3;
            CanDrag = true;
            SizeChanged += new SizeChangedEventHandler(SliderPanel_SizeChanged);
        }

        void SliderPanel_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (DateTime.Now - mouseWheelTime < TimeSpan.FromMilliseconds(150))
                return;
            mouseWheelTime = DateTime.Now;
            var delta = e.Delta;
            if (delta < 0)
                SelectedIndex++;
            else
                SelectedIndex--;
        }

        void SliderPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //改变大小后重新计算
            Animate(SelectedIndex, 0);
        }

        #endregion

        #region 回调函数

        void SliderPanel_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            isMouseDown = true;
            CaptureMouse();

            mouseStart = e.GetPosition(this);
            mouseNow = mouseStart;
            mouseFirst = mouseStart;

            MouseMove -= new System.Windows.Input.MouseEventHandler(SliderPanel_MouseMove);
            MouseMove += new System.Windows.Input.MouseEventHandler(SliderPanel_MouseMove);
        }

        void SliderPanel_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            mouseNow = e.GetPosition(this);

            //移动所有子元素
            for (int i = 0; i < Children.Count; i++)
            {
                TranslateTransform yu = new TranslateTransform(Children[i].RenderTransform.Value.OffsetX + (mouseNow.X - mouseStart.X), 0);
                Children[i].RenderTransform = yu;
            }

            mouseStart = mouseNow;
        }

        void SliderPanel_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!isMouseDown)
                return;
            isMouseDown = false;
            try
            {
                MouseMove -= new System.Windows.Input.MouseEventHandler(SliderPanel_MouseMove);

                mouseFinal = e.GetPosition(this);
                ReleaseMouseCapture();

                if ((mouseFinal.X - mouseFirst.X) > 0)
                {
                    if (Math.Abs(mouseFinal.X - mouseFirst.X) > 50)
                        Counter = Counter + 1;
                }
                else
                {
                    if (Math.Abs(mouseFinal.X - mouseFirst.X) > 50)
                        Counter = Counter - 1;
                }

                double pTo, pFrom;

                pTo = Counter * (finalWidth + span) + 2 * span;
                pFrom = (mouseFinal.X - mouseFirst.X) > 0 ? (pTo - finalWidth) + (mouseFinal.X - mouseFirst.X) : (pTo + finalWidth) + (mouseFinal.X - mouseFirst.X);

                if (Math.Abs(mouseFinal.X - mouseFirst.X) < 50)
                    pFrom = pTo + (mouseFinal.X - mouseFirst.X);

                if (Counter > 0)
                {
                    pTo = (Counter - 1) * finalWidth + 2 * span;
                    Counter = Counter - 1;
                }
                else if (Counter <= Children.Count * -1)
                {
                    pTo = (Counter + 1) * finalWidth;
                    Counter = Counter + 1;
                }


                for (int i = 0; i < Children.Count; i++)
                {
                    DoubleAnimation da = new DoubleAnimation(pFrom, pTo, new Duration(TimeSpan.FromSeconds(Duration)));
                    ((TranslateTransform)Children[i].RenderTransform).BeginAnimation(TranslateTransform.XProperty, da);
                }

                SelectedIndex = Math.Abs(Counter);
            }
            catch { }
        }

        #endregion

        #region 公开方法

        #endregion

        #region 私有方法

        /// <summary>
        /// 动画选中第几页
        /// </summary>
        /// <param name="index"></param>
        protected void AnimateBySelectedIndex(int index)
        {
            if (index < 0 || index > Children.Count - 1 || index * -1 == Counter)
                return;

            Animate(index, Duration);
        }

        private void Animate(int index, double d)
        {
            index *= -1;
            double pTo, pFrom;
            pTo = -GetTargetX(-index) + 2 * span;
            pFrom = index > Counter ? (pTo - finalWidth - span) : (pTo + finalWidth + span);

            Counter = index;

            for (int i = 0; i < Children.Count; i++)
            {
                DoubleAnimation da = new DoubleAnimation(pFrom, pTo, new Duration(TimeSpan.FromSeconds(d)));
                TranslateTransform yu = new TranslateTransform(Children[i].RenderTransform.Value.OffsetX, 0);
                Children[i].RenderTransform = yu;

                ((TranslateTransform)Children[i].RenderTransform).BeginAnimation(TranslateTransform.XProperty, da);
            }
        }

        #endregion

        #region 重载方法

        /// <summary>
        /// 当在派生类中重写时，请测量子元素在布局中所需的大小，然后确定 <see cref="T:System.Windows.FrameworkElement"/> 派生类的大小。
        /// </summary>
        /// <param name="availableSize">此元素可以赋给子元素的可用大小。可以指定无穷大值，这表示元素的大小将调整为内容的可用大小。</param>
        /// <returns>此元素在布局过程中所需的大小，这是由此元素根据对其子元素大小的计算而确定的。</returns>
        protected override System.Windows.Size MeasureOverride(System.Windows.Size availableSize)
        {
            Size resultSize = new Size(0, 0);

            foreach (UIElement child in Children)
            {
                child.Measure(availableSize);
                resultSize.Width = Math.Max(resultSize.Width, DesiredSize.Width);
                resultSize.Height = Math.Max(resultSize.Height, child.DesiredSize.Height);
            }

            resultSize.Width =
                double.IsPositiveInfinity(availableSize.Width) ?
                resultSize.Width : availableSize.Width;

            resultSize.Height =
                double.IsPositiveInfinity(availableSize.Height) ?
                resultSize.Height : availableSize.Height;

            return resultSize;
        }

        /// <summary>
        /// 在派生类中重写时，请为 <see cref="T:System.Windows.FrameworkElement"/> 派生类定位子元素并确定大小。
        /// </summary>
        /// <param name="finalSize">父级中此元素应用来排列自身及其子元素的最终区域。</param>
        /// <returns>所用的实际大小。</returns>
        protected override System.Windows.Size ArrangeOverride(System.Windows.Size finalSize)
        {
            if (ShowNeighbor)
            {
                finalWidth = finalSize.Width * 0.8;
                span = finalSize.Width * 0.05;
            }
            else
                finalWidth = finalSize.Width;

            if (finalWidth < 0)
                finalWidth = 1;
            double selectedX = finalWidth * SelectedIndex + SelectedIndex * span;

            for (int i = 0; i < Children.Count; i++)
            {
                double x = GetTargetX(i);
                Children[i].Arrange(new Rect(x, (double)0, finalWidth, finalSize.Height));
            }

            var clipWidth = DesiredSize.Width == 0 ? ActualWidth : DesiredSize.Width;
            if (clipWidth == 0)
                clipWidth = finalSize.Width;

            Clip = new RectangleGeometry(new Rect(0, 0, clipWidth, DesiredSize.Height));

            return base.ArrangeOverride(finalSize);
        }

        //获取每一页的开始坐标
        private double GetTargetX(int selectedIndex)
        {
            if (selectedIndex < 0)
                return 0;

            double x = 0;
            double selectedX = finalWidth * SelectedIndex + SelectedIndex * span;

            if (selectedIndex < SelectedIndex)
            {
                double diff = SelectedIndex - selectedIndex - 1;
                x = selectedX - diff * (finalWidth + span) - finalWidth - span;
            }
            else if (selectedIndex > SelectedIndex)
            {
                double diff = selectedIndex - SelectedIndex - 1;
                x = selectedX + diff * (finalWidth + span) + span + finalWidth;
            }
            else if (selectedIndex == SelectedIndex)
            {
                x = selectedX;
            }

            return x;
        }

        #endregion
    }
}
