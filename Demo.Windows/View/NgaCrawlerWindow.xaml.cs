using Leen.Practices.Mvvm;
using System.Windows;

namespace Demo.Windows.View
{
    /// <summary>
    /// NgaCrawlerWindow.xaml 的交互逻辑
    /// </summary>
    public partial class NgaCrawlerWindow : Window, IView
    {
        public NgaCrawlerWindow()
        {
            InitializeComponent();
        }

        public FrameworkElement ActualView => this;
    }
}
