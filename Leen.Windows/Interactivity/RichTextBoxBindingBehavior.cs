using System.Windows.Controls;
using System.Windows.Documents;

namespace System.Windows.Interactivity
{
    /// <summary>
    /// 使 <see cref="RichTextBox"/> 控件支持文本绑定。
    /// </summary>
    public class RichTextBoxBindingBehavior : Behavior<RichTextBox>
    {
        private bool userInputting;

        /// <summary>
        /// 获取或设置到此行为附加到的 <see cref="RichTextBox"/> 需要显示的文本。
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        /// 提供对 <see cref="Text"/> 的依赖属性支持。
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(RichTextBoxBindingBehavior), new PropertyMetadata(String.Empty, OnTextPropertyChanged));

        private static void OnTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = d as RichTextBoxBindingBehavior;
            if (behavior != null && behavior.AssociatedObject != null && !behavior.userInputting)
            {
                behavior.AssociatedObject.Document.Blocks.Clear();
                if (e.NewValue != null)
                {
                    behavior.AssociatedObject.Document.Blocks.Add(new Paragraph(new Run(e.NewValue.ToString())));
                }
            }
        }

        /// <summary>
        /// 在行为附加到 AssociatedObject 后调用。
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            if (AssociatedObject != null)
            {
                AssociatedObject.PreviewKeyUp += AssociatedObject_PreviewKeyDown;
            }
        }

        /// <summary>
        /// 在行为与其 AssociatedObject 分离时（但在它实际发生之前）调用。
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            if (AssociatedObject != null)
            {
                AssociatedObject.PreviewKeyUp -= AssociatedObject_PreviewKeyDown;
            }
        }

        private void AssociatedObject_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            userInputting = true;
            var richTextBox = sender as RichTextBox;
            TextRange textTag = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
            Text = textTag.Text.Replace("\r\n", "");
            userInputting = false;
        }
    }
}
