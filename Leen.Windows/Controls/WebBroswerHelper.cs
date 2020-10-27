using System;
using System.Windows;
using System.Windows.Controls;

namespace Leen.Windows.Controls
{
    /// <summary>
    /// 使 <see cref="WebBrowser"/> 支持在Xaml markUp中的Binding，以便通过MVVM方式进行导航。
    /// </summary>
    public static class WebBroswerHelper
    {
        /// <summary>
        /// 获取当前导航路径。
        /// </summary>
        /// <param name="obj"><see cref="WebBrowser"/>组件。</param>
        /// <returns></returns>
        public static Uri GetNavigateUri(DependencyObject obj)
        {
            return (Uri)obj.GetValue(NavigateUriProperty);
        }

        /// <summary>
        /// 设置当前导航路径。
        /// </summary>
        /// <param name="obj"><see cref="WebBrowser"/>组件。</param>
        /// <param name="value"></param>
        public static void SetNavigateUri(DependencyObject obj, Uri value)
        {
            obj.SetValue(NavigateUriProperty, value);
        }

        /// <summary>
        /// 当前导航路径的依赖属性。
        /// </summary>
        public static readonly DependencyProperty NavigateUriProperty =
            DependencyProperty.RegisterAttached("NavigateUri", typeof(Uri), typeof(WebBroswerHelper), new PropertyMetadata(null, NavigateUriPropertyChanged));

        /// <summary>
        /// 获取一个值指示浏览器组件是否已释放。
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool GetIsDisposed(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsDisposedProperty);
        }

        /// <summary>
        /// 设置一个值指示浏览器组件是否已释放。
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetIsDisposed(DependencyObject obj, bool value)
        {
            obj.SetValue(IsDisposedProperty, value);
        }

        /// <summary>
        /// 是否已经释放的依赖属性。
        /// </summary>
        public static readonly DependencyProperty IsDisposedProperty =
            DependencyProperty.RegisterAttached("IsDisposed", typeof(bool), typeof(WebBroswerHelper), new PropertyMetadata(false));

        private static void NavigateUriPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is WebBrowser browser && !GetIsDisposed(d))
            {
                try
                {
                    browser.Source = e.NewValue as Uri;
                }
                catch (ObjectDisposedException)
                {
                    //Since Microsoft decided not to public 'IsDisposed' property,
                    //this is the way we figure out if a WebBrowser has been disposed.
                    SetIsDisposed(d, true);
                }
            }
        }
    }
}
