using Leen.Media.Renderer;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

namespace Leen.Media.Controls.Primitives
{
    class DirectRenderTarget : FrameworkElement, IRenderTarget
    {
        private readonly Image _renderTarget;
        private Int32Rect _renderSourceRect;
        private readonly D3DImage _backgroundRenderSource;
        private D3DImage _foregroundRenderSource;

        /// <summary>
        /// DependencyProperty for Stretch property.
        /// </summary>
        /// <seealso cref="Viewbox.Stretch" />
        public static readonly DependencyProperty StretchProperty =
                Viewbox.StretchProperty.AddOwner(typeof(DirectRenderTarget));

        /// <summary>
        /// DependencyProperty for StretchDirection property.
        /// </summary>
        /// <seealso cref="Viewbox.Stretch" />
        public static readonly DependencyProperty StretchDirectionProperty =
                Viewbox.StretchDirectionProperty.AddOwner(typeof(DirectRenderTarget));

        /// <summary>
        /// Gets/Sets the Stretch on this Image.
        /// The Stretch property determines how large the Image will be drawn.
        /// </summary>
        /// <seealso cref="Viewbox.StretchProperty" />
        public Stretch Stretch
        {
            get { return (Stretch)GetValue(StretchProperty); }
            set { SetValue(StretchProperty, value); }
        }

        /// <summary>
        /// Gets/Sets the stretch direction of the Viewbox, which determines the restrictions on
        /// scaling that are applied to the content inside the Viewbox.  For instance, this property
        /// can be used to prevent the content from being smaller than its native size or larger than
        /// its native size.
        /// </summary>
        /// <seealso cref="Viewbox.StretchDirectionProperty" />
        public StretchDirection StretchDirection
        {
            get { return (StretchDirection)GetValue(StretchDirectionProperty); }
            set { SetValue(StretchDirectionProperty, value); }
        }

        static DirectRenderTarget()
        {
            //
            // The Stretch & StretchDirection properties are AddOwner'ed from a class which is not
            // base class for Image so the metadata with flags get lost. We need to override them
            // here to make it work again.
            //
            StretchProperty.OverrideMetadata(
                typeof(DirectRenderTarget),
                new FrameworkPropertyMetadata(
                    Stretch.Uniform,
                    FrameworkPropertyMetadataOptions.AffectsMeasure
                    )
                );

            StretchDirectionProperty.OverrideMetadata(
                typeof(DirectRenderTarget),
                new FrameworkPropertyMetadata(
                    StretchDirection.Both,
                    FrameworkPropertyMetadataOptions.AffectsMeasure
                    )
                );
        }

        public DirectRenderTarget()
        {
            _backgroundRenderSource = new D3DImage();
            _backgroundRenderSource.IsFrontBufferAvailableChanged += BackgroundImageSource_IsFrontBufferAvailableChanged;
            _renderTarget = new Image()
            {
                Source = _backgroundRenderSource,
                Stretch = Stretch,
                StretchDirection = StretchDirection,
            };
            RenderOptions.SetBitmapScalingMode(_renderTarget, BitmapScalingMode.HighQuality);
            AddVisualChild(_renderTarget);
        }

        public void Initialize(IMediaPlayer player, IVideoRenderer renderer)
        {
            Player = player;
            Renderer = renderer;
            renderer.BackBufferRefreshed += Renderer_BackBufferRefreshed;
            renderer.IsBackBufferAvailableChanged += Renderer_IsBackBufferAvailableChanged;
        }

        /// <summary>
        /// 获取此元素的子视图数量。
        /// </summary>
        protected override int VisualChildrenCount
        {
            get
            {
                return 1;
            }
        }

        protected IMediaPlayer Player { get; set; }

        protected IVideoRenderer Renderer { get; set; }

        public D3DImage ForegroundRenderSource
        {
            get
            {
                if (_foregroundRenderSource == null)
                {
                    _foregroundRenderSource = new D3DImage();
                    _foregroundRenderSource.IsFrontBufferAvailableChanged += ForegroundImageSource_IsFrontBufferAvailableChanged;
                    SetForegroundSourceBackBuffer();
                    InvalidateForegroundImage();
                }

                return _foregroundRenderSource;
            }
            set
            {
                if (_foregroundRenderSource != value)
                {
                    if (_foregroundRenderSource != null)
                    {
                        _foregroundRenderSource.IsFrontBufferAvailableChanged -= ForegroundImageSource_IsFrontBufferAvailableChanged;
                    }
                    _foregroundRenderSource = value;
                }
            }
        }

        public void Clear()
        {
            ClearRenderTarget();
        }

        public void Setup(Size renderSize)
        {
            SetupRenderTarget();
        }

        public void ClearRenderTarget()
        {
            if (_renderTarget == null)
            {
                return;
            }

            _renderTarget.Source = null;
        }

        public void SetupRenderTarget()
        {
            if (_renderTarget == null)
            {
                return;
            }

            _renderTarget.Source = _backgroundRenderSource;
        }

        /// <summary>
        /// 获取指定索引处的视图对象。
        /// </summary>
        /// <param name="index">要获取的视图对象的索引。</param>
        /// <returns>返回指定索引处的视图对象。</returns>
        protected override Visual GetVisualChild(int index)
        {
            if (index == 0)
            {
                return _renderTarget;
            }

            throw new IndexOutOfRangeException(nameof(index));
        }

        /// <summary>
        /// 在布局系统进行测量时调用。
        /// </summary>
        /// <param name="constraint">布局系统测定的元素大小。</param>
        /// <returns>返回此元素测量完成后计算出的需求大小。</returns>
        protected override Size MeasureOverride(Size constraint)
        {
            var uiSize = new Size();
            _renderTarget.Measure(constraint);
            uiSize.Width = _renderTarget.DesiredSize.Width;
            uiSize.Height = _renderTarget.DesiredSize.Height;
            return uiSize;
        }

        /// <summary>
        /// 在布局系统进行排列定位时调用。
        /// </summary>
        /// <param name="finalSize">布局系统在测量完成后得到的控件最终大小。</param>
        /// <returns></returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            _renderTarget.Arrange(new Rect(finalSize));
            return base.ArrangeOverride(finalSize);
        }

        private void Renderer_BackBufferRefreshed(object sender, EventArgs e)
        {
            InvalidateBackgroundImage();
            InvalidateForegroundImage();
        }

        private void Renderer_IsBackBufferAvailableChanged(object sender, EventArgs e)
        {
            SetBackgroundSourceBackBuffer();
            SetForegroundSourceBackBuffer();
        }

        private void BackgroundImageSource_IsFrontBufferAvailableChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_backgroundRenderSource.IsFrontBufferAvailable)
            {
                SetImageSourceBackBuffer(_backgroundRenderSource, Renderer.BackBuffer);
            }
        }

        private void ForegroundImageSource_IsFrontBufferAvailableChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_foregroundRenderSource.IsFrontBufferAvailable)
            {
                SetImageSourceBackBuffer(_foregroundRenderSource, Renderer.BackBuffer);
            }
        }

        private void SetForegroundSourceBackBuffer()
        {
            if (_foregroundRenderSource == null)
            {
                return;
            }
            if (!_foregroundRenderSource.Dispatcher.CheckAccess())
            {
                _foregroundRenderSource.Dispatcher.Invoke((Action)(() => SetForegroundSourceBackBuffer()));
                return;
            }

            SetImageSourceBackBuffer(_foregroundRenderSource, Renderer.BackBuffer);
        }

        private void SetBackgroundSourceBackBuffer()
        {
            if (_backgroundRenderSource == null)
            {
                return;
            }

            if (!_backgroundRenderSource.Dispatcher.CheckAccess())
            {
                _backgroundRenderSource.Dispatcher.Invoke((Action)(() => SetBackgroundSourceBackBuffer()));
                return;
            }

            SetImageSourceBackBuffer(_backgroundRenderSource, Renderer.BackBuffer);
            _renderSourceRect = new Int32Rect(0, 0, _backgroundRenderSource.PixelWidth, _backgroundRenderSource.PixelHeight);
        }

        private void InvalidateBackgroundImage()
        {
            if (!_backgroundRenderSource.Dispatcher.CheckAccess())
            {
                _backgroundRenderSource.Dispatcher.BeginInvoke(new Action(() => InvalidateBackgroundImage()), DispatcherPriority.Render);
                return;
            }

            if (!_backgroundRenderSource.IsFrontBufferAvailable || Renderer.BackBuffer == IntPtr.Zero)
            {
                return;
            }

            InvalidateImage(_backgroundRenderSource);
        }

        private void InvalidateForegroundImage()
        {
            if (_foregroundRenderSource == null)
            {
                return;
            }
            if (!_foregroundRenderSource.Dispatcher.CheckAccess())
            {
                _foregroundRenderSource.Dispatcher.BeginInvoke(new Action(() => InvalidateForegroundImage()), DispatcherPriority.Render);
                return;
            }

            if (!_foregroundRenderSource.IsFrontBufferAvailable || Renderer.BackBuffer == IntPtr.Zero)
            {
                return;
            }

            InvalidateImage(_foregroundRenderSource);
        }

        private void SetImageSourceBackBuffer(D3DImage imageSource, IntPtr backBuffer)
        {
            imageSource.Lock();
            imageSource.SetBackBuffer(D3DResourceType.IDirect3DSurface9, backBuffer);
            imageSource.Unlock();
        }

        private void InvalidateImage(D3DImage imageSource)
        {
            imageSource.Lock();
            imageSource.AddDirtyRect(_renderSourceRect);
            imageSource.Unlock();
        }
    }
}
