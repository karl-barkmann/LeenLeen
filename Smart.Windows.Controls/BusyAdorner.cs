using Leen.Windows.Controls;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Leen.Windows.Documents
{
    public class BusyAdorner : Adorner
    {
        public static readonly RoutedEvent CancelEvent =
            EventManager.RegisterRoutedEvent("Cancel", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(BusyAdorner));

        private BusyIndicator busyIndicator;

        public BusyAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            IsHitTestVisible = true;

            busyIndicator = new BusyIndicator();
            busyIndicator.DataContext = adornedElement;
            AddVisualChild(busyIndicator);
        }

        public event RoutedEventHandler Cancel
        {
            add { AddHandler(CancelEvent, value); }
            remove { RemoveHandler(CancelEvent, value); }
        }

        protected override int VisualChildrenCount
        {
            get { return 1; }
        }

        protected override Visual GetVisualChild(int index)
        {
            return busyIndicator;
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            busyIndicator.Arrange(new Rect(arrangeBounds));
            return arrangeBounds;
        }

        internal void FireCancel()
        {
            RaiseEvent(new RoutedEventArgs(CancelEvent, this));
        }
    }
}
