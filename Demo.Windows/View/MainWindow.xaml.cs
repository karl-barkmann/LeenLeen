using Leen.Practices.Mvvm;
using System.Windows;

namespace Demo.Windows.View
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window,IView
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public FrameworkElement ActualView => this;
    }
}
