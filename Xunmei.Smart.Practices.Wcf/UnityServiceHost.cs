using System;
using System.ServiceModel.Web;
using Microsoft.Practices.Unity;

namespace Xunmei.AlarmCentre.Server
{
    class UnityServiceHost : WebServiceHost
    {
        private readonly IUnityContainer container;

        public UnityServiceHost(IUnityContainer container, Type serviceType)
            : base(serviceType)
        {
            this.container = container;
        }

        protected override void OnOpening()
        {
            base.OnOpening();

            if (Description.Behaviors.Find<UnityServiceBehavior>()==null)
            {
                Description.Behaviors.Add(new UnityServiceBehavior(container));
            }
        }
    }
}
