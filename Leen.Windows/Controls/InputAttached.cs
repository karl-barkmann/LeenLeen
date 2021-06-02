
using System;
using System.Windows;
using System.Windows.Input;

namespace Leen.Windows.Controls
{
    /// <summary>
    /// 用于在 <see cref="Style"/> 中为元素定义输入行为，比如<see cref="InputBindingCollection"/>输入绑定或触碰按键控制。
    /// </summary>
    /// <remarks>
    /// <example>
    ///     <Setter Property="prefix:InputAttached.InputBindings">
    ///         <Setter.Value>
    ///             <InputBindingCollection>
    ///                 <KeyBinding Key="A" Modifiers="Ctrl" Command="{Binding SomeCommand}" />
    ///             </InputBindingCollection>
    ///         </Setter.Value>
    ///     </Setter>
    /// </example>
    /// </remarks>
    public static class InputAttached
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty InputBindingsProperty =
            DependencyProperty.RegisterAttached("InputBindings", typeof(InputBindingCollection), typeof(InputAttached),
            new FrameworkPropertyMetadata(new InputBindingCollection(),
            (sender, e) =>
            {
                if (!(sender is UIElement element)) return;
                element.InputBindings.Clear();
                element.InputBindings.AddRange((InputBindingCollection)e.NewValue);
            }));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static InputBindingCollection GetInputBindings(UIElement element)
        {
            return (InputBindingCollection)element.GetValue(InputBindingsProperty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="inputBindings"></param>
        public static void SetInputBindings(UIElement element, InputBindingCollection inputBindings)
        {
            element.SetValue(InputBindingsProperty, inputBindings);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool GetTouchAttached(DependencyObject obj)
        {
            return (bool)obj.GetValue(TouchAttachedProperty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetTouchAttached(DependencyObject obj, bool value)
        {
            obj.SetValue(TouchAttachedProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty TouchAttachedProperty =
            DependencyProperty.RegisterAttached("TouchAttached", typeof(bool), typeof(InputAttached), new PropertyMetadata(false, OnTouchAttachedPropertyChanged));

        private static void OnTouchAttachedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is UIElement sender))
                return;

            if ((bool)e.NewValue)
            {
                sender.GotFocus += Sender_GotFocus;
                sender.LostFocus += Sender_LostFocus;
            }
            else
            {
                sender.GotFocus -= Sender_GotFocus;
                sender.LostFocus -= Sender_LostFocus;
            }
        }

        private static void Sender_LostFocus(object sender, RoutedEventArgs e)
        {
            TouchKeyboardProvider.HideTouchKeyboard();
        }

        private static void Sender_GotFocus(object sender, RoutedEventArgs e)
        {
            TouchKeyboardProvider.ShowTouchKeyboard((error) =>
            {
                GetTouchCommand(sender as DependencyObject)?.Execute(error);
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static ICommand GetTouchCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(TouchCommandProperty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetTouchCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(TouchCommandProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty TouchCommandProperty =
            DependencyProperty.RegisterAttached("TouchCommand", typeof(ICommand), typeof(InputAttached), new PropertyMetadata(null));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static InputGesture GetAttachedGesture(DependencyObject obj)
        {
            return (InputGesture)obj.GetValue(AttachedGestureProperty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetAttachedGesture(DependencyObject obj, InputGesture value)
        {
            obj.SetValue(AttachedGestureProperty, value);
        }

        /// <summary>
        /// 提供对目标附加的设备笔势的依赖属性支持。
        /// </summary>
        public static readonly DependencyProperty AttachedGestureProperty =
            DependencyProperty.RegisterAttached("AttachedGesture", typeof(InputGesture), typeof(InputAttached), new FrameworkPropertyMetadata(null, AttachedGesturePropertyChanged));

        private static void AttachedGesturePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is InputBinding inputBinding)) return;

            inputBinding.Gesture = e.NewValue as InputGesture;
        }
    }
}
