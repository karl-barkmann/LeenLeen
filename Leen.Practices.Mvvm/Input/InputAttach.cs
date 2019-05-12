
namespace System.Windows.Input
{
    /// <summary>
    /// 用于在 <see cref="Style"/> 中为元素定义 <see cref="InputBindingCollection"/>的帮助类。
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
    public class InputAttached
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty InputBindingsProperty =
            DependencyProperty.RegisterAttached("InputBindings", typeof(InputBindingCollection), typeof(InputAttached),
            new FrameworkPropertyMetadata(new InputBindingCollection(),
            (sender, e) =>
            {
                var element = sender as UIElement;
                if (element == null) return;
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
            var inputBinding = d as InputBinding;
            if (inputBinding == null) return;

            inputBinding.Gesture = e.NewValue as InputGesture;
        }
    }
}
