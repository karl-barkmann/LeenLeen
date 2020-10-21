using System.Windows;
using System.Windows.Controls;

namespace Leen.Windows.Controls
{
    [TemplatePart(Name = PART_Prepend, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = PART_Append, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = PART_Prefix, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = PART_Suffix, Type = typeof(FrameworkElement))]
    public class TextInput : RoundedTextBox
    {
        public const string PART_Prepend = "PART_Prepend";
        public const string PART_Append = "PART_Append";
        public const string PART_Prefix = "PART_Prefix";
        public const string PART_Suffix = "PART_Suffix";

        static TextInput()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TextInput), new FrameworkPropertyMetadata(typeof(TextInput)));
        }

        #region Prepend

        /// <summary>
        /// Gets or sets the <see cref="Prepend"/> value.
        /// </summary>
        public object Prepend
        {
            get { return (object)GetValue(PrependProperty); }
            set { SetValue(PrependProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="Prepend"/> property.
        /// </summary>
        public static readonly DependencyProperty PrependProperty =
            DependencyProperty.Register(
                                nameof(Prepend),
                                typeof(object),
                                typeof(TextInput),
                                new PropertyMetadata(null, PrependPropertyChanged));

        private static void PrependPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TextInput)d).OnPrependChanged((object)e.OldValue, (object)e.NewValue);
        }

        /// <summary>
        /// Called when <see cref="Prepend"/> changed.
        /// </summary>
        /// <param name="oldValue">Old value of <see cref="Prepend"/>.</param>
        /// <param name="newValue">New value of <see cref="Prepend"/>.</param>
        protected virtual void OnPrependChanged(object oldValue, object newValue)
        {

        }

        #endregion

        #region Prefix

        /// <summary>
        /// Gets or sets the <see cref="Prefix"/> value.
        /// </summary>
        public object Prefix
        {
            get { return (object)GetValue(PrefixProperty); }
            set { SetValue(PrefixProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="Prefix"/> property.
        /// </summary>
        public static readonly DependencyProperty PrefixProperty =
            DependencyProperty.Register(
                                nameof(Prefix),
                                typeof(object),
                                typeof(TextInput),
                                new PropertyMetadata(null, PrefixPropertyChanged));

        private static void PrefixPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TextInput)d).OnPrefixChanged((object)e.OldValue, (object)e.NewValue);
        }

        /// <summary>
        /// Called when <see cref="Prefix"/> changed.
        /// </summary>
        /// <param name="oldValue">Old value of <see cref="Prefix"/>.</param>
        /// <param name="newValue">New value of <see cref="Prefix"/>.</param>
        protected virtual void OnPrefixChanged(object oldValue, object newValue)
        {
            if (GetTemplateChild(PART_Prefix) is TextBlock prefixIcon)
            {
                prefixIcon.Text = Prefix?.ToString();
            }
        }

        #endregion

        #region Suffix

        /// <summary>
        /// Gets or sets the <see cref="Suffix"/> value.
        /// </summary>
        public object Suffix
        {
            get { return (object)GetValue(SuffixProperty); }
            set { SetValue(SuffixProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="Suffix"/> property.
        /// </summary>
        public static readonly DependencyProperty SuffixProperty =
            DependencyProperty.Register(
                                nameof(Suffix),
                                typeof(object),
                                typeof(TextInput),
                                new PropertyMetadata(null, SuffixPropertyChanged));

        private static void SuffixPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TextInput)d).OnSuffixChanged((object)e.OldValue, (object)e.NewValue);
        }

        /// <summary>
        /// Called when <see cref="Suffix"/> changed.
        /// </summary>
        /// <param name="oldValue">Old value of <see cref="Suffix"/>.</param>
        /// <param name="newValue">New value of <see cref="Suffix"/>.</param>
        protected virtual void OnSuffixChanged(object oldValue, object newValue)
        {
            if (GetTemplateChild(PART_Suffix) is TextBlock suffixIcon)
            {
                suffixIcon.Text = Suffix?.ToString();
            }
        }

        #endregion

        #region Append

        /// <summary>
        /// Gets or sets the <see cref="Append"/> value.
        /// </summary>
        public object Append
        {
            get { return (object)GetValue(AppendProperty); }
            set { SetValue(AppendProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="Append"/> property.
        /// </summary>
        public static readonly DependencyProperty AppendProperty =
            DependencyProperty.Register(
                                nameof(Append),
                                typeof(object),
                                typeof(TextInput),
                                new PropertyMetadata(null, AppendPropertyChanged));

        private static void AppendPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TextInput)d).OnAppendChanged((object)e.OldValue, (object)e.NewValue);
        }

        /// <summary>
        /// Called when <see cref="Append"/> changed.
        /// </summary>
        /// <param name="oldValue">Old value of <see cref="Append"/>.</param>
        /// <param name="newValue">New value of <see cref="Append"/>.</param>
        protected virtual void OnAppendChanged(object oldValue, object newValue)
        {

        }

        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (GetTemplateChild(PART_Prefix) is TextBlock prefixIcon)
            {
                prefixIcon.Text = Prefix?.ToString();
            }

            if (GetTemplateChild(PART_Suffix) is TextBlock suffixIcon)
            {
                suffixIcon.Text = Suffix?.ToString();
            }
        }
    }
}
