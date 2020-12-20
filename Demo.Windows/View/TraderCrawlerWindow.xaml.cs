using Leen.Practices.Mvvm;
using System.Windows;

namespace Demo.Windows.View
{
    /// <summary>
    /// SimpleTraderCrawlerWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TraderCrawlerWindow : Window, IView
    {
        public TraderCrawlerWindow()
        {
            InitializeComponent();
        }

        public FrameworkElement ActualView => this;
    }
}
