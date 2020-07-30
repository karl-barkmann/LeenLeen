using Leen.Windows;
using System;
using System.Windows;
using System.Windows.Input;

namespace Leen.Media.Controls.Primitives
{
    /// <summary>
    /// 定义客户端产品中支持画面缩放的媒体播放控件，提供用于缩放的参数和实现。
    /// </summary>
    public abstract class ZoomableRenderElement : MediaRenderElement
    {
        #region MaxZoomFactor

        /// <summary>
        /// Dependency property key for <see cref="MaxZoomFactor"/> property.
        /// </summary>
        public static readonly DependencyPropertyKey MaxZoomFactorPropertyKey =
            DependencyProperty.RegisterReadOnly(
                                nameof(MaxZoomFactor),
                                typeof(double),
                                typeof(ZoomableRenderElement),
                                new PropertyMetadata(32.0d));

        /// <summary>
        /// Dependency property for <see cref="MaxZoomFactor"/> property.
        /// </summary>
        public static readonly DependencyProperty MaxZoomFactorProperty =
            MaxZoomFactorPropertyKey.DependencyProperty;

        /// <summary>
        /// Maximum zoom factor when zooming.
        /// </summary>
        public double MaxZoomFactor
        {
            get { return (double)GetValue(MaxZoomFactorProperty); }
        }

        /// <summary>
        /// 设置允许的最大缩放参数的内部实现。
        /// </summary>
        /// <param name="value">允许的最大缩放参数。</param>
        protected void SetMaxZoomFactor(double value)
        {
            SetValue(MaxZoomFactorPropertyKey, value);
        }

        #endregion

        #region MinZoomFactor

        /// <summary>
        /// Dependency property key for <see cref="MinZoomFactor"/> property.
        /// </summary>
        public static readonly DependencyPropertyKey MinZoomFactorPropertyKey =
            DependencyProperty.RegisterReadOnly(
                                nameof(MinZoomFactor),
                                typeof(double),
                                typeof(ZoomableRenderElement),
                                new PropertyMetadata(1.0d));

        /// <summary>
        /// Dependency property for <see cref="MinZoomFactor"/> property.
        /// </summary>
        public static readonly DependencyProperty MinZoomFactorProperty =
            MinZoomFactorPropertyKey.DependencyProperty;

        /// <summary>
        /// Minimum zoom factor when zooming.
        /// </summary>
        public double MinZoomFactor
        {
            get { return (double)GetValue(MinZoomFactorProperty); }
        }

        /// <summary>
        /// 设置允许的最大缩放参数的内部实现。
        /// </summary>
        /// <param name="value">允许的最大缩放参数。</param>
        protected void SetMinZoomFactor(double value)
        {
            SetValue(MinZoomFactorPropertyKey, value);
        }

        #endregion

        #region ZoomFactor

        /// <summary>
        /// Dependency property for <see cref="ZoomFactor"/> property.
        /// </summary>
        public static readonly DependencyProperty ZoomFactorProperty =
           DependencyProperty.Register(
                               nameof(ZoomFactor),
                               typeof(double),
                               typeof(ZoomableRenderElement),
                               new PropertyMetadata(1.0d, ZoomFactorPropertyChanged));

        /// <summary>
        /// Current zoom factor when zooming.
        /// </summary>
        public double ZoomFactor
        {
            get { return (double)GetValue(ZoomFactorProperty); }
            set { SetValue(ZoomFactorProperty, value); }
        }

        private static void ZoomFactorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ZoomableRenderElement)d).OnZoomFactorChanged((double)e.OldValue, (double)e.NewValue);
        }

        /// <summary>
        /// 当缩放系数发生改变时调用。
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        protected virtual void OnZoomFactorChanged(double oldValue, double newValue)
        {
            if (newValue > oldValue)
            {
                ZoomIn(oldValue, newValue);
            }
            else
            {
                ZoomOut(oldValue, newValue);
            }
        }

        #endregion

        #region ZoomDelta

        /// <summary>
        /// 获取或设置用于计算缩放画面时的缩放系数变量。
        /// </summary>
        public double ZoomDelta
        {
            get { return (double)GetValue(ZoomDeltaProperty); }
            set { SetValue(ZoomDeltaProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="ZoomDelta"/> property.
        /// </summary>
        public static readonly DependencyProperty ZoomDeltaProperty =
            DependencyProperty.Register(
                nameof(ZoomDelta),
                typeof(double),
                typeof(ZoomableRenderElement),
                new PropertyMetadata(50d));

        #endregion

        #region EnableMouseWhellZooming

        /// <summary>
        /// 获取或设置一个值指示是否启用鼠标滚轮进行缩放。
        /// </summary>
        public bool EnableMouseWheelZooming
        {
            get { return (bool)GetValue(EnableMouseWheelZoomingProperty); }
            set { SetValue(EnableMouseWheelZoomingProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="EnableMouseWheelZooming"/> property.
        /// </summary>
        public static readonly DependencyProperty EnableMouseWheelZoomingProperty =
            DependencyProperty.Register(
                nameof(EnableMouseWheelZooming),
                typeof(bool),
                typeof(D3DRenderElement),
                new PropertyMetadata(true));

        #endregion

        #region Commands

        private static RoutedUICommand _zoomInCommand;
        private static RoutedUICommand _zoomOutCommand;

        /// <summary>
        /// Video zooming in command.
        /// </summary>
        public static RoutedUICommand ZoomInCommand
        {
            get { return _zoomInCommand; }
        }

        /// <summary>
        /// Video zooming out command.
        /// </summary>
        public static RoutedUICommand ZoomOutCommand
        {
            get { return _zoomOutCommand; }
        }

        private static void InitializeCommands()
        {
            _zoomInCommand = new RoutedUICommand(
                                    Properties.Resources.STR_Command_ZoomIn,
                                    "ZoomIn",
                                    typeof(ZoomableRenderElement));
            _zoomOutCommand = new RoutedUICommand(
                                    Properties.Resources.STR_Command_ZoomIn,
                                    "ZoomOut",
                                    typeof(ZoomableRenderElement));

            CommandHelpers.RegisterCommandHandler(
                                typeof(ZoomableRenderElement),
                                ZoomInCommand,
                                OnZoomIn,
                                OnCanZoomIn);
            CommandHelpers.RegisterCommandHandler(
                                typeof(ZoomableRenderElement),
                                ZoomOutCommand,
                                OnZoomOut,
                                OnCanZoomOut);
        }

        private static void OnCanZoomOut(object sender, CanExecuteRoutedEventArgs e)
        {
            ZoomableRenderElement element = sender as ZoomableRenderElement;
            e.CanExecute = element.GetCanZoomOut();
        }

        private static void OnZoomOut(object sender, ExecutedRoutedEventArgs e)
        {
            ZoomableRenderElement element = sender as ZoomableRenderElement;
            if (!element.GetCanZoomOut())
                return;

            double oldZoomFactor = element.ZoomFactor;
            double curZoomFactor = oldZoomFactor * (-(double)Mouse.MouseWheelDeltaForOneLine / 10000.0 * element.ZoomDelta + 1);

            if (curZoomFactor < element.MinZoomFactor)
            {
                curZoomFactor = element.MinZoomFactor;
            }

            curZoomFactor = Math.Max(element.MinZoomFactor, curZoomFactor);

            element.SetCurrentValue(ZoomFactorProperty, curZoomFactor);
            CommandManager.InvalidateRequerySuggested();
        }

        private static void OnCanZoomIn(object sender, CanExecuteRoutedEventArgs e)
        {
            ZoomableRenderElement element = sender as ZoomableRenderElement;
            e.CanExecute = element.GetCanZoomIn();
        }

        private static void OnZoomIn(object sender, ExecutedRoutedEventArgs e)
        {
            ZoomableRenderElement element = sender as ZoomableRenderElement;
            if (!element.GetCanZoomIn())
                return;

            double oldZoomFactor = element.ZoomFactor;
            double curZoomFactor = oldZoomFactor * ((double)Mouse.MouseWheelDeltaForOneLine / 10000.0 * element.ZoomDelta + 1);

            if (curZoomFactor > element.MaxZoomFactor)
            {
                curZoomFactor = element.MaxZoomFactor;
            }

            curZoomFactor = Math.Min(element.MaxZoomFactor, curZoomFactor);

            element.SetCurrentValue(ZoomFactorProperty, curZoomFactor);

            CommandManager.InvalidateRequerySuggested();
        }

        /// <summary>
        /// 缩小媒体渲染的视频画面。
        /// </summary>
        /// <param name="curZoomFactor">当前缩放比例参数。</param>
        /// <param name="oldZoomFactor">前一缩放比例参数。</param>
        protected virtual void ZoomOut(double oldZoomFactor, double curZoomFactor)
        {

        }

        /// <summary>
        /// 放大媒体渲染的视频画面。
        /// </summary>
        /// <param name="curZoomFactor">当前缩放比例参数。</param>
        /// <param name="oldZoomFactor">前一缩放比例参数。</param>
        protected virtual void ZoomIn(double oldZoomFactor, double curZoomFactor)
        {

        }

        /// <summary>
        /// 获取一个值指示是否可以按照现有缩放参数进行画面缩小。
        /// </summary>
        /// <returns></returns>
        protected virtual bool GetCanZoomOut()
        {
            double zoomFactor = ZoomFactor;
            return HasInitialized && zoomFactor > MinZoomFactor && PlayState != MediaPlayState.None;
        }

        /// <summary>
        /// 获取一个值指示是否可以按照现有缩放参数进行画面放大。
        /// </summary>
        /// <returns></returns>
        protected virtual bool GetCanZoomIn()
        {
            double zoomFactor = ZoomFactor;
            return HasInitialized && zoomFactor < MaxZoomFactor && PlayState != MediaPlayState.None;
        }

        #endregion

        static ZoomableRenderElement()
        {
            InitializeCommands();
            // Mouse wheel zooming implemention
            //EventManager.RegisterClassHandler(
            //     typeof(ZoomableRenderElement),
            //     Mouse.MouseWheelEvent,
            //     new MouseWheelEventHandler(_MouseWhellEvent), true);
        }

        /// <summary>
        /// 构造 <see cref="ZoomableRenderElement"/> 类的实例。
        /// </summary>
        protected ZoomableRenderElement()
        {

        }

        /// <summary>
        /// 获取当前渲染元素对应的媒体播放器。
        /// </summary>
        protected new IZoomableMediaPlayer MediaPlayer
        {
            get
            {
                return base.MediaPlayer as IZoomableMediaPlayer;
            }
        }

        /// <summary>
        /// 释放播放资源。
        /// </summary>
        /// <param name="disposing">一个值指示是否正在经由调用 <see cref="Dispose"/> 方法进行资源释放。</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
            if (!EnableMouseWheelZooming)
            {
                return;
            }

            if (e.Delta > 0)
            {
                if (GetCanZoomIn())
                {
                    var oldZoomFactor = ZoomFactor;
                    var curZoomFactor = oldZoomFactor * (e.Delta / 10000.0 * ZoomDelta + 1);

                    if (curZoomFactor > MaxZoomFactor)
                    {
                        curZoomFactor = MaxZoomFactor;
                    }

                    curZoomFactor = Math.Min(MaxZoomFactor, curZoomFactor);

                    SetCurrentValue(ZoomFactorProperty, curZoomFactor);
                    CommandManager.InvalidateRequerySuggested();
                }
                e.Handled = true;
            }
            else if (e.Delta < 0)
            {
                if (GetCanZoomOut())
                {
                    var oldZoomFactor = ZoomFactor;
                    var curZoomFactor = oldZoomFactor * (e.Delta / 10000.0 * ZoomDelta + 1);

                    if (curZoomFactor < MinZoomFactor)
                    {
                        curZoomFactor = MinZoomFactor;
                    }

                    curZoomFactor = Math.Max(MinZoomFactor, curZoomFactor);

                    SetCurrentValue(ZoomFactorProperty, curZoomFactor);
                    CommandManager.InvalidateRequerySuggested();
                }
                e.Handled = true;
            }
        }

        private static void _MouseWhellEvent(object sender, MouseWheelEventArgs e)
        {
            //var parent = (e.OriginalSource as UIElement).GetVisualParent<ZoomableRenderElement>();

            //if (!parent.EnableMouseWheelZooming)
            //{
            //    return;
            //}

            //if (e.Delta > 0)
            //{
            //    if (GetCanZoomIn())
            //    {
            //        var oldZoomFactor = ZoomFactor;
            //        var curZoomFactor = oldZoomFactor * (e.Delta / 10000.0 * ZoomDelta + 1);

            //        if (curZoomFactor > MaxZoomFactor)
            //        {
            //            curZoomFactor = MaxZoomFactor;
            //        }

            //        curZoomFactor = Math.Min(MaxZoomFactor, curZoomFactor);

            //        SetCurrentValue(ZoomFactorProperty, curZoomFactor);
            //        CommandManager.InvalidateRequerySuggested();
            //    }
            //}
            //else if (e.Delta < 0)
            //{
            //    if (GetCanZoomOut())
            //    {
            //        var oldZoomFactor = ZoomFactor;
            //        var curZoomFactor = oldZoomFactor * (e.Delta / 10000.0 * ZoomDelta + 1);

            //        if (curZoomFactor < MinZoomFactor)
            //        {
            //            curZoomFactor = MinZoomFactor;
            //        }

            //        curZoomFactor = Math.Max(MinZoomFactor, curZoomFactor);

            //        SetCurrentValue(ZoomFactorProperty, curZoomFactor);
            //        CommandManager.InvalidateRequerySuggested();
            //    }
            //}
        }
    }
}
