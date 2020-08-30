using Leen.Practices.Mvvm;
using System;
using System.Windows;

namespace Demo.Windows.View
{
    /// <summary>
    /// ChildWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TingCrawlerWindow : Window,IView
    {
        public TingCrawlerWindow()
        {
            InitializeComponent();
        }

        public FrameworkElement ActualView => this;
    }
}
