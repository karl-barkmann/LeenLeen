using System.Linq;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using Microsoft.Practices.Unity;

namespace Smart.Practices.Wcf
{
    class UnityServiceBehavior : IServiceBehavior
    {
        private readonly IUnityContainer container;

        public UnityServiceBehavior(IUnityContainer container)
        {
            this.container = container;
        }

        public void AddBindingParameters(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
            foreach (var channel in serviceHostBase.ChannelDispatchers)
            {
                (channel as ChannelDispatcher).ErrorHandlers.Add(container.Resolve<IErrorHandler>());
            }
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase)
        {
            foreach (ChannelDispatcher channelDispatcher in serviceHostBase.ChannelDispatchers)
            {
                foreach (var endPoint in channelDispatcher.Endpoints)
                {
                    if (endPoint.ContractName != "IMetadataExchange")
                    {
                        string contractName = endPoint.ContractName;
                        ServiceEndpoint serviceEndpoint = serviceDescription.Endpoints.FirstOrDefault(
                            service => service.Contract.Name == contractName);
                        endPoint.DispatchRuntime.InstanceProvider = new UnityInstanceProvider(container,
                            serviceEndpoint.Contract.ContractType);
                    }
                }
            }
        }

        public void Validate(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase)
        {

        }
    }
}
