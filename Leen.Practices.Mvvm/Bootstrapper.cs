using CommonServiceLocator;
using Leen.Logging;
using System;
using System.Windows;
using System.Windows.Threading;

namespace Leen.Practices.Mvvm
{
    /// <summary>
    /// A mechanism to initialize application framework.
    /// </summary>
    public abstract class Bootstrapper
    {
        /// <summary>
        /// Gets the shell user interface
        /// </summary>
        /// <value>The shell user interface.</value>
        public DependencyObject Shell { get; protected set; }

        /// <summary>
        /// Gets the <see cref="ILogger"/> for the application.
        /// </summary>
        /// <value>A <see cref="ILogger"/> instance.</value>
        protected ILogger Logger { get; set; }

        /// <summary>
        /// Runs the bootstrapper process.
        /// </summary>
        /// <param name="args">program startup parameters</param>
        public virtual void Run(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Dispatcher.CurrentDispatcher.UnhandledException += App_DispatcherUnhandledException;
        }

        /// <summary>
        /// Create the <see cref="ILogger" /> used by the bootstrapper.
        /// </summary>
        /// <remarks>
        /// The base implementation returns a new <see cref="ConsoleLogger"/>.
        /// </remarks>
        protected virtual ILogger CreateLogger()
        {
            return new DebugLogger();
        }

        /// <summary>
        /// Creates the shell or main window of the application.
        /// </summary>
        /// <returns>The shell of the application.</returns>
        protected abstract DependencyObject CreateShell(string[] args);

        /// <summary>
        /// Initializes the shell.
        /// </summary>
        /// <param name="args">program startup parameters</param>
        protected virtual void InitializeShell(string[] args)
        {

        }

        /// <summary>
        /// Configures the IOC container. 
        /// May be overwritten in a derived class to add specific type mappings required by the application.
        /// </summary>
        protected virtual void ConfigureContainer()
        {

        }

        /// <summary>
        /// Configures the LocatorProvider for the <see cref="ServiceLocator" />.
        /// </summary>
        protected virtual void ConfigureServiceProvider()
        {

        }

        /// <summary>
        /// Configures the missing type mapping for naming convention.
        /// </summary>
        protected virtual void ConfigureMissingTypeMapping()
        {

        }

        /// <summary>
        /// Configures the "PATH" environment variable of current process.
        /// </summary>
        protected virtual void ConfigureEnvironment()
        {
            
        }

        /// <summary>
        /// Called when an unhandled exception detected.
        /// </summary>
        /// <param name="error">The unhandled exception.</param>
        /// <param name="isTerminating">A value indicates wether the Application is shutting down.</param>
        /// <param name="handled">Set to true if exception is handled.</param>
        protected virtual void OnUnhandleException(Exception error, bool isTerminating, ref bool handled)
        {

        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            bool handled = false;
            OnUnhandleException((Exception)e.ExceptionObject, e.IsTerminating, ref handled);
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            bool handled = false;
            OnUnhandleException(e.Exception, false, ref handled);
            e.Handled = handled;
        }
    }
}
