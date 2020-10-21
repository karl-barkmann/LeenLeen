using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Leen.Windows.Controls
{
    [TemplatePart(Name = PART_Popup, Type = typeof(Popup))]
    [TemplatePart(Name = PART_TextInput, Type = typeof(TextBox))]
    [TemplatePart(Name = PART_Hours, Type = typeof(ListBox))]
    [TemplatePart(Name = PART_Minutes, Type = typeof(ListBox))]
    [TemplatePart(Name = PART_Seconds, Type = typeof(ListBox))]
    public class TimePicker : Control
    {
        public const string PART_Popup = "PART_Popup";
        public const string PART_Hours = "PART_Hours";
        public const string PART_Minutes = "PART_Minutes";
        public const string PART_Seconds = "PART_Seconds";
        public const string PART_TextInput = "PART_TextInput";

        private ListBox _partHours;
        private ListBox _partMinutes;
        private ListBox _partSeconds;
        private TextBox _partInput;
        private Popup _partPopup;

        static TimePicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TimePicker), new FrameworkPropertyMetadata(typeof(TimePicker)));
        }
        
        public TimePicker()
        {

        }

        #region TimeFormat

        /// <summary>
        /// Gets or sets the <see cref="TimeFormat"/> value.
        /// </summary>
        public string TimeFormat
        {
            get { return (string)GetValue(TimeFormatProperty); }
            set { SetValue(TimeFormatProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="TimeFormat"/> property.
        /// </summary>
        public static readonly DependencyProperty TimeFormatProperty =
            DependencyProperty.Register(
                                nameof(TimeFormat),
                                typeof(string),
                                typeof(TimePicker),
                                new PropertyMetadata("hh:mm:ss", TimeFormatPropertyChanged));

        private static void TimeFormatPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TimePicker)d).OnTimeFormatChanged((string)e.OldValue, (string)e.NewValue);
        }

        /// <summary>
        /// Called when <see cref="TimeFormat"/> changed.
        /// </summary>
        /// <param name="oldValue">Old value of <see cref="TimeFormat"/>.</param>
        /// <param name="newValue">New value of <see cref="TimeFormat"/>.</param>
        protected virtual void OnTimeFormatChanged(string oldValue, string newValue)
        {

        }

        #endregion

        #region BeginTime

        /// <summary>
        /// Gets or sets the <see cref="BeginTime"/> value.
        /// </summary>
        public TimeSpan BeginTime
        {
            get { return (TimeSpan)GetValue(BeginTimeProperty); }
            set { SetValue(BeginTimeProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="BeginTime"/> property.
        /// </summary>
        public static readonly DependencyProperty BeginTimeProperty =
            DependencyProperty.Register(
                                nameof(BeginTime),
                                typeof(TimeSpan),
                                typeof(TimePicker),
                                new PropertyMetadata(TimeSpan.Zero, BeginTimePropertyChanged));

        private static void BeginTimePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TimePicker)d).OnBeginTimeChanged((TimeSpan)e.OldValue, (TimeSpan)e.NewValue);
        }

        /// <summary>
        /// Called when <see cref="BeginTime"/> changed.
        /// </summary>
        /// <param name="oldValue">Old value of <see cref="BeginTime"/>.</param>
        /// <param name="newValue">New value of <see cref="BeginTime"/>.</param>
        protected virtual void OnBeginTimeChanged(TimeSpan oldValue, TimeSpan newValue)
        {

        }

        #endregion

        #region SelectedTime

        /// <summary>
        /// Gets or sets the <see cref="SelectedTime"/> value.
        /// </summary>
        public TimeSpan SelectedTime
        {
            get { return (TimeSpan)GetValue(SelectedTimeProperty); }
            set { SetValue(SelectedTimeProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="SelectedTime"/> property.
        /// </summary>
        public static readonly DependencyProperty SelectedTimeProperty =
            DependencyProperty.Register(
                                nameof(SelectedTime),
                                typeof(TimeSpan),
                                typeof(TimePicker),
                                new PropertyMetadata(TimeSpan.Zero, SelectedTimePropertyChanged));

        private static void SelectedTimePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TimePicker)d).OnSelectedTimeChanged((TimeSpan)e.OldValue, (TimeSpan)e.NewValue);
        }

        /// <summary>
        /// Called when <see cref="SelectedTime"/> changed.
        /// </summary>
        /// <param name="oldValue">Old value of <see cref="SelectedTime"/>.</param>
        /// <param name="newValue">New value of <see cref="SelectedTime"/>.</param>
        protected virtual void OnSelectedTimeChanged(TimeSpan oldValue, TimeSpan newValue)
        {

        }

        #endregion

        #region EndTime

        /// <summary>
        /// Gets or sets the <see cref="EndTime"/> value.
        /// </summary>
        public TimeSpan EndTime
        {
            get { return (TimeSpan)GetValue(EndTimeProperty); }
            set { SetValue(EndTimeProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="EndTime"/> property.
        /// </summary>
        public static readonly DependencyProperty EndTimeProperty =
            DependencyProperty.Register(
                                nameof(EndTime),
                                typeof(TimeSpan),
                                typeof(TimePicker),
                                new PropertyMetadata(TimeSpan.FromDays(1).Subtract(TimeSpan.FromSeconds(1)), EndTimePropertyChanged));

        private static void EndTimePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TimePicker)d).OnEndTimeChanged((TimeSpan)e.OldValue, (TimeSpan)e.NewValue);
        }

        /// <summary>
        /// Called when <see cref="EndTime"/> changed.
        /// </summary>
        /// <param name="oldValue">Old value of <see cref="EndTime"/>.</param>
        /// <param name="newValue">New value of <see cref="EndTime"/>.</param>
        protected virtual void OnEndTimeChanged(TimeSpan oldValue, TimeSpan newValue)
        {

        }

        #endregion

        public override void OnApplyTemplate()
        {
            if (_partHours != null)
                _partHours.SelectionChanged -= OnHoursSelectionChanged;
            _partHours = GetTemplateChild(PART_Hours) as ListBox;
            if (_partMinutes != null)
                _partMinutes.SelectionChanged -= OnMinutesSelectionChanged;
            _partMinutes = GetTemplateChild(PART_Minutes) as ListBox;
            if (_partSeconds != null)
                _partSeconds.SelectionChanged -= OnSecondsSelectionChanged;
            _partSeconds = GetTemplateChild(PART_Seconds) as ListBox;

            if (_partInput != null)
                _partInput.GotFocus -= _partInput_GotFocus;
            _partInput = GetTemplateChild(PART_TextInput) as TextBox;
            _partInput.GotFocus += _partInput_GotFocus;

            _partPopup = GetTemplateChild(PART_Popup) as Popup;

            var hourRange = Math.Abs(EndTime.Hours - BeginTime.Hours);
            var minuteRange = Math.Abs(EndTime.Minutes - BeginTime.Minutes);
            var secondRange = Math.Abs(EndTime.Seconds - BeginTime.Seconds);

            TimeFormat = TimeFormat ?? "hh:mm:ss";
            var timerParts = TimeFormat.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            if ((timerParts.Contains("hh") || timerParts.Contains("h")) && hourRange >= 0 && _partHours != null)
            {
                var hours = new List<string>();
                var minum = Math.Min(EndTime.Hours, BeginTime.Hours);
                for (int i = minum; i <= hourRange; i++)
                {
                    if (timerParts.Contains("hh"))
                        hours.Add(i.ToString().PadLeft(2, '0'));
                    else
                        hours.Add(i.ToString());
                }

                _partHours.SelectionChanged += OnHoursSelectionChanged;
                _partHours.ItemsSource = hours;
            }

            if ((timerParts.Contains("mm") || timerParts.Contains("m")) && minuteRange >= 0 && _partMinutes != null)
            {
                var minutes = new List<string>();
                var minum = Math.Min(EndTime.Minutes, BeginTime.Minutes);
                for (int i = minum; i <= minuteRange; i++)
                {
                    if (timerParts.Contains("mm"))
                        minutes.Add(i.ToString().PadLeft(2, '0'));
                    else
                        minutes.Add(i.ToString());
                }

                _partMinutes.SelectionChanged += OnMinutesSelectionChanged;
                _partMinutes.ItemsSource = minutes;
            }

            if ((timerParts.Contains("ss") || timerParts.Contains("s")) && secondRange >= 0 && _partSeconds != null)
            {
                var seconds = new List<string>();
                var minum = Math.Min(EndTime.Seconds, BeginTime.Seconds);
                for (int i = minum; i <= secondRange; i++)
                {
                    if (timerParts.Contains("ss"))
                        seconds.Add(i.ToString().PadLeft(2, '0'));
                    else
                        seconds.Add(i.ToString());
                }

                _partSeconds.SelectionChanged += OnSecondsSelectionChanged;
                _partSeconds.ItemsSource = seconds;
            }

            base.OnApplyTemplate();
        }

        private void _partInput_GotFocus(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                _partPopup.IsOpen = true;
            }), System.Windows.Threading.DispatcherPriority.Background);
        }

        private void OnSecondsSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var index = _partSeconds.SelectedIndex;
            var seconds = _partSeconds.ItemsSource as List<string>;
            var second = int.Parse(seconds[index]);
            SelectedTime = new TimeSpan(SelectedTime.Hours, SelectedTime.Minutes, second);
        }

        private void OnMinutesSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var index = _partMinutes.SelectedIndex;
            var minutes = _partMinutes.ItemsSource as List<string>;
            var minute = int.Parse(minutes[index]);
            SelectedTime = new TimeSpan(SelectedTime.Hours, minute, SelectedTime.Seconds);
        }

        private void OnHoursSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var index = _partHours.SelectedIndex;
            var hours = _partHours.ItemsSource as List<string>;
            var hour = int.Parse(hours[index]);
            SelectedTime = new TimeSpan(hour, SelectedTime.Minutes, SelectedTime.Seconds);
        }
    }
}
