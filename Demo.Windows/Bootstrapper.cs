using Demo.Windows.View;
using Leen.Practices.Mvvm;
using Leen.Windows.Interaction;
using System.ComponentModel.Composition;
using System.Windows;

namespace Demo.Windows
{
    class Bootstrapper : MefBoostrapper
    {
        protected override DependencyObject CreateShell()
        {
            return new MainWindow();
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();
            Container.ComposeExportedValue<IUIInteractionService>(new UIInteractionService());
        }

        protected override void InitializeShell(object[] args)
        {
            base.InitializeShell(args);

            var shell = Shell as MainWindow;
            Container.GetExportedValue<IUIInteractionService>().Shell = shell.DataContext;
            shell.ShowDialog();
        }
    }
}
