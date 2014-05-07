//Copyright \x00a9 Microsoft 2008

using Microsoft.Practices.ServiceLocation;
namespace Smart.Practices.ServiceLocation
{
    public static class ServiceLocator
    {
        private static ServiceLocatorProvider currentProvider;

        public static void SetLocatorProvider(ServiceLocatorProvider newProvider)
        {
            currentProvider = newProvider;
        }

        public static IServiceLocator Current
        {
            get
            {
                return currentProvider();
            }
        }
    }
}

