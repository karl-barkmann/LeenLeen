using System;
using System.ComponentModel;
using System.Windows;

namespace Leen.Practices.Mvvm
{
    /// <summary>
    /// 该类定义一个依赖属性使ViewLocationProvider能够定位任意视图。
    /// </summary>
    public static class ViewLocator
    {
        /// <summary>
        /// 获取是否已注册的依赖属性。
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:验证公共方法的参数", MessageId = "0")]
        public static bool GetIsRegistered(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsRegisteredProperty);
        }

        /// <summary>
        /// 设置是否已注册的依赖属性
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:验证公共方法的参数", MessageId = "0")]
        public static void SetIsRegistered(DependencyObject obj, bool value)
        {
            obj.SetValue(IsRegisteredProperty, value);
        }

        /// <summary>
        /// 视图是否已注册的依赖属性。
        /// </summary>
        public static readonly DependencyProperty IsRegisteredProperty =
            DependencyProperty.RegisterAttached("IsRegistered", typeof(bool), typeof(ViewLocator),
            new UIPropertyMetadata(IsRegisterChanged));

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes")]
        private static void IsRegisterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(d)) return;

            if (!(d is IView view))
            {
                throw new Exception(string.Format("Your view '{0}' must implement IView interface", d.GetType()));
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

        /// <summary>
        /// Gets the value of the Alias attached property.
        /// </summary>
        /// <remarks>
        /// We can support multiple views over the same View-Model by setting a ViewLocator.Alias in Xaml.
        /// </remarks>
        /// <param name="obj">The dependency object that has this attached property.</param>
        /// <returns>The view's alias.</returns>
        internal static string GetAlias(DependencyObject obj)
        {
            return (string)obj.GetValue(AliasProperty);
        }

        /// <summary>
        /// Sets the value of the Alias attached property.
        /// </summary>
        /// <param name="obj">The dependency object that has this attached property.</param>
        /// <param name="value">The view's alias.</param>
        internal static void SetAlias(DependencyObject obj, string value)
        {
            obj.SetValue(AliasProperty, value);
        }

        /// <summary>
        /// The Alias attached property.
        /// </summary>
        internal static readonly DependencyProperty AliasProperty =
            DependencyProperty.RegisterAttached("Alias", typeof(string), typeof(ViewLocator), new PropertyMetadata(null));
    }
}
