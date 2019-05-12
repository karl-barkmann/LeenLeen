using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Leen.Windows.Controls.Primitives
{
    /// <summary>
    /// 一个使用圆圈展示加载进度的进度提示控件。
    /// <para>
    /// 基于圆圈的交互设计，此控件的宽度和高度应该是一致才能确保正确展示。
    /// </para>
    /// </summary>
    public class ProgressCircle : FrameworkElement
    {
        private static readonly Brush defaultStokeBrsuh = new SolidColorBrush(Colors.Green);
        private static readonly Brush defaultFillBrsuh = new SolidColorBrush(Colors.Transparent);
        private static readonly Brush defaultForeground = new SolidColorBrush(Colors.Black);

        #region Progress

        /// <summary>
        /// Gets or sets the <see cref="Progress"/> value.
        /// </summary>
        public float Progress
        {
            get { return (float)GetValue(ProgressProperty); }
            set { SetValue(ProgressProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="Progress"/> property.
        /// </summary>
        public static readonly DependencyProperty ProgressProperty =
            DependencyProperty.Register(
                                nameof(Progress),
                                typeof(float),
                                typeof(ProgressCircle),
                                new PropertyMetadata(0F, ProgressPropertyChanged));

        private static void ProgressPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ProgressCircle)d).OnProgressChanged((float)e.OldValue, (float)e.NewValue);
        }

        /// <summary>
        /// Called when <see cref="Progress"/> changed.
        /// </summary>
        /// <param name="oldValue">Old value of <see cref="Progress"/>.</param>
        /// <param name="newValue">New value of <see cref="Progress"/>.</param>
        protected virtual void OnProgressChanged(float oldValue, float newValue)
        {
            UpdateProgressVisual(newValue, Thickness);
        }

        #endregion

        #region Thickness

        /// <summary>
        /// Gets or sets the <see cref="Thickness"/> value.
        /// </summary>
        public double Thickness
        {
            get { return (double)GetValue(ThicknessProperty); }
            set { SetValue(ThicknessProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="Thickness"/> property.
        /// </summary>
        public static readonly DependencyProperty ThicknessProperty =
            DependencyProperty.Register(
                                nameof(Thickness),
                                typeof(double),
                                typeof(ProgressCircle),
                                new PropertyMetadata(4d, ThicknessPropertyChanged, CoerceThicnessCallback));

        private static object CoerceThicnessCallback(DependencyObject d, object baseValue)
        {
            var thickness = (double)baseValue;

            if (thickness <= 0)
            {
                return 1;
            }

            return baseValue;
        }

        private static void ThicknessPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ProgressCircle)d).OnThicknessChanged((double)e.OldValue, (double)e.NewValue);
        }

        /// <summary>
        /// Called when <see cref="Thickness"/> changed.
        /// </summary>
        /// <param name="oldValue">Old value of <see cref="Thickness"/>.</param>
        /// <param name="newValue">New value of <see cref="Thickness"/>.</param>
        protected virtual void OnThicknessChanged(double oldValue, double newValue)
        {
            UpdateProgressVisual(Progress, newValue);
        }

        #endregion

        #region Fill

        /// <summary>
        /// Gets or sets the <see cref="Fill"/> value.
        /// </summary>
        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="Fill"/> property.
        /// </summary>
        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register(
                                nameof(Fill),
                                typeof(Brush),
                                typeof(ProgressCircle),
                                new PropertyMetadata(defaultFillBrsuh, FillPropertyChanged));

        private static void FillPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ProgressCircle)d).OnFillChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        /// <summary>
        /// Called when <see cref="Fill"/> changed.
        /// </summary>
        /// <param name="oldValue">Old value of <see cref="Fill"/>.</param>
        /// <param name="newValue">New value of <see cref="Fill"/>.</param>
        protected virtual void OnFillChanged(Brush oldValue, Brush newValue)
        {
            innerCircle.Fill = newValue;
        }

        #endregion

        #region Stroke

        /// <summary>
        /// Gets or sets the <see cref="Stroke"/> value.
        /// </summary>
        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="Stroke"/> property.
        /// </summary>
        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register(
                                nameof(Stroke),
                                typeof(Brush),
                                typeof(ProgressCircle),
                                new PropertyMetadata(defaultStokeBrsuh, StrokePropertyChanged));

        private static void StrokePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ProgressCircle)d).OnStrokeChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        /// <summary>
        /// Called when <see cref="Stroke"/> changed.
        /// </summary>
        /// <param name="oldValue">Old value of <see cref="Stroke"/>.</param>
        /// <param name="newValue">New value of <see cref="Stroke"/>.</param>
        protected virtual void OnStrokeChanged(Brush oldValue, Brush newValue)
        {
            outterCircle.Stroke = newValue;
        }

        #endregion

        #region Foreground

        /// <summary>
        /// Gets or sets the <see cref="Foreground"/> value.
        /// </summary>
        public Brush Foreground
        {
            get { return (Brush)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="Foreground"/> property.
        /// </summary>
        public static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.Register(
                                nameof(Foreground),
                                typeof(Brush),
                                typeof(ProgressCircle),
                                new PropertyMetadata(defaultForeground, ForegroundPropertyChanged));

        private static void ForegroundPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ProgressCircle)d).OnForegroundChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        /// <summary>
        /// Called when <see cref="Foreground"/> changed.
        /// </summary>
        /// <param name="oldValue">Old value of <see cref="Foreground"/>.</param>
        /// <param name="newValue">New value of <see cref="Foreground"/>.</param>
        protected virtual void OnForegroundChanged(Brush oldValue, Brush newValue)
        {
            _progressText.Foreground = newValue;
        }

        #endregion

        #region FontFamily

        /// <summary>
        /// Gets or sets the <see cref="FontFamily"/> value.
        /// </summary>
        public FontFamily FontFamily
        {
            get { return (FontFamily)GetValue(FontFamilyProperty); }
            set { SetValue(FontFamilyProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="FontFamily"/> property.
        /// </summary>
        public static readonly DependencyProperty FontFamilyProperty =
            DependencyProperty.Register(
                                nameof(FontFamily),
                                typeof(FontFamily),
                                typeof(ProgressCircle),
                                new PropertyMetadata(new FontFamily("微软雅黑"), FontFamilyPropertyChanged));

        private static void FontFamilyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ProgressCircle)d).OnFontFamilyChanged((FontFamily)e.OldValue, (FontFamily)e.NewValue);
        }

        /// <summary>
        /// Called when <see cref="FontFamily"/> changed.
        /// </summary>
        /// <param name="oldValue">Old value of <see cref="FontFamily"/>.</param>
        /// <param name="newValue">New value of <see cref="FontFamily"/>.</param>
        protected virtual void OnFontFamilyChanged(FontFamily oldValue, FontFamily newValue)
        {
            _progressText.FontFamily = newValue;
        }

        #endregion

        #region FontSize

        /// <summary>
        /// Gets or sets the <see cref="FontSize"/> value.
        /// </summary>
        public double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="FontSize"/> property.
        /// </summary>
        public static readonly DependencyProperty FontSizeProperty =
            DependencyProperty.Register(
                                nameof(FontSize),
                                typeof(double),
                                typeof(ProgressCircle),
                                new PropertyMetadata(14d, FontSizePropertyChanged));

        private static void FontSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ProgressCircle)d).OnFontSizeChanged((double)e.OldValue, (double)e.NewValue);
        }

        /// <summary>
        /// Called when <see cref="FontSize"/> changed.
        /// </summary>
        /// <param name="oldValue">Old value of <see cref="FontSize"/>.</param>
        /// <param name="newValue">New value of <see cref="FontSize"/>.</param>
        protected virtual void OnFontSizeChanged(double oldValue, double newValue)
        {
            _progressText.FontSize = newValue;
        }

        #endregion

        #region IsProgressVisible

        /// <summary>
        /// Gets or sets the <see cref="IsProgressVisible"/> value.
        /// </summary>
        public bool IsProgressVisible
        {
            get { return (bool)GetValue(IsProgressVisibleProperty); }
            set { SetValue(IsProgressVisibleProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="IsProgressVisible"/> property.
        /// </summary>
        public static readonly DependencyProperty IsProgressVisibleProperty =
            DependencyProperty.Register(
                                nameof(IsProgressVisible),
                                typeof(bool),
                                typeof(ProgressCircle),
                                new PropertyMetadata(true, IsProgressLabelVisiblePropertyChanged));

        private static void IsProgressLabelVisiblePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ProgressCircle)d).OnIsProgressLabelVisibleChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        /// <summary>
        /// Called when <see cref="IsProgressVisible"/> changed.
        /// </summary>
        /// <param name="oldValue">Old value of <see cref="IsProgressVisible"/>.</param>
        /// <param name="newValue">New value of <see cref="IsProgressVisible"/>.</param>
        protected virtual void OnIsProgressLabelVisibleChanged(bool oldValue, bool newValue)
        {
            _progressText.Visibility = newValue ? Visibility.Visible : Visibility.Collapsed;
        }

        #endregion

        private readonly TextBlock _progressText;
        private readonly Grid _rootGrid;
        private readonly Ellipse innerCircle;
        private readonly Ellipse outterCircle;

        /// <summary>
        /// 构造 <see cref="ProgressCircle"/> 类的实例。
        /// </summary>
        public ProgressCircle()
        {
            _rootGrid = new Grid();
            innerCircle = new Ellipse
            {
                Fill = new SolidColorBrush(Colors.Green),
                Opacity = 0.45,
                StrokeThickness = 5,
            };
            outterCircle = new Ellipse
            {
                RenderTransformOrigin = new Point(0.5, 0.5),
                Stroke = new SolidColorBrush(Colors.DarkBlue),
                StrokeThickness = 3,
                StrokeDashArray = new DoubleCollection(new double[] { 0, 500 })
            };
            var loadingRotate = new RotateTransform { Angle = -90 };
            outterCircle.RenderTransform = loadingRotate;
            _progressText = new TextBlock
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 14,
                FontFamily = new FontFamily("微软雅黑"),
                Foreground = new SolidColorBrush(Colors.Black)
            };
            _rootGrid.Children.Add(innerCircle);
            _rootGrid.Children.Add(outterCircle);
            _rootGrid.Children.Add(_progressText);
            AddVisualChild(_rootGrid);
            Loaded += ProgressCircle_Loaded;
        }

        #region viusal overrides

        /// <summary>
        /// 获取此元素内可视子元素的数目。
        /// </summary>
        protected override int VisualChildrenCount
        {
            get
            {
                return base.VisualChildrenCount + 1;
            }
        }

        /// <summary>
        /// 获取指定索引处的子元素。
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected override Visual GetVisualChild(int index)
        {
            if (index == VisualChildrenCount - 1)
            {
                return _rootGrid;
            }
            return base.GetVisualChild(index);
        }

        /// <summary>
        /// 测量此元素在布局中需要的大小。
        /// </summary>
        /// <param name="availableSize">布局空间中可用的大小。</param>
        /// <returns></returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            _rootGrid.Measure(availableSize);
            return base.MeasureOverride(availableSize);
        }

        /// <summary>
        /// 定位此元素并确定此元素的大小。
        /// </summary>
        /// <param name="finalSize">布局完成时得到的布局大小。</param>
        /// <returns></returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            _rootGrid.Arrange(new Rect(finalSize));
            return base.ArrangeOverride(finalSize);
        }

        /// <summary>
        /// 引发 <see cref="FrameworkElement.SizeChanged"/> 事件。
        /// </summary>
        /// <param name="sizeInfo">大小改变参数。</param>
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            UpdateProgressVisual(Progress, Thickness);
        }

        #endregion

        private void ProgressCircle_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateProgressVisual(Progress, Thickness);
        }

        private void UpdateProgressVisual(float progress, double thickness)
        {
            var factor = 100 * (ActualWidth / 100d);
            double value = ((factor - thickness) * Math.PI * progress) / (100 * thickness);
            outterCircle.StrokeDashArray = new DoubleCollection() { value, 500 };
            outterCircle.StrokeThickness = thickness;
            _progressText.Text = $"{progress}%";
        }
    }
}
