//zhongying 2012.9.10

using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Leen.Windows.Controls
{
    /// <summary>
    /// 动画化的UniformGrid
    /// </summary>
    public class AnimatedUniformGrid : UniformGrid
    {
        #region 成员变量

        private int _columns;
        private int _rows;
        private DateTime sizeChangedTime;
        private Thread calcThread;
        private DateTime animationTime;

        #endregion

        #region 构造函数

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimatedUniformGrid"/> class.
        /// </summary>
        public AnimatedUniformGrid()
        {
            SizeChanged += new SizeChangedEventHandler(CustomControl_SizeChanged);
        }

        #endregion

        #region 重写

        /// <summary>
        /// 通过在所有子元素之间平均分配空间来定义 <see cref="T:System.Windows.Controls.Primitives.UniformGrid"/> 的布局。
        /// </summary>
        /// <param name="arrangeSize">供网格使用的区域的 <see cref="T:System.Windows.Size"/>。</param>
        /// <returns>
        /// 为显示可见子元素而呈现的网格的实际 <see cref="T:System.Windows.Size"/>。
        /// </returns>
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            //1秒内就表示为改变控件大小触发的方法
            //bool isSizeChanged = DateTime.Now - sizeChangedTime <= TimeSpan.FromSeconds(1);
            bool isSizeChanged = false;

            UpdateComputedValues();
            Rect finalRect = new Rect(0.0, 0.0, arrangeSize.Width / ((double)_columns), arrangeSize.Height / ((double)_rows));
            double width = finalRect.Width;
            double num2 = arrangeSize.Width - 1.0;
            finalRect.X += finalRect.Width * FirstColumn;
            foreach (UIElement element in base.InternalChildren)
            {
                if (element.RenderTransform as TransformGroup == null)
                {
                    TransformGroup transfromGroup = new TransformGroup();
                    transfromGroup.Children.Add(new TranslateTransform());
                    transfromGroup.Changed += new EventHandler(transfromGroup_Changed);
                    element.RenderTransform = transfromGroup;
                }

                //默认位置全部放到0,0 然后由动画展开
                Rect temp = new Rect(0, 0, finalRect.Width, finalRect.Height);
                if (isSizeChanged)
                {
                    Animation(element, new Rect(), 0);
                    element.Arrange(finalRect);
                }
                else
                {
                    element.Arrange(temp);
                    Animation(element, finalRect, Duration);
                }

                if (element.Visibility != Visibility.Collapsed)
                {
                    finalRect.X += width;
                    if (finalRect.X >= num2)
                    {
                        finalRect.Y += finalRect.Height;
                        finalRect.X = 0.0;
                    }
                }
            }


            return arrangeSize;
        }

        #endregion

        #region 私有方法

        private void Animation(UIElement element, Rect finalRect, double duration)
        {
            var group = (TransformGroup)element.RenderTransform;
            var translate = (TranslateTransform)group.Children[0];

            translate.BeginAnimation(TranslateTransform.XProperty, MakeAnimation(finalRect.X, duration));
            translate.BeginAnimation(TranslateTransform.YProperty, MakeAnimation(finalRect.Y, duration));
        }


        void transfromGroup_Changed(object sender, EventArgs e)
        {
            RefreshAnimatingTime(null);
        }

        //刷新动画时间
        void RefreshAnimatingTime(DateTime? refreshTime)
        {

            IsAnimating = true;


            if (refreshTime != null)
                animationTime = refreshTime.Value;
            else
                animationTime = DateTime.Now;
            if (calcThread != null)
                return;

            int duration = (int)Duration;
            calcThread = new Thread(new ThreadStart(() =>
            {
                //动画刷新时间大于300毫秒，就认定为动画结束
                while (DateTime.Now - animationTime < TimeSpan.FromMilliseconds(300))
                {
                    Thread.Sleep(41);
                }

                Dispatcher.Invoke(new Action(() =>
                {
                    IsAnimating = false;
                }));

                calcThread = null;
            }));
            calcThread.IsBackground = true;
            calcThread.Start();
        }

        private DoubleAnimation MakeAnimation(double to, double duration)
        {
            DoubleAnimation animation = new DoubleAnimation(to, new Duration(TimeSpan.FromMilliseconds(duration)));
            return animation;
        }

        private void UpdateComputedValues()
        {
            _columns = Columns;
            _rows = Rows;
            if (FirstColumn >= _columns)
            {
                FirstColumn = 0;
            }
            if ((_rows == 0) || (_columns == 0))
            {
                int num = 0;
                int num2 = 0;
                int count = base.InternalChildren.Count;
                while (num2 < count)
                {
                    UIElement element = base.InternalChildren[num2];
                    if (element.Visibility != Visibility.Collapsed)
                    {
                        num++;
                    }
                    num2++;
                }
                if (num == 0)
                {
                    num = 1;
                }
                if (_rows == 0)
                {
                    if (_columns > 0)
                    {
                        _rows = ((num + FirstColumn) + (_columns - 1)) / _columns;
                    }
                    else
                    {
                        _rows = (int)Math.Sqrt((double)num);
                        if ((_rows * _rows) < num)
                        {
                            _rows++;
                        }
                        _columns = _rows;
                    }
                }
                else if (_columns == 0)
                {
                    _columns = (num + (_rows - 1)) / _rows;
                }
            }
        }

        void CustomControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            sizeChangedTime = DateTime.Now;
        }

        #endregion

        #region 公开属性

        #region 依赖属性

        #region Duration

        /// <summary>
        /// The <see cref="Duration" /> dependency property's name.
        /// </summary>
        public const string DurationPropertyName = "Duration";

        /// <summary>
        /// Gets or sets the value of the <see cref="Duration" />
        /// property. This is a dependency property.
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
            typeof(AnimatedUniformGrid),
            new PropertyMetadata(300d));

        #endregion

        #region IsAnimating

        /// <summary>
        /// The <see cref="IsAnimating" /> dependency property's name.
        /// </summary>
        public const string IsAnimatingPropertyName = "IsAnimating";

        /// <summary>
        /// Gets or sets the value of the <see cref="IsAnimating" />
        /// property. This is a dependency property.
        /// </summary>
        public bool IsAnimating
        {
            get
            {
                return (bool)GetValue(IsAnimatingProperty);
            }
            set
            {
                SetValue(IsAnimatingProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="IsAnimating" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsAnimatingProperty = DependencyProperty.Register(
            IsAnimatingPropertyName,
            typeof(bool),
            typeof(AnimatedUniformGrid),
            new PropertyMetadata(false));

        #endregion

        #endregion

        #endregion
    }
}
