using System.Windows;
using System.Windows.Controls;

namespace Leen.Windows.Controls.Helper
{
    /// <summary>
    /// PasswordBox控件绑定帮助类。
    /// </summary>
    public static class PasswordBoxBindingHelper
    {
        /// <summary>
        /// Using a DependencyProperty as the backing store for Password. 
        /// This enables animation, styling, binding, etc... 
        /// </summary>
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.RegisterAttached("Password", typeof(string), typeof(PasswordBoxBindingHelper), new FrameworkPropertyMetadata(string.Empty, OnPasswordChanged));

        /// <summary>
        /// 附加。
        /// </summary>
        public static readonly DependencyProperty AttachProperty =
            DependencyProperty.RegisterAttached("Attach", typeof(bool), typeof(PasswordBoxBindingHelper), new FrameworkPropertyMetadata(false, Attach));

        /// <summary>
        /// 是否正在更新。
        /// </summary>
        public static readonly DependencyProperty IsUpdatingProperty =
            DependencyProperty.RegisterAttached("IsUpdating", typeof(bool), typeof(PasswordBoxBindingHelper));

        /// <summary>
        /// Sets the attach.
        /// </summary>
        /// <param name="dp">The dp.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetAttach(DependencyObject dp, bool value)
        {
            dp.SetValue(AttachProperty, value);
        }

        /// <summary>
        /// Gets the attach.
        /// </summary>
        /// <param name="dp">The dp.</param>
        /// <returns></returns>
        public static bool GetAttach(DependencyObject dp)
        {
            return (bool)dp.GetValue(AttachProperty);
        }

        /// <summary>
        /// Sets the password.
        /// </summary>
        /// <param name="dp">The dp.</param>
        /// <param name="value">The value.</param>
        public static void SetPassword(DependencyObject dp, string value)
        {
            dp.SetValue(PasswordProperty, value);
        }

        /// <summary>
        /// Gets the password.
        /// </summary>
        /// <param name="dp">The dp.</param>
        /// <returns></returns>
        public static string GetPassword(DependencyObject dp)
        {
            return (string)dp.GetValue(PasswordProperty);
        }

        /// <summary>
        /// Sets the isupdating.
        /// </summary>
        /// <param name="dp">The dp.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetIsUpdating(DependencyObject dp, bool value)
        {
            dp.SetValue(IsUpdatingProperty, value);
        }

        /// <summary>
        /// Gets the isupdating.
        /// </summary>
        /// <param name="dp">The dp.</param>
        /// <returns></returns>
        public static bool GetIsUpdating(DependencyObject dp)
        {
            return (bool)dp.GetValue(IsUpdatingProperty);
        }

        #region 私有方法

        private static void OnPasswordChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            PasswordBox pb = sender as PasswordBox;
            pb.PasswordChanged -= new RoutedEventHandler(pb_PasswordChanged);
            if (!GetIsUpdating(pb))
            {
                pb.Password = (string)e.NewValue;
            }

            pb.PasswordChanged += new RoutedEventHandler(pb_PasswordChanged);
        }

        private static void Attach(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            PasswordBox pb = sender as PasswordBox;
            if (pb == null) return;

            if ((bool)e.OldValue)
            {
                pb.PasswordChanged -= new RoutedEventHandler(pb_PasswordChanged);
            }
            if ((bool)e.NewValue)
            {
                pb.PasswordChanged += new RoutedEventHandler(pb_PasswordChanged);
            }
        }

        static void pb_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox pb = sender as PasswordBox;
            SetIsUpdating(pb, true);
            SetPassword(pb, pb.Password);
            SetIsUpdating(pb, false);
        }

        #endregion
    }
}
