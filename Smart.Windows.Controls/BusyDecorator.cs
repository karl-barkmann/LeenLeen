using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Linq;
using Smart.Windows.Documents;
using Smart.Windows.Utils;

namespace Smart.Windows.Controls
{
    public class BusyDecorator : Control
    {
        public static readonly RoutedEvent CancelEvent =
            EventManager.RegisterRoutedEvent("Cancel", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(BusyDecorator));

        public static readonly DependencyProperty IsBusyProperty =
            DependencyProperty.Register("IsBusy", typeof(bool), typeof(BusyDecorator), new PropertyMetadata(false, OnIsBusyPropertyChanged));

        private BusyAdorner busyAdorner;

        public bool IsBusy
        {
            get { return (bool)GetValue(IsBusyProperty); }
            set { SetValue(IsBusyProperty, value); }
        }

        public event RoutedEventHandler Cancel
        {
            add { AddHandler(CancelEvent, value); }
            remove { RemoveHandler(CancelEvent, value); }
        }

        private static void OnIsBusyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var busyDecorator = d as BusyDecorator;
            if (busyDecorator != null)
            {
                if (busyDecorator.IsBusy)
                {
                    if (!busyDecorator.IsLoaded)
                    {
                        busyDecorator.Loaded += busyDecorator_Loaded;
                    }
                    else
                    {
                        busyDecorator.ShowBusyAdorner();
                    }
                }
                else
                {
                    busyDecorator.HideBusyAdorner();
                }
            }
        }

        static void busyDecorator_Loaded(object sender, RoutedEventArgs e)
        {
            var busyDecorator = sender as BusyDecorator;
            if (busyDecorator != null)
            {
                if (busyDecorator.IsBusy)
                {
                    busyDecorator.ShowBusyAdorner();
                }
                else
                {
                    busyDecorator.HideBusyAdorner();
                }
            }
        }

        private void ShowBusyAdorner()
        {
            if (busyAdorner != null)
            {
                busyAdorner.Visibility = Visibility.Visible;
            }
            else
            {
                var adornerLayer = GetAdornerLayer(this);

                if (adornerLayer != null)
                {
                    var parent = this.Parent as UIElement;
                    busyAdorner = new BusyAdorner(parent);
                    busyAdorner.Cancel += busyAdorner_Cancel;
                    adornerLayer.Add(busyAdorner);
                }
            }
        }

        void busyAdorner_Cancel(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(CancelEvent, this));
        }

        private void HideBusyAdorner()
        {
            if (busyAdorner != null)
            {
                busyAdorner.Visibility = Visibility.Collapsed;
            }
        }

        private static AdornerLayer GetAdornerLayer(DependencyObject dp)
        {
            var window = Window.GetWindow(dp);
            if (window != null)
            {
                var decorator = window.FindChild<AdornerDecorator>();
                AdornerLayer adornerLayer = null;
                if (decorator != null)
                {
                    adornerLayer = decorator.AdornerLayer;
                }

                return adornerLayer;
            }
            else
            {
                return null;
            }
        }
    }
}
