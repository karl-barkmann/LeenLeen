using Leen.Media.Renderer;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using System.Threading.Tasks;
using Leen.Windows.Controls;

namespace Leen.Media.Controls.Primitives
{
    class BackgroundRenderTarget : UIThreadHostControl, IRenderTarget
    {
        private Image _renderTarget;
        private Int32Rect _renderSourceRect;
        private D3DImage _backgroundRenderSource;
        private D3DImage _foregroundRenderSource;

        /// <summary>
        /// DependencyProperty for Stretch property.
        /// </summary>
        /// <seealso cref="Viewbox.Stretch" />
        public static readonly DependencyProperty StretchProperty =
                Viewbox.StretchProperty.AddOwner(typeof(BackgroundRenderTarget));

        /// <summary>
        /// DependencyProperty for StretchDirection property.
        /// </summary>
        /// <seealso cref="Viewbox.Stretch" />
        public static readonly DependencyProperty StretchDirectionProperty =
                Viewbox.StretchDirectionProperty.AddOwner(typeof(BackgroundRenderTarget));

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

        static BackgroundRenderTarget()
        {
            //
            // The Stretch & StretchDirection properties are AddOwner'ed from a class which is not
            // base class for Image so the metadata with flags get lost. We need to override them
            // here to make it work again.
            //
            StretchProperty.OverrideMetadata(
                typeof(BackgroundRenderTarget),
                new FrameworkPropertyMetadata(
                    Stretch.Uniform,
                    FrameworkPropertyMetadataOptions.AffectsMeasure
                    )
                );

            StretchDirectionProperty.OverrideMetadata(
                typeof(BackgroundRenderTarget),
                new FrameworkPropertyMetadata(
                    StretchDirection.Both,
                    FrameworkPropertyMetadataOptions.AffectsMeasure
                    )
                );
        }

        public BackgroundRenderTarget()
        {

        }

        public void Initialize(IMediaPlayer player, IVideoRenderer renderer)
        {
            Player = player;
            Renderer = renderer;
            renderer.BackBufferRefreshed += Renderer_BackBufferRefreshed;
            renderer.IsBackBufferAvailableChanged += Renderer_IsBackBufferAvailableChanged;
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
            Width = renderSize.Width;
            Height = renderSize.Height;
            if (TargetDispatcher != null)
            {
                TargetDispatcher.BeginInvoke(() =>
                {
                    _renderTarget.Width = renderSize.Width;
                    _renderTarget.Height = renderSize.Height;
                }, DispatcherPriority.Background);
                SetupRenderTarget();
            }
        }

        protected override FrameworkElement CreateUIThreadControl()
        {
            _backgroundRenderSource = new D3DImage();
            _backgroundRenderSource.IsFrontBufferAvailableChanged += BackgroundImageSource_IsFrontBufferAvailableChanged;
            SetBackgroundSourceBackBuffer();

            Stretch stretch = Stretch.None;
            StretchDirection stretchDirection = StretchDirection.Both;
            Size renderSize = Size.Empty;
            Dispatcher.Invoke(() =>
            {
                renderSize = new Size(Width, Height);
                stretch = Stretch;
                stretchDirection = StretchDirection;
            }, DispatcherPriority.Send);

            _renderTarget = new Image()
            {
                Source = _backgroundRenderSource,
                Stretch = stretch,
                StretchDirection = stretchDirection,
                Width = renderSize.Width,
                Height = renderSize.Height,
            };

            RenderOptions.SetBitmapScalingMode(_renderTarget, BitmapScalingMode.HighQuality);
            return _renderTarget;
        }

        private void ClearRenderTarget()
        {
            if (TargetDispatcher == null)
            {
                return;
            }

            TargetDispatcher.BeginInvoke(() =>
            {
                _renderTarget.Source = null;
            }, DispatcherPriority.Background);
        }

        private void SetupRenderTarget()
        {
            if (TargetDispatcher == null)
            {
                return;
            }

            TargetDispatcher.BeginInvoke(() =>
            {
                _renderTarget.Source = _backgroundRenderSource;
            }, DispatcherPriority.Background);
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

        private void Renderer_IsBackBufferAvailableChanged(object sender, EventArgs e)
        {
            SetBackgroundSourceBackBuffer();
            SetForegroundSourceBackBuffer();
        }

        private void Renderer_BackBufferRefreshed(object sender, EventArgs e)
        {
            InvalidateBackgroundImage();
            InvalidateForegroundImage();
        }

        private void SetForegroundSourceBackBuffer()
        {
            if (_foregroundRenderSource == null || Renderer == null)
            {
                return;
            }
            if (!_foregroundRenderSource.Dispatcher.CheckAccess())
            {
                _foregroundRenderSource.Dispatcher.Invoke(() => SetForegroundSourceBackBuffer());
                return;
            }

            SetImageSourceBackBuffer(_foregroundRenderSource, Renderer.BackBuffer);
        }

        private void SetBackgroundSourceBackBuffer()
        {
            if (_backgroundRenderSource == null || Renderer == null)
            {
                return;
            }

            if (_backgroundRenderSource.Dispatcher.HasShutdownStarted)
            {
                return;
            }

            if (!_backgroundRenderSource.Dispatcher.CheckAccess())
            {
                try
                {
                    _backgroundRenderSource.Dispatcher.Invoke(() => SetBackgroundSourceBackBuffer());
                }
                catch(TaskCanceledException)
                {
                    //Dispathcer has been shutdown before invoke completed.
                }
                return;
            }

            SetImageSourceBackBuffer(_backgroundRenderSource, Renderer.BackBuffer);
            _renderSourceRect = new Int32Rect(0, 0, _backgroundRenderSource.PixelWidth, _backgroundRenderSource.PixelHeight);
        }

        private void InvalidateBackgroundImage()
        {
            if (_backgroundRenderSource == null)
            {
                return;
            }

            if (_backgroundRenderSource.Dispatcher.HasShutdownStarted)
            {
                return;
            }

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
