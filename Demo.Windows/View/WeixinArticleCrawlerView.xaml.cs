using Leen.Practices.Mvvm;
using System.Windows;

namespace Demo.Windows.View
{
    /// <summary>
    /// SimpleTraderCrawlerWindow.xaml 的交互逻辑
    /// </summary>
    public partial class WeixinArticleCrawlerView : Window, IView
    {
        public WeixinArticleCrawlerView()
        {
            InitializeComponent();
        }

        public FrameworkElement ActualView => this;
    }
}
