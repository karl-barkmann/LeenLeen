using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace Leen.Media.Controls.Primitives
{
    /// <summary>
    /// 实现一个基于DirectX9接口的媒体渲染控件，支持画面缩放及平移。
    /// </summary>
    public abstract class D3DRenderElement : SeekableRenderElement
    {
        #region EnableZoomingAnimation

        /// <summary>
        /// Dependency property for <see cref="EnableZoomingAnimation"/>.
        /// </summary>
        public static readonly DependencyProperty EnableZoomingAnimationProperty =
            DependencyProperty.Register(
                                nameof(EnableZoomingAnimation),
                                typeof(bool),
                                typeof(ZoomableRenderElement),
                                new PropertyMetadata(true));

        /// <summary>
        /// 获取嚯设置一个值指示是否启用缩放动画。
        /// </summary>
        public bool EnableZoomingAnimation
        {
            get { return (bool)GetValue(EnableZoomingAnimationProperty); }
            set { SetValue(EnableZoomingAnimationProperty, value); }
        }

        #endregion

        #region ZoomAnimationDuration

        /// <summary>
        /// Dependency property from <see cref="ZoomAnimationDuration"/> property。
        /// </summary>
        public static readonly DependencyProperty ZoomAnimationDurationProperty =
            DependencyProperty.Register(
                nameof(ZoomAnimationDuration),
                typeof(double),
                typeof(D3DRenderElement),
                new PropertyMetadata(300d));

        /// <summary>
        /// 获取或设置缩放及平移动画的持续时长（毫秒）。
        /// </summary>
        public double ZoomAnimationDuration
        {
            get { return (double)GetValue(ZoomAnimationDurationProperty); }
            set { SetValue(ZoomAnimationDurationProperty, value); }
        }

        #endregion

        #region EnablePanning

        /// <summary>
        /// Dependency property for <see cref="EnablePanning"/> property.
        /// </summary>
        public static readonly DependencyProperty EnablePanningProperty =
            DependencyProperty.Register(
                                nameof(EnablePanning),
                                typeof(bool),
                                typeof(D3DRenderElement),
                                new PropertyMetadata(true, EnablePanningPropertyChanged));

        private static void EnablePanningPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((D3DRenderElement)d).OnEnablePanningChanged(e);
        }

        /// <summary>
        /// 当 <see cref="EnablePanning"/> 改变时调用。
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnEnablePanningChanged(DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                RenderTarget.MouseDown += RenderingImage_MouseDown;
                RenderTarget.PreviewMouseMove += RenderingImage_MouseMove;
                RenderTarget.MouseUp += RenderingImage_MouseUp;
            }
            else
            {
                RenderTarget.MouseDown -= RenderingImage_MouseDown;
                RenderTarget.PreviewMouseMove -= RenderingImage_MouseMove;
                RenderTarget.MouseUp -= RenderingImage_MouseUp;
            }
        }

        /// <summary>
        /// 获取或设置一个值指示是否启用缩放时的视频画面拖动。
        /// </summary>
        public bool EnablePanning
        {
            get { return (bool)GetValue(EnablePanningProperty); }
            set { SetValue(EnablePanningProperty, value); }
        }

        #endregion

        #region EnablePanningAnimation

        /// <summary>
        /// 获取或设置一个值表示是否启用平移动画。
        /// </summary>
        public bool EnablePanningAnimation
        {
            get { return (bool)GetValue(EnablePanningAnimationProperty); }
            set { SetValue(EnablePanningAnimationProperty, value); }
        }

        /// <summary>
        /// Dependency propery of <see cref="EnablePanningAnimation"/>.
        /// </summary>
        public static readonly DependencyProperty EnablePanningAnimationProperty =
            DependencyProperty.Register(
                nameof(EnablePanningAnimation),
                typeof(bool),
                typeof(D3DRenderElement),
                new PropertyMetadata(true));

        #endregion

        #region PanningAnimationDuration

        /// <summary>
        /// 获取或设置平移动画持续时长。
        /// </summary>
        public double PanningAnimationDuration
        {
            get { return (double)GetValue(PanningAnimationDurationProperty); }
            set { SetValue(PanningAnimationDurationProperty, value); }
        }

        /// <summary>
        /// Dependency propery of <see cref="PanningAnimationDuration"/>.
        /// </summary>
        public static readonly DependencyProperty PanningAnimationDurationProperty =
            DependencyProperty.Register(
                nameof(PanningAnimationDuration),
                typeof(double),
                typeof(D3DRenderElement),
                new PropertyMetadata(100d));

        #endregion

        #region EnableThumbnailView

        /// <summary>
        /// Gets or sets the <see cref="EnableThumbnailView"/> value.
        /// </summary>
        public bool EnableThumbnailView
        {
            get { return (bool)GetValue(EnableThumbnailViewProperty); }
            set { SetValue(EnableThumbnailViewProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="EnableThumbnailView"/> property.
        /// </summary>
        public static readonly DependencyProperty EnableThumbnailViewProperty =
            DependencyProperty.Register(
                                nameof(EnableThumbnailView),
                                typeof(bool),
                                typeof(D3DRenderElement),
                                new PropertyMetadata(false, EnableThumbnailViewPropertyChanged));

        private static void EnableThumbnailViewPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((D3DRenderElement)d).OnEnableThumbnailViewChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        /// <summary>
        /// Called when <see cref="EnableThumbnailView"/> changed.
        /// </summary>
        /// <param name="oldValue">Old value of <see cref="EnableThumbnailView"/>.</param>
        /// <param name="newValue">New value of <see cref="EnableThumbnailView"/>.</param>
        protected virtual void OnEnableThumbnailViewChanged(bool oldValue, bool newValue)
        {
            if (newValue)
            {
                m_ThubmnailImage.Source = m_RenderTarget.ForegroundRenderSource; 
                m_PanningThumbContainer.Visibility = Visibility.Visible;
                m_PanningThumb.DragDelta += PanningThumb_DragDelta;
                m_ThubmnailImage.PreviewMouseLeftButtonDown += ThubmnailImage_PreviewMouseDown;
                m_ThubmnailImage.PreviewMouseMove += ThubmnailImage_PreviewMouseMove;
                m_ThubmnailImage.PreviewMouseLeftButtonUp += ThubmnailImage_PreviewMouseLeftButtonUp;
            }
            else
            {
                m_ThubmnailImage.Source = null;
                m_RenderTarget.ForegroundRenderSource = null;
                m_PanningThumbContainer.Visibility = Visibility.Collapsed;
                m_PanningThumb.DragDelta -= PanningThumb_DragDelta;
                m_ThubmnailImage.PreviewMouseLeftButtonDown -= ThubmnailImage_PreviewMouseDown;
                m_ThubmnailImage.PreviewMouseMove -= ThubmnailImage_PreviewMouseMove;
                m_ThubmnailImage.PreviewMouseLeftButtonUp -= ThubmnailImage_PreviewMouseLeftButtonUp;
            }
        }

        #endregion

        #region EnablePanningAnyWhere

        /// <summary>
        /// 获取或设置一个值指示是否允许将画面平移出控件边界。
        /// </summary>
        public bool EnablePanningAnyWhere
        {
            get { return (bool)GetValue(EnablePanningAnyWhereProperty); }
            set { SetValue(EnablePanningAnyWhereProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="EnablePanningAnyWhere"/> property.
        /// </summary>
        public static readonly DependencyProperty EnablePanningAnyWhereProperty =
            DependencyProperty.Register(
                nameof(EnablePanningAnyWhere), 
                typeof(bool), 
                typeof(D3DRenderElement),
                new PropertyMetadata(true));

        #endregion

        #region ThumbnailViewRatio

        /// <summary>
        /// Gets or sets the <see cref="ThumbnailViewRatio"/> value.
        /// </summary>
        public double ThumbnailViewRatio
        {
            get { return (double)GetValue(ThumbnailViewRatioProperty); }
            set { SetValue(ThumbnailViewRatioProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="ThumbnailViewRatio"/> property.
        /// </summary>
        public static readonly DependencyProperty ThumbnailViewRatioProperty =
            DependencyProperty.Register(
                                nameof(ThumbnailViewRatio),
                                typeof(double),
                                typeof(D3DRenderElement),
                                new PropertyMetadata(0.25d, ThumbnailViewRatioPropertyChanged));

        private static void ThumbnailViewRatioPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((D3DRenderElement)d).OnThumbnailViewRatioChanged((double)e.OldValue, (double)e.NewValue);
        }

        /// <summary>
        /// Called when <see cref="ThumbnailViewRatio"/> changed.
        /// </summary>
        /// <param name="oldValue">Old value of <see cref="ThumbnailViewRatio"/>.</param>
        /// <param name="newValue">New value of <see cref="ThumbnailViewRatio"/>.</param>
        protected virtual void OnThumbnailViewRatioChanged(double oldValue, double newValue)
        {
            if (EnableThumbnailView)
            {
                m_ThubmnailImage.Width = RenderTarget.RenderSize.Width * newValue;
                m_ThubmnailImage.Height = RenderTarget.RenderSize.Height * newValue;
            }
        }

        #endregion

        #region IsThumbnailViewLocked

        /// <summary>
        /// Gets or sets the <see cref="IsThumbnailViewLocked"/> value.
        /// </summary>
        public bool IsThumbnailViewLocked
        {
            get { return (bool)GetValue(IsThumbnailViewLockedProperty); }
            set { SetValue(IsThumbnailViewLockedProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="IsThumbnailViewLocked"/> property.
        /// </summary>
        public static readonly DependencyProperty IsThumbnailViewLockedProperty =
            DependencyProperty.Register(
                                nameof(IsThumbnailViewLocked),
                                typeof(bool),
                                typeof(D3DRenderElement),
                                new PropertyMetadata(false, IsThumbnailViewLockedPropertyChanged));

        private static void IsThumbnailViewLockedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((D3DRenderElement)d).OnIsThumbnailViewLockedChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        /// <summary>
        /// Called when <see cref="IsThumbnailViewLocked"/> changed.
        /// </summary>
        /// <param name="oldValue">Old value of <see cref="IsThumbnailViewLocked"/>.</param>
        /// <param name="newValue">New value of <see cref="IsThumbnailViewLocked"/>.</param>
        protected virtual void OnIsThumbnailViewLockedChanged(bool oldValue, bool newValue)
        {
            if (!newValue)
            {
                m_ThubmnailImage.PreviewMouseLeftButtonDown += ThubmnailImage_PreviewMouseDown;
                m_ThubmnailImage.PreviewMouseMove += ThubmnailImage_PreviewMouseMove;
                m_ThubmnailImage.PreviewMouseLeftButtonUp += ThubmnailImage_PreviewMouseLeftButtonUp;
            }
            else
            {
                m_ThubmnailImage.PreviewMouseLeftButtonDown -= ThubmnailImage_PreviewMouseDown;
                m_ThubmnailImage.PreviewMouseMove -= ThubmnailImage_PreviewMouseMove;
                m_ThubmnailImage.PreviewMouseLeftButtonUp -= ThubmnailImage_PreviewMouseLeftButtonUp;
            }
        }

        #endregion

        #region Matrix

        /// <summary>
        /// Dependency property of <see cref="Matrix"/> property.
        /// </summary>
        public static readonly DependencyProperty MatrixProperty =
            DependencyProperty.Register(
                nameof(Matrix),
                typeof(Matrix),
                typeof(D3DRenderElement),
                new PropertyMetadata(default(Matrix), MatrixPropertyChanged));

        /// <summary>
        /// 获取或设置用于进行缩放及平移渲染转换参数。
        /// <para>
        /// 设置此属性以实现画面缩放及平移。
        /// </para>
        /// </summary>
        public Matrix Matrix
        {
            get { return (Matrix)GetValue(MatrixProperty); }
            set { SetValue(MatrixProperty, value); }
        }

        private static void MatrixPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((D3DRenderElement)d).OnMatrixChanged((Matrix)e.OldValue, (Matrix)e.NewValue);
        }

        /// <summary>
        /// 当缩放及平移渲染转换参数变化时调用。
        /// </summary>
        /// <param name="from">原有的转换参数。</param>
        /// <param name="to">将要进行转换的参数。</param>
        protected virtual void OnMatrixChanged(Matrix from, Matrix to)
        {
            var transform = m_RenderTarget.RenderTransform as MatrixTransform;
            transform.Matrix = to;

            UpdateThumbnailViewSizeAndOffset(to);
        }

        #endregion

        private Grid m_RootLayout;
        private ScrollViewer m_RenderingViewport;
        private Thumb m_PanningThumb;
        private Canvas m_PanningThumbContainer;
        private IRenderTarget m_RenderTarget;
        private Image m_ThubmnailImage;

        /// <summary>
        /// 构造 <see cref="D3DRenderElement"/> 类的实例。
        /// </summary>
        protected D3DRenderElement()
        {
            CreateVisualChild();
            BindToTemplateParent();
            InitializeTransform();
        }

        /// <summary>
        /// 承载渲染表层的 <see cref="Image"/> 对象。
        /// </summary>
        protected FrameworkElement RenderTarget
        {
            get
            {
                return m_RenderTarget as FrameworkElement;
            }
        }

        /// <summary>
        /// Indicates that the initialization process for the element is complete.
        /// </summary>
        public override void EndInit()
        {
            base.EndInit();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                m_RenderTarget.Initialize(MediaPlayer, MediaRenderer);

                if (EnablePanning)
                {
                    RenderTarget.MouseDown += RenderingImage_MouseDown;
                    RenderTarget.MouseMove += RenderingImage_MouseMove;
                    RenderTarget.MouseUp += RenderingImage_MouseUp;
                }

                if (EnableThumbnailView)
                {
                    m_ThubmnailImage.Source = m_RenderTarget.ForegroundRenderSource; 
                    m_PanningThumbContainer.Visibility = Visibility.Visible;
                    m_PanningThumb.DragDelta += PanningThumb_DragDelta;
                    if (!IsThumbnailViewLocked)
                    {
                        m_ThubmnailImage.PreviewMouseLeftButtonDown += ThubmnailImage_PreviewMouseDown;
                        m_ThubmnailImage.PreviewMouseMove += ThubmnailImage_PreviewMouseMove;
                        m_ThubmnailImage.PreviewMouseLeftButtonUp += ThubmnailImage_PreviewMouseLeftButtonUp;
                    }
                }
            }
        }

        /// <summary>
        /// 当媒体打开时调用。
        /// <para>
        /// 我们通常在媒体打开时获取或设置媒体播放器的音量时长等信息。
        /// 继承类应正确的在媒体打开时获取或设置相应媒体信息。
        /// </para>
        /// </summary>
        protected override void OnMediaOpened()
        {
            base.OnMediaOpened();
            if (EnableThumbnailView)
            {
                m_ThubmnailImage.Source = m_RenderTarget.ForegroundRenderSource;
            }

            double controlAspectRatio = ActualWidth / ActualHeight;
            double videoAspectRatio = (double)NaturalVideoWidth / NaturalVideoHeight;
            var renderSize = new Size(ActualWidth, ActualHeight);
            if (controlAspectRatio < videoAspectRatio)
            {
                //横向完全填充
                renderSize = new Size(ActualWidth, ActualWidth / videoAspectRatio);
            }
            else if (controlAspectRatio > videoAspectRatio)
            {
                //纵向完全填充
                renderSize = new Size(ActualHeight * videoAspectRatio, ActualHeight);
            }

            m_RenderTarget.Setup(renderSize);
        }

        /// <summary>
        /// 当关闭媒体时调用。
        /// </summary>
        protected override void OnMediaClosed()
        {
            base.OnMediaClosed();
            m_RenderTarget.Clear();
            MediaRenderer.ClearSurface();
        }

        /// <summary>
        /// 当布局系统开始测量控件大小时调用。
        /// </summary>
        /// <param name="availableSize">可用的最大控件大小。</param>
        /// <returns></returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            m_RenderingViewport.Measure(availableSize);
            return base.MeasureOverride(availableSize);
        }

        /// <summary>
        /// 当布局系统开始设置控件位置时调用。
        /// </summary>
        /// <param name="finalSize">测量得到的控件大小。</param>
        /// <returns></returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            m_RenderingViewport.Arrange(new Rect(finalSize));

            var videoRenderSize = RenderTarget.RenderSize;

            SetRenderingVideoWidth(videoRenderSize.Width);
            SetRenderingVideoHeight(videoRenderSize.Height);

            m_ThubmnailImage.Width = videoRenderSize.Width * ThumbnailViewRatio;
            m_ThubmnailImage.Height = videoRenderSize.Height * ThumbnailViewRatio;

            return base.ArrangeOverride(finalSize);
        }

        /// <summary>
        /// 获取子视图元素的数量。
        /// </summary>
        protected override int VisualChildrenCount
        {
            get
            {
                return base.VisualChildrenCount + 1;
            }
        }

        /// <summary>
        /// 获取指定索引的子视图。
        /// </summary>
        /// <param name="index">子视图的索引。</param>
        /// <returns></returns>
        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= VisualChildrenCount)
            {
                throw new IndexOutOfRangeException(nameof(index));
            }

            if (index == 0)
            {
                return m_RenderingViewport;
            }

            return base.GetVisualChild(index - 1);
        }

        /// <summary>
        /// 放大媒体渲染的视频画面。
        /// </summary>
        /// <param name="curZoomFactor">当前缩放比例参数。</param>
        /// <param name="oldZoomFactor">前一缩放比例参数。</param>
        protected override void ZoomIn(double oldZoomFactor, double curZoomFactor)
        {
            Zoom(oldZoomFactor, curZoomFactor);
        }

        /// <summary>
        /// 缩小媒体渲染的视频画面。
        /// </summary>
        /// <param name="curZoomFactor">当前缩放比例参数。</param>
        /// <param name="oldZoomFactor">前一缩放比例参数。</param>
        protected override void ZoomOut(double oldZoomFactor, double curZoomFactor)
        {
            Zoom(oldZoomFactor, curZoomFactor);
        }

        /// <summary>
		/// 获取一个值指示是否可以进行截图。
        /// </summary>
        /// <returns></returns>
        protected override bool GetCanTakeSnapshot()
        {
            return base.GetCanTakeSnapshot();
        }

        /// <summary>
        /// 获取最后一次成功进行的截帧图片文件名.
        /// </summary>
        protected string SnapshotFile
        {
            get;
            private set;
        }

        /// <summary>
		/// 截取当前帧并生成图片文件。
        /// </summary>
        protected override void TakeSnapshot()
        {
            base.TakeSnapshot();
            Guid guid = Guid.NewGuid();
            string thumbnailPath = Path.Combine(
                Path.GetTempPath(),
                guid.ToString() + ".png");
            using (FileStream stream = new FileStream(thumbnailPath, FileMode.Create))
            {
                DrawingVisual vis = new DrawingVisual();
                using (DrawingContext cont = vis.RenderOpen())
                {
                    cont.DrawImage(m_RenderTarget.ForegroundRenderSource,
                        new Rect(new Size(NaturalVideoWidth, NaturalVideoHeight)));
                    m_RenderTarget.ForegroundRenderSource = null;
                }

                var presentationSource = PresentationSource.FromVisual(this);
                var dpiX = presentationSource.CompositionTarget.TransformToDevice.M11;
                var dpiY = presentationSource.CompositionTarget.TransformToDevice.M22;
                var rtb = new RenderTargetBitmap(
                    NaturalVideoWidth,
                    NaturalVideoHeight,
                    dpiX * 96,
                    dpiY * 96,
                    PixelFormats.Default);
                rtb.Render(vis);

                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(rtb));
                encoder.Save(stream);
                SnapshotFile = thumbnailPath;
            }
        }

        /// <summary>
        /// 释放媒体渲染资源。
        /// </summary>
        /// <param name="disposing">一个值指示是否正在通过调用 <see cref="IDisposable.Dispose"/> 进行资源释放。</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                RenderTarget.MouseDown -= RenderingImage_MouseDown;
                RenderTarget.MouseMove -= RenderingImage_MouseMove;
                RenderTarget.MouseUp -= RenderingImage_MouseUp;

                m_ThubmnailImage.PreviewMouseLeftButtonDown -= ThubmnailImage_PreviewMouseDown;
                m_ThubmnailImage.PreviewMouseMove -= ThubmnailImage_PreviewMouseMove;
                m_ThubmnailImage.PreviewMouseLeftButtonUp -= ThubmnailImage_PreviewMouseLeftButtonUp;

                m_PanningThumb.DragDelta -= PanningThumb_DragDelta;
            }
        }

        private void CreateVisualChild()
        {
            m_RenderTarget = new BackgroundRenderTarget()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
            m_ThubmnailImage = new Image();
            m_PanningThumb = new Thumb();
            m_RootLayout = new Grid();
            m_PanningThumbContainer = new Canvas();

            m_PanningThumb.Background = Brushes.Transparent;
            m_PanningThumb.BorderBrush = Brushes.Red;
            m_PanningThumb.BorderThickness = new Thickness(1);
            var template = "<ControlTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' TargetType=\"Thumb\">" +
                "<Border Background = \"Transparent\" BorderBrush = \"Red\" BorderThickness = \"1\" /></ControlTemplate>";
            m_PanningThumb.Template = (ControlTemplate)XamlReader.Parse(template);

            RenderOptions.SetBitmapScalingMode(m_ThubmnailImage, BitmapScalingMode.Linear);

            m_PanningThumbContainer.Children.Add(m_PanningThumb);
            m_PanningThumbContainer.Visibility = Visibility.Collapsed;

            m_RootLayout.Children.Add(RenderTarget);
            m_RootLayout.Children.Add(m_ThubmnailImage);
            m_RootLayout.Children.Add(m_PanningThumbContainer);

            var binding = new Binding("Margin")
            {
                Source = m_ThubmnailImage
            };
            m_PanningThumbContainer.SetBinding(MarginProperty, binding);
            binding = new Binding("Width")
            {
                Source = m_ThubmnailImage
            };
            m_PanningThumbContainer.SetBinding(WidthProperty, binding);
            binding = new Binding("Height")
            {
                Source = m_ThubmnailImage
            };
            m_PanningThumbContainer.SetBinding(HeightProperty, binding);

            m_ThubmnailImage.HorizontalAlignment = HorizontalAlignment.Right;
            m_ThubmnailImage.VerticalAlignment = VerticalAlignment.Bottom;
            m_PanningThumbContainer.VerticalAlignment = VerticalAlignment.Bottom;
            m_PanningThumbContainer.HorizontalAlignment = HorizontalAlignment.Right;

            m_RenderingViewport = new ScrollViewer()
            {
                Focusable = false,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                VerticalScrollBarVisibility = ScrollBarVisibility.Disabled,
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                Content = m_RootLayout
            };
            AddVisualChild(m_RenderingViewport);
        }

        private void PanningThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var fromMatrix = m_LastMatrix;
            var toMatrix = fromMatrix;

            var offsetx = Canvas.GetLeft(m_PanningThumb);
            var offsety = Canvas.GetTop(m_PanningThumb);

            double horizontalChange = e.HorizontalChange;
            double verticalChange = e.VerticalChange;

            if (!EnablePanningAnyWhere)
            {
                if (e.HorizontalChange < 0)
                {
                    horizontalChange = Math.Max(e.HorizontalChange, -offsetx);
                }
                else
                {
                    horizontalChange = Math.Min(e.HorizontalChange, m_ThubmnailImage.RenderSize.Width - m_PanningThumb.RenderSize.Width - offsetx);
                }

                if (e.VerticalChange < 0)
                {
                    verticalChange = Math.Max(e.VerticalChange, -offsety);
                }
                else
                {
                    verticalChange = Math.Min(e.VerticalChange, m_ThubmnailImage.RenderSize.Height - m_PanningThumb.RenderSize.Height - offsety);
                }
            }

            var offset = new Point(horizontalChange / ThumbnailViewRatio, verticalChange / ThumbnailViewRatio);
            toMatrix.OffsetX -= offset.X;
            toMatrix.OffsetY -= offset.Y;

            DoPanning(fromMatrix, toMatrix);
            m_LastMatrix = toMatrix;
        }

        /// <summary>
        ///  通过使用指定的信息作为最终事件数据的一部分来引发 <see cref="FrameworkElement.SizeChanged"/> 事件。
        /// </summary>
        /// <param name="sizeInfo">更改中涉及的新旧大小的详细信息。</param>
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            double controlAspectRatio = ActualWidth / ActualHeight;
            double videoAspectRatio = (double)NaturalVideoWidth / NaturalVideoHeight;
            var renderSize = new Size(ActualWidth, ActualHeight);
            if (controlAspectRatio < videoAspectRatio)
            {
                //横向完全填充
                renderSize = new Size(ActualWidth, ActualWidth / videoAspectRatio);
            }
            else if (controlAspectRatio > videoAspectRatio)
            {
                //纵向完全填充
                renderSize = new Size(ActualHeight * videoAspectRatio, ActualHeight);
            }

            m_RenderTarget.Setup(renderSize);
            var matrix = ((MatrixTransform)m_RenderTarget.RenderTransform).Matrix;
            UpdateThumbnailViewSizeAndOffset(matrix);

            base.OnRenderSizeChanged(sizeInfo);
        }

        private void UpdateThumbnailViewSizeAndOffset(Matrix matrix)
        {
            Point thumbOffset = CalculateThumbOffset(matrix);

            Canvas.SetLeft(m_PanningThumb, thumbOffset.X);
            Canvas.SetTop(m_PanningThumb, thumbOffset.Y);
        }

        private Point CalculateThumbOffset(Matrix matrix)
        {
            var diffWidth = RenderSize.Width - RenderTarget.RenderSize.Width;
            var diffHeight = RenderSize.Height - RenderTarget.RenderSize.Height;

            m_PanningThumb.Width = RenderSize.Width / matrix.M11 * ThumbnailViewRatio;
            m_PanningThumb.Height = RenderSize.Height / matrix.M22 * ThumbnailViewRatio;

            var contentOffsetPoint = new Point((-matrix.OffsetX - diffWidth / 2) / matrix.M11, (-matrix.OffsetY - diffHeight / 2) / matrix.M22);
            var thumbOffset = new Point(contentOffsetPoint.X * ThumbnailViewRatio, contentOffsetPoint.Y * ThumbnailViewRatio);

            return thumbOffset;
        }

        private Point m_OriThumbViewlPoint;
        private bool m_IsMovingThumbView;
        private Thickness m_CurMarginOfThumbView;

        private void ThubmnailImage_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Released)
            {
                return;
            }
            m_OriThumbViewlPoint = new Point();
            m_IsMovingThumbView = false;
        }

        private void ThubmnailImage_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                return;
            }

            if (m_IsMovingThumbView)
            {
                var curThumbViewPoint = e.GetPosition(this);
                var offset = m_OriThumbViewlPoint - curThumbViewPoint;

                var right = Math.Min(
                    RenderSize.Width - m_ThubmnailImage.Width,
                    Math.Max(0, m_CurMarginOfThumbView.Right + offset.X));
                var bottom = Math.Min(
                    RenderSize.Height - m_ThubmnailImage.Height,
                    Math.Max(0, m_CurMarginOfThumbView.Bottom + offset.Y));
                m_ThubmnailImage.Margin = new Thickness(0, 0, right, bottom);
            }
        }

        private void ThubmnailImage_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                return;
            }
            m_OriThumbViewlPoint = e.GetPosition(this);
            m_CurMarginOfThumbView = m_ThubmnailImage.Margin;
            m_IsMovingThumbView = true;
        }

        private void InitializeTransform()
        {
            m_RenderTarget.RenderTransform = new MatrixTransform();
        }

        private void BindToTemplateParent()
        {
            Binding binding = new Binding("Stretch")
            {
                Source = this
            };
            BindingOperations.SetBinding((DependencyObject)m_RenderTarget, BackgroundRenderTarget.StretchProperty, binding);
            BindingOperations.SetBinding(m_ThubmnailImage, Image.StretchProperty, binding);

            binding = new Binding("StretchDirection")
            {
                Source = this
            };
            BindingOperations.SetBinding((DependencyObject)m_RenderTarget, BackgroundRenderTarget.StretchDirectionProperty, binding);
            BindingOperations.SetBinding(m_ThubmnailImage, Image.StretchDirectionProperty, binding);
        }

        #region Zooming And Panning Support

        //保留在动画完成之前时最后一次缩放平移参数
        private Matrix m_LastMatrix;
        private int m_RunningAnimationCount;
        private bool m_Panning;
        private Point m_PanningStartMousePoint;
        private Point m_PanningLastMousePoint;
        private Point m_PanningOriginPoint;

        private void Zoom(double oldZoomFactor, double curZoomFactor)
        {
            bool mouseWheeling = IsMouseOverRenderAera();
            var centerX = RenderTarget.ActualWidth / 2;
            var centerY = RenderTarget.ActualHeight / 2;
            var scale = curZoomFactor / oldZoomFactor;

            if (mouseWheeling)
            {
                centerX = Mouse.GetPosition((UIElement)m_RenderTarget).X;
                centerY = Mouse.GetPosition((UIElement)m_RenderTarget).Y;
            }

            var fromMatrix = m_LastMatrix;
            var toMatrix = fromMatrix;
            toMatrix.ScaleAtPrepend(scale, scale, centerX, centerY);
            if (curZoomFactor <= MinZoomFactor)
            {
                toMatrix.OffsetX = 0;
                toMatrix.OffsetY = 0;
                toMatrix.M11 = MinZoomFactor;
                toMatrix.M22 = MinZoomFactor;
                SetCurrentValue(EnableThumbnailViewProperty, false);
            }
            else
            {
                SetCurrentValue(EnableThumbnailViewProperty, true);
            }

            DoZooming(fromMatrix, toMatrix);

            m_LastMatrix = toMatrix;
        }

        private bool IsMouseOverRenderAera()
        {
            var mousePos = Mouse.GetPosition(this);
            var offsetX = (RenderSize.Width - RenderTarget.RenderSize.Width) / 2;
            var offsetY = (RenderSize.Height - RenderTarget.RenderSize.Height) / 2;
            var renderArea = new Rect(
                offsetX + m_LastMatrix.OffsetX,
                offsetY + m_LastMatrix.OffsetY,
                RenderTarget.RenderSize.Width * m_LastMatrix.M11,
                RenderTarget.RenderSize.Height * m_LastMatrix.M22);

            return renderArea.Contains(mousePos) && mousePos.X >= 0 && mousePos.Y >= 0;
        }

        private void DoZooming(Matrix fromMatrix, Matrix toMatrix)
        {
            BeginAnimation(MatrixProperty, null);
            if (!EnableZoomingAnimation)
            {
                SetCurrentValue(MatrixProperty, toMatrix);
            }
            else
            {
                MatrixAnimation animation = new MatrixAnimation(
                    fromMatrix,
                    toMatrix,
                    TimeSpan.FromMilliseconds(ZoomAnimationDuration),
                    FillBehavior.HoldEnd);
                m_RunningAnimationCount++;
                animation.Completed += Animation_Completed;
                BeginAnimation(MatrixProperty, animation, HandoffBehavior.Compose);
            }
        }

        private void DoPanning(Matrix fromMatrix, Matrix toMatrix)
        {
            BeginAnimation(MatrixProperty, null);
            if (!EnablePanningAnimation)
            {
                SetCurrentValue(MatrixProperty, toMatrix);
            }
            else
            {
                MatrixAnimation animation = new MatrixAnimation(
                    fromMatrix,
                    toMatrix,
                    TimeSpan.FromMilliseconds(PanningAnimationDuration),
                    FillBehavior.HoldEnd);
                m_RunningAnimationCount++;
                animation.Completed += Animation_Completed;
                BeginAnimation(MatrixProperty, animation, HandoffBehavior.SnapshotAndReplace);
            }
        }

        private void Animation_Completed(object sender, EventArgs e)
        {
            m_RunningAnimationCount--;
            if (m_RunningAnimationCount == 0)
            {
                SetCurrentValue(MatrixProperty, m_LastMatrix);
            }
        }

        private void RenderingImage_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Released)
            {
                return;
            }

            if (m_Panning)
            {
                m_Panning = false;
                m_PanningStartMousePoint = new Point(0, 0);
            }
        }

        private void RenderingImage_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                return;
            }

            var mousePos = e.GetPosition(this);

            if (m_Panning)
            {
                Point curContentMousePoint = mousePos;
                Vector dragOffsetSinceStart = m_PanningStartMousePoint - curContentMousePoint;
                var fromMatrix = m_LastMatrix;
                var toMatrix = fromMatrix;
                var x = m_PanningOriginPoint.X - dragOffsetSinceStart.X;
                var y = m_PanningOriginPoint.Y - dragOffsetSinceStart.Y;
                toMatrix.OffsetX = x;
                toMatrix.OffsetY = y;
                DoPanning(fromMatrix, toMatrix);
                m_LastMatrix = toMatrix;
            }
            m_PanningLastMousePoint = mousePos;
        }

        private void RenderingImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                return;
            }
            var mousePos = e.GetPosition(this);
            if (ZoomFactor > MinZoomFactor)
            {
                m_Panning = true;
                m_PanningStartMousePoint = mousePos;
                m_PanningLastMousePoint = mousePos;
                var transform = m_RenderTarget.RenderTransform as MatrixTransform;
                var matrix = transform.Matrix;
                m_PanningOriginPoint = new Point(matrix.OffsetX, matrix.OffsetY);
            }
        }

        #endregion
    }
}
