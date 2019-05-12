//Copyright \x00a9 Microsoft 2008

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Leen.Practices.ServiceLocation
{
    /// <summary>
    /// 简单服务检索器。
    /// </summary>
    public static class SimpleServiceLocator
    {
        private static Dictionary<Type, ServiceInfo> services = new Dictionary<Type, ServiceInfo>();
        private static readonly object syncLock = new object();

        public static bool IsRegistered<TInterface>() where TInterface : class
        {
            lock (syncLock)
            {
                return services.ContainsKey(typeof(TInterface));
            }
        }

        /// <summary>
        /// 注册服务。
        /// </summary>
        /// <typeparam name="TInterface">服务接口。</typeparam>
        /// <typeparam name="TImplemention">服务实现。</typeparam>
        public static void Register<TInterface, TImplemention>() where TImplemention : TInterface
        {
            Register<TInterface, TImplemention>(false);
        }

        /// <summary>
        /// 注册单例服务。
        /// </summary>
        /// <typeparam name="TInterface">服务接口。</typeparam>
        /// <typeparam name="TImplemention">服务实现。</typeparam>
        public static void RegisterSingleton<TInterface, TImplemention>() where TImplemention : TInterface
        {
            Register<TInterface, TImplemention>(true);
        }

        /// <summary>
        /// 获取服务。
        /// </summary>
        /// <typeparam name="TInterface">服务接口。</typeparam>
        /// <returns></returns>
        public static TInterface Resolve<TInterface>()
        {
            return (TInterface)services[typeof(TInterface)].ServiceImplemention;
        }

        /// <summary>
        /// 注册服务。
        /// </summary>
        /// <typeparam name="TInterface">服务接口。</typeparam>
        /// <typeparam name="TImplemention">服务实现。</typeparam>
        /// <param name="isSingleton">是否以单例形式注册服务。</param>
        public static void Register<TInterface, TImplemention>(bool isSingleton) where TImplemention : TInterface
        {
            lock (syncLock)
            {
                services.Add(typeof(TInterface), new ServiceInfo(typeof(TImplemention), isSingleton));
            }
        }

        private class ServiceInfo
        {
            private object serviceImplemention;
            private Type serviceImplementionType;
            private bool isSingleton;

            public ServiceInfo(Type serviceImplementionType, bool isSingleton)
            {
                this.serviceImplementionType = serviceImplementionType;
                this.isSingleton = isSingleton;
            }

            public object ServiceImplemention
            {
                get
                {
                    if (isSingleton)
                    {
                        if (serviceImplemention == null)
                            serviceImplemention = CreateInstance(serviceImplementionType);
                        return serviceImplemention;
                    }
                    else
                    {
                        return CreateInstance(serviceImplementionType);
                    }
                }
            }

            private static object CreateInstance(Type type)
            {
                if (services.ContainsKey(type))
                {
                    return services[type].ServiceImplemention;
                }

                ConstructorInfo constructorInfo = type.GetConstructors().First();
                var parameters = from parameter in constructorInfo.GetParameters()
                                 select CreateInstance(parameter.ParameterType);

                return Activator.CreateInstance(type, parameters.ToArray());
            }
        }
    }
}
