using CommonServiceLocator;
using Leen.Logging;
using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;

namespace Leen.Practices.Mvvm
{
    /// <summary>
    /// A mechanism to initialize application framework by MEF.
    /// </summary>
    public abstract class MefBoostrapper : Bootstrapper
    {
        /// <summary>
        /// Gets or sets the default <see cref="CompositionContainer"/> for the application.
        /// </summary>
        /// <value>The default <see cref="CompositionContainer"/> instance.</value>
        protected CompositionContainer Container { get; set; }

        /// <summary>
        /// Gets or sets the default <see cref="AggregateCatalog"/> for the application.
        /// </summary>
        /// <value>The default <see cref="AggregateCatalog"/> instance.</value>
        protected AggregateCatalog AggregateCatalog { get; set; }

        /// <summary>
        /// Configures the <see cref="AggregateCatalog"/> used by MEF.
        /// </summary>
        /// <remarks>
        /// The base implementation returns a new AggregateCatalog.
        /// </remarks>
        /// <returns>An <see cref="AggregateCatalog"/> to be used by the bootstrapper.</returns>
        protected virtual AggregateCatalog CreateAggregateCatalog()
        {
            return new AggregateCatalog();
        }

        /// <summary>
        /// Configures the <see cref="AggregateCatalog"/> used by MEF.
        /// </summary>
        /// <remarks>
        /// The base implementation does nothing.
        /// </remarks>
        protected virtual void ConfigureAggregateCatalog()
        {

        }

        /// <summary>
        /// Creates the <see cref="CompositionContainer"/> that will be used as the default container.
        /// </summary>
        /// <returns>A new instance of <see cref="CompositionContainer"/>.</returns>
        /// <remarks>
        /// The base implementation registers a default MEF catalog of exports of key Prism types.
        /// Exporting your own types will replace these defaults.
        /// </remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability",
            "CA2000:Dispose objects before losing scope",
            Justification = "The default export provider is in the container and disposed by MEF.")]
        protected virtual CompositionContainer CreateContainer()
        {
            CompositionContainer container = new CompositionContainer(AggregateCatalog);
            return container;
        }

        /// <summary>
        /// Helper method for configuring the <see cref="CompositionContainer"/>. 
        /// Registers all the types direct instantiated by the bootstrapper with the container.
        /// </summary>
        protected virtual void RegisterBootstrapperProvidedTypes()
        {
            Container.ComposeExportedValue(Logger);
            Container.ComposeExportedValue<IServiceLocator>(new MefServiceLocatorAdapter(Container));
            Container.ComposeExportedValue(AggregateCatalog);
        }

        /// <summary>
        /// Configures the <see cref="CompositionContainer"/>. 
        /// May be overwritten in a derived class to add specific type mappings required by the application.
        /// </summary>
        /// <remarks>
        /// The base implementation registers all the types direct instantiated by the bootstrapper with the container.
        /// If the method is overwritten, the new implementation should call the base class version.
        /// </remarks>
        protected override void ConfigureContainer()
        {
            RegisterBootstrapperProvidedTypes();
        }

        /// <summary>
        /// Configures the LocatorProvider for the <see cref="ServiceLocator" />.
        /// </summary>
        /// <remarks>
        /// The base implementation also sets the ServiceLocator provider singleton.
        /// </remarks>
        protected override void ConfigureServiceProvider()
        {
            IServiceLocator serviceLocator = Container.GetExportedValue<IServiceLocator>();
            ServiceLocator.SetLocatorProvider(() => serviceLocator);
        }

        /// <summary>
        /// Initializes the shell.
        /// </summary>
        /// <remarks>
        /// The base implementation ensures the shell is composed in the container.
        /// </remarks>
        protected override void InitializeShell(object[] args)
        {
            Container.ComposeParts(Shell);
        }

        /// <summary>
        /// Start up application framework.
        /// </summary>
        /// <param name="args">Startup arguments.</param>
        public override void Run(string[] args)
        {
            base.Run(args);

            Logger = CreateLogger();
            if (Logger == null)
            {
                throw new InvalidOperationException("Logger can not be null.");
            }

            Logger.Log("Logger Created.", LogLevel.Debug, LogPriority.Medium);

            Logger.Log("Configuring process PATH environment variable.", LogLevel.Debug, LogPriority.Medium);
            ConfigureEnvironment();

            Logger.Log("Creating catalog for MEF.", LogLevel.Debug, LogPriority.Medium);
            AggregateCatalog = CreateAggregateCatalog();

            Logger.Log("Configuring catalog for MEF.", LogLevel.Debug, LogPriority.Medium);
            ConfigureAggregateCatalog();

            Logger.Log("Creating Ioc Container.", LogLevel.Debug, LogPriority.Medium);
            Container = CreateContainer();

            Logger.Log("Configuring Ioc Container.", LogLevel.Debug, LogPriority.Medium);
            ConfigureContainer();

            Logger.Log("Configuring ServiceLocator.", LogLevel.Debug, LogPriority.Medium);
            ConfigureServiceProvider();

            Logger.Log("Adding missing type mapping for naming conventions.", LogLevel.Debug, LogPriority.Medium);
            ConfigureMissingTypeMapping();

            Logger.Log("Creating application shell.", LogLevel.Debug, LogPriority.Medium);
            Shell = CreateShell();

            Logger.Log("Initializing application shell.", LogLevel.Debug, LogPriority.Medium);
            InitializeShell(args);
        }
    }
}
