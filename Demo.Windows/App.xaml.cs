using Demo.Windows.ViewModel;
using Leen.Practices.Mvvm;
using System;
using System.Reflection;
using System.Windows;
using System.Windows.Media;

namespace Demo.Windows
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var bootstrapper = new Bootstrapper();
            bootstrapper.Run(e.Args);
        }

        private void OnAction(int a)
        {
        }
    }
}
