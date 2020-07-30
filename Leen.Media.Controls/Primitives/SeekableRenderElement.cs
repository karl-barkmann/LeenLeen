using Leen.Windows;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Leen.Media.Controls.Primitives
{
    /// <summary>
    /// 定义客户端产品中支持多种播放控制的播放控件，提供一组实现播控的属性和方法。
    /// </summary>
    public abstract class SeekableRenderElement : ZoomableRenderElement
    {
        #region CurrentPosition

        /// <summary>
        /// Dependency property for <see cref="CurrentPosition"/> property.
        /// </summary>
        public static readonly DependencyProperty CurrentPositionProperty
            = DependencyProperty.Register(
                                nameof(CurrentPosition),
                                typeof(TimeSpan),
                                typeof(SeekableRenderElement),
                                new PropertyMetadata(
                                    TimeSpan.Zero,
                                    CurrentPositionPropertyChanged, 
                                    OnCoerceCurrentPosition),
                                OnValidateCurrentPostion);

        /// <summary>
        /// Gets or sets current position of the media.
        /// </summary>
        public TimeSpan CurrentPosition
        {
            get { return (TimeSpan)GetValue(CurrentPositionProperty); }
            set { SetValue(CurrentPositionProperty, value); }
        }

        private static bool OnValidateCurrentPostion(object value)
        {
            if ((TimeSpan)value < TimeSpan.Zero)
            {
                return false;
            }

            return true;
        }

        private static object OnCoerceCurrentPosition(DependencyObject d, object baseValue)
        {
            var renderElement = (SeekableRenderElement)d;

            TimeSpan value = (TimeSpan)baseValue;

            if (value < TimeSpan.Zero)
            {
                return TimeSpan.Zero;
            }

            if (renderElement.PlayState != MediaPlayState.None && 
                renderElement.PlayState != MediaPlayState.Opened && 
                value > renderElement.NaturalDuration)
            {
                return renderElement.NaturalDuration;
            }

            return baseValue;
        }

        private static void CurrentPositionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as SeekableRenderElement;
            if (element.m_ingorePropertyChangedCallback)
            {
                return;
            }
            element.OnCurrentPostionPropertChanged((TimeSpan)e.OldValue, (TimeSpan)e.NewValue);
        }

        /// <summary>
        /// Called when <see cref="CurrentPosition"/> changed.
        /// </summary>
        /// <param name="oldValue">Old value of <see cref="CurrentPosition"/>.</param>
        /// <param name="newValue">New value of <see cref="CurrentPosition"/>.</param>
        protected virtual void OnCurrentPostionPropertChanged(TimeSpan oldValue, TimeSpan newValue)
        {
            SetPlayerCurrentPosition();
        }

        /// <summary>
        /// Internal method to set the <see cref="CurrentPosition"/> DP.
        /// </summary>
        protected virtual void SetPlayerCurrentPosition()
        {
            var position = CurrentPosition;
            if (position >= TimeSpan.Zero)
            {
                MediaPlayer.CurrentPosition = position;
            }
        }

        #endregion

        #region StopPosition

        /// <summary>
        /// Dependency property for <see cref="StopPosition"/> property.
        /// </summary>
        public static readonly DependencyProperty StopPositionProperty
            = DependencyProperty.Register(
                                    nameof(StopPosition),
                                    typeof(TimeSpan),
                                    typeof(SeekableRenderElement),
                                    new PropertyMetadata(
                                        TimeSpan.Zero, 
                                        StopPositionPropertyChanged, 
                                        OnCoerceStopPosition),
                                    OnValidateStopPosition);

        /// <summary>
        /// Gets or sets stop position of the media.
        /// Media will stop playing at the stop position if it's value is valid.
        /// </summary>
        public TimeSpan StopPosition
        {
            get { return (TimeSpan)GetValue(StopPositionProperty); }
            set { SetValue(StopPositionProperty, value); }
        }

        private static bool OnValidateStopPosition(object value)
        {
            if ((TimeSpan)value < TimeSpan.Zero)
            {
                return false;
            }

            return true;
        }

        private static object OnCoerceStopPosition(DependencyObject d, object baseValue)
        {
            var renderElement = (SeekableRenderElement)d;

            TimeSpan value = (TimeSpan)baseValue;

            if (value < TimeSpan.Zero)
            {
                return TimeSpan.Zero;
            }

            if (renderElement.PlayState != MediaPlayState.None &&
                renderElement.PlayState != MediaPlayState.Opened &&
                value > renderElement.NaturalDuration)
            {
                return renderElement.NaturalDuration;
            }

            return baseValue;
        }

        private static void StopPositionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as SeekableRenderElement;
            if (element.m_ingorePropertyChangedCallback)
            {
                return;
            }
            element.OnStopPositionChanged((TimeSpan)e.OldValue, (TimeSpan)e.NewValue);
        }

        /// <summary>
        /// Called when <see cref="StopPosition"/> changed.
        /// </summary>
        /// <param name="oldValue">Old value of <see cref="StopPosition"/>.</param>
        /// <param name="newValue">New value of <see cref="StopPosition"/>.</param>
        protected virtual void OnStopPositionChanged(TimeSpan oldValue, TimeSpan newValue)
        {
            SetPlayerStopPosition();
        }

        /// <summary>
        /// Internal method to set the <see cref="StopPosition"/> DP.
        /// </summary>
        protected virtual void SetPlayerStopPosition()
        {
            var position = StopPosition;
            MediaPlayer.StopPosition = position;
        }

        #endregion

        #region NaturalDuration

        /// <summary>
        /// Dependency property key for <see cref="NaturalDuration"/> property.
        /// </summary>
        private static readonly DependencyPropertyKey NaturalDurationPropertyKey
            = DependencyProperty.RegisterReadOnly(
                                    nameof(NaturalDuration),
                                    typeof(TimeSpan),
                                    typeof(SeekableRenderElement),
                                    new PropertyMetadata(TimeSpan.Zero));

        /// <summary>
        /// Dependency property for <see cref="NaturalDuration"/> property.
        /// </summary>
        public static readonly DependencyProperty NaturalDurationProperty
           = NaturalDurationPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the natural duration of the media.
        /// </summary>
        public TimeSpan NaturalDuration
        {
            get { return (TimeSpan)GetValue(NaturalDurationProperty); }
        }

        /// <summary>
        /// Internal method to set the <see cref="NaturalDuration"/> DP.
        /// </summary>
        /// <param name="naturalDuration">Natural duration of the media.</param>
        protected void SetNaturalDuration(TimeSpan naturalDuration)
        {
            SetValue(NaturalDurationPropertyKey, naturalDuration);
        }

        #endregion

        #region SpeedLevel

        /// <summary>
        /// Dependency property for <see cref="SpeedLevel"/> property.
        /// </summary>
        public static readonly DependencyProperty SpeedLevelProperty
            = DependencyProperty.Register(
                nameof(SpeedLevel),
                typeof(int),
                typeof(SeekableRenderElement),
                new FrameworkPropertyMetadata(0, new PropertyChangedCallback(OnSpeedLevelChanged)));

        /// <summary>
        /// Gets or sets the rate the media is played back.
        /// </summary>
        public int SpeedLevel
        {
            get { return (int)GetValue(SpeedLevelProperty); }
            set { SetValue(SpeedLevelProperty, value); }
        }

        private static void OnSpeedLevelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as SeekableRenderElement;
            if (element.m_ingorePropertyChangedCallback)
            {
                return;
            }
            element.OnSpeedLevelChanged(e);
        }

        /// <summary>
        /// Called when <see cref="SpeedLevel"/> changed.
        /// </summary>
        /// <param name="e">Related event args</param>
        protected virtual void OnSpeedLevelChanged(DependencyPropertyChangedEventArgs e)
        {
            if (HasInitialized)
            {
                SetPlayerSpeedLevel();
            }
        }

        /// <summary>
        /// Internal method to set the player's <see cref="ISeekableMediaPlayer.CurrentSpeed"/>.
        /// </summary>
        protected virtual void SetPlayerSpeedLevel()
        {
            var speedLevel = SpeedLevel;

            MediaPlayer.CurrentSpeed = (MediaSpeedLevel)speedLevel;
        }

        #endregion

        #region CanPlayBackwards

        /// <summary>
        /// Dependency property key for <see cref="CanPlayBackwards"/> property.
        /// </summary>
        public static readonly DependencyPropertyKey CanPlayBackwardsPropertyKey
            = DependencyProperty.RegisterReadOnly(
                                    nameof(CanPlayBackwards),
                                    typeof(bool),
                                    typeof(SeekableRenderElement),
                                    new PropertyMetadata(false));

        /// <summary>
        /// Dependency property for <see cref="CanPlayBackwards"/> property.
        /// </summary>
        public static readonly DependencyProperty CanPlayBackwardsProperty
            = CanPlayBackwardsPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the <see cref="CanPlayBackwards"/> value.
        /// </summary>
        public bool CanPlayBackwards
        {
            get { return (bool)GetValue(CanPlayBackwardsProperty); }
        }

        /// <summary>
        /// Internal method for setting the read-only <see cref="CanPlayBackwards"/> DP
        /// </summary>
        protected void SetCanPlayBackwards(bool value)
        {
            SetValue(CanPlayBackwardsPropertyKey, value);
        }

        #endregion

        #region CanSpeedControl

        /// <summary>
        /// Dependency property key for <see cref="CanSpeedControl"/> property.
        /// </summary>
        public static readonly DependencyPropertyKey CanSpeedControlPropertyKey
            = DependencyProperty.RegisterReadOnly(
                                    nameof(CanSpeedControl),
                                    typeof(bool),
                                    typeof(SeekableRenderElement),
                                    new PropertyMetadata(true));

        /// <summary>
        /// Dependency property for <see cref="CanSpeedControl"/> property.
        /// </summary>
        public static readonly DependencyProperty CanSpeedControlProperty
            = CanSpeedControlPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the <see cref="CanSpeedControl"/> value.
        /// </summary>
        public bool CanSpeedControl
        {
            get { return (bool)GetValue(CanSpeedControlProperty); }
        }

        /// <summary>
        /// Internal method for setting the read-only <see cref="CanSpeedControl"/> DP.
        /// </summary>
        protected void SetCanSpeedControl(bool value)
        {
            SetValue(CanSpeedControlPropertyKey, value);
        }

        #endregion

        #region CanSeek

        /// <summary>
        /// Dependency property key for <see cref="CanSeek"/> property.
        /// </summary>
        public static readonly DependencyPropertyKey CanSeekPropertyKey
            = DependencyProperty.RegisterReadOnly(
                                    nameof(CanSeek),
                                    typeof(bool),
                                    typeof(SeekableRenderElement),
                                    new PropertyMetadata(true));

        /// <summary>
        /// Dependency property for <see cref="CanSeek"/> property.
        /// </summary>
        public static readonly DependencyProperty CanSeekProperty
            = CanSeekPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the <see cref="CanSeek"/> value.
        /// </summary>
        public bool CanSeek
        {
            get { return (bool)GetValue(CanSeekProperty); }
        }

        /// <summary>
        /// Internal method for setting the read-only <see cref="CanSeek"/> DP.
        /// </summary>
        protected void SetCanSeek(bool value)
        {
            SetValue(CanSeekPropertyKey, value);
        }

        #endregion

        #region CanFrameForwards

        /// <summary>
        /// Dependency property key for <see cref="CanFrameForwards"/> property.
        /// </summary>
        public static readonly DependencyPropertyKey CanFrameForwardsPropertyKey
            = DependencyProperty.RegisterReadOnly(
                                    nameof(CanFrameForwards),
                                    typeof(bool),
                                    typeof(SeekableRenderElement),
                                    new PropertyMetadata(true));

        /// <summary>
        /// Dependency property for <see cref="CanFrameForwards"/> property.
        /// </summary>
        public static readonly DependencyProperty CanFrameForwardsProperty
            = CanFrameForwardsPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the <see cref="CanFrameForwards"/> value.
        /// </summary>
        public bool CanFrameForwards
        {
            get { return (bool)GetValue(CanFrameForwardsProperty); }
        }

        /// <summary>
        /// Internal method for setting the read-only <see cref="CanFrameForwards"/> DP.
        /// </summary>
        protected void SetCanFrameForwards(bool value)
        {
            SetValue(CanFrameForwardsPropertyKey, value);
        }

        #endregion

        #region CanFrameBackwards

        /// <summary>
        /// Dependency property key for <see cref="CanFrameBackwards"/> property.
        /// </summary>
        public static readonly DependencyPropertyKey CanFrameBackwardsPropertyKey
            = DependencyProperty.RegisterReadOnly(
                                    nameof(CanFrameBackwards),
                                    typeof(bool),
                                    typeof(SeekableRenderElement),
                                    new PropertyMetadata(true));

        /// <summary>
        /// Dependency property for <see cref="CanFrameBackwards"/> property.
        /// </summary>
        public static readonly DependencyProperty CanFrameBackwardsProperty
            = CanFrameBackwardsPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the <see cref="CanFrameBackwards"/> value.
        /// </summary>
        public bool CanFrameBackwards
        {
            get { return (bool)GetValue(CanFrameBackwardsProperty); }
        }

        /// <summary>
        /// Internal method for setting the read-only <see cref="CanFrameBackwards"/> DP.
        /// </summary>
        protected void SetCanFrameBackwards(bool value)
        {
            SetValue(CanFrameBackwardsPropertyKey, value);
        }

        #endregion

        #region Commands

        private static RoutedUICommand _FrameForwardsCommand;
        private static RoutedUICommand _FrameBackwardsCommand;
        private static RoutedUICommand _playBackwardsCommand;
        private static RoutedUICommand _startExportCommand;
        private static RoutedUICommand _stopExportCommand;
        private static RoutedUICommand _pauseExportCommand;
        private static RoutedUICommand _resumePlayCommand;

        /// <summary>
        /// 获取用于前进到下一帧的命令。
        /// </summary>
        public static RoutedUICommand FrameForwardsCommand
        {
            get { return _FrameForwardsCommand; }
        }

        /// <summary>
        /// 获取用于回到上一帧的命令。
        /// </summary>
        public static RoutedUICommand FrameBackwardsCommand
        {
            get { return _FrameBackwardsCommand; }
        }

        /// <summary>
        /// 获取用于开始向后播放的命令
        /// </summary>
        public static RoutedUICommand PlayBackwardsCommand
        {
            get { return _playBackwardsCommand; }
        }

        /// <summary>
        /// 获取开始视频导出的命令。
        /// </summary>
        public static RoutedUICommand StartExportCommand
        {
            get { return _startExportCommand; }
        }

        /// <summary>
        /// 获取暂停视频导出的命令。
        /// </summary>
        public static RoutedUICommand PauseExportCommand
        {
            get { return _pauseExportCommand; }
        }

        /// <summary>
        /// 获取停止视频导出的命令。
        /// </summary>
        public static RoutedUICommand StopExportCommand
        {
            get { return _stopExportCommand; }
        }

        /// <summary>
        /// 获取用于恢复正常播放的命令。
        /// </summary>
        public static RoutedUICommand ResumePlayCommand
        {
            get { return _resumePlayCommand; }
        }

        private static void InitializeCommands()
        {
            _FrameForwardsCommand = new RoutedUICommand(
                Properties.Resources.STR_Command_FrameForwards, 
                "FrameForwards",
                typeof(SeekableRenderElement));
            _FrameBackwardsCommand = new RoutedUICommand(
                Properties.Resources.STR_Command_FrameBackwards,
                "FrameBackwards",
                typeof(SeekableRenderElement));
            _playBackwardsCommand = new RoutedUICommand(
                Properties.Resources.STR_Command_PlayBackwards,
                "Playbackward",
                typeof(SeekableRenderElement));
            _startExportCommand = new RoutedUICommand(
                Properties.Resources.STR_Command_StartExport,
                "StartExport",
                typeof(SeekableRenderElement));
            _pauseExportCommand = new RoutedUICommand(
                Properties.Resources.STR_Command_PauseExport,
                "PauseExport",
                typeof(SeekableRenderElement));
            _stopExportCommand = new RoutedUICommand(
                Properties.Resources.STR_Command_StopExport,
                "StopExport",
                typeof(SeekableRenderElement));
            _resumePlayCommand = new RoutedUICommand(
               Properties.Resources.STR_Command_ResumePlay,
               "ResumePlay",
               typeof(MediaRenderElement));

            CommandHelpers.RegisterCommandHandler(
                typeof(SeekableRenderElement),
                FrameForwardsCommand,
                OnFrameForwards,
                OnCanFrameForwards);
            CommandHelpers.RegisterCommandHandler(
                typeof(SeekableRenderElement), 
                FrameBackwardsCommand,
                OnFrameBackwards,
                OnCanFrameBackwards);
            CommandHelpers.RegisterCommandHandler(
                typeof(SeekableRenderElement),
                PlayBackwardsCommand,
                OnPlayBackwards,
                OnCanPlayBackwards);
            CommandHelpers.RegisterCommandHandler(
                typeof(SeekableRenderElement),
                StartExportCommand,
                OnStartExport,
                OnCanStartExport);
            CommandHelpers.RegisterCommandHandler(
                typeof(SeekableRenderElement),
                PauseExportCommand,
                OnPauseExport,
                OnCanPauseExport);
            CommandHelpers.RegisterCommandHandler(
                typeof(SeekableRenderElement),
                StopExportCommand,
                OnStopExport,
                OnCanStopExport);
            CommandHelpers.RegisterCommandHandler(
               typeof(MediaRenderElement),
               ResumePlayCommand,
               OnResumePlay,
               OnCanResumePlay);
        }

        private static void OnCanStopExport(object sender, CanExecuteRoutedEventArgs e)
        {
            var element = sender as SeekableRenderElement;
            e.CanExecute = element.GetCanStopExport();
        }

        private static void OnStopExport(object sender, ExecutedRoutedEventArgs e)
        {
            var element = sender as SeekableRenderElement;
            element.StopExport();
        }

        private static void OnCanPauseExport(object sender, CanExecuteRoutedEventArgs e)
        {
            var element = sender as SeekableRenderElement;
            e.CanExecute = element.GetCanPauseExport();
        }

        private static void OnPauseExport(object sender, ExecutedRoutedEventArgs e)
        {
            var element = sender as SeekableRenderElement;
            element.PauseExport();
        }

        private static void OnCanStartExport(object sender, CanExecuteRoutedEventArgs e)
        {
            var element = sender as SeekableRenderElement;
            e.CanExecute = element.GetCanStartExport();
        }

        private static void OnStartExport(object sender, ExecutedRoutedEventArgs e)
        {
            var element = sender as SeekableRenderElement;
            element.StartExport();
        }

        private static void OnCanFrameBackwards(object sender, CanExecuteRoutedEventArgs e)
        {
            var element = sender as SeekableRenderElement;
            e.CanExecute = element.GetCanFrameBackwards();
        }

        private static void OnFrameBackwards(object sender, ExecutedRoutedEventArgs e)
        {
            var element = sender as SeekableRenderElement;
            if(element.GetCanFrameBackwards())
            {
                element.FrameBackwards();
            }
        }

        private static void OnCanFrameForwards(object sender, CanExecuteRoutedEventArgs e)
        {
            var element = sender as SeekableRenderElement;
            e.CanExecute = element.GetCanFrameForwards();
        }

        private static void OnFrameForwards(object sender, ExecutedRoutedEventArgs e)
        {
            var element = sender as SeekableRenderElement;
            if (element.GetCanFrameForwards())
            {
                element.FrameForwards();
            }
        }

        private static void OnCanPlayBackwards(object sender, CanExecuteRoutedEventArgs e)
        {
            var element = sender as SeekableRenderElement;
            e.CanExecute = element.GetCanPlayBackward();
        }

        private static void OnPlayBackwards(object sender, ExecutedRoutedEventArgs e)
        {
            var element = sender as SeekableRenderElement;
            if (element.GetCanFrameForwards())
            {
                element.PlayBackwards();
            }
        }

        private static void OnCanResumePlay(object sender, CanExecuteRoutedEventArgs e)
        {
            SeekableRenderElement element = sender as SeekableRenderElement;
            e.CanExecute = element.GetCanResumePlay();
        }

        private static void OnResumePlay(object sender, ExecutedRoutedEventArgs e)
        {
            SeekableRenderElement element = sender as SeekableRenderElement;
            element.ResumePlay();
        }

        /// <summary>
        /// 获取一个值指示是否可以前进到下一帧。
        /// </summary>
        /// <returns></returns>
        protected virtual bool GetCanFrameForwards()
        {
            return PlayState != MediaPlayState.None && PlayState != MediaPlayState.Stopped;
        }

        /// <summary>
        /// 获取一个值指示是否可以回到上一帧。
        /// </summary>
        /// <returns></returns>
        protected virtual bool GetCanFrameBackwards()
        {
            return PlayState != MediaPlayState.None && PlayState != MediaPlayState.Stopped;
        }

        /// <summary>
        /// 获取一个值指示是否可以执行倒播。
        /// </summary>
        /// <returns></returns>
        protected virtual bool GetCanPlayBackward()
        {
            return PlayState != MediaPlayState.None && PlayState != MediaPlayState.Stopped;
        }

        /// <summary>
        /// 将媒体播放前进到下一帧。
        /// </summary>
        protected virtual void FrameForwards()
        {
            if (HasInitialized)
            {
                MediaPlayer.SeekCurrentPosition(TimeSpan.FromTicks(1), MediaSeekingFlags.SeekToKeyFrame);
            }
        }

        /// <summary>
        /// 将媒体播放回到上一帧。
        /// </summary>
        protected virtual void FrameBackwards()
        {
            if (HasInitialized)
            {
                MediaPlayer.SeekCurrentPosition(TimeSpan.FromTicks(-1), MediaSeekingFlags.SeekToKeyFrame);
            }
        }

        /// <summary>
        /// 将媒体播放切换到倒播。
        /// </summary>
        protected virtual void PlayBackwards()
        {
            if (HasInitialized)
            {
                MediaPlayer.PlayBackwards();
            }
        }

        /// <summary>
        /// 获取一个值指示释放可以停止导出。
        /// </summary>
        /// <returns></returns>
        protected virtual bool GetCanStopExport()
        {
            return PlayState != MediaPlayState.None && PlayState != MediaPlayState.Stopped;
        }

        /// <summary>
        /// 停止导出。
        /// </summary>
        protected virtual void StopExport()
        {
            
        }

        /// <summary>
        /// 获取一个值指示释放可以暂停导出。
        /// </summary>
        /// <returns></returns>
        protected virtual bool GetCanPauseExport()
        {
            return PlayState != MediaPlayState.None && PlayState != MediaPlayState.Stopped;
        }

        /// <summary>
        /// 暂停导出。
        /// </summary>
        protected virtual void PauseExport()
        {
            
        }

        /// <summary>
        /// 获取一个值指示释放可以开始导出。
        /// </summary>
        /// <returns></returns>
        protected virtual bool GetCanStartExport()
        {
            return PlayState != MediaPlayState.None && PlayState != MediaPlayState.Stopped;
        }

        /// <summary>
        /// 开始导出。
        /// </summary>
        /// <returns></returns>
        protected virtual void StartExport()
        {

        }

        /// <summary>
        /// 获取一个值表示是否可以切换播放/暂停的内部实现。
        /// </summary>
        /// <returns></returns>
        protected virtual bool GetCanResumePlay()
        {
            return PlayState != MediaPlayState.None && PlayState != MediaPlayState.Stopped;
        }

        /// <summary>
        /// 恢复正常播放。
        /// </summary>
        protected void ResumePlay()
        {
            Resume();

            m_ingorePropertyChangedCallback = true; 
            SetCurrentValue(SpeedLevelProperty, (int)MediaPlayer.CurrentSpeed);
            m_ingorePropertyChangedCallback = false;
        }

        /// <summary>
		/// 切换播放/暂停的内部实现。
		/// </summary>
        protected override void TogglePlayPause()
        {
            if (PlayState == MediaPlayState.Complete)
            {
                CurrentPosition = TimeSpan.Zero;
            }
            else if (PlayState == MediaPlayState.Paused && StopPosition >= TimeSpan.Zero)
            {
                StopPosition = TimeSpan.Zero;
            }
            base.TogglePlayPause();
        }

        #endregion

        static SeekableRenderElement()
        {
            InitializeCommands();
        }

        private bool m_ingorePropertyChangedCallback = false;

        /// <summary>
        /// 恢复正常播放(重置播放速度及播放方向)。
        /// </summary>
        public void Resume()
        {
            MediaPlayer.Resume();
        }

        /// <summary>
        /// Indicates that the initialization process for the element is complete.
        /// </summary>
        public override void EndInit()
        {
            base.EndInit();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                SetPlayerCurrentPosition();
                SetPlayerSpeedLevel();
                SetPlayerStopPosition();
            }
        }

        /// <summary>
        /// 获取关联的播放器接口。
        /// </summary>
        protected new ISeekableMediaPlayer MediaPlayer
        {
            get { return base.MediaPlayer as ISeekableMediaPlayer; }
        }

        /// <summary>
        /// 当媒体播放播放器进度发生改变时调用。
        /// </summary>
        protected virtual void OnMediaPlayerPositionChanged()
        {
            var position = MediaPlayer.CurrentPosition;
            var duration = MediaPlayer.NaturalDuration;

            BeginEnsureAccess(() => 
            {
                if (PlayState == MediaPlayState.None)
                {
                    //Media closing has been made.
                    return;
                }

                if (NaturalDuration != duration)
                {
                    SetNaturalDuration(duration);
                }

                SetCurrentPosition(position);
            });
        }

        /// <summary>
        /// 设置控件的当前播放进度。
        /// </summary>
        /// <param name="position">当前播放进度。</param>
        protected void SetCurrentPosition(TimeSpan position)
        {
            m_ingorePropertyChangedCallback = true;
            SetCurrentValue(CurrentPositionProperty, position);
            m_ingorePropertyChangedCallback = false;
        }

        /// <summary>
        /// 释放播放资源。
        /// </summary>
        /// <param name="disposing">一个值指示是否正在经由调用 <see cref="Dispose"/> 方法进行资源释放。</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (MediaPlayer != null)
            {
                MediaPlayer.ProgressChanged -= MediaPlayer_ProgressChanged;
            }
        }

        /// <summary>
        /// 初始化媒体呈现接口。
        /// </summary>
        protected override void InitializeMediaPresenter()
        {
            base.InitializeMediaPresenter();

            if (MediaPlayer == null)
            {
                throw new InvalidOperationException($"{GetType()} should a create valid media presenter which contains a {nameof(ISeekableMediaPlayer)} player.");
            }

            MediaPlayer.ProgressChanged += MediaPlayer_ProgressChanged;
        }

        /// <summary>
        /// 当发生媒体播放错误时调用。
        /// </summary>
        /// <param name="errorException">描述错误的异常对象。</param>
        protected override void OnMediaFailed(Exception errorException)
        {
            SetNaturalDuration(TimeSpan.Zero);
            SetCurrentPosition(TimeSpan.Zero);
            m_ingorePropertyChangedCallback = true;
            SetCurrentValue(StopPositionProperty, TimeSpan.Zero);
            m_ingorePropertyChangedCallback = false;

            base.OnMediaFailed(errorException);
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
            var duration = MediaPlayer.NaturalDuration;
            var mediaCpas = MediaPlayer.ControlCaps;
            var canSeek = mediaCpas.HasFlag(MediaControlCaps.CanSeek);
            var canPlayBackwards = mediaCpas.HasFlag(MediaControlCaps.CanPlayBackwards);
            var canFrameForwards = mediaCpas.HasFlag(MediaControlCaps.CanFrameForwards);
            var canFrameBackwards = mediaCpas.HasFlag(MediaControlCaps.CanFrameBackwards);
            var canSpeedControl = mediaCpas.HasFlag(MediaControlCaps.CanSpeedControl);

            SetCanSeek(canSeek);
            SetCanSpeedControl(canSpeedControl);
            SetCanFrameForwards(canFrameForwards);
            SetCanFrameBackwards(canFrameBackwards);
            SetCanPlayBackwards(canSeek);
            SetNaturalDuration(duration);

            MediaPlayer.CurrentPosition = CurrentPosition;
            MediaPlayer.CurrentSpeed = (MediaSpeedLevel)SpeedLevel;
            MediaPlayer.Volume = Volume;

            base.OnMediaOpened();
        }

        /// <summary>
        /// 当媒体关闭时调用。
        /// </summary>
        protected override void OnMediaClosed()
        {
            SetCurrentPosition(TimeSpan.Zero);
            m_ingorePropertyChangedCallback = true;
            SetCurrentValue(SpeedLevelProperty, (int)MediaSpeedLevel.Normal);
            m_ingorePropertyChangedCallback = false;

            base.OnMediaClosed();
        }

        /// <summary>
		/// 打开媒体源进行播放, 并设置媒体播控能力。
		/// </summary>
		/// <param name="source">要播放的媒体源。</param>
        protected override void OpenOverride(object source)
        {
            SetPlayerCurrentPosition();
            SetPlayerSpeedLevel();
            SetPlayerStopPosition();

            base.OpenOverride(source);
        }

        /// <summary>
        /// 异步打开媒体源进行播放, 并设置媒体播控能力。
        /// </summary>
        /// <param name="source">要播放的媒体源。</param>
        protected override async Task OpenAsyncOverride(object source)
        {
            SetPlayerCurrentPosition();
            SetPlayerSpeedLevel();
            SetPlayerStopPosition();

            await base.OpenAsyncOverride(source);
        }

        private void MediaPlayer_ProgressChanged(object sender, PlayProgressChangedEventArgs e)
        {
            OnMediaPlayerPositionChanged();
        }
    }
}
