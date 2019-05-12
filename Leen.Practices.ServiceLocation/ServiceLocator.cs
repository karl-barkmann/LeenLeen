//Copyright \x00a9 Microsoft 2008

using Microsoft.Practices.ServiceLocation;
namespace Leen.Practices.ServiceLocation
{
    public static class ServiceLocator
    {
        private static ServiceLocatorProvider _currentProvider;

        public static void SetLocatorProvider(ServiceLocatorProvider newProvider)
        {
            _currentProvider = newProvider;
        }

        public static bool IsLocationProviderSet => _currentProvider != null;

        public static IServiceLocator Current
        {
            get
            {
                return _currentProvider();
            }
        }
    }
}

