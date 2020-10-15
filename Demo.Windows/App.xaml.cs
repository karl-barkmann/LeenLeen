using System.Windows;

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

            var shell = new TestWindow("Shell ");
            shell.ShowDialog();

            //var bootstrapper = new Bootstrapper();
            //bootstrapper.Run(e.Args);
        }
    }
}
