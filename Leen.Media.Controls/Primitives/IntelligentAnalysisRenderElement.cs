using Leen.Common.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Leen.Media.Controls.Primitives
{
    /// <summary>
    /// 定义客户端产品中支持智能分析的播放控件，提供一组实现智能分析的属性和方法。
    /// </summary>
    public abstract class D3DIntelligentAnalysisRenderElement : D3DRenderElement
    {
        #region IsTrackerEnabled

        /// <summary>
        /// 获取或设置一个值指示是否在播放画面中显示目标跟踪指示器。
        /// </summary>
        public bool IsTrackerEnabled
        {
            get { return (bool)GetValue(IsTrackerEnabledProperty); }
            set { SetValue(IsTrackerEnabledProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="IsTrackerEnabled"/> property.
        /// </summary>
        public static readonly DependencyProperty IsTrackerEnabledProperty =
            DependencyProperty.Register(
                                nameof(IsTrackerEnabled),
                                typeof(bool),
                                typeof(D3DIntelligentAnalysisRenderElement),
                                new PropertyMetadata(true, IsTrackerEnabledPropertyChanged));

        private static void IsTrackerEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((D3DIntelligentAnalysisRenderElement)d).OnIsTrackerEnabledChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        /// <summary>
        /// Called when <see cref="IsTrackerEnabled"/> changed.
        /// </summary>
        /// <param name="oldValue">Old value of <see cref="IsTrackerEnabled"/>.</param>
        /// <param name="newValue">New value of <see cref="IsTrackerEnabled"/>.</param>
        protected virtual void OnIsTrackerEnabledChanged(bool oldValue, bool newValue)
        {
            if (HasInitialized)
            {
                MediaPlayer.IsTrackerVisible = newValue;
            }
        }

        #endregion

        #region IsMarkerEnabled

        /// <summary>
        /// 获取或设置一个值指示是否在播放中显示目标标记指示器。
        /// </summary>
        public bool IsMarkerEnabled
        {
            get { return (bool)GetValue(IsMarkerEnabledProperty); }
            set { SetValue(IsMarkerEnabledProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="IsMarkerEnabled"/> property.
        /// </summary>
        public static readonly DependencyProperty IsMarkerEnabledProperty =
            DependencyProperty.Register(
                                nameof(IsMarkerEnabled),
                                typeof(bool),
                                typeof(D3DIntelligentAnalysisRenderElement),
                                new PropertyMetadata(true, IsMarkerEnabledPropertyChanged));

        private static void IsMarkerEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((D3DIntelligentAnalysisRenderElement)d).OnIsMarkerEnabledChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        /// <summary>
        /// Called when <see cref="IsMarkerEnabled"/> changed.
        /// </summary>
        /// <param name="oldValue">Old value of <see cref="IsMarkerEnabled"/>.</param>
        /// <param name="newValue">New value of <see cref="IsMarkerEnabled"/>.</param>
        protected virtual void OnIsMarkerEnabledChanged(bool oldValue, bool newValue)
        {
            m_ThreadFreeMakerVisible = newValue;

            if (HasInitialized && MediaPlayer != null)
            {
                MediaPlayer.IsMarkerVisible = newValue;
            }

            if (!newValue)
            {
                m_IntelAnalysisMarkerVisuals?.Clear();
            }
        }

        #endregion

        #region TrackingSubject

        /// <summary>
        /// 获取或设置正在请求跟踪的目标对象。
        /// </summary>
        public object TrackingSubject
        {
            get { return (object)GetValue(TrackingSubjectProperty); }
            set { SetValue(TrackingSubjectProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="TrackingSubject"/> property.
        /// </summary>
        public static readonly DependencyProperty TrackingSubjectProperty =
            DependencyProperty.Register(
                                nameof(TrackingSubject),
                                typeof(object),
                                typeof(D3DIntelligentAnalysisRenderElement),
                                new PropertyMetadata(null, TrackingSubjectPropertyChanged));

        private static void TrackingSubjectPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((D3DIntelligentAnalysisRenderElement)d).OnTrackingSubjectChanged((object)e.OldValue, (object)e.NewValue);
        }

        /// <summary>
        /// Called when <see cref="TrackingSubject"/> changed.
        /// </summary>
        /// <param name="oldValue">Old value of <see cref="TrackingSubject"/>.</param>
        /// <param name="newValue">New value of <see cref="TrackingSubject"/>.</param>
        protected virtual void OnTrackingSubjectChanged(object oldValue, object newValue)
        {
            if (m_IngoreTrakingRequested)
            {
                return;
            }

            if (HasInitialized)
            {
                MediaPlayer.TrackingSubject(newValue);
            }
        }

        #endregion

        #region MarkerShape

        /// <summary>
        /// Gets or sets the <see cref="MarkerShape"/> value.
        /// </summary>
        public MarkerShapes MarkerShape
        {
            get { return (MarkerShapes)GetValue(MarkerShapeProperty); }
            set { SetValue(MarkerShapeProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="MarkerShape"/> property.
        /// </summary>
        public static readonly DependencyProperty MarkerShapeProperty =
            DependencyProperty.Register(
                                nameof(MarkerShape),
                                typeof(MarkerShapes),
                                typeof(D3DIntelligentAnalysisRenderElement),
                                new PropertyMetadata(MarkerShapes.FiiledAndStrokedRetangle));
       
        #endregion

        #region MarkerFill

        /// <summary>
        /// 获取或设置显示目标追踪标记时的边框笔刷。
        /// </summary>
        public Brush MarkerFill
        {
            get { return (Brush)GetValue(MarkerFillProperty); }
            set { SetValue(MarkerFillProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="MarkerFill"/> property.
        /// </summary>
        public static readonly DependencyProperty MarkerFillProperty =
            DependencyProperty.Register(
                                nameof(MarkerFill),
                                typeof(Brush),
                                typeof(D3DIntelligentAnalysisRenderElement),
                                new PropertyMetadata(Brushes.Green));

        #endregion

        #region MarkerStroke

        /// <summary>
        /// 获取或设置显示目标追踪标记时的填充笔刷。
        /// </summary>
        public Brush MarkerStroke
        {
            get { return (Brush)GetValue(MarkerStrokeProperty); }
            set { SetValue(MarkerStrokeProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="MarkerStroke"/> property.
        /// </summary>
        public static readonly DependencyProperty MarkerStrokeProperty =
            DependencyProperty.Register(
                                nameof(MarkerStroke),
                                typeof(Brush),
                                typeof(D3DIntelligentAnalysisRenderElement),
                                new PropertyMetadata(Brushes.Red, MarkerStrokePropertyChanged));

        private static void MarkerStrokePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((D3DIntelligentAnalysisRenderElement)d).OnMarkerStrokeChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        /// <summary>
        /// Called when <see cref="MarkerStroke"/> changed.
        /// </summary>
        /// <param name="oldValue">Old value of <see cref="MarkerStroke"/>.</param>
        /// <param name="newValue">New value of <see cref="MarkerStroke"/>.</param>
        protected virtual void OnMarkerStrokeChanged(Brush oldValue, Brush newValue)
        {
            m_MarkerPen = new Pen(newValue, MarkerThickness);
            m_MarkerPen.Freeze();
        }

        #endregion

        #region TrackingMarkerStroke


        /// <summary>
        /// 获取或设置显示正在请求的目标追踪标记时的填充笔刷。
        /// </summary>
        public Brush TrackingMarkerStroke
        {
            get { return (Brush)GetValue(TrackingMarkerStorkeProperty); }
            set { SetValue(TrackingMarkerStorkeProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="TrackingMarkerStroke"/> property.
        /// </summary>
        public static readonly DependencyProperty TrackingMarkerStorkeProperty =
            DependencyProperty.Register(
                nameof(TrackingMarkerStroke),
                typeof(Brush),
                typeof(D3DIntelligentAnalysisRenderElement),
                new PropertyMetadata(Brushes.Orange, TrackingMarkerStrokePropertyChanged));

        private static void TrackingMarkerStrokePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((D3DIntelligentAnalysisRenderElement)d).OnTrackingMarkerStrokeChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        /// <summary>
        /// Called when <see cref="TrackingMarkerPen"/> changed.
        /// </summary>
        /// <param name="oldValue">Old value of <see cref="TrackingMarkerPen"/>.</param>
        /// <param name="newValue">New value of <see cref="TrackingMarkerPen"/>.</param>
        protected virtual void OnTrackingMarkerStrokeChanged(Brush oldValue, Brush newValue)
        {
            m_TrackingMarkerPen = new Pen(newValue, MarkerThickness);
            m_TrackingMarkerPen.Freeze();
        }

        #endregion

        #region FocusedMarkerStroke

        /// <summary>
        /// 获取或设置用于显示鼠标悬停焦点的目标标记的填充笔刷。
        /// </summary>
        public Brush FocusedMarkerStroke
        {
            get { return (Brush)GetValue(FocusedMarkerStrokeProperty); }
            set { SetValue(FocusedMarkerStrokeProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="FocusedMarkerStroke"/> property.
        /// </summary>
        public static readonly DependencyProperty FocusedMarkerStrokeProperty =
            DependencyProperty.Register(
                                nameof(FocusedMarkerStroke),
                                typeof(Brush),
                                typeof(D3DIntelligentAnalysisRenderElement),
                                new PropertyMetadata(Brushes.OrangeRed, FocusedMarkerStrokePropertyChanged));

        private static void FocusedMarkerStrokePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((D3DIntelligentAnalysisRenderElement)d).OnFocusedMarkerStrokeChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        /// <summary>
        /// Called when <see cref="FocusedMarkerStroke"/> changed.
        /// </summary>
        /// <param name="oldValue">Old value of <see cref="FocusedMarkerStroke"/>.</param>
        /// <param name="newValue">New value of <see cref="FocusedMarkerStroke"/>.</param>
        protected virtual void OnFocusedMarkerStrokeChanged(Brush oldValue, Brush newValue)
        {
            m_FocusedMarkerPen = new Pen(FocusedMarkerStroke, MarkerThickness);
            m_FocusedMarkerPen.Freeze();
        }

        #endregion

        #region MarkerOpacity

        /// <summary>
        /// Gets or sets the <see cref="MarkerOpacity"/> value.
        /// </summary>
        public double MarkerOpacity
        {
            get { return (double)GetValue(MarkerOpacityProperty); }
            set { SetValue(MarkerOpacityProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="MarkerOpacity"/> property.
        /// </summary>
        public static readonly DependencyProperty MarkerOpacityProperty =
            DependencyProperty.Register(
                                nameof(MarkerOpacity),
                                typeof(double),
                                typeof(D3DIntelligentAnalysisRenderElement),
                                new PropertyMetadata(0.5d));

        #endregion

        #region MarkerThickness

        /// <summary>
        /// 获取或设置显示目标追踪标记时线条宽度。
        /// </summary>
        public double MarkerThickness
        {
            get { return (double)GetValue(MarkerThicknessProperty); }
            set { SetValue(MarkerThicknessProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="MarkerThickness"/> property.
        /// </summary>
        public static readonly DependencyProperty MarkerThicknessProperty =
            DependencyProperty.Register(
                nameof(MarkerThickness),
                typeof(double),
                typeof(D3DIntelligentAnalysisRenderElement),
                new PropertyMetadata(1.5d, MarkerThicknessPropertyChanged));

        private static void MarkerThicknessPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((D3DIntelligentAnalysisRenderElement)d).OnMarkerThicknessChanged((double)e.OldValue, (double)e.NewValue);
        }

        /// <summary>
        /// Called when <see cref="MarkerThickness"/> value changed.
        /// </summary>
        /// <param name="oldValue">Old value of <see cref="MarkerThickness"/>.</param>
        /// <param name="newValue">New value of <see cref="MarkerThickness"/>.</param>
        protected virtual void OnMarkerThicknessChanged(double oldValue, double newValue)
        {
            m_MarkerPen = new Pen(MarkerStroke, newValue);
            m_MarkerPen.Freeze();
        }

        #endregion

        #region IsSubjectInfoVisible

        /// <summary>
        /// 获取或设置一个值指示是否默认显示目标结构化信息。
        /// </summary>
        public bool IsSubjectInfoVisible
        {
            get { return (bool)GetValue(IsSubjectInfoVisibleProperty); }
            set { SetValue(IsSubjectInfoVisibleProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="IsSubjectInfoVisible"/> property.
        /// </summary>
        public static readonly DependencyProperty IsSubjectInfoVisibleProperty =
            DependencyProperty.Register(
                                nameof(IsSubjectInfoVisible),
                                typeof(bool),
                                typeof(D3DIntelligentAnalysisRenderElement),
                                new PropertyMetadata(false, IsSubjectInfoVisiblePropertyChanged));

        private static void IsSubjectInfoVisiblePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((D3DIntelligentAnalysisRenderElement)d).OnIsSubjectInfoVisibleChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        /// <summary>
        /// Called when <see cref="IsSubjectInfoVisible"/> changed.
        /// </summary>
        /// <param name="oldValue">Old value of <see cref="IsSubjectInfoVisible"/>.</param>
        /// <param name="newValue">New value of <see cref="IsSubjectInfoVisible"/>.</param>
        protected virtual void OnIsSubjectInfoVisibleChanged(bool oldValue, bool newValue)
        {
            m_ThreadFreeSubjectInfoVisible = newValue;
        }

        #endregion

        #region ShowFocusedSubjectInfo

        /// <summary>
        /// 获取或设置一个值指示是否显示获取鼠标焦点的目标结构化信息。
        /// </summary>
        public bool ShowFocusedSubjectInfo
        {
            get { return (bool)GetValue(ShowFocusedSubjectInfoProperty); }
            set { SetValue(ShowFocusedSubjectInfoProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="ShowFocusedSubjectInfo"/> property.
        /// </summary>
        public static readonly DependencyProperty ShowFocusedSubjectInfoProperty =
            DependencyProperty.Register(
                                nameof(ShowFocusedSubjectInfo),
                                typeof(bool),
                                typeof(D3DIntelligentAnalysisRenderElement),
                                new PropertyMetadata(false, ShowFocusedSubjectInfoPropertyChanged));

        private static void ShowFocusedSubjectInfoPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((D3DIntelligentAnalysisRenderElement)d).OnShowFocusedSubjectInfoChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        /// <summary>
        /// Called when <see cref="ShowFocusedSubjectInfo"/> changed.
        /// </summary>
        /// <param name="oldValue">Old value of <see cref="ShowFocusedSubjectInfo"/>.</param>
        /// <param name="newValue">New value of <see cref="ShowFocusedSubjectInfo"/>.</param>
        protected virtual void OnShowFocusedSubjectInfoChanged(bool oldValue, bool newValue)
        {

        }

        #endregion

        #region SubjectInfoOpacity

        /// <summary>
        /// Gets or sets the <see cref="SubjectInfoOpacity"/> value.
        /// </summary>
        public double SubjectInfoOpacity
        {
            get { return (double)GetValue(SubjectInfoOpacityProperty); }
            set { SetValue(SubjectInfoOpacityProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="SubjectInfoOpacity"/> property.
        /// </summary>
        public static readonly DependencyProperty SubjectInfoOpacityProperty =
            DependencyProperty.Register(
                                nameof(SubjectInfoOpacity),
                                typeof(double),
                                typeof(D3DIntelligentAnalysisRenderElement),
                                new PropertyMetadata(1.0d));

        #endregion

        #region SubjectInfoPlacement

        /// <summary>
        /// Gets or sets the <see cref="SubjectInfoPreferedPlacement"/> value.
        /// </summary>
        public TickBarPlacement SubjectInfoPreferedPlacement
        {
            get { return (TickBarPlacement)GetValue(SubjectInfoPreferedPlacementProperty); }
            set { SetValue(SubjectInfoPreferedPlacementProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="SubjectInfoPreferedPlacement"/> property.
        /// </summary>
        public static readonly DependencyProperty SubjectInfoPreferedPlacementProperty =
            DependencyProperty.Register(
                                nameof(SubjectInfoPreferedPlacement),
                                typeof(TickBarPlacement),
                                typeof(D3DIntelligentAnalysisRenderElement),
                                new PropertyMetadata(TickBarPlacement.Right));

        #endregion

        #region SubjectInfoFontFamily

        /// <summary>
        /// 获取或设置用于显示目标结构化信息的字体名称。
        /// </summary>
        public string SubjectInfoFontFamily
        {
            get { return (string)GetValue(SubjectInfoFontFamilyProperty); }
            set { SetValue(SubjectInfoFontFamilyProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="SubjectInfoFontFamily"/>.
        /// </summary>
        public static readonly DependencyProperty SubjectInfoFontFamilyProperty =
            DependencyProperty.Register(
                nameof(SubjectInfoFontFamily),
                typeof(string),
                typeof(D3DIntelligentAnalysisRenderElement), 
                new PropertyMetadata("微软雅黑"));

        #endregion

        #region SubjectInfoFontSize

        /// <summary>
        /// 获取或设置用于显示目标结构化信息的字体大小。
        /// </summary>
        public double SubjectInfoFontSize
        {
            get { return (double)GetValue(SubjectInfoFontSizeProperty); }
            set { SetValue(SubjectInfoFontSizeProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="SubjectInfoFontSize"/>.
        /// </summary>
        public static readonly DependencyProperty SubjectInfoFontSizeProperty =
            DependencyProperty.Register(
                nameof(SubjectInfoFontSize),
                typeof(double), 
                typeof(D3DIntelligentAnalysisRenderElement), 
                new PropertyMetadata(14d));

        #endregion

        #region SubjectInfoForeground

        /// <summary>
        /// 获取或设置用于显示目标结构化信息的字体颜色。
        /// </summary>
        public Brush SubjectInfoForeground
        {
            get { return (Brush)GetValue(SubjectInfoForegroundProperty); }
            set { SetValue(SubjectInfoForegroundProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="SubjectInfoForeground"/>.
        /// </summary>
        public static readonly DependencyProperty SubjectInfoForegroundProperty =
            DependencyProperty.Register(
                nameof(SubjectInfoForeground), 
                typeof(Brush), 
                typeof(D3DIntelligentAnalysisRenderElement),
                new PropertyMetadata(Brushes.Red));

        #endregion

        #region SubjectSummaryDensity

        /// <summary>
        /// 获取货设置提取目标摘要时目标密度。
        /// </summary>
        public int SubjectSummaryDensity
        {
            get { return (int)GetValue(SubjectSummaryDensityProperty); }
            set { SetValue(SubjectSummaryDensityProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="SubjectSummaryDensity"/> property.
        /// </summary>
        public static readonly DependencyProperty SubjectSummaryDensityProperty =
            DependencyProperty.Register(
                                nameof(SubjectSummaryDensity),
                                typeof(int),
                                typeof(D3DIntelligentAnalysisRenderElement),
                                new PropertyMetadata(2, SubjectSummaryDensityPropertyChanged));

        private static void SubjectSummaryDensityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((D3DIntelligentAnalysisRenderElement)d).OnSubjectSummaryDensityChanged((int)e.OldValue, (int)e.NewValue);
        }

        /// <summary>
        /// Called when <see cref="SubjectSummaryDensity"/> changed.
        /// </summary>
        /// <param name="oldValue">Old value of <see cref="SubjectSummaryDensity"/>.</param>
        /// <param name="newValue">New value of <see cref="SubjectSummaryDensity"/>.</param>
        protected virtual void OnSubjectSummaryDensityChanged(int oldValue, int newValue)
        {
            if (SubjectSummarySource != null)
            {
                MediaPlayer.ConfigSubjectSummary(newValue, SubjectSummarySource);
            }
        }

        #endregion

        #region SubjectSummarySource

        /// <summary>
        /// 获取或设置视频智能分析得到的目标摘要源。
        /// </summary>
        public object SubjectSummarySource
        {
            get { return (object)GetValue(SubjectSummarySourceProperty); }
            set { SetValue(SubjectSummarySourceProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="SubjectSummarySource"/> property.
        /// </summary>
        public static readonly DependencyProperty SubjectSummarySourceProperty =
            DependencyProperty.Register(
                                nameof(SubjectSummarySource),
                                typeof(object),
                                typeof(D3DIntelligentAnalysisRenderElement),
                                new PropertyMetadata(null, SubjectSummarySourcePropertyChanged));

        private static void SubjectSummarySourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((D3DIntelligentAnalysisRenderElement)d).OnSubjectSummarySourceChanged((object)e.OldValue, (object)e.NewValue);
        }

        /// <summary>
        /// Called when <see cref="SubjectSummarySource"/> changed.
        /// </summary>
        /// <param name="oldValue">Old value of <see cref="SubjectSummarySource"/>.</param>
        /// <param name="newValue">New value of <see cref="SubjectSummarySource"/>.</param>
        protected virtual void OnSubjectSummarySourceChanged(object oldValue, object newValue)
        {
            if (newValue != null)
            {
                MediaPlayer.ConfigSubjectSummary(SubjectSummaryDensity, newValue);
            }
        }

        #endregion

        private bool m_IngoreTrakingRequested;
        private readonly VisualCollection m_IntelAnalysisMarkerVisuals;
        private volatile bool m_ThreadFreeMakerVisible = true;
        private volatile bool m_ThreadFreeSubjectInfoVisible = false;
        private Pen m_MarkerPen;
        private Pen m_TrackingMarkerPen;
        private Pen m_FocusedMarkerPen;

        /// <summary>
        /// 构造 <see cref="D3DIntelligentAnalysisRenderElement"/> 类的实例。
        /// </summary>
        protected D3DIntelligentAnalysisRenderElement()
        {
            m_IntelAnalysisMarkerVisuals = new VisualCollection(this);
            m_MarkerPen = new Pen(MarkerStroke, MarkerThickness);
            m_MarkerPen.Freeze();
            m_TrackingMarkerPen = new Pen(TrackingMarkerStroke, MarkerThickness);
            m_TrackingMarkerPen.Freeze();
            m_FocusedMarkerPen = new Pen(FocusedMarkerStroke, MarkerThickness);
            m_FocusedMarkerPen.Freeze();
        }

        /// <summary>
        /// 获取显示目标追踪标记的画笔。
        /// </summary>
        protected Pen MarkerPen
        {
            get
            {
                return m_MarkerPen;
            }
        }

        /// <summary>
        /// 获取显示正在请求的目标追踪标记的画笔。
        /// </summary>
        protected Pen TrackingMarkerPen
        {
            get
            {
                return m_TrackingMarkerPen;
            }
        }

        /// <summary>
        /// 获取显示鼠标焦点下的目标追踪标记的画笔。
        /// </summary>
        protected Pen FocusedMarkerPen
        {
            get
            {
                return m_FocusedMarkerPen;
            }
        }

        /// <summary>
        /// 获取子视图元素的数量。
        /// </summary>
        protected override int VisualChildrenCount
        {
            get
            {
                return base.VisualChildrenCount + m_IntelAnalysisMarkerVisuals.Count;
            }
        }

        /// <summary>
        /// 获取支持视频智能分析的播放器。
        /// </summary>
        protected new IIntelligentAnalysisVideoPlayer MediaPlayer
        {
            get
            {
                return base.MediaPlayer as IIntelligentAnalysisVideoPlayer;
            }
        }

        /// <summary>
        /// 创建并初始化媒体呈现接口。
        /// </summary>
        protected override void InitializeMediaPresenter()
        {
            base.InitializeMediaPresenter();

            if (MediaPlayer == null)
            {
                throw new InvalidOperationException($"{GetType()} should a create valid media presenter which contains a {nameof(IIntelligentAnalysisVideoPlayer)} player.");
            }

            MediaPlayer.TrackingRequested += MediaPlayer_TrackingRequested;
            MediaPlayer.FrameAllocated += MediaPlayer_FrameAllocated;
        }

        /// <summary>
        /// 当关闭媒体时调用。
        /// </summary>
        protected override void OnMediaClosed()
        {
            base.OnMediaClosed();
            m_IntelAnalysisMarkerVisuals?.Clear();
        }

        /// <summary>
        /// 释放媒体渲染资源。
        /// </summary>
        /// <param name="disposing">一个值指示是否正在通过调用 <see cref="IDisposable.Dispose()"/> 进行资源释放。</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                //Nediaplayer may be null in design mode.
                if (MediaPlayer != null)
                {
                    MediaPlayer.TrackingRequested -= MediaPlayer_TrackingRequested;
                    MediaPlayer.FrameAllocated -= MediaPlayer_FrameAllocated;
                }
            }
        }

        /// <summary>
        /// 当未处理的 <see cref="Mouse.PreviewMouseDownEvent"/> 附加路由事件在其路由中到达派生自此类的元素时，调用此方法。实现此方法可为此事件添加类处理。
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            if (PlayState != MediaPlayState.None)
            {
                var mousePos = e.GetPosition(RenderTarget);
                var widthRatio = RenderingVideoWidth / NaturalVideoWidth;
                var heightRatio = RenderingVideoHeight / NaturalVideoHeight;

                m_IngoreTrakingRequested = true;

                try
                {
                    MediaPlayer.RequestTracking(mousePos.X / widthRatio, mousePos.Y / heightRatio);
                }
                finally
                {
                    m_IngoreTrakingRequested = false;
                }
            }
        }

        /// <summary>
        /// 获取指定索引处的子视图。
        /// </summary>
        /// <param name="index">子视图索引。</param>
        /// <returns></returns>
        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= (m_IntelAnalysisMarkerVisuals.Count + base.VisualChildrenCount))
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            if (index < base.VisualChildrenCount)
            {
                return base.GetVisualChild(index);
            }

            return m_IntelAnalysisMarkerVisuals[index - base.VisualChildrenCount];
        }

        /// <summary>
        /// 根据目标区域参数绘制一个目标跟踪标记形状。
        /// </summary>
        /// <param name="subject">目标在视频智能分析中得到目标参数。</param>
        /// <returns></returns>
        protected virtual DrawingVisual CreateMarkerVisual(Subject subject)
        {
            var ratioX = RenderingVideoWidth / NaturalVideoWidth;
            var ratioY = RenderingVideoHeight / NaturalVideoHeight;

            var subjectArea = new Rect(
                subject.Cordinate.Left,
                subject.Cordinate.Top,
                subject.Cordinate.Width,
                subject.Cordinate.Height);

            subjectArea.Scale(ratioX, ratioY);
            subjectArea.Scale(ZoomFactor, ZoomFactor);

            Point zero = new Point(0, 0);
            var point = RenderTarget.TranslatePoint(zero, this);
            var offset = (Vector)point;

            subjectArea.Offset(offset);

            bool mouseFocused = false;

            var hwndSource = PresentationSource.FromDependencyObject(this);
            if (hwndSource != null)
            {
                var isMouseOver = ((UIElement)hwndSource.RootVisual).IsMouseOver;
                if (isMouseOver)
                {
                    var mousePos = Mouse.GetPosition(this);
                    mouseFocused = subjectArea.Contains(mousePos);
                }
            }

            DrawingVisual drawingVisual = null;
            switch (MarkerShape)
            {
                case MarkerShapes.Line:
                    drawingVisual = CreateLineMarker(subject, subjectArea, mouseFocused);
                    break;
                case MarkerShapes.FilledRectangle:
                    drawingVisual = CreateFilledRectMarker(subject, subjectArea, mouseFocused);
                    break;
                case MarkerShapes.StrokedRectangle:
                    drawingVisual = CreateStrokedRectMarker(subject, subjectArea, mouseFocused);
                    break;
                case MarkerShapes.CornerStrokedRectangle:
                    drawingVisual = CreateCornerStrokedRectMarker(subject, subjectArea, mouseFocused);
                    break;
                case MarkerShapes.FiiledAndStrokedRetangle:
                    drawingVisual = CreateStrokedAndFilledRectMarker(subject, subjectArea, mouseFocused);
                    break;
            }

            return drawingVisual;
        }

        /// <summary>
        /// 绘制目标结构化信息。
        /// </summary>
        /// <param name="subject">在视频智能分析中得到目标。</param>
        /// <param name="rect">在视频智能分析中得到目标区域。</param>
        /// <param name="drawingContext">绘制上下文。</param>
        /// <param name="mouseFocused">是否包含鼠标焦点。</param>
        protected virtual void DrawSubjectInfo(Subject subject, Rect rect, DrawingContext drawingContext, bool mouseFocused)
        {
            var textOrigin = rect.BottomLeft;
            string subjectInfo;
            if (mouseFocused)
            {
                var subjectType = EnumHelper.GetDescription(subject.Type);
                subjectInfo = $"出现时间：{subject.AppearenceTime:hh\\:mm\\:ss}\r\n目标类型：{subjectType}\r\n目标:{subject.Id}";
            }
            else
            {
                subjectInfo = $"{subject.AppearenceTime:hh\\:mm\\:ss}";
            }
            var foreground = mouseFocused ? FocusedMarkerStroke : SubjectInfoForeground;
            var text = new FormattedText(
                                    subjectInfo,
                                    CultureInfo.CurrentUICulture,
                                    FlowDirection.LeftToRight,
                                    new Typeface(SubjectInfoFontFamily),
                                    SubjectInfoFontSize,
                                    foreground);

            switch (SubjectInfoPreferedPlacement)
            {
                case TickBarPlacement.Left:
                    textOrigin = rect.TopLeft;
                    textOrigin.Offset(-text.Width, 0);
                    break;
                case TickBarPlacement.Top:
                    textOrigin = rect.TopLeft;
                    textOrigin.Offset(0, -text.Height);
                    break;
                case TickBarPlacement.Right:
                    textOrigin = rect.TopRight;
                    break;
                case TickBarPlacement.Bottom:
                    textOrigin = rect.BottomLeft;
                    textOrigin.Offset(0, -text.Height);
                    break;
            }

            drawingContext.PushOpacity(SubjectInfoOpacity);
            drawingContext.DrawText(text, textOrigin);
        }

        private void MediaPlayer_TrackingRequested(object sender, SubjectTrackingRequestedEventArgs e)
        {
            EnsureAccess(() =>
            {
                if (!m_IngoreTrakingRequested)
                {
                    SetCurrentValue(CurrentPositionProperty, e.TrackingPosition);
                }

                SetCurrentValue(TrackingSubjectProperty, e.TrackingSubject);
            });
        }

        private void MediaPlayer_FrameAllocated(object sender, FrameAllocatedEventArgs e)
        {
            if (!m_ThreadFreeMakerVisible)
            {
                return;
            }

            var subjects = MediaPlayer.GetRecentSubjects();
                     
            BeginEnsureAccess(() =>
            {
                m_IntelAnalysisMarkerVisuals?.Clear();

                if (PlayState == MediaPlayState.None)
                {
                    //Media closing has been made.
                    return;
                }

                if (subjects == null || subjects.Length < 1)
                {
                    return;
                }

                List<Subject> drawingSubjects = new List<Subject>();
                for (int i = 0; i < subjects.Length; i++)
                {
                    var cordinate = subjects[i].Cordinate;
                    if (cordinate.Width == 0 || cordinate.Height == 0)
                    {
                        continue;
                    }

                    var ratioX = ThreadFreeRenderVideoWidth / ThreadFreeNatrualVideoWidth;
                    var ratioY = ThreadFreeRenderVideoHeight / ThreadFreeNatrualVideoHeight;

                    var subjectRect = new Rect(cordinate.Left, cordinate.Top, cordinate.Width, cordinate.Height);
                    subjectRect.Scale(ratioX, ratioY);

                    var viewportRect = new Rect(0, 0, ThreadFreeRenderVideoWidth, ThreadFreeRenderVideoHeight);
                    if (ZoomFactor > 1)
                    {
                        var diffWidth = RenderSize.Width - ThreadFreeRenderVideoWidth;
                        var diffHeight = RenderSize.Height - ThreadFreeRenderVideoHeight;
                        //获取画面最左上角坐标
                        var viewportOrigin = new Point((-Matrix.OffsetX - diffWidth / 2) / Matrix.M11, (-Matrix.OffsetY - diffHeight / 2) / Matrix.M22);
                        var viewportWidth = RenderSize.Width / Matrix.M11;
                        var viewportHeight = RenderSize.Height / Matrix.M22;
                        viewportRect = new Rect(viewportOrigin, new Size(viewportWidth, viewportHeight));
                    }

                    if (viewportRect.Contains(subjectRect))
                    {
                        drawingSubjects.Add(subjects[i]);
                    }
                }

                if (drawingSubjects != null)
                {
                    foreach (var subject in drawingSubjects)
                    {
                        var visual = CreateMarkerVisual(subject);
                        m_IntelAnalysisMarkerVisuals.Add(visual);
                    }
                }
            }, DispatcherPriority.Render);
        }

        private DrawingVisual CreateLineMarker(Subject subject, Rect subjectRect, bool mouseFocused)
        {
            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();

            var lineStart = subjectRect.BottomLeft;
            var lineEnd = subjectRect.BottomRight;

            switch (SubjectInfoPreferedPlacement)
            {
                case TickBarPlacement.Left:
                    lineStart = subjectRect.TopLeft;
                    lineEnd = subjectRect.BottomLeft;
                    break;
                case TickBarPlacement.Top:
                    lineStart = subjectRect.TopLeft;
                    lineEnd = subjectRect.BottomRight;
                    break;
                case TickBarPlacement.Right:
                    lineStart = subjectRect.TopRight;
                    lineEnd = subjectRect.BottomRight;
                    break;
                case TickBarPlacement.Bottom:
                    lineStart = subjectRect.BottomLeft;
                    lineEnd = subjectRect.BottomRight;
                    break;
            }

            drawingContext.PushOpacity(MarkerOpacity);

            Pen drawingPen = GetMarkerDrawingPen(subject, mouseFocused);
            drawingContext.DrawLine(drawingPen, lineStart, lineEnd);

            if (IsSubjectInfoVisible || (ShowFocusedSubjectInfo && mouseFocused))
            {
                DrawSubjectInfo(subject, subjectRect, drawingContext, mouseFocused);
            }

            drawingContext.Close();
            return drawingVisual;
        }

        private DrawingVisual CreateFilledRectMarker(Subject subject, Rect subjectRect, bool mouseFocused)
        {
            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();
            drawingContext.PushOpacity(MarkerOpacity);
            drawingContext.DrawRectangle(MarkerFill, null, subjectRect);

            if (IsSubjectInfoVisible || (ShowFocusedSubjectInfo && mouseFocused))
            {
                DrawSubjectInfo(subject, subjectRect, drawingContext, mouseFocused);
            }

            drawingContext.Close();
            return drawingVisual;
        }

        private DrawingVisual CreateStrokedRectMarker(Subject subject, Rect subjectRect, bool mouseFocused)
        {
            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();
            drawingContext.PushOpacity(MarkerOpacity);

            Pen drawingPen = GetMarkerDrawingPen(subject, mouseFocused);
            drawingContext.DrawRectangle(null, drawingPen, subjectRect);

            if (IsSubjectInfoVisible || (ShowFocusedSubjectInfo && mouseFocused))
            {
                DrawSubjectInfo(subject, subjectRect, drawingContext, mouseFocused);
            }

            drawingContext.Close();
            return drawingVisual;
        }

        private DrawingVisual CreateCornerStrokedRectMarker(Subject subject, Rect subjectRect, bool mouseFocused)
        {
            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();
            drawingContext.PushOpacity(MarkerOpacity);

            var cornerWidth = subjectRect.Width / 5;
            var cornerHeight = subjectRect.Height / 5;

            Pen drawingPen = GetMarkerDrawingPen(subject, mouseFocused);
           
            //Draw top left corner
            var rightPoint = subjectRect.TopLeft;
            rightPoint.Offset(cornerWidth, 0);
            drawingContext.DrawLine(drawingPen, subjectRect.TopLeft, rightPoint);
            var bottomPoint = subjectRect.TopLeft;
            bottomPoint.Offset(0, cornerHeight);
            drawingContext.DrawLine(drawingPen, subjectRect.TopLeft, bottomPoint);

            //Draw bottom left corner
            rightPoint = subjectRect.BottomLeft;
            rightPoint.Offset(cornerWidth, 0);
            drawingContext.DrawLine(drawingPen, subjectRect.BottomLeft, rightPoint);
            var topPoint = subjectRect.BottomLeft;
            topPoint.Offset(0, -cornerHeight);
            drawingContext.DrawLine(drawingPen, subjectRect.BottomLeft, topPoint);

            //Draw top right corner
            var leftPoint = subjectRect.TopRight;
            leftPoint.Offset(-cornerWidth, 0);
            drawingContext.DrawLine(drawingPen, subjectRect.TopRight, leftPoint);
            bottomPoint = subjectRect.TopRight;
            bottomPoint.Offset(0, cornerHeight);
            drawingContext.DrawLine(drawingPen, subjectRect.TopRight, bottomPoint);

            //Draw bottom right corner
            leftPoint = subjectRect.BottomRight;
            leftPoint.Offset(-cornerWidth, 0);
            drawingContext.DrawLine(drawingPen, subjectRect.BottomRight, leftPoint);
            topPoint = subjectRect.BottomRight;
            topPoint.Offset(0, -cornerHeight);
            drawingContext.DrawLine(drawingPen, subjectRect.BottomRight, topPoint);

            if (IsSubjectInfoVisible || (ShowFocusedSubjectInfo && mouseFocused))
            {
                DrawSubjectInfo(subject, subjectRect, drawingContext, mouseFocused);
            }
            drawingContext.Close();
            return drawingVisual;
        }

        private DrawingVisual CreateStrokedAndFilledRectMarker(Subject subject, Rect subjectRect, bool mouseFocused)
        {
            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();
            drawingContext.PushOpacity(MarkerOpacity);
            Pen drawingPen = GetMarkerDrawingPen(subject, mouseFocused);
            drawingContext.DrawRectangle(MarkerFill, drawingPen, subjectRect);

            if (IsSubjectInfoVisible || (ShowFocusedSubjectInfo && mouseFocused))
            {
                DrawSubjectInfo(subject, subjectRect, drawingContext, mouseFocused);
            }

            drawingContext.Close();
            return drawingVisual;
        }

        private Pen GetMarkerDrawingPen(Subject subject, bool mouseFocused)
        {
            Pen drawingPen;
            if (mouseFocused)
            {
                drawingPen = FocusedMarkerPen;
            }
            else if (subject.IsTrackingRequested)
            {
                drawingPen = TrackingMarkerPen;
            }
            else
            {
                drawingPen = MarkerPen;
            }

            return drawingPen;
        }
    }
}
