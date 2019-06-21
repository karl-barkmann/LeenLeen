using System.Globalization;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace System.Windows.Interactivity
{
    /// <summary>
    /// 实现一种使 <see cref="TextBox"/> 控件支持水印文字显示的行为。
    /// </summary>
    public class WatermarkBehavior : Behavior<Control>
    {
        private WatermarkAdorner _adorner;
        private AdornerLayer _adornerLayer;

        #region WatermarkText

        /// <summary>
        /// 获取或设置要显示的水印文字。
        /// </summary>
        public string WatermarkText
        {
            get { return (string)GetValue(WatermarkTextProperty); }
            set { SetValue(WatermarkTextProperty, value); }
        }

        /// <summary>
        /// 获取 <see cref="WatermarkText"/> 的依赖属性。
        /// </summary>
        public static readonly DependencyProperty WatermarkTextProperty =
            DependencyProperty.Register(nameof(WatermarkText), typeof(string), typeof(WatermarkBehavior),
            new PropertyMetadata(string.Empty, WatermarkTextPropertyChanged));

        private static void WatermarkTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var watermarkBehavior = d as WatermarkBehavior;
            watermarkBehavior.OnWatermarkTextChanged((string)e.OldValue, (string)e.NewValue);
        }

        private void OnWatermarkTextChanged(string oldValue, string newValue)
        {
            if (oldValue != newValue)
            {
                if (_adornerLayer != null)
                {
                    if (_adorner != null)
                    {
                        _adornerLayer.Remove(_adorner);
                    }
                    _adorner = CreateProperAdorner();
                    _adornerLayer.Add(_adorner);
                }
            }
        }

        #endregion

        #region WatermarkForeground

        /// <summary>
        /// Gets or sets the <see cref="WatermarkForeground"/> value.
        /// </summary>
        public Brush WatermarkForeground
        {
            get { return (Brush)GetValue(WatermarkForegroundProperty); }
            set { SetValue(WatermarkForegroundProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="WatermarkForeground"/> property.
        /// </summary>
        public static readonly DependencyProperty WatermarkForegroundProperty =
            DependencyProperty.Register(
                                nameof(WatermarkForeground),
                                typeof(Brush),
                                typeof(WatermarkBehavior),
                                new PropertyMetadata(Brushes.White, WatermarkForegroundPropertyChanged));

        private static void WatermarkForegroundPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((WatermarkBehavior)d).OnWatermarkForegroundChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        /// <summary>
        /// Called when <see cref="WatermarkForeground"/> changed.
        /// </summary>
        /// <param name="oldValue">Old value of <see cref="WatermarkForeground"/>.</param>
        /// <param name="newValue">New value of <see cref="WatermarkForeground"/>.</param>
        protected virtual void OnWatermarkForegroundChanged(Brush oldValue, Brush newValue)
        {

        }

        #endregion

        #region WatermarkFontFamily

        /// <summary>
        /// Gets or sets the <see cref="WatermarkFontFamily"/> value.
        /// </summary>
        public FontFamily WatermarkFontFamily
        {
            get { return (FontFamily)GetValue(WatermarkFontFamilyProperty); }
            set { SetValue(WatermarkFontFamilyProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="WatermarkFontFamily"/> property.
        /// </summary>
        public static readonly DependencyProperty WatermarkFontFamilyProperty =
            DependencyProperty.Register(
                                nameof(WatermarkFontFamily),
                                typeof(FontFamily),
                                typeof(WatermarkBehavior),
                                new PropertyMetadata(SystemFonts.CaptionFontFamily, WatermarkFontFamilyPropertyChanged));

        private static void WatermarkFontFamilyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((WatermarkBehavior)d).OnWatermarkFontFamilyChanged((FontFamily)e.OldValue, (FontFamily)e.NewValue);
        }

        /// <summary>
        /// Called when <see cref="WatermarkFontFamily"/> changed.
        /// </summary>
        /// <param name="oldValue">Old value of <see cref="WatermarkFontFamily"/>.</param>
        /// <param name="newValue">New value of <see cref="WatermarkFontFamily"/>.</param>
        protected virtual void OnWatermarkFontFamilyChanged(FontFamily oldValue, FontFamily newValue)
        {

        }

        #endregion

        #region WatermarkFontSize

        /// <summary>
        /// Gets or sets the <see cref="WatermarkFontSize"/> value.
        /// </summary>
        public double WatermarkFontSize
        {
            get { return (double)GetValue(WatermarkFontSizeProperty); }
            set { SetValue(WatermarkFontSizeProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="WatermarkFontSize"/> property.
        /// </summary>
        public static readonly DependencyProperty WatermarkFontSizeProperty =
            DependencyProperty.Register(
                                nameof(WatermarkFontSize),
                                typeof(double),
                                typeof(WatermarkBehavior),
                                new PropertyMetadata(SystemFonts.CaptionFontSize, WatermarkFontSizePropertyChanged));

        private static void WatermarkFontSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((WatermarkBehavior)d).OnWatermarkFontSizeChanged((double)e.OldValue, (double)e.NewValue);
        }

        /// <summary>
        /// Called when <see cref="WatermarkFontSize"/> changed.
        /// </summary>
        /// <param name="oldValue">Old value of <see cref="WatermarkFontSize"/>.</param>
        /// <param name="newValue">New value of <see cref="WatermarkFontSize"/>.</param>
        protected virtual void OnWatermarkFontSizeChanged(double oldValue, double newValue)
        {

        }

        #endregion

        #region WatermarkFontStyle

        /// <summary>
        /// Gets or sets the <see cref="WatermarkFontStyle"/> value.
        /// </summary>
        public FontStyle WatermarkFontStyle
        {
            get { return (FontStyle)GetValue(WatermarkFontStyleProperty); }
            set { SetValue(WatermarkFontStyleProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="WatermarkFontStyle"/> property.
        /// </summary>
        public static readonly DependencyProperty WatermarkFontStyleProperty =
            DependencyProperty.Register(
                                nameof(WatermarkFontStyle),
                                typeof(FontStyle),
                                typeof(WatermarkBehavior),
                                new PropertyMetadata(SystemFonts.CaptionFontStyle, WatermarkFontStylePropertyChanged));

        private static void WatermarkFontStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((WatermarkBehavior)d).OnWatermarkFontStyleChanged((FontStyle)e.OldValue, (FontStyle)e.NewValue);
        }

        /// <summary>
        /// Called when <see cref="WatermarkFontStyle"/> changed.
        /// </summary>
        /// <param name="oldValue">Old value of <see cref="WatermarkFontStyle"/>.</param>
        /// <param name="newValue">New value of <see cref="WatermarkFontStyle"/>.</param>
        protected virtual void OnWatermarkFontStyleChanged(FontStyle oldValue, FontStyle newValue)
        {

        }

        #endregion

        #region WatermarkFontStretch

        /// <summary>
        /// Gets or sets the <see cref="WatermarkFontStretch"/> value.
        /// </summary>
        public FontStretch WatermarkFontStretch
        {
            get { return (FontStretch)GetValue(WatermarkFontStretchProperty); }
            set { SetValue(WatermarkFontStretchProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="WatermarkFontStretch"/> property.
        /// </summary>
        public static readonly DependencyProperty WatermarkFontStretchProperty =
            DependencyProperty.Register(
                                nameof(WatermarkFontStretch),
                                typeof(FontStretch),
                                typeof(WatermarkBehavior),
                                new PropertyMetadata(FontStretches.Normal, WatermarkFontStretchPropertyChanged));

        private static void WatermarkFontStretchPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((WatermarkBehavior)d).OnWatermarkFontStretchChanged((FontStretch)e.OldValue, (FontStretch)e.NewValue);
        }

        /// <summary>
        /// Called when <see cref="WatermarkFontStretch"/> changed.
        /// </summary>
        /// <param name="oldValue">Old value of <see cref="WatermarkFontStretch"/>.</param>
        /// <param name="newValue">New value of <see cref="WatermarkFontStretch"/>.</param>
        protected virtual void OnWatermarkFontStretchChanged(FontStretch oldValue, FontStretch newValue)
        {

        }

        #endregion

        #region WatermarkFontWeight

        /// <summary>
        /// Gets or sets the <see cref="WatermarkFontWeight"/> value.
        /// </summary>
        public FontWeight WatermarkFontWeight
        {
            get { return (FontWeight)GetValue(WatermarkFontWeightProperty); }
            set { SetValue(WatermarkFontWeightProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="WatermarkFontWeight"/> property.
        /// </summary>
        public static readonly DependencyProperty WatermarkFontWeightProperty =
            DependencyProperty.Register(
                                nameof(WatermarkFontWeight),
                                typeof(FontWeight),
                                typeof(WatermarkBehavior),
                                new PropertyMetadata(FontWeights.Normal, WatermarkFontWeightPropertyChanged));

        private static void WatermarkFontWeightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((WatermarkBehavior)d).OnWatermarkFontWeightChanged((FontWeight)e.OldValue, (FontWeight)e.NewValue);
        }

        /// <summary>
        /// Called when <see cref="WatermarkFontWeight"/> changed.
        /// </summary>
        /// <param name="oldValue">Old value of <see cref="WatermarkFontWeight"/>.</param>
        /// <param name="newValue">New value of <see cref="WatermarkFontWeight"/>.</param>
        protected virtual void OnWatermarkFontWeightChanged(FontWeight oldValue, FontWeight newValue)
        {

        }

        #endregion

        #region IsEnable

        /// <summary>
        /// 获取或设置一个值指示是否启用显示水印，设置这个值表示是否启用水印。
        /// </summary>
        public bool IsEnabled
        {
            get { return (bool)GetValue(IsEnabledProperty); }
            set { SetValue(IsEnabledProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="IsEnabled"/> property.
        /// </summary>
        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.Register(
                                nameof(IsEnabled),
                                typeof(bool),
                                typeof(WatermarkBehavior),
                                new PropertyMetadata(true, IsEnabledPropertyChanged));

        private static void IsEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((WatermarkBehavior)d).OnIsEnabledChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        /// <summary>
        /// Called when <see cref="IsEnabled"/> changed.
        /// </summary>
        /// <param name="oldValue">Old value of <see cref="IsEnabled"/>.</param>
        /// <param name="newValue">New value of <see cref="IsEnabled"/>.</param>
        protected virtual void OnIsEnabledChanged(bool oldValue, bool newValue)
        {
            if (newValue)
                AddAdorner();
            else
                RemoveAdorner();
        }

        #endregion

        /// <summary>
        /// 当行为附加到控件时发生。
        /// </summary>
        protected override void OnAttached()
        {
            AssociatedObject.IsVisibleChanged += AssociatedObject_IsVisibleChanged;

            if (AssociatedObject.IsLoaded)
            {
                if(IsEnabled)
                    AddAdorner();
            }
            else
            {
                AssociatedObject.Loaded += AssociatedObject_Loaded;
            }
        }

        /// <summary>
        /// 当行为从当前 <see cref="TextBox"/> 控件分离时发生。
        /// </summary>
        protected override void OnDetaching()
        {
            AssociatedObject.IsVisibleChanged -= AssociatedObject_IsVisibleChanged;
            RemoveAdorner();
            base.OnDetaching();
        }

        private void AssociatedObject_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue && IsEnabled)
            {
                AddAdorner();
            }
            else
            {
                RemoveAdorner();
            }
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            if(IsEnabled)
                AddAdorner();
            AssociatedObject.Loaded -= AssociatedObject_Loaded;
            base.OnAttached();
        }

        private static AdornerLayer GetAdornerLayer(Visual dp)
        {
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(dp);
            if (adornerLayer == null)
            {
                var ownerWindow = Window.GetWindow(dp);
                if (ownerWindow != null)
                {
                    var adornerDecorator = ownerWindow.GetVisualChild<AdornerDecorator>();
                    if (adornerDecorator != null)
                    {
                        adornerLayer = adornerDecorator.AdornerLayer;
                    }
                }
            }

            return adornerLayer;
        }

        private void AddAdorner()
        {
            if (_adorner == null)
            {
                _adorner = CreateProperAdorner();
                _adornerLayer = GetAdornerLayer(AssociatedObject);
                if (_adornerLayer != null && !_adornerLayer.IsAncestorOf(_adorner))
                {
                    _adornerLayer.Add(_adorner);
                }
                _adorner.Hook();
            }
        }

        private void RemoveAdorner()
        {
            if (_adorner != null)
            {
                if (_adornerLayer != null)
                {
                    _adornerLayer.Remove(_adorner);
                }
                _adorner.UnHook();
                _adorner = null;
            }
        }

        private WatermarkAdorner CreateProperAdorner()
        {
            WatermarkAdorner adorner;
            switch (AssociatedObject)
            {
                case TextBox textbox:
                    adorner = new TextboxWatermarkAdorner(textbox, WatermarkText);
                    break;
                case PasswordBox passwordBox:
                    adorner = new PasswordBoxWatermarkAdorner(passwordBox, WatermarkText);
                    break;
                case ComboBox comboBox:
                    adorner = new ComboBoxWatermarkAdorner(comboBox, WatermarkText);
                    break;
                default:
                    adorner = new WatermarkAdorner(AssociatedObject, WatermarkText);
                    break;
            }

            adorner.FontFamily = WatermarkFontFamily;
            adorner.FontSize = WatermarkFontSize;
            adorner.FontStretch = WatermarkFontStretch;
            adorner.FontStyle = WatermarkFontStyle;
            adorner.FontWeight = WatermarkFontWeight;
            adorner.Foreground = WatermarkForeground;
            return adorner;
        }
    }

    class WatermarkAdorner : Adorner 
    {
        public WatermarkAdorner(Control target, string watermark)
            : base(target)
        {
            IsHitTestVisible = false;
            Watermark = watermark;
        }

        public double FontSize { get; set; }

        public Brush Foreground { get; set; }

        public FontFamily FontFamily { get; set; }

        public FontWeight FontWeight { get; set; }

        public FontStretch FontStretch { get; set; }

        public FontStyle FontStyle { get; set; }

        public string Watermark { get; }

        public virtual new Control AdornedElement
        {
            get
            {
                return base.AdornedElement as Control;
            }
        }

        public virtual void Hook()
        {
            AdornedElement.Loaded += adornedTextBox_Loaded;
            AdornedElement.GotFocus += adornedTextBox_GotFocus;
            AdornedElement.LostFocus += adornedTextBox_LostFocus;
        }

        public virtual void UnHook()
        {
            AdornedElement.Loaded -= adornedTextBox_Loaded;
            AdornedElement.GotFocus -= adornedTextBox_GotFocus;
            AdornedElement.LostFocus -= adornedTextBox_LostFocus;
        }

        protected override void OnRender(DrawingContext dc)
        {
            if (CanRender())
            {
                var typeface = new Typeface(FontFamily, FontStyle, FontWeight, FontStretch);
                var fmt = new FormattedText(Watermark,
                                            CultureInfo.CurrentUICulture,
                                            AdornedElement.FlowDirection,
                                            typeface,
                                            FontSize,
                                            Foreground);
                dc.DrawText(fmt, new Point(AdornedElement.Padding.Left, (AdornedElement.RenderSize.Height - fmt.Height) / 2));
            }
        }

        protected virtual bool CanRender()
        {
            return !AdornedElement.IsFocused;
        }

        void adornedTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            InvalidateVisual();
        }

        void adornedTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            InvalidateVisual();
        }

        void adornedTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            InvalidateVisual();
        }
    }

    class TextboxWatermarkAdorner : WatermarkAdorner
    {
        public TextboxWatermarkAdorner(TextBox target, string watermark)
            : base(target, watermark)
        {
            Target = target;
        }

        public TextBox Target
        {
            get;
        }

        protected override bool CanRender()
        {
            return string.IsNullOrEmpty(Target.Text) && base.CanRender();
        }

        public override void Hook()
        {
            base.Hook();
            Target.TextChanged += adornedTextBox_TextChanged;
        }

        public override void UnHook()
        {
            base.UnHook();
            Target.TextChanged -= adornedTextBox_TextChanged;
        }

        private void adornedTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            InvalidateVisual();
        }
    }

    class PasswordBoxWatermarkAdorner : WatermarkAdorner
    {
        public PasswordBoxWatermarkAdorner(PasswordBox target, string watermark) : base(target, watermark)
        {
            Target = target;
        }

        public PasswordBox Target { get; }

        protected override bool CanRender()
        {
            return string.IsNullOrEmpty(Target.Password) && base.CanRender();
        }

        public override void Hook()
        {
            base.Hook();
            Target.PasswordChanged += Target_PasswordChanged;
        }

        public override void UnHook()
        {
            base.UnHook();
            Target.PasswordChanged -= Target_PasswordChanged;
        }

        private void Target_PasswordChanged(object sender, RoutedEventArgs e)
        {
            InvalidateVisual();
        }
    }

    class ComboBoxWatermarkAdorner : WatermarkAdorner
    {
        public ComboBoxWatermarkAdorner(ComboBox target, string watermark) : base(target, watermark)
        {
            Target = target;
        }

        public ComboBox Target { get; }

        protected override bool CanRender()
        {
            return Target.SelectedItem == null && base.CanRender();
        }

        public override void Hook()
        {
            base.Hook();
            Target.SelectionChanged += Target_SelectionChanged;
        }

        public override void UnHook()
        {
            base.UnHook();
            Target.SelectionChanged -= Target_SelectionChanged;
        }

        private void Target_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            InvalidateVisual();
        }
    }
}
