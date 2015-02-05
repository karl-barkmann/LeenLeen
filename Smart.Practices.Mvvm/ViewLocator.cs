using System;
using System.ComponentModel;
using System.Windows;

namespace Smart.Practices.Mvvm
{
    public class ViewLocator
    {
        public static bool GetIsRegistered(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsRegisteredProperty);
        }

        public static void SetIsRegistered(DependencyObject obj, bool value)
        {
            obj.SetValue(IsRegisteredProperty, value);
        }

        public static readonly DependencyProperty IsRegisteredProperty =
            DependencyProperty.RegisterAttached("IsRegistered", typeof(bool), typeof(ViewLocator),
            new UIPropertyMetadata(IsRegisterChanged));

        private static void IsRegisterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(d)) return;

            var view = d as IView;
            if (view == null)
            {
                // TODO: Improve exception
                throw new Exception("Your views must implement IView");
            }

            if ((bool)e.NewValue)
            {
                ViewLocationProvider.Register(view);
            }
            else 
            {
                ViewLocationProvider.Unregister(view);
            }
        }
    }
}
