using System.Windows;
using System.Windows.Controls;

namespace Leen.Windows.Controls
{
    /// <summary>
    /// 支持边框圆角的文本输入框。
    /// </summary>
    public class RoundedTextBox : TextBox
    {
        #region CornerRadiusProperty

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(nameof(CornerRadius),
                                        typeof(CornerRadius),
                                        typeof(RoundedTextBox),
                                        new PropertyMetadata(default(CornerRadius)));
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        #endregion

        static RoundedTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RoundedTextBox), new FrameworkPropertyMetadata(typeof(RoundedTextBox)));
        }
    }
}
