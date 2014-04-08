namespace Microsoft.Practices.ServiceLocation
{
    using System;
    using System.Collections.Generic;

    public interface IServiceLocator : IServiceProvider
    {
        IEnumerable<TService> GetAllInstances<TService>();
        IEnumerable<object> GetAllInstances(Type serviceType);
        TService GetInstance<TService>();
        TService GetInstance<TService>(string key);
        object GetInstance(Type serviceType);
        object GetInstance(Type serviceType, string key);
    }
}

