using System;
using System.Windows;

namespace Demo.Windows
{
    /// <summary>
    /// TestWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TestWindow : Window
    {
        public TestWindow(string title)
        {
            InitializeComponent();
            Title = title;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var window = new TestWindow("Show " + DateTime.Now.ToLongTimeString());
            window.Owner = this;
            window.Show();
        }

        private void Button_Click1(object sender, RoutedEventArgs e)
        {
            var window = new TestWindow("Show " + DateTime.Now.ToLongTimeString());
            window.Owner = this;
            window.ShowDialog();
        }
    }
}
