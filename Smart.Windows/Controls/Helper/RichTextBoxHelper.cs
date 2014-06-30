/* * * * * * * * * * * * * * * * * * * * * * * * * * * 
 * 作者：李平
 * 日期：2012/8/14 16:07:42
 * 描述：RichTextBox绑定帮助类。
 * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System;
using System.Windows;
using System.Windows.Controls;

namespace Smart.Windows.Controls.Helper
{
    /// <summary>
    /// RichTextBox绑定帮助类。
    /// </summary>
    internal class RichTextBoxHelper : DependencyObject
    {
        public static readonly DependencyProperty DocumentXamlProperty =
            DependencyProperty.RegisterAttached("DocumentXaml", typeof(String), typeof(RichTextBoxHelper),
                new FrameworkPropertyMetadata
                {
                    BindsTwoWayByDefault = true,
                    PropertyChangedCallback = (source, e) =>
                    {
                        var richTextBox = source as RichTextBox;
                    }
                });
    }
}
