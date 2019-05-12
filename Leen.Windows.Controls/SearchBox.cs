using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace Leen.Windows.Controls
{
    /// <summary>
    /// 支持水印的搜索框。
    /// </summary>
    [TemplatePart(Name = PART_CLEAR_BUTTON, Type = typeof(ButtonBase))]
    [TemplateVisualState(GroupName = VISUAL_STATE_WATERMARK_STATES, Name = VISUAL_STATE_WATERMARK_VISIBLE)]
    [TemplateVisualState(GroupName = VISUAL_STATE_WATERMARK_STATES, Name = VISUAL_STATE_SEARCH_BUTTONS_VISIBLE)]
    [TemplateVisualState(GroupName = VISUAL_STATE_WATERMARK_STATES, Name = VISUAL_STATE_WATERMARK_HIDDEN)]
    public class SearchBox : TextBox
    {
        #region private fields

        private const string VISUAL_STATE_SEARCH_BUTTONS_VISIBLE = "SearchOptionVisible";
        private const string VISUAL_STATE_WATERMARK_HIDDEN = "WatermarkHidden";
        private const string VISUAL_STATE_WATERMARK_STATES = "WatermarkStates";
        private const string VISUAL_STATE_WATERMARK_VISIBLE = "WatermarkVisible";
        private const string PART_CLEAR_BUTTON = "PART_ClearButton";


        private ButtonBase _clearButton;
        private int _millisecondsDelay = 300;
        private readonly DispatcherTimer _textTransferTimer = new DispatcherTimer();

        #endregion

        #region SearchTextProperty

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty SearchTextProperty =
            DependencyProperty.Register("SearchText", typeof(string), typeof(SearchBox),
                new FrameworkPropertyMetadata(string.Empty, new PropertyChangedCallback(searchTextPropertyChanged)));

        /// <summary>
        /// 
        /// </summary>
        public string SearchText
        {
            get
            {
                return (string)base.GetValue(SearchTextProperty);
            }
            set
            {
                base.SetValue(SearchTextProperty, value);
            }
        }

        private static void searchTextPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            SearchBox box = sender as SearchBox;
            if (box.Text != box.SearchText)
            {
                box.SetCurrentValue(TextBox.TextProperty, e.NewValue);
                box.SelectionStart = box.Text.Length;
            }
        }

        #endregion

        #region WatermarkTextProperty

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty WatermarkTextProperty = DependencyProperty.Register("WatermarkText", typeof(string), typeof(SearchBox));

        /// <summary>
        /// 
        /// </summary>
        public string WatermarkText
        {
            get
            {
                return (string)base.GetValue(WatermarkTextProperty);
            }
            set
            {
                base.SetValue(WatermarkTextProperty, value);
            }
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public int MillisecondsDelay
        {
            get
            {
                return _millisecondsDelay;
            }
            set
            {
                if (_millisecondsDelay != value)
                {
                    _millisecondsDelay = value;
                    _textTransferTimer.Interval = TimeSpan.FromMilliseconds(_millisecondsDelay);
                }
            }
        }

        #region 构造函数

        public SearchBox()
        {
            _textTransferTimer.Interval = TimeSpan.FromMilliseconds(_millisecondsDelay);
            IsKeyboardFocusWithinChanged += new DependencyPropertyChangedEventHandler(SearchBox_IsKeyboardFocusWithinChanged);
            Loaded += new RoutedEventHandler(SearchBox_Loaded);
            Unloaded += SearchBox_Unloaded;
        }

        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (_clearButton != null)
            {
                _clearButton.Click -= new RoutedEventHandler(_clearTextBoxElement_Click);
            }
            _clearButton = GetTemplateChild("PART_ClearButton") as ButtonBase;
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
            SetCurrentValue(SearchTextProperty, base.Text);
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
            if (string.IsNullOrEmpty(base.Text) && !base.IsKeyboardFocusWithin)
            {
                VisualStateManager.GoToState(this, VISUAL_STATE_WATERMARK_VISIBLE, true);
            }
            else if (string.IsNullOrEmpty(base.Text))
            {
                VisualStateManager.GoToState(this, VISUAL_STATE_WATERMARK_HIDDEN, true);
            }
            else
            {
                VisualStateManager.GoToState(this, VISUAL_STATE_SEARCH_BUTTONS_VISIBLE, true);
            }
        }

        #endregion
    }
}
