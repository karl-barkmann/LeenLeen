//Copyright \x00a9 Microsoft 2008
namespace Microsoft.Practices.ServiceLocation
{
    using System;

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

