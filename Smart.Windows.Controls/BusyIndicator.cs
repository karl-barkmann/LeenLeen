using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Leen.Windows.Controls
{
    [TemplatePart(Name = "PART_CancelButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_TargetHolder", Type = typeof(VisualBrush))]
    public class BusyIndicator : Control
    {
        public static readonly RoutedEvent CancelEvent =
            EventManager.RegisterRoutedEvent("Cancel", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(BusyIndicator));

        public static readonly DependencyProperty AllowCancelProperty =
           DependencyProperty.Register("AllowCancel", typeof(bool), typeof(BusyIndicator), new PropertyMetadata(true, OnAllowCancelPropertyChanged));

        public static readonly DependencyProperty CancelCommandProperty =
            DependencyProperty.Register("CancelCommand", typeof(ICommand), typeof(BusyIndicator), new PropertyMetadata(null));

        public static readonly DependencyProperty CancelCommandParameterProperty =
            DependencyProperty.Register("CancelCommandParameter", typeof(object), typeof(BusyIndicator), new PropertyMetadata(null));

        public static readonly DependencyProperty IndicateTargetProperty =
            DependencyProperty.Register("IndicateTarget", typeof(UIElement), typeof(BusyIndicator), new PropertyMetadata(null, OnIndicateTargetPropertyChanged));

        public static readonly DependencyProperty IsBusyProperty =
            DependencyProperty.Register("IsBusy", typeof(bool), typeof(BusyIndicator), new PropertyMetadata(true, OnIsBusyPropertyChanged));

        public static readonly DependencyProperty BusyMessageProperty =
            DependencyProperty.Register("BusyMessage", typeof(string), typeof(BusyIndicator), new PropertyMetadata("loading...", OnBusyMessagePropertyChanged));

        private Button btnCancel;
        private VisualBrush targetHolder;

        static BusyIndicator()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BusyIndicator), new FrameworkPropertyMetadata(typeof(BusyIndicator)));
        }

        public bool AllowCancel
        {
            get { return (bool)GetValue(AllowCancelProperty); }
            set { SetValue(AllowCancelProperty, value); }
        }

        public ICommand CancelCommand
        {
            get { return (ICommand)GetValue(CancelCommandProperty); }
            set { SetValue(CancelCommandProperty, value); }
        }

        public bool IsBusy
        {
            get { return (bool)GetValue(IsBusyProperty); }
            set { SetValue(IsBusyProperty, value); }
        }

        public string BusyMessage
        {
            get { return (string)GetValue(BusyMessageProperty); }
            set { SetValue(BusyMessageProperty, value); }
        }

        public UIElement IndicateTarget
        {
            get { return (UIElement)GetValue(IndicateTargetProperty); }
            set { SetValue(IndicateTargetProperty, value); }
        }

        public event RoutedEventHandler Cancel
        {
            add { AddHandler(CancelEvent, value); }
            remove { RemoveHandler(CancelEvent, value); }
        }

        public object CancelCommandParameter
        {
            get { return (object)GetValue(CancelCommandParameterProperty); }
            set { SetValue(CancelCommandParameterProperty, value); }
        }

        public override void OnApplyTemplate()
        {
            if (btnCancel != null)
            {
                btnCancel.Click -= btnCancel_Click;
                BindingOperations.ClearAllBindings(btnCancel);
            }
            btnCancel = GetTemplateChild("PART_Cancel") as Button;
            if (btnCancel != null)
            {
                btnCancel.Click += btnCancel_Click;
                if (AllowCancel)
                {
                    btnCancel.Visibility = Visibility.Visible;
                }
                else
                {
                    btnCancel.Visibility = Visibility.Collapsed;
                }
            }

            targetHolder = GetTemplateChild("PART_TargetHolder") as VisualBrush;
            if (targetHolder != null)
            {
                if (Parent == IndicateTarget)
                {
                    throw new InvalidOperationException("You cant not set one's parent as it's 'IndicateTarget' !");
                }
                targetHolder.Visual = IndicateTarget;
            }
            base.OnApplyTemplate();
        }

        void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            FireCancel();
        }

        internal void FireCancel()
        {
            RaiseEvent(new RoutedEventArgs(CancelEvent, this));
        }

        private static void OnAllowCancelPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var busyIndicator = d as BusyIndicator;
            if (busyIndicator != null && busyIndicator.btnCancel != null)
            {
                if (busyIndicator.AllowCancel)
                {
                    busyIndicator.btnCancel.Visibility = Visibility.Visible;
                }
                else
                {
                    busyIndicator.btnCancel.Visibility = Visibility.Collapsed;
                }
            }
        }

        private static void OnIndicateTargetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var busyIndicator = d as BusyIndicator;
            if (busyIndicator != null && busyIndicator.targetHolder != null)
            {
                busyIndicator.targetHolder.Visual = e.NewValue as Visual;
            }
        }

        private static void OnIsBusyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var busyIndicator = d as BusyIndicator;
            if (busyIndicator != null)
            {
                if (busyIndicator.Parent == e.NewValue)
                {
                    throw new InvalidOperationException("You cant not set one's parent as it's 'IndicateTarget' !");
                }
                if (!busyIndicator.IsBusy)
                {
                    busyIndicator.Visibility = Visibility.Collapsed;
                }
                else
                {
                    busyIndicator.Visibility = Visibility.Visible;
                }
            }
        }

        private static void OnBusyMessagePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }
    }
}
