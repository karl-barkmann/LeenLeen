using System.Windows;
using System.Windows.Controls;

namespace Leen.Windows.Controls
{
    [TemplateVisualState(GroupName = BusyStatesGroup, Name = ActiveStateName)]
    [TemplateVisualState(GroupName = BusyStatesGroup, Name = NormalStateName)]
    public class ProgressRing : Control
    {
        #region VisualStateNames

        internal const string BusyStatesGroup = "BusyStates";

        internal const string ActiveStateName = "Active";

        internal const string NormalStateName = "Normal";

        #endregion

        public double ModalOpacity
        {
            get { return (double)GetValue(ModalOpacityProperty); }
            set { SetValue(ModalOpacityProperty, value); }
        }

        public static readonly DependencyProperty ModalOpacityProperty =
            DependencyProperty.Register(
                nameof(ModalOpacity),
                typeof(double),
                typeof(ProgressRing),
                new PropertyMetadata(0.75d));

        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }

        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register(
                nameof(IsActive),
                typeof(bool),
                typeof(ProgressRing),
                new PropertyMetadata(true, IsActivePropertyChanged));

        private static void IsActivePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ProgressRing ctrl = d as ProgressRing;
            ctrl.OnIsActiveChanged(e.OldValue, e.NewValue);
        }

        private void OnIsActiveChanged(object oldValue, object newValue)
        {
            bool isActive = (bool)newValue;
            if (isActive)
            {
                VisualStateManager.GoToState(this, ActiveStateName, true);
            }
            else
            {
                VisualStateManager.GoToState(this, NormalStateName, true);
            }
        }

        public string ActiveMessage
        {
            get { return (string)GetValue(ActiveMessageProperty); }
            set { SetValue(ActiveMessageProperty, value); }
        }

        public static readonly DependencyProperty ActiveMessageProperty =
            DependencyProperty.Register(
                nameof(ActiveMessage),
                typeof(string),
                typeof(ProgressRing),
                new PropertyMetadata(DefaultActiveMessage));

        static ProgressRing()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ProgressRing), new FrameworkPropertyMetadata(typeof(ProgressRing)));
        }

        const string DefaultActiveMessage = "加载中...";

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (IsActive)
            {
                VisualStateManager.GoToState(this, ActiveStateName, true);
            }
            else
            {
                VisualStateManager.GoToState(this, NormalStateName, true);
            }
        }
    }
}
