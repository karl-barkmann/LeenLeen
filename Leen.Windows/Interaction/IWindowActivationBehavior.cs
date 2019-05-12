
namespace Leen.Windows.Interaction
{
    /// <summary>
    /// Defines a behavior that creates a Dialog to display the active view 。
    /// </summary>
    public interface IWindowActivationBehavior
    {
        /// <summary>
        /// Override this method to create an instance of the <see cref="IWindow"/> that 
        /// will be shown when a view is activated.
        /// </summary>
        /// <returns>
        /// An instance of <see cref="IWindow"/> that will be shown when a 
        /// view is activated .
        /// </returns>
        IWindow CreateWindow();
    }
}
