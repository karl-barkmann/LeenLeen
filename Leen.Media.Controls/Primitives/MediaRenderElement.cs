using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Leen.Media.Presenter;
using Leen.Media.Renderer;
using Leen.Windows;

namespace Leen.Media.Controls.Primitives
{
	/// <summary>
	/// 定义客户端产品中的媒体播放控件，提供一组用于媒体渲染的基础属性。
	/// </summary>
	public abstract class MediaRenderElement : FrameworkElement, IDisposable
	{
		#region HasVideo

		/// <summary>
		/// Read-only dependency property key for <see cref="HasVideo"/> property.
		/// </summary>
		private static readonly DependencyPropertyKey HasVideoPropertyKey
			= DependencyProperty.RegisterReadOnly(
									nameof(HasVideo),
									typeof(bool),
									typeof(MediaRenderElement),
									new FrameworkPropertyMetadata(false));

		/// <summary>
		/// Dependency property for <see cref="HasVideo"/> property.
		/// </summary>
		public static readonly DependencyProperty HasVideoProperty
			= HasVideoPropertyKey.DependencyProperty;

		/// <summary>
		/// Is true if the media contains renderable video
		/// </summary>
		public bool HasVideo
		{
			get { return (bool)GetValue(HasVideoProperty); }
		}

		/// <summary>
		/// Internal method for setting the read-only <see cref="HasVideo"/>.
		/// </summary>
		protected void SetHasVideo(bool value)
		{
			SetValue(HasVideoPropertyKey, value);
		}

		#endregion

		#region HasAudio

		/// <summary>
		/// Read-only dependency property key for <see cref="HasAudio"/> property.
		/// </summary>
		private static readonly DependencyPropertyKey HasAudioPropertyKey
			= DependencyProperty.RegisterReadOnly(
									nameof(HasAudio),
									typeof(bool),
									typeof(MediaRenderElement),
									new FrameworkPropertyMetadata(false));

		/// <summary>
		/// Dependency property for <see cref="HasAudio"/> property.
		/// </summary>
		public static readonly DependencyProperty HasAudioProperty
			= HasAudioPropertyKey.DependencyProperty;

		/// <summary>
		/// Is true if the media contains renderable audio
		/// </summary>
		public bool HasAudio
		{
			get { return (bool)GetValue(HasVideoProperty); }
		}

		/// <summary>
		/// Internal method for setting the read-only <see cref="HasAudio"/>.
		/// </summary>
		protected void SetHasAudio(bool value)
		{
			SetValue(HasAudioPropertyKey, value);
		}

		#endregion

		#region NaturalVideoHeight

		/// <summary>
		/// Read-only dependency property key for <see cref="NaturalVideoHeight"/> property.
		/// </summary>
		private static readonly DependencyPropertyKey NaturalVideoHeightPropertyKey
			= DependencyProperty.RegisterReadOnly(
									nameof(NaturalVideoHeight),
									typeof(int),
									typeof(MediaRenderElement),
									new FrameworkPropertyMetadata(0));

		/// <summary>
		/// Dependency property for <see cref="NaturalVideoHeight"/> property.
		/// </summary>
		public static readonly DependencyProperty NaturalVideoHeightProperty
			= NaturalVideoHeightPropertyKey.DependencyProperty;

		/// <summary>
		/// Gets the natural pixel height of the current media.  
		/// The value will be 0 if there is no video in the media.
		/// </summary>
		public int NaturalVideoHeight
		{
			get { return (int)GetValue(NaturalVideoHeightProperty); }
		}

		/// <summary>
		/// Internal method to set the read-only <see cref="NaturalVideoHeight"/>.
		/// </summary>
		protected void SetNaturalVideoHeight(int value)
		{
			SetValue(NaturalVideoHeightPropertyKey, value);
			m_ThreadFreeNatrualVideoHeight = value;
		}

		#endregion

		#region NaturalVideoWidth

		/// <summary>
		/// Read-only dependency property key for <see cref="NaturalVideoWidth"/> property.
		/// </summary>
		private static readonly DependencyPropertyKey NaturalVideoWidthPropertyKey
			= DependencyProperty.RegisterReadOnly(nameof(NaturalVideoWidth), typeof(int), typeof(MediaRenderElement),
				new FrameworkPropertyMetadata(0));

		/// <summary>
		/// Dependency property for <see cref="NaturalVideoWidth"/> property.
		/// </summary>
		public static readonly DependencyProperty NaturalVideoWidthProperty
			= NaturalVideoWidthPropertyKey.DependencyProperty;

		/// <summary>
		/// Gets the natural pixel width of the current media.
		/// The value will be 0 if there is no video in the media.
		/// </summary>
		public int NaturalVideoWidth
		{
			get { return (int)GetValue(NaturalVideoWidthProperty); }
		}

		/// <summary>
		/// Internal method to set the read-only <see cref="NaturalVideoWidth"/>.
		/// </summary>
		protected void SetNaturalVideoWidth(int value)
		{
			SetValue(NaturalVideoWidthPropertyKey, value);
			m_ThreadFreeNatrualVideoWidth = value;
		}

		#endregion

		#region VideoFrameRate

		/// <summary>
		/// Read-only dependency property key for <see cref="VideoFrameRate"/> property.
		/// </summary>
		private static readonly DependencyPropertyKey VideoFrameRatePropertyKey =
			DependencyProperty.RegisterReadOnly(
								nameof(VideoFrameRate),
								typeof(float),
								typeof(MediaRenderElement),
								new PropertyMetadata(0.0F));

		/// <summary>
		/// Dependency property for <see cref="VideoFrameRate"/> property.
		/// </summary>
		public static readonly DependencyProperty VideoFrameRateProperty =
			VideoFrameRatePropertyKey.DependencyProperty;

		/// <summary>
		/// Gets the frame rate of the current media.
		/// </summary>
		public float VideoFrameRate
		{
			get { return (float)GetValue(VideoFrameRateProperty); }
		}

		/// <summary>
		/// Internal method to set the read-only <see cref="VideoFrameRate"/>.
		/// </summary>
		protected void SetVideoFrameRate(float value)
		{
			SetValue(VideoFrameRatePropertyKey, value);
		}

		#endregion

		#region VideoBitRate 

		/// <summary>
		/// Read-only dependency property key for <see cref="VideoBitRate"/> property.
		/// </summary>
		private static readonly DependencyPropertyKey VideoBitRatePropertyKey =
			DependencyProperty.RegisterReadOnly(
								nameof(VideoBitRate),
								typeof(float),
								typeof(MediaRenderElement),
								new PropertyMetadata(0.0F));

		/// <summary>
		/// Dependency property for <see cref="VideoBitRate"/> property.
		/// </summary>
		public static readonly DependencyProperty VideoBitRateProperty =
			VideoBitRatePropertyKey.DependencyProperty;

		/// <summary>
		/// Gets the bit rate of the current media.
		/// </summary>
		public float VideoBitRate
		{
			get { return (float)GetValue(VideoBitRateProperty); }
		}

		/// <summary>
		/// Internal method to set the read-only <see cref="VideoBitRate"/>.
		/// </summary>
		protected void SetVideoBitRate(float value)
		{
			SetValue(VideoBitRatePropertyKey, value);
		}

		#endregion

		#region RenderingVideoHeight

		/// <summary>
		/// Read-only dependency property key for <see cref="RenderingVideoHeight"/> property.
		/// </summary>
		private static readonly DependencyPropertyKey RenderingVideoHeightPropertyKey
			= DependencyProperty.RegisterReadOnly(
									nameof(RenderingVideoHeight),
									typeof(double),
									typeof(MediaRenderElement),
									new FrameworkPropertyMetadata(0.0d));

		/// <summary>
		/// Dependency property for <see cref="RenderingVideoHeight"/> property.
		/// </summary>
		public static readonly DependencyProperty RenderingVideoHeightProperty
			= RenderingVideoHeightPropertyKey.DependencyProperty;

		/// <summary>
		/// Gets the rendering video aera height of the current media.  
		/// The value will be 0 if there is no video in the media.
		/// </summary>
		public double RenderingVideoHeight
		{
			get { return (double)GetValue(RenderingVideoHeightProperty); }
		}

		/// <summary>
		/// Internal method to set the read-only <see cref="RenderingVideoHeight"/>.
		/// </summary>
		protected void SetRenderingVideoHeight(double value)
		{
			SetValue(RenderingVideoHeightPropertyKey, value);
			m_ThreadFreeRenderVideoHeight = value;
		}

		#endregion

		#region RenderingVideoWidth

		/// <summary>
		/// Read-only dependency property key for <see cref="RenderingVideoWidth"/> property.
		/// </summary>
		private static readonly DependencyPropertyKey RenderingVideoWidthPropertyKey
			= DependencyProperty.RegisterReadOnly(
									nameof(RenderingVideoWidth),
									typeof(double),
									typeof(MediaRenderElement),
									new FrameworkPropertyMetadata(0.0d));

		/// <summary>
		/// Dependency property for <see cref="RenderingVideoWidth"/> property.
		/// </summary>
		public static readonly DependencyProperty RenderingVideoWidthProperty
			= RenderingVideoWidthPropertyKey.DependencyProperty;

		/// <summary>
		/// Gets the rendering video aera width of the current media.
		/// The value will be 0 if there is no video in the media.
		/// </summary>
		public double RenderingVideoWidth
		{
			get { return (double)GetValue(RenderingVideoWidthProperty); }
		}

		/// <summary>
		/// Internal method to set the read-only <see cref="RenderingVideoWidth"/>.
		/// </summary>
		protected void SetRenderingVideoWidth(double value)
		{
			SetValue(RenderingVideoWidthPropertyKey, value);
			m_ThreadFreeRenderVideoWidth = value;
		}

		#endregion

		#region Stretch

		/// <summary>
		/// Dependency property for <see cref="Stretch"/> property.
		/// </summary>
		public static readonly DependencyProperty StretchProperty =
			DependencyProperty.Register(
								nameof(Stretch),
								typeof(Stretch),
								typeof(MediaRenderElement),
								new FrameworkPropertyMetadata(Stretch.Uniform,
									new PropertyChangedCallback(OnStretchChanged)));

		/// <summary>
		/// Defines what rules are applied to the stretching of the video content.
		/// </summary>
		public Stretch Stretch
		{
			get { return (Stretch)GetValue(StretchProperty); }
			set { SetValue(StretchProperty, value); }
		}

		private static void OnStretchChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((MediaRenderElement)d).OnStretchChanged(e);
		}

		/// <summary>
		/// Called when <see cref="Stretch"/> changed.
		/// </summary>
		/// <param name="e">Related event args.</param>
		protected virtual void OnStretchChanged(DependencyPropertyChangedEventArgs e)
		{

		}

		#endregion

		#region StretchDirection

		/// <summary>
		/// Dependency property for <see cref="StretchDirection"/> property.
		/// </summary>
		public static readonly DependencyProperty StretchDirectionProperty =
			DependencyProperty.Register(
								nameof(StretchDirection),
								typeof(StretchDirection),
								typeof(MediaRenderElement),
								new FrameworkPropertyMetadata(StretchDirection.Both,
									new PropertyChangedCallback(OnStretchDirectionChanged)));

		/// <summary>
		/// Gets or Sets the value that indicates how the video is scaled.  This is a dependency property.
		/// </summary>
		public StretchDirection StretchDirection
		{
			get { return (StretchDirection)GetValue(StretchDirectionProperty); }
			set { SetValue(StretchDirectionProperty, value); }
		}

		private static void OnStretchDirectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((MediaRenderElement)d).OnStretchDirectionChanged(e);
		}

		/// <summary>
		/// Called when <see cref="StretchDirection"/> changed.
		/// </summary>
		/// <param name="e">Related event args.</param>
		protected virtual void OnStretchDirectionChanged(DependencyPropertyChangedEventArgs e)
		{

		}

		#endregion

		#region IsRenderingEnabled

		/// <summary>
		/// Dependency property for <see cref="IsRenderingEnabled"/> property.
		/// </summary>
		public static readonly DependencyProperty IsRenderingEnabledProperty =
			DependencyProperty.Register(
								nameof(IsRenderingEnabled),
								typeof(bool),
								typeof(MediaRenderElement),
								new FrameworkPropertyMetadata(true));

		/// <summary>
		/// Enables or disables rendering of the video
		/// </summary>
		public bool IsRenderingEnabled
		{
			get { return (bool)GetValue(IsRenderingEnabledProperty); }
			set { SetValue(IsRenderingEnabledProperty, value); }
		}

		private static void OnIsRenderingEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((MediaRenderElement)d).OnIsRenderingEnabledChanged(e);
		}

		/// <summary>
		/// Called when <see cref="IsRenderingEnabled"/> changed.
		/// </summary>
		/// <param name="e">Related event args.</param>
		protected virtual void OnIsRenderingEnabledChanged(DependencyPropertyChangedEventArgs e)
		{
			if (!HasInitialized)
			{
				return;
			}

			MediaPresenter.IsPresenting = (bool)e.NewValue;
		}

		#endregion

		#region UnloadedBehavior

		/// <summary>
		/// Dependency property for <see cref="UnloadedBehavior"/> property.
		/// </summary>
		public static readonly DependencyProperty UnloadedBehaviorProperty =
			DependencyProperty.Register(
								nameof(UnloadedBehavior),
								typeof(MediaState),
								typeof(MediaRenderElement),
								new FrameworkPropertyMetadata(MediaState.Close));

		/// <summary>
		/// Defines the behavior of the control when it is unloaded
		/// </summary>
		public MediaState UnloadedBehavior
		{
			get { return (MediaState)GetValue(UnloadedBehaviorProperty); }
			set { SetValue(UnloadedBehaviorProperty, value); }
		}

		#endregion

		#region LoadedBehavior

		/// <summary>
		/// Dependency property for <see cref="LoadedBehavior"/> property.
		/// </summary>
		public static readonly DependencyProperty LoadedBehaviorProperty =
			DependencyProperty.Register(
								nameof(LoadedBehavior),
								typeof(MediaState),
								typeof(MediaRenderElement),
								new FrameworkPropertyMetadata(MediaState.Play));

		/// <summary>
		/// Defines the behavior of the control when it is loaded
		/// </summary>
		public MediaState LoadedBehavior
		{
			get { return (MediaState)GetValue(LoadedBehaviorProperty); }
			set { SetValue(LoadedBehaviorProperty, value); }
		}

        #endregion

        #region OpenedBehavior

        /// <summary>
        /// Defines the behavior of the control when it's media source is opened.
        /// </summary>
        public MediaState OpenedBehavior
		{
			get { return (MediaState)GetValue(OpenedBehaviorProperty); }
			set { SetValue(OpenedBehaviorProperty, value); }
		}

		/// <summary>
		/// Dependency property for <see cref="OpenedBehavior"/> property.
		/// </summary>
		public static readonly DependencyProperty OpenedBehaviorProperty =
			DependencyProperty.Register(
				nameof(OpenedBehavior),
				typeof(MediaState),
				typeof(MediaRenderElement),
				new PropertyMetadata(MediaState.Manual), OnValidateOpenedBehavior);

		private static bool OnValidateOpenedBehavior(object value)
		{
			var mediaState = (MediaState)value;
			return mediaState == MediaState.Manual ||
				mediaState == MediaState.Pause ||
				mediaState == MediaState.Play;
		}

        #endregion

        #region Source

        /// <summary>
        /// DependencyProperty for <see cref="MediaRenderElement"/> <see cref="Source"/> property.
        /// </summary>
        /// <seealso cref="Source" />
        /// This property is cached (_source).
        public static readonly DependencyProperty SourceProperty =
				DependencyProperty.Register(
						nameof(Source),
						typeof(object),
						typeof(MediaRenderElement),
						new FrameworkPropertyMetadata(
								null,
								new PropertyChangedCallback(OnSourceChanged)));
		/// <summary>
		/// Gets/Sets the source on this <see cref="MediaRenderElement"/>.
		///
		/// The <see cref="Source"/> property is the Uri of the media to be played.
		/// </summary>
		public object Source
		{
			get { return GetValue(SourceProperty); }
			set { SetValue(SourceProperty, value); }
		}

		private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var element = d as MediaRenderElement;
			if (element.m_IgnoreSourceChangedCallback)
			{
				element.m_IgnoreSourceChangedCallback = false;
				return;
			}

			element.OnSourceChanged(e);
		}

		/// <summary>
		/// Called when <see cref="Source"/> changed.
		/// </summary>
		/// <param name="e">Related event args.</param>
		protected virtual void OnSourceChanged(DependencyPropertyChangedEventArgs e)
		{
			if (PlayState == MediaPlayState.None)
			{
				OpenImpl(e.NewValue);
			}
		}

		#endregion

		#region IsMuted

		/// <summary>
		/// The DependencyProperty for the <see cref="IsMuted"/> property.
		/// <seealso cref="IsMuted" />
		/// </summary>
		public static readonly DependencyProperty IsMutedProperty =
			DependencyProperty.Register(
						nameof(IsMuted),
						typeof(bool),
						typeof(MediaRenderElement),
						new FrameworkPropertyMetadata(
							false,
							FrameworkPropertyMetadataOptions.None,
							new PropertyChangedCallback(IsMutedPropertyChanged)));

		/// <summary>
		/// Gets/Sets the IsMuted property on the MediaElement.
		/// </summary>
		public bool IsMuted
		{
			get
			{
				return (bool)GetValue(IsMutedProperty);
			}
			set
			{
				SetValue(IsMutedProperty, value);
			}
		}

		private static void IsMutedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((MediaRenderElement)d).OnIsMutedChanged(e);
		}

		/// <summary>
		/// Called when <see cref="IsMuted"/> changed.
		/// </summary>
		/// <param name="e">Related event args.</param>
		protected virtual void OnIsMutedChanged(DependencyPropertyChangedEventArgs e)
		{
			if (!HasInitialized)
			{
				return;
			}

			MediaPlayer.IsMuted = (bool)e.NewValue;
		}

		#endregion

		#region Volume

		/// <summary>
		/// The DependencyProperty for the <see cref="Volume"/> property.
		/// <seealso cref="Volume" />
		/// </summary>
		public static readonly DependencyProperty VolumeProperty =
			DependencyProperty.Register(
								nameof(Volume),
								typeof(double),
								typeof(MediaRenderElement),
								new FrameworkPropertyMetadata(1.0d,
								new PropertyChangedCallback(OnVolumeChanged)));

		/// <summary>
		/// Gets or sets the audio volume.  Specifies the volume, as a 
		/// number from 0 to 1.  Full volume is 1, and 0 is silence.
		/// </summary>
		public double Volume
		{
			get { return (double)GetValue(VolumeProperty); }
			set { SetValue(VolumeProperty, value); }
		}

		private static void OnVolumeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((MediaRenderElement)d).OnVolumeChanged(e);
		}

		/// <summary>
		/// Called when <see cref="Volume"/> changed.
		/// </summary>
		/// <param name="e">Related event args.</param>
		protected virtual void OnVolumeChanged(DependencyPropertyChangedEventArgs e)
		{
			if (!HasInitialized)
			{
				return;
			}

			MediaPlayer.Volume = (double)e.NewValue;
		}

		#endregion

		#region Balance

		/// <summary>
		/// The DependencyProperty for the <see cref="Balance"/> property.
		/// <seealso cref="Balance" />
		/// </summary>
		public static readonly DependencyProperty BalanceProperty =
			DependencyProperty.Register(
								nameof(Balance),
								typeof(double),
								typeof(MediaRenderElement),
								new FrameworkPropertyMetadata(0d,
								new PropertyChangedCallback(OnBalanceChanged)));

		/// <summary>
		/// Gets or sets the balance on the audio.
		/// The value can range from -1 to 1. The value -1 means the right channel is attenuated by 100 dB 
		/// and is effectively silent. The value 1 means the left channel is silent. The neutral value is 0, 
		/// which means that both channels are at full volume. When one channel is attenuated, the other 
		/// remains at full volume.
		/// </summary>
		public double Balance
		{
			get { return (double)GetValue(BalanceProperty); }
			set { SetValue(BalanceProperty, value); }
		}

		private static void OnBalanceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((MediaRenderElement)d).OnBalanceChanged(e);
		}

		/// <summary>
		/// Called when <see cref="Balance"/> changed.
		/// </summary>
		/// <param name="e">Related event args.</param>
		protected virtual void OnBalanceChanged(DependencyPropertyChangedEventArgs e)
		{
			if (!HasInitialized)
			{
				return;
			}

			MediaPlayer.Balance = (double)e.NewValue;
		}

		#endregion

		#region PlayState

		/// <summary>
		/// The DependencyProperty for the <see cref="PlayState"/> property.
		/// <seealso cref="PlayState" />
		/// </summary>

		public static readonly DependencyProperty PlayStateProperty
			= DependencyProperty.Register(
									nameof(PlayState),
									typeof(MediaPlayState),
									typeof(MediaRenderElement),
									new FrameworkPropertyMetadata(MediaPlayState.None, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, PlayStatePropertyChanged),
									OnValidateState);

		/// <summary>
		/// Gets or sets the play state of the media.
		/// </summary>
		public MediaPlayState PlayState
		{
			get { return (MediaPlayState)GetValue(PlayStateProperty); }
			set { SetValue(PlayStateProperty, value); }
		}

		private static bool OnValidateState(object value)
		{
			var playState = (MediaPlayState)value;
			return playState == MediaPlayState.None ||
				playState == MediaPlayState.Opened ||
				playState == MediaPlayState.Complete ||
				playState == MediaPlayState.Paused ||
				playState == MediaPlayState.Playing ||
				playState == MediaPlayState.Rewinding ||
				playState == MediaPlayState.Stopped;
		}

		private static void PlayStatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var element = d as MediaRenderElement;
			if (element.m_IgnoreStateChangedCallback)
			{
				element.m_IgnoreStateChangedCallback = false;
				return;
			}

			element.OnPlayStateChanged((MediaPlayState)e.OldValue, (MediaPlayState)e.NewValue);
		}

		/// <summary>
		/// Called when <see cref="PlayState"/> changed.
		/// </summary>
		/// <param name="oldState">Old value of <see cref="PlayState"/>.</param>
		/// <param name="newState">New value of <see cref="PlayState"/>.</param>
		protected virtual void OnPlayStateChanged(MediaPlayState oldState, MediaPlayState newState)
		{
			SetPlayerPlayState(PlayState);
		}

		/// <summary>
		/// Internal method for setting the read-only <see cref="PlayState"/> DP
		/// </summary>
		/// <param name="playState">New play state which the media player will be.</param>
		protected void SetPlayerPlayState(MediaPlayState playState)
		{
			if (HasInitialized)
			{
				switch (playState)
				{
					case MediaPlayState.None:
						break;
					case MediaPlayState.Opened:
						break;
					case MediaPlayState.Playing:
						MediaPlayer.Play();
						break;
					case MediaPlayState.Stopped:
						MediaPlayer.Stop();
						break;
					case MediaPlayState.Paused:
						MediaPlayer.Pause();
						break;
					case MediaPlayState.Complete:
						break;
					case MediaPlayState.Rewinding:
						break;
					default:
						break;
				}
			}
		}

		#endregion

		#region CanPause

		/// <summary>
		/// Dependency property key for <see cref="CanPause"/> property.
		/// </summary>
		public static readonly DependencyPropertyKey CanPausePropertyKey
			= DependencyProperty.RegisterReadOnly(
									nameof(CanPause),
									typeof(bool),
									typeof(MediaRenderElement),
									new PropertyMetadata(true));

		/// <summary>
		/// Dependency property for <see cref="CanPause"/> property.
		/// </summary>
		public static readonly DependencyProperty CanPauseProperty
			= CanPausePropertyKey.DependencyProperty;

		/// <summary>
		/// Gets the <see cref="CanPause"/> value.
		/// </summary>
		public bool CanPause
		{
			get { return (bool)GetValue(CanPauseProperty); }
		}

		/// <summary>
		/// Internal method for setting the read-only <see cref="CanPause"/> DP
		/// </summary>
		protected void SetCanPause(bool value)
		{
			SetValue(CanPausePropertyKey, value);
		}

		#endregion

		#region Fields

		Window m_CurrentWindow;
		bool m_WindowHooked;
		bool m_IgnoreStateChangedCallback;
		bool m_IgnoreSourceChangedCallback;
		int m_ThreadFreeNatrualVideoHeight;
		int m_ThreadFreeNatrualVideoWidth;
		double m_ThreadFreeRenderVideoWidth;
		double m_ThreadFreeRenderVideoHeight;

		#endregion

		#region Constructors

		static MediaRenderElement()
		{
			InitializeCommands();
		}

		/// <summary>
		/// 构造 <see cref="MediaRenderElement"/> 类的实例。
		/// </summary>
		protected MediaRenderElement() : base()
		{
			if (!DesignerProperties.GetIsInDesignMode(this))
			{
				InitializeMediaPresenterImpl();
				Loaded += MediaRenderElement_Loaded;
				Unloaded += MediaRenderElement_Unloaded;
			}
		}

		#endregion

		/// <summary>
		/// 非线程关联的 <see cref="NaturalVideoHeight"/> 属性。
		/// </summary>
		protected int ThreadFreeNatrualVideoHeight
		{
			get
			{
				return m_ThreadFreeNatrualVideoHeight;
			}
		}

		/// <summary>
		/// 非线程关联的 <see cref="NaturalVideoWidth"/> 属性。
		/// </summary>
		protected int ThreadFreeNatrualVideoWidth
		{
			get { return m_ThreadFreeNatrualVideoWidth; }
		}

		/// <summary>
		/// 非线程关联的 <see cref="RenderingVideoHeight"/> 属性。
		/// </summary>
		protected double ThreadFreeRenderVideoHeight
		{
			get
			{
				return m_ThreadFreeRenderVideoHeight;
			}
		}

		/// <summary>
		/// 非线程关联的 <see cref="RenderingVideoWidth"/> 属性。
		/// </summary>
		protected double ThreadFreeRenderVideoWidth
		{
			get { return m_ThreadFreeRenderVideoWidth; }
		}


		#region Routed Events

		/// <summary>
		/// MediaFailedEvent is a routed event.
		/// </summary>
		public static readonly RoutedEvent MediaFailedEvent =
			EventManager.RegisterRoutedEvent(
							nameof(MediaFailed),
							RoutingStrategy.Bubble,
							typeof(EventHandler<MediaFailedRoutedEventArgs>),
							typeof(MediaRenderElement));
		/// <summary>
		/// Raised when there is a failure in media.
		/// </summary>
		public event EventHandler<MediaFailedRoutedEventArgs> MediaFailed
		{
			add { AddHandler(MediaFailedEvent, value); }
			remove { RemoveHandler(MediaFailedEvent, value); }
		}

		/// <summary>
		/// MediaOpened is a routed event.
		/// </summary>
		public static readonly RoutedEvent MediaOpenedEvent =
			EventManager.RegisterRoutedEvent(
							nameof(MediaOpened),
							RoutingStrategy.Bubble,
							typeof(RoutedEventHandler),
							typeof(MediaRenderElement));

		/// <summary>
		/// Raised when the media is opened
		/// </summary>
		public event RoutedEventHandler MediaOpened
		{
			add { AddHandler(MediaOpenedEvent, value); }
			remove { RemoveHandler(MediaOpenedEvent, value); }
		}

		/// <summary>
		/// MediaEnded is a routed event
		/// </summary>
		public static readonly RoutedEvent MediaEndedEvent =
			EventManager.RegisterRoutedEvent(
							nameof(MediaEnded),
							RoutingStrategy.Bubble,
							typeof(RoutedEventHandler),
							typeof(MediaRenderElement));

		/// <summary>
		/// Raised when the corresponding media ends (completes playing).
		/// </summary>
		public event RoutedEventHandler MediaEnded
		{
			add { AddHandler(MediaEndedEvent, value); }
			remove { RemoveHandler(MediaEndedEvent, value); }
		}

		/// <summary>
		/// MediaClosed is a routed event.
		/// </summary>
		public static readonly RoutedEvent MediaClosedEvent =
			EventManager.RegisterRoutedEvent(
							nameof(MediaClosed),
							RoutingStrategy.Bubble,
							typeof(RoutedEventHandler),
							typeof(MediaRenderElement));

		/// <summary>
		/// Raised when media is closed
		/// </summary>
		public event RoutedEventHandler MediaClosed
		{
			add { AddHandler(MediaClosedEvent, value); }
			remove { RemoveHandler(MediaClosedEvent, value); }
		}

		/// <summary>
		/// 当媒体播放完成时调用。
		/// </summary>
		protected virtual void OnMediaEnded()
		{
			RaiseEvent(new RoutedEventArgs(MediaEndedEvent));
		}

        /// <summary>
        /// 当发生媒体播放错误时调用。
        /// </summary>
        /// <param name="errorException">描述错误的异常对象。</param>
        protected virtual void OnMediaFailed(Exception errorException)
        {
            RaiseEvent(new MediaFailedRoutedEventArgs(MediaFailedEvent, this, errorException));
        }

        /// <summary>
        /// 当媒体打开时调用。
        /// <para>
        /// 我们通常在媒体打开时获取或设置媒体播放器的音量时长等信息。
        /// 继承类应正确的在媒体打开时获取或设置相应媒体信息。
        /// </para>
        /// </summary>
        protected virtual void OnMediaOpened()
        {
            bool hasVideo = MediaPlayer.HasVideo;
            bool hasAudio = MediaPlayer.HasAudio;
            int videoWidth = MediaPlayer.NaturalVideoWidth;
            int videoHeight = MediaPlayer.NaturalVideoHeight;
            float videoFrameRate = MediaPlayer.FrameRate;
            float videoBitRate = MediaPlayer.BitRate;
            var mediaCaps = MediaPlayer.ControlCaps;
            var canPause = mediaCaps.HasFlag(MediaControlCaps.CanPause) && mediaCaps.HasFlag(MediaControlCaps.CanPlayForwards);

            SetCanPause(canPause);

            SetNaturalVideoWidth(videoWidth);
            SetNaturalVideoHeight(videoHeight);

            SetHasVideo(hasVideo);
            SetHasAudio(hasAudio);

            SetVideoFrameRate(videoFrameRate);
            SetVideoBitRate(videoBitRate);

            MediaPlayer.Volume = Volume;
            MediaPlayer.Balance = Balance;
            MediaPlayer.IsMuted = IsMuted;

            RaiseEvent(new RoutedEventArgs(MediaOpenedEvent));
        }

		/// <summary>
		/// 当媒体关闭时调用。
		/// </summary>
		protected virtual void OnMediaClosed()
		{
			RaiseEvent(new RoutedEventArgs(MediaClosedEvent));
		}

		#endregion

		#region Commands

		private static RoutedUICommand _playerStateCommand;
		private static RoutedUICommand _togglePlayPauseCommand;
		private static RoutedUICommand _takeSnapshotCommand;
		private static RoutedUICommand _takeSnapshotsCommand;

		/// <summary>
		/// 获取用于切换播放器状态的命令。
		/// <para>
		/// 接收一个 <see cref="MediaState"/> 播放器状态值的参数表示要切换到的状态。
		/// </para>
		/// </summary>
		public static RoutedUICommand PlayerStateCommand
		{
			get { return _playerStateCommand; }
		}

		/// <summary>
		/// 获取用于切换播放/暂停的命令。
		/// </summary>
		public static RoutedUICommand TogglePlayPauseCommand
		{
			get { return _togglePlayPauseCommand; }
		}

		/// <summary>
		/// 获取用于生成当前帧图片的命令。
		/// </summary>
		public static RoutedUICommand TakeSnapshotCommand
		{
			get { return _takeSnapshotCommand; }
		}

		/// <summary>
		/// 获取用于从当前播放位置开始连续生成帧图片的命令。
		/// </summary>
		public static RoutedUICommand TakeSnapshotsCommand
		{
			get { return _takeSnapshotsCommand; }
		}

		private static void InitializeCommands()
		{
			_playerStateCommand = new RoutedUICommand();
			_togglePlayPauseCommand = new RoutedUICommand(
				Properties.Resources.STR_Command_TogglePlayPause,
				"TogglePlayPause",
				typeof(MediaRenderElement));
			_takeSnapshotCommand = new RoutedUICommand(
				Properties.Resources.STR_Command_TakeSnapshot,
				"TaskSnapshot",
				typeof(MediaRenderElement));
			_takeSnapshotsCommand = new RoutedUICommand(
				Properties.Resources.STR_Command_TakeSnapshot,
				"TaskSnapshots",
				typeof(MediaRenderElement));

			CommandHelpers.RegisterCommandHandler(
				typeof(MediaRenderElement),
				TogglePlayPauseCommand,
				OnTogglePlayPause,
				OnCanTogglePlayPause);
			CommandHelpers.RegisterCommandHandler(
				typeof(MediaRenderElement),
				PlayerStateCommand,
				OnChanegPlayerState,
				OnCanChangePlayerState);
			CommandHelpers.RegisterCommandHandler(
				typeof(MediaRenderElement),
				TakeSnapshotCommand,
				OnTakeSnapshot,
				OnCanTakeSnapshot);
			CommandHelpers.RegisterCommandHandler(
				typeof(MediaRenderElement),
				TakeSnapshotsCommand,
				OnTakeSnapshots,
				OnCanTakeSnapshots);
		}

		private static void OnCanChangePlayerState(object sender, CanExecuteRoutedEventArgs e)
		{
			MediaRenderElement element = sender as MediaRenderElement;
			e.CanExecute = element.GetCanChangePlayerState(e.Parameter);
		}

		private static void OnChanegPlayerState(object sender, ExecutedRoutedEventArgs e)
		{
			MediaRenderElement element = sender as MediaRenderElement;
			element.ChangePlayerState(e.Parameter);
		}

		private static void OnCanTogglePlayPause(object sender, CanExecuteRoutedEventArgs e)
		{
			MediaRenderElement element = sender as MediaRenderElement;
			e.CanExecute = element.GetCanTogglePlayPause();
		}

		private static void OnTogglePlayPause(object sender, ExecutedRoutedEventArgs e)
		{
			MediaRenderElement element = sender as MediaRenderElement;
			element.TogglePlayPause();
		}

		private static void OnCanTakeSnapshots(object sender, CanExecuteRoutedEventArgs e)
		{
			MediaRenderElement element = sender as MediaRenderElement;
			e.CanExecute = element.GetCanTakeSnapshots();
		}

		private static void OnTakeSnapshots(object sender, ExecutedRoutedEventArgs e)
		{
			MediaRenderElement element = sender as MediaRenderElement;
			element.TakeSnapshots();
		}

		private static void OnCanTakeSnapshot(object sender, CanExecuteRoutedEventArgs e)
		{
			MediaRenderElement element = sender as MediaRenderElement;
			e.CanExecute = element.GetCanTakeSnapshot();
		}

		private static void OnTakeSnapshot(object sender, ExecutedRoutedEventArgs e)
		{
			MediaRenderElement element = sender as MediaRenderElement;
			element.TakeSnapshot();
		}

		/// <summary>
		/// 获取一个值表示是否可以改变播放器状态的内部实现。
		/// </summary>
		/// <param name="parameter">播放状态参数。</param>
		/// <returns></returns>
		protected virtual bool GetCanChangePlayerState(object parameter)
		{
            var validState = Enum.TryParse(parameter.ToString(), out MediaState state);
            if (validState)
			{
				validState = state >= MediaState.Manual && state <= MediaState.Stop;
				if (validState)
				{
					switch (state)
					{
						case MediaState.Manual:
							return true;
						case MediaState.Play:
							return PlayState != MediaPlayState.None && PlayState != MediaPlayState.Playing && 
								PlayState != MediaPlayState.Stopped;
						case MediaState.Close:
							return PlayState != MediaPlayState.None;
						case MediaState.Pause:
							return PlayState != MediaPlayState.None &&
								PlayState != MediaPlayState.Stopped &&
								PlayState != MediaPlayState.Paused;
						case MediaState.Stop:
							return PlayState != MediaPlayState.None &&
								PlayState != MediaPlayState.Stopped;
					}
				}
			}

			return validState;
		}

		/// <summary>
		/// 改变播放器状态的内部实现。
		/// </summary>
		/// <param name="parameter">播放状态参数。</param>
		protected virtual void ChangePlayerState(object parameter)
		{
            Enum.TryParse(parameter.ToString(), out MediaState state);
            ExecuteMediaState(state);
		}

		/// <summary>
		/// 获取一个值表示是否可以切换播放/暂停的内部实现。
		/// </summary>
		/// <returns></returns>
		protected virtual bool GetCanTogglePlayPause()
		{
			return PlayState != MediaPlayState.None && PlayState != MediaPlayState.Stopped;
		}

		/// <summary>
		/// 切换播放/暂停的内部实现。
		/// </summary>
		protected virtual void TogglePlayPause()
		{
			if (PlayState == MediaPlayState.Paused)
			{
				Play();
			}
			else if (PlayState == MediaPlayState.Opened)
			{
				Play();
			}
			else if (PlayState == MediaPlayState.Playing)
			{
				Pause();
			}
			else if (PlayState == MediaPlayState.Rewinding)
			{
				Play();
			}
			else if (PlayState == MediaPlayState.Complete)
			{
				MediaPlayer.Play();
			}
		}

		/// <summary>
		/// 获取一个值表示是否可以进行连续截图。
		/// </summary>
		/// <returns></returns>
		protected virtual bool GetCanTakeSnapshots()
		{
			return PlayState != MediaPlayState.None && PlayState != MediaPlayState.Stopped;
		}

		/// <summary>
		/// 启动连续截图。
		/// </summary>
		protected virtual void TakeSnapshots()
		{

		}

		/// <summary>
		/// 获取一个值指示是否可以进行截图。
		/// </summary>
		/// <returns></returns>
		protected virtual bool GetCanTakeSnapshot()
		{
			return PlayState != MediaPlayState.None && PlayState != MediaPlayState.Stopped;
		}

		/// <summary>
		/// 截取当前帧并生成图片。
		/// </summary>
		protected virtual void TakeSnapshot()
		{

		}

		#endregion

		#region Load/Unload Behaviors

		private void MediaRenderElement_Unloaded(object sender, RoutedEventArgs e)
		{
			OnUnloadedOverride();

			if (Application.Current == null)
				return;

			m_WindowHooked = false;

			if (m_CurrentWindow == null)
				return;

			m_CurrentWindow.Closed -= WindowOwnerClosed;
			m_CurrentWindow = null;
		}

		private void MediaRenderElement_Loaded(object sender, RoutedEventArgs e)
		{
			m_CurrentWindow = Window.GetWindow(this);

			if (m_CurrentWindow != null && !m_WindowHooked)
			{
				m_CurrentWindow.Closed += WindowOwnerClosed;
				m_WindowHooked = true;
			}

			OnLoadedOverride();
		}

		private void WindowOwnerClosed(object sender, EventArgs e)
		{
			ExecuteMediaState(UnloadedBehavior);
		}

		/// <summary>
		/// Runs when the Loaded event is fired and executes
		/// the LoadedBehavior
		/// </summary>
		protected virtual void OnLoadedOverride()
		{
			ExecuteMediaState(LoadedBehavior);
		}

		/// <summary>
		/// Runs when the Unloaded event is fired and executes
		/// the UnloadedBehavior
		/// </summary>
		protected virtual void OnUnloadedOverride()
		{
			ExecuteMediaState(UnloadedBehavior);
		}

		#endregion

		#region IDisposable Support

		/// <summary>
		/// 指示是否已通过<see cref="IDisposable.Dispose"/>进行了资源回收。
		/// </summary>
		protected bool m_Disposed = false;

		/// <summary>
		/// 释放播放资源。
		/// </summary>
		/// <param name="disposing">一个值指示是否正在经由调用 <see cref="IDisposable.Dispose"/> 方法进行资源释放。</param>
		protected virtual void Dispose(bool disposing)
		{
			if (!m_Disposed)
			{
				if (disposing)
				{
					if (MediaPlayer != null)
					{
						MediaPlayer.StateChanged -= MediaPlayer_StateChanged;
						MediaPlayer.MediaFailed -= MediaPlayer_MediaFailed;
						MediaPlayer.FrameAllocated -= MediaPlayer_FrameAllocated;
						if (!MediaPlayer.IsMediaClosed)
						{
							MediaPlayer.Close();
						}
						MediaPlayer.Dispose();
					}

					if (MediaRenderer != null)
					{
						MediaRenderer.Dispose();
					}
				}

				m_Disposed = true;
			}
		}

		/// <summary>
		/// 释放播放资源。
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
		}

		#endregion

		/// <summary>
		/// 获取一个值指示控件是否已完成初始化。
		/// </summary>
		public bool HasInitialized
		{
			get;
			protected set;
		}

		/// <summary>
		/// Starts the initialization process for this element.
		/// </summary>
		public override void BeginInit()
		{
			HasInitialized = false;
			base.BeginInit();
		}

		/// <summary>
		/// Indicates that the initialization process for the element is complete.
		/// </summary>
		public override void EndInit()
		{
			base.EndInit();

			HasInitialized = true;

			if (!DesignerProperties.GetIsInDesignMode(this))
			{
				double balance = Balance;
				double volume = Volume;

				MediaPlayer.Balance = balance;
				MediaPlayer.Volume = volume;
			}
		}

		/// <summary>
		/// 打开媒体源。
		/// </summary>
		/// <param name="source">需要打开的媒体源。</param>
		public void Open(object source)
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}

			EnsureInitialized();

			OpenImpl(source);

			m_IgnoreSourceChangedCallback = true;
            SetCurrentValue(SourceProperty, source);
		}

        /// <summary>
		/// 异步打开媒体源。
		/// </summary>
		/// <param name="source">需要打开的媒体源。</param>
        public async Task OpenAysnc(object source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            EnsureInitialized();

            await OpenAsyncImpl(source);

            m_IgnoreSourceChangedCallback = true;
            SetCurrentValue(SourceProperty, source);
        }

		/// <summary>
		/// 播放媒体。
		/// </summary>
		public void Play()
		{
			EnsureInitialized();
			EnsureMediaOpened();

			MediaPlayer.Play();
		}

		/// <summary>
		/// 暂停媒体播放。
		/// </summary>
		public void Pause()
		{
			EnsureInitialized();
			EnsureMediaOpened();

			MediaPlayer.Pause();
		}

        /// <summary>
        /// 关闭媒体，释放播放资源。
        /// </summary>
        public void Close()
        {
            EnsureInitialized();
            EnsureMediaOpened();

            SetNaturalVideoHeight(0);
            SetNaturalVideoWidth(0);
            SetVideoBitRate(0);
            SetVideoFrameRate(0);

            MediaPlayer.Close();
        }

		/// <summary>
		/// 停止媒体播放。
		/// </summary>
		public void Stop()
		{
			EnsureInitialized();
			EnsureMediaOpened();

			MediaPlayer.Stop();
		}

		/// <summary>
		/// Executes the actions associated to a MediaState
		/// </summary>
		/// <param name="state">The MediaState to execute</param>
		protected void ExecuteMediaState(MediaState state)
		{
			switch (state)
			{
				case MediaState.Manual:
					break;
				case MediaState.Play:
					if (!MediaPlayer.IsMediaClosed && PlayState != MediaPlayState.Playing)
					{
						Play();
					}
					break;
				case MediaState.Stop:
					if (!MediaPlayer.IsMediaClosed && PlayState != MediaPlayState.Stopped)
					{
						Stop();
					}
					break;
				case MediaState.Close:
					if (!MediaPlayer.IsMediaClosed)
					{
						Close();
					}
					break;
				case MediaState.Pause:
					if (!MediaPlayer.IsMediaClosed && PlayState != MediaPlayState.Paused)
					{
						Pause();
					}
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		/// <summary>
		/// 获取关联的媒体播放器接口。
		/// </summary>
		protected IMediaPlayer MediaPlayer { get; set; }

		/// <summary>
		/// 获取关联的媒体呈现接口。
		/// </summary>
		protected IVideoPresenter MediaPresenter { get; set; }

		/// <summary>
		/// 获取关联的媒体 渲染接口。
		/// </summary>
		protected IVideoRenderer MediaRenderer { get; set; }

		/// <summary>
		/// 初始化媒体呈现接口。
		/// </summary>
		protected virtual void InitializeMediaPresenter()
		{
			if (MediaPresenter != null)
			{
				return;
			}

			MediaPresenter = CreateMediaPresenter();

			if (MediaPresenter == null || MediaPresenter.Player == null || MediaPresenter.Renderer == null)
			{
				throw new InvalidOperationException($"{GetType()} should create a valid media presenter!");
			}

			MediaPresenter.IsPresenting = IsRenderingEnabled;
			MediaRenderer = MediaPresenter.Renderer;
			MediaPlayer = MediaPresenter.Player;
			MediaPlayer.StateChanged += MediaPlayer_StateChanged;
			MediaPlayer.MediaFailed += MediaPlayer_MediaFailed;
			MediaPlayer.FrameAllocated += MediaPlayer_FrameAllocated;
		}

		/// <summary>
		/// 创建媒体呈现接口。
		/// </summary>
		/// <returns></returns>
		protected abstract IVideoPresenter CreateMediaPresenter();

		/// <summary>
		/// 打开媒体源进行播放, 并设置媒体播控能力。
		/// </summary>
		/// <param name="source">要播放的媒体源。</param>
		protected virtual void OpenOverride(object source)
		{
			MediaPlayer.Open(source);
		}

        /// <summary>
		/// 异步打开媒体源进行播放, 并设置媒体播控能力。
		/// </summary>
		/// <param name="source">要播放的媒体源。</param>
        protected virtual async Task OpenAsyncOverride(object source)
        {
            await MediaPlayer.OpenAsync(source);
        }

        /// <summary>
        /// 确保工作在调度器上正确执行。
        /// </summary>
        /// <param name="work">调度的工作委托。</param>
        protected void EnsureAccess(Action work)
		{
			if (CheckAccess())
			{
				work();
			}
			else
			{
				Dispatcher.Invoke(work);
			}
		}

		/// <summary>
		/// 确保工作在调度器上正确执行。
		/// </summary>
		/// <param name="work">调度的工作委托。</param>
		/// <param name="priority">工作优先级。</param>
		protected void BeginEnsureAccess(Action work, DispatcherPriority priority = DispatcherPriority.Normal)
		{
			if (CheckAccess())
			{
				work();
			}
			else
			{
				Dispatcher.BeginInvoke(work, priority);
			}
		}

		/// <summary>
		/// 当前播放器播放状态改变时调用。
		/// </summary>
		private void OnPlayerStateChanged()
		{
			var newState = MediaPlayer.PlayState;

			EnsureAccess(() =>
			{
                CommandManager.InvalidateRequerySuggested();

                if (MediaPlayer.IsMediaClosed)
                {
                    return;
                }

				m_IgnoreStateChangedCallback = true;
				SetCurrentValue(PlayStateProperty, newState);

				switch (newState)
				{
					case MediaPlayState.None:
						OnMediaClosed();
						break;
					case MediaPlayState.Opened:
						OnMediaOpened();
						break;
					case MediaPlayState.Playing:
						break;
					case MediaPlayState.Stopped:
						break;
					case MediaPlayState.Paused:
						break;
					case MediaPlayState.Complete:
						OnMediaEnded();
						break;
					case MediaPlayState.Rewinding:
						break;
					default:
						break;
				}
			});
		}

		private void MediaPlayer_StateChanged(object sender, PlayStateChangedEventArgs e)
		{
			OnPlayerStateChanged();
		}

		private void MediaPlayer_MediaFailed(object sender, MediaFailedEventArgs e)
		{
            EnsureAccess(() =>
            {
                OnMediaFailed(e.ErrorException);
            });
		}

		private void MediaPlayer_FrameAllocated(object sender, FrameAllocatedEventArgs e)
		{
			if (m_ThreadFreeNatrualVideoWidth != e.PixelWidth)
			{
				BeginEnsureAccess(() =>
				{
					SetNaturalVideoWidth(e.PixelWidth);
				});
			}

			if (m_ThreadFreeNatrualVideoHeight != e.PixelHeight)
			{
				BeginEnsureAccess(() =>
				{
					SetNaturalVideoHeight(e.PixelHeight);
				});
			}
		}

		private void InitializeMediaPresenterImpl()
		{
			InitializeMediaPresenter();
		}

		private void OpenImpl(object source)
		{
            if (MediaPlayer.IsMediaOpened)
            {
                throw new InvalidOperationException(Properties.Resources.STR_ERR_MediaOpened);
			}

			OpenOverride(source);

            if (MediaPlayer.IsMediaOpened)
            {
                ExecuteMediaState(OpenedBehavior);
            }
        }

        private async Task OpenAsyncImpl(object source)
        {
            if (MediaPlayer.IsMediaOpened)
            {
                throw new InvalidOperationException(Properties.Resources.STR_ERR_MediaOpened);
            }

            await OpenAsyncOverride(source);

            if (MediaPlayer.IsMediaOpened)
            {
                ExecuteMediaState(OpenedBehavior);
            }
        }

        private void EnsureMediaOpened()
		{
			if (!MediaPlayer.IsMediaOpened && !MediaPlayer.IsMediaOpening)
			{
				throw new InvalidOperationException(Properties.Resources.STR_ERR_MediaNotOpened);
			}
		}

		private void EnsureInitialized()
		{
			if (!HasInitialized)
			{
				throw new InvalidOperationException(Properties.Resources.STR_ERR_NotInitialized);
			}
		}
	}
}
