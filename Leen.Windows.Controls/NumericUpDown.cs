using Leen.Windows.Utils;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace Leen.Windows.Controls
{
    [DefaultEvent("ValueChanged"), DefaultProperty("Value")]
    [TemplatePart(Name = "PART_UP", Type = typeof(RepeatButton))]
    [TemplatePart(Name = "PART_DOWN", Type = typeof(RepeatButton))]
    [TemplatePart(Name = "PART_INPUT", Type = typeof(TextBox))]
    public class NumericUpDown : RangeBase 
    {
        #region DelayProperty

        /// <summary>
        ///     The Property for the Delay property.
        /// </summary>
        public static readonly DependencyProperty DelayProperty = RepeatButton.DelayProperty.AddOwner(typeof(NumericUpDown),
            new FrameworkPropertyMetadata(KeyboardUtil.GetKeyboardDelay()));

        /// <summary>
        ///     Specifies the amount of time, in milliseconds, to wait before repeating begins.
        /// Must be non-negative.
        /// </summary>
        [Bindable(true), Category("Behavior")]
        public int Delay
        {
            get
            {
                return (int)GetValue(DelayProperty);
            }
            set
            {
                SetValue(DelayProperty, value);
            }
        }

        #endregion Delay Property

        #region IntervalProperty

        /// <summary>
        ///     The Property for the Interval property.
        /// </summary>
        public static readonly DependencyProperty IntervalProperty = RepeatButton.IntervalProperty.AddOwner(typeof(NumericUpDown),
            new FrameworkPropertyMetadata(KeyboardUtil.GetKeyboardSpeed()));

        /// <summary>
        ///     Specifies the amount of time, in milliseconds, between repeats once repeating starts.
        /// Must be non-negative
        /// </summary>
        [Bindable(true), Category("Behavior")]
        public int Interval
        {
            get
            {
                return (int)GetValue(IntervalProperty);
            }
            set
            {
                SetValue(IntervalProperty, value);
            }
        }

        #endregion Interval Property

        #region InputTextForOvserverProperty

        internal string InputTextForObserver
        {
            get { return (string)GetValue(InputTextForObserverProperty); }
            set { SetValue(InputTextForObserverProperty, value); }
        }

        internal static readonly DependencyProperty InputTextForObserverProperty =
            DependencyProperty.Register(nameof(InputTextForObserver),
                                        typeof(string),
                                        typeof(NumericUpDown),
                                        new PropertyMetadata(null, InputTextForObserverPropertyChanged));

        private static void InputTextForObserverPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumericUpDown ctrl = d as NumericUpDown;
            ctrl.OnInputTextForObserver(e.OldValue, e.NewValue);
        }

        protected virtual void OnInputTextForObserver(object oldValue, object newValue)
        {
            if (_input != null && _input.IsFocused && !_inputValidating)
            {
                if (_timer != null)
                {
                    if (_timer.IsEnabled)
                    {
                        _timer.Stop();
                    }
                    _timer.Start();
                }
            }
        }

        #endregion

        #region StringFormatProperty

        public string StringFormat
        {
            get { return (string)GetValue(StringFormatProperty); }
            set { SetValue(StringFormatProperty, value); }
        }

        public static readonly DependencyProperty StringFormatProperty =
            DependencyProperty.Register(nameof(StringFormat), typeof(string), typeof(NumericUpDown), new PropertyMetadata(null));

        #endregion

        #region UnitProperty

        public object Unit
        {
            get { return (object)GetValue(UnitProperty); }
            set { SetValue(UnitProperty, value); }
        }

        public static readonly DependencyProperty UnitProperty =
            DependencyProperty.Register(nameof(Unit), typeof(object), typeof(NumericUpDown), new PropertyMetadata(null));

        #endregion

        #region Commands

        private static RoutedCommand _increaseLargeCommand = null;
        private static RoutedCommand _increaseSmallCommand = null;
        private static RoutedCommand _decreaseLargeCommand = null;
        private static RoutedCommand _decreaseSmallCommand = null;
        private static RoutedCommand _minimizeValueCommand = null;
        private static RoutedCommand _maximizeValueCommand = null;

        /// <summary>
        /// Increase value
        /// </summary>
        public static RoutedCommand IncreaseLarge
        {
            get { return _increaseLargeCommand; }
        }
        /// <summary>
        /// Decrease value
        /// </summary>
        public static RoutedCommand DecreaseLarge
        {
            get { return _decreaseLargeCommand; }
        }
        /// <summary>
        /// Increase value
        /// </summary>
        public static RoutedCommand IncreaseSmall
        {
            get { return _increaseSmallCommand; }
        }
        /// <summary>
        /// Decrease value
        /// </summary>
        public static RoutedCommand DecreaseSmall
        {
            get { return _decreaseSmallCommand; }
        }
        /// <summary>
        /// Set value to mininum
        /// </summary>
        public static RoutedCommand MinimizeValue
        {
            get { return _minimizeValueCommand; }
        }
        /// <summary>
        /// Set value to maximum
        /// </summary>
        public static RoutedCommand MaximizeValue
        {
            get { return _maximizeValueCommand; }
        }

        static void InitializeCommands()
        {
            _increaseLargeCommand = new RoutedCommand("IncreaseLarge", typeof(NumericUpDown));
            _decreaseLargeCommand = new RoutedCommand("DecreaseLarge", typeof(NumericUpDown));
            _increaseSmallCommand = new RoutedCommand("IncreaseSmall", typeof(NumericUpDown));
            _decreaseSmallCommand = new RoutedCommand("DecreaseSmall", typeof(NumericUpDown));
            _minimizeValueCommand = new RoutedCommand("MinimizeValue", typeof(NumericUpDown));
            _maximizeValueCommand = new RoutedCommand("MaximizeValue", typeof(NumericUpDown));

            CommandHelpers.RegisterCommandHandler(typeof(NumericUpDown), _increaseLargeCommand, new ExecutedRoutedEventHandler(OnIncreaseLargeCommand),
                                                  new CanExecuteRoutedEventHandler(OnCanIncreaseLargeCommand), new KeyGesture(Key.PageUp));

            CommandHelpers.RegisterCommandHandler(typeof(NumericUpDown), _decreaseLargeCommand, new ExecutedRoutedEventHandler(OnDecreaseLargeCommand),
                                                  new CanExecuteRoutedEventHandler(OnCanDecreaseLargeCommand), new KeyGesture(Key.PageDown));

            CommandHelpers.RegisterCommandHandler(typeof(NumericUpDown), _increaseSmallCommand, new ExecutedRoutedEventHandler(OnIncreaseSmallCommand),
                                                  new CanExecuteRoutedEventHandler(OnCanIncreaseSmallCommand), new KeyGesture(Key.Up), new KeyGesture(Key.Right));

            CommandHelpers.RegisterCommandHandler(typeof(NumericUpDown), _decreaseSmallCommand, new ExecutedRoutedEventHandler(OnDecreaseSmallCommand),
                                                  new CanExecuteRoutedEventHandler(OnCanDecreaseSmallCommand), new KeyGesture(Key.Down), new KeyGesture(Key.Left));

            CommandHelpers.RegisterCommandHandler(typeof(NumericUpDown), _minimizeValueCommand, new ExecutedRoutedEventHandler(OnMinimizeValueCommand),
                                                  Key.Home);

            CommandHelpers.RegisterCommandHandler(typeof(NumericUpDown), _maximizeValueCommand, new ExecutedRoutedEventHandler(OnMaximizeValueCommand),
                                                  Key.End);
        }

        private static void OnCanDecreaseSmallCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            if (sender is NumericUpDown control)
            {
                e.CanExecute = (control.Value - control.SmallChange) >= control.Minimum;
            }
        }

        private static void OnCanIncreaseSmallCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            if (sender is NumericUpDown control)
            {
                e.CanExecute = (control.Value + control.SmallChange) <= control.Maximum;
            }
        }

        private static void OnCanDecreaseLargeCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            if (sender is NumericUpDown control)
            {
                e.CanExecute = (control.Value - control.LargeChange) >= control.Minimum;
            }
        }

        private static void OnCanIncreaseLargeCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            if (sender is NumericUpDown control)
            {
                e.CanExecute = (control.Value + control.LargeChange) <= control.Maximum;
            }
        }

        private static void OnIncreaseSmallCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender is NumericUpDown control)
            {
                control.OnIncreaseSmall();
            }
        }

        private static void OnDecreaseSmallCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender is NumericUpDown control)
            {
                control.OnDecreaseSmall();
            }
        }

        private static void OnMaximizeValueCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender is NumericUpDown control)
            {
                control.OnMaximizeValue();
            }
        }

        private static void OnMinimizeValueCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender is NumericUpDown control)
            {
                control.OnMinimizeValue();
            }
        }

        private static void OnIncreaseLargeCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender is NumericUpDown control)
            {
                control.OnIncreaseLarge();
            }
        }

        private static void OnDecreaseLargeCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender is NumericUpDown control)
            {
                control.OnDecreaseLarge();
            }
        }

        #endregion Commands

        #region Virtual Functions

        /// <summary>
        /// Call when IntgerUpDown.IncreaseLarge command is invoked.
        /// </summary>
        protected virtual void OnIncreaseLarge()
        {
            MoveToNextValue(LargeChange);
        }

        /// <summary>
        /// Call when IntgerUpDown.DecreaseLarge command is invoked.
        /// </summary>
        protected virtual void OnDecreaseLarge()
        {
            MoveToNextValue(-LargeChange);
        }

        /// <summary>
        /// Call when IntgerUpDown.IncreaseSmall command is invoked.
        /// </summary>
        protected virtual void OnIncreaseSmall()
        {
            MoveToNextValue(SmallChange);
        }

        /// <summary>
        /// Call when IntgerUpDown.DecreaseSmall command is invoked.
        /// </summary>
        protected virtual void OnDecreaseSmall()
        {
            MoveToNextValue(-SmallChange);
        }

        /// <summary>
        /// Call when IntgerUpDown.MaximizeValue command is invoked.
        /// </summary>
        protected virtual void OnMaximizeValue()
        {
            SetCurrentValue(ValueProperty, Maximum);
        }

        /// <summary>
        /// Call when IntgerUpDown.MinimizeValue command is invoked.
        /// </summary>
        protected virtual void OnMinimizeValue()
        {
            SetCurrentValue(ValueProperty, Minimum);
        }

        #endregion Virtual Functions

        static NumericUpDown()
        {
            InitializeCommands();

            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericUpDown), new FrameworkPropertyMetadata(typeof(NumericUpDown)));
        }

        private RepeatButton _increaseButton;
        private RepeatButton _decreaseButton;
        private TextBox _input;
        private readonly DispatcherTimer _timer;
        private bool _inputValidating;

        public NumericUpDown()
        {
            _timer = new DispatcherTimer(TimeSpan.FromMilliseconds(Delay), DispatcherPriority.Input, OnTimerCallback, Dispatcher);
            _timer.IsEnabled = false;
            Unloaded += (s, e) =>
            {
                if (_input != null)
                {
                    _input.PreviewKeyDown -= _input_PreviewKeyDown;
                    _input.PreviewTextInput -= _input_PreviewTextInput;
                    BindingOperations.ClearBinding(this, InputTextForObserverProperty);
                }

                if (_timer != null)
                {
                    _timer.Tick -= OnTimerCallback;
                }
            };
        }

        private void OnTimerCallback(object sender, EventArgs e)
        {
            _inputValidating = true;
            bool isValid = double.TryParse(_input.Text, out double value);
            double validValue = Math.Max(Minimum, Math.Min(value, Maximum));
            SetCurrentValue(ValueProperty, validValue);

            if (!isValid || validValue != value)
            {
                _input.Text = StringFormat == null ? validValue.ToString() : validValue.ToString(StringFormat);
                _input.CaretIndex = _input.Text.Length;
            }
            _inputValidating = false;
            _timer.Stop();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _increaseButton = GetTemplateChild("PART_UP") as RepeatButton;
            if (_increaseButton != null)
            {
                CommandManager.InvalidateRequerySuggested(); // Should post an idle queue item to update IsEnabled on button
            }
            _decreaseButton = GetTemplateChild("PART_UP") as RepeatButton;
            if (_decreaseButton != null)
            {
                CommandManager.InvalidateRequerySuggested(); // Should post an idle queue item to update IsEnabled on button
            }

            if (_input != null)
            {
                BindingOperations.ClearBinding(this, InputTextForObserverProperty);
                _input.PreviewKeyDown -= _input_PreviewKeyDown;
                _input.PreviewTextInput -= _input_PreviewTextInput;
            }
            _input = GetTemplateChild("PART_INPUT") as TextBox;
            if (_input != null)
            {
                _input.PreviewTextInput += _input_PreviewTextInput;
                _input.PreviewKeyDown += _input_PreviewKeyDown;
                _input.Text = StringFormat == null ? Value.ToString() : Value.ToString(StringFormat);
                Binding binding = new Binding("Text");
                binding.Source = _input;
                binding.Mode = BindingMode.OneWay;
                binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                SetBinding(InputTextForObserverProperty, binding);
            }
        }

        private void _input_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            //仅允许输入数字
            e.Handled = !IsInputAllowed(_input.Text);
        }

        protected virtual bool IsInputAllowed(string text)
        {
            return int.TryParse(text, out _);
        }

        private void _input_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //When this keys pointed down while the input textbox got focus
            //we force to  move focus to repeatbuttons
            if (e.Key == Key.Up || e.Key == Key.Right)
            {
                if (_increaseButton != null)
                {
                    _increaseButton.Focus();
                }
                OnIncreaseSmall();
            }
            else if (e.Key == Key.Down || e.Key == Key.Left)
            {
                if (_decreaseButton != null)
                {
                    _decreaseButton.Focus();
                }
                OnDecreaseSmall();
            }
            else if (e.Key == Key.PageUp)
            {
                if (_increaseButton != null)
                {
                    _increaseButton.Focus();
                }
                OnIncreaseLarge();
            }
            else if (e.Key == Key.PageDown)
            {
                if (_decreaseButton != null)
                {
                    _decreaseButton.Focus();
                }
                OnDecreaseLarge();
            }
            else if (e.Key == Key.Home)
            {
                OnMinimizeValue();
                if (_increaseButton != null)
                {
                    _increaseButton.Focus();
                }
            }
            else if (e.Key == Key.End)
            {
                OnMaximizeValue();
                if (_decreaseButton != null)
                {
                    _decreaseButton.Focus();
                }
            }
        }

        protected override void OnValueChanged(double oldValue, double newValue)
        {
            base.OnValueChanged(oldValue, newValue);
            if (_input != null)
            {
                _input.Text = StringFormat == null ? newValue.ToString() : newValue.ToString(StringFormat);
                _input.CaretIndex = _input.Text.Length;
            }

            CommandManager.InvalidateRequerySuggested();
        }

        protected override void OnMaximumChanged(double oldMaximum, double newMaximum)
        {
            base.OnMaximumChanged(oldMaximum, newMaximum);

            CommandManager.InvalidateRequerySuggested();
        }

        protected override void OnMinimumChanged(double oldMinimum, double newMinimum)
        {
            base.OnMinimumChanged(oldMinimum, newMinimum);

            CommandManager.InvalidateRequerySuggested();
        }

        private void MoveToNextValue(double change)
        {
            double newValue = Value + change;

            double validValue = Math.Max(Math.Min(newValue, Maximum), Minimum);

            SetCurrentValue(ValueProperty, validValue);
        }
    }

    public class DoubleUpDown : NumericUpDown
    {
        protected override bool IsInputAllowed(string text)
        {
            return double.TryParse(text, out _);
        }
    }
}
