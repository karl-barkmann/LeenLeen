using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Interactivity;

namespace Smart.Windows.Interactivity
{
    public class RichTextBoxBindingBehavior : Behavior<RichTextBox>
    {
        private bool userInputting;

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

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

        protected override void OnAttached()
        {
            base.OnAttached();
            if (AssociatedObject != null)
            {
                AssociatedObject.PreviewKeyUp += AssociatedObject_PreviewKeyDown;
            }
        }

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
