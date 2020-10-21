using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;

namespace Leen.Windows.Controls
{
    /// <summary>
    /// 支持水印的搜索框。
    /// </summary>
    [TemplatePart(Name = PART_ClearButton, Type = typeof(ButtonBase))]
    [TemplateVisualState(GroupName = STATEGROUP_WatermarkStates, Name = STATE_WatermarkVisible)]
    [TemplateVisualState(GroupName = STATEGROUP_WatermarkStates, Name = STATE_WatermarkDismiss)]
    [TemplateVisualState(GroupName = STATEGROUP_WatermarkStates, Name = STATE_WatermarkHidden)]
    public class SearchBox : RoundedTextBox
    {
        #region private fields

        private const string STATE_WatermarkDismiss = "WatermarkDismiss";
        private const string STATE_WatermarkHidden = "WatermarkHidden";
        private const string STATEGROUP_WatermarkStates = "WatermarkStates";
        private const string STATE_WatermarkVisible = "WatermarkVisible";
        private const string PART_ClearButton = "PART_ClearButton";

        private ButtonBase _clearButton;
        private readonly DispatcherTimer _textTransferTimer = new DispatcherTimer();

        #endregion

        #region SearchTextProperty

        public static readonly DependencyProperty SearchTextProperty =
            DependencyProperty.Register(nameof(SearchText),
                                        typeof(string),
                                        typeof(SearchBox),
                                        new FrameworkPropertyMetadata(string.Empty, new PropertyChangedCallback(searchTextPropertyChanged)));

        public string SearchText
        {
            get
            {
                return (string)GetValue(SearchTextProperty);
            }
            set
            {
                SetValue(SearchTextProperty, value);
            }
        }

        private static void searchTextPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            SearchBox box = sender as SearchBox;
            if (box.Text != box.SearchText)
            {
                box.SetCurrentValue(TextProperty, e.NewValue);
                box.SelectionStart = box.Text.Length;
            }
        }

        #endregion

        #region SearchDelayProperty

        public double SearchDelay
        {
            get { return (double)GetValue(SearchDelayProperty); }
            set { SetValue(SearchDelayProperty, value); }
        }

        public static readonly DependencyProperty SearchDelayProperty =
            DependencyProperty.Register(nameof(SearchDelay),
                                        typeof(double),
                                        typeof(SearchBox),
                                        new PropertyMetadata(300d, SearchDelayPropertyChanged));

        private static void SearchDelayPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var searchBox = d as SearchBox;
            searchBox.OnSearchDelayChanged((double)e.NewValue);
        }

        protected void OnSearchDelayChanged(double newValue)
        {
            _textTransferTimer.Interval = TimeSpan.FromMilliseconds(newValue);
        }

        #endregion

        #region WatermarkTextProperty

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty WatermarkTextProperty = 
            DependencyProperty.Register(nameof(WatermarkText),
                                        typeof(string),
                                        typeof(SearchBox));

        /// <summary>
        /// 
        /// </summary>
        public string WatermarkText
        {
            get
            {
                return (string)GetValue(WatermarkTextProperty);
            }
            set
            {
                SetValue(WatermarkTextProperty, value);
            }
        }

        #endregion

        #region WatermarkForegroundProperty

        public Brush WatermarkForeground
        {
            get { return (Brush)GetValue(WatermarkForegroundProperty); }
            set { SetValue(WatermarkForegroundProperty, value); }
        }

        public static readonly DependencyProperty WatermarkForegroundProperty =
            DependencyProperty.Register(nameof(WatermarkForeground), typeof(Brush), typeof(SearchBox), new PropertyMetadata(Brushes.DimGray));

        #endregion

        #region 构造函数

        public SearchBox()
        {
            _textTransferTimer.Interval = TimeSpan.FromMilliseconds(SearchDelay);
            IsKeyboardFocusWithinChanged += new DependencyPropertyChangedEventHandler(SearchBox_IsKeyboardFocusWithinChanged);
            Loaded += new RoutedEventHandler(SearchBox_Loaded);
            Unloaded += SearchBox_Unloaded;
        }

        static SearchBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SearchBox), new FrameworkPropertyMetadata(typeof(SearchBox)));
        }

        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (_clearButton != null)
            {
                _clearButton.Click -= new RoutedEventHandler(_clearTextBoxElement_Click);
            }
            _clearButton = GetTemplateChild(PART_ClearButton) as ButtonBase;
            if (_clearButton != null)
            {
                _clearButton.Click += new RoutedEventHandler(_clearTextBoxElement_Click);
            }
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            _textTransferTimer.Stop();
            base.OnTextChanged(e);
            _textTransferTimer.Start();
            UpdateVisualState();
        }

        #region private helpers

        private void _clearTextBoxElement_Click(object sender, RoutedEventArgs e)
        {
            SetCurrentValue(SearchTextProperty, string.Empty);
        }

        private void _textTransferTimer_Tick(object sender, EventArgs e)
        {
            SetCurrentValue(SearchTextProperty, Text);
            _textTransferTimer.Stop();
        }

        private void SearchBox_IsKeyboardFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdateVisualState();
        }

        private void SearchBox_Loaded(object sender, RoutedEventArgs e)
        {
            _textTransferTimer.Tick += new EventHandler(_textTransferTimer_Tick);
            ContextMenu = null;
            UpdateVisualState();
        }

        private void SearchBox_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_clearButton != null)
            {
                _clearButton.Click -= new RoutedEventHandler(_clearTextBoxElement_Click);
            }

            _textTransferTimer.Tick -= new EventHandler(_textTransferTimer_Tick);
        }

        private void UpdateVisualState()
        {
            if (string.IsNullOrEmpty(Text) && !IsKeyboardFocusWithin)
            {
                VisualStateManager.GoToState(this, STATE_WatermarkVisible, true);
            }
            else if (string.IsNullOrEmpty(Text))
            {
                VisualStateManager.GoToState(this, STATE_WatermarkHidden, true);
            }
            else
            {
                VisualStateManager.GoToState(this, STATE_WatermarkDismiss, true);
            }
        }

        #endregion
    }
}
