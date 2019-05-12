//Copyright \x00a9 Microsoft 2008

using System;
using System.Collections.Generic;
using System.Globalization;
using Leen.Practices.ServiceLocation.Properties;

namespace Leen.Practices.ServiceLocation
{
    public abstract class ServiceLocatorImplBase : IServiceLocator, IServiceProvider
    {
        protected ServiceLocatorImplBase()
        {
        }

        protected abstract IEnumerable<object> DoGetAllInstances(Type serviceType);
        protected abstract object DoGetInstance(Type serviceType, string key);
        protected virtual string FormatActivateAllExceptionMessage(Exception actualException, Type serviceType)
        {
            return string.Format(CultureInfo.CurrentUICulture, Resources.ActivateAllExceptionMessage, new object[] { serviceType.Name });
        }

        protected virtual string FormatActivationExceptionMessage(Exception actualException, Type serviceType, string key)
        {
            return string.Format(CultureInfo.CurrentUICulture, Resources.ActivationExceptionMessage, new object[] { serviceType.Name, key });
        }

        public virtual IEnumerable<TService> GetAllInstances<TService>()
        {
            foreach (object iteratorVariable0 in this.GetAllInstances(typeof(TService)))
            {
                yield return (TService) iteratorVariable0;
            }
        }

        public virtual IEnumerable<object> GetAllInstances(Type serviceType)
        {
            IEnumerable<object> enumerable;
            try
            {
                enumerable = this.DoGetAllInstances(serviceType);
            }
            catch (Exception exception)
            {
                throw new ActivationException(this.FormatActivateAllExceptionMessage(exception, serviceType), exception);
            }
            return enumerable;
        }

        public virtual TService GetInstance<TService>()
        {
            return (TService) this.GetInstance(typeof(TService), null);
        }

        public virtual TService GetInstance<TService>(string key)
        {
            return (TService) this.GetInstance(typeof(TService), key);
        }

        public virtual object GetInstance(Type serviceType)
        {
            return this.GetInstance(serviceType, null);
        }

        public virtual object GetInstance(Type serviceType, string key)
        {
            object obj2;
            try
            {
                obj2 = this.DoGetInstance(serviceType, key);
            }
            catch (Exception exception)
            {
                throw new ActivationException(this.FormatActivationExceptionMessage(exception, serviceType, key), exception);
            }
            return obj2;
        }

        public virtual object GetService(Type serviceType)
        {
            return this.GetInstance(serviceType, null);
        }
    }
}

