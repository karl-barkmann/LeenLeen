/* * * * * * * * * * * * * * * * * * * * * * * * * * * 
 * 作者：李平
 * 日期：2012/7/11 14:58:37
 * 描述：TextBox帮助类。
 * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Leen.Windows.Controls.Helper
{
    /// <summary>
    /// TextBox控件绑定帮助类，支持按下Enter键时更新绑定源。
    /// </summary>
    public static class TextBoxHelper
    {
        /// <summary>
        /// 按下Enter键更新TextBox控件Text绑定源。
        /// </summary>
        public static readonly DependencyProperty EnterUpdatesTextSourceProperty =
            DependencyProperty.RegisterAttached("EnterUpdatesTextSource",
                                                typeof(bool),
                                                typeof(TextBoxHelper),
                                                new PropertyMetadata(false, EnterUpdatesTextSourcePropertyChanged));

        /// <summary>
        /// 获取值。
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool GetEnterUpdatesTextSource(DependencyObject obj)
        {
            return (bool)obj.GetValue(EnterUpdatesTextSourceProperty);
        }

        /// <summary>
        /// 设置值。
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetEnterUpdatesTextSource(DependencyObject obj, bool value)
        {
            obj.SetValue(EnterUpdatesTextSourceProperty, value);
        }

        #region 私有方法

        private static void EnterUpdatesTextSourcePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is UIElement sender)
            {
                if ((bool)e.NewValue)
                {
                    sender.PreviewKeyDown += UpdatesTextSourceOnEnter;
                    sender.PreviewMouseLeftButtonDown += UpdatesTextSourceOnMouseLeftButtonDown;
                }
                else
                {
                    sender.PreviewKeyDown -= UpdatesTextSourceOnEnter;
                    sender.PreviewMouseLeftButtonDown -= UpdatesTextSourceOnMouseLeftButtonDown;
                }
            }
        }

        static void UpdatesTextSourceOnMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                //在控件内按下鼠标则不更新，因为用户正在可能需要编辑文本
                if (textBox.InputHitTest(e.MouseDevice.GetPosition(textBox)) != null)
                    return;
                if (GetEnterUpdatesTextSource(textBox))
                {
                    var bindingExpresssion = BindingOperations.GetBindingExpression(
                        textBox,
                        TextBox.TextProperty);
                    if (bindingExpresssion != null)
                        bindingExpresssion.UpdateSource();
                }
            }
        }

        static void UpdatesTextSourceOnEnter(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                if (GetEnterUpdatesTextSource((DependencyObject)sender))
                {
                    var bindingExpresssion = BindingOperations.GetBindingExpression(
                        (DependencyObject)sender,
                        TextBox.TextProperty);
                    if (bindingExpresssion != null)
                        bindingExpresssion.UpdateSource();
                }
            }
        }

        #endregion
    }
}
