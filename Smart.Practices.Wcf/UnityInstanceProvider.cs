using System;
using System.ServiceModel.Dispatcher;
using Microsoft.Practices.Unity;

namespace Smart.Practices.Wcf
{
    class UnityInstanceProvider : IInstanceProvider
    {
        private readonly IUnityContainer container;
        private readonly Type contractType;

        public UnityInstanceProvider(IUnityContainer container,Type contractType)
        {
            this.container = container;
            this.contractType = contractType;
        }

        public object GetInstance(System.ServiceModel.InstanceContext instanceContext, System.ServiceModel.Channels.Message message)
        {
            return container.Resolve(contractType);
        }

        public object GetInstance(System.ServiceModel.InstanceContext instanceContext)
        {
            return GetInstance(instanceContext, null);
        }

        public void ReleaseInstance(System.ServiceModel.InstanceContext instanceContext, object instance)
        {
            container.Teardown(instance);
        }
    }
}
