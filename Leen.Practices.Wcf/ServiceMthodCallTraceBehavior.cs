using System;
using System.Collections.Generic;
using Microsoft.Practices.Unity.InterceptionExtension;
using Leen.Logging;

namespace Leen.Practices.Wcf
{
    class ServiceMthodCallTraceBehavior : IInterceptionBehavior
    {
        private ILogWriter logger;

        public ServiceMthodCallTraceBehavior(ILogWriter logger)
        {
            if (logger == null) throw new ArgumentNullException("source");
            this.logger = logger;
        }

        public IEnumerable<Type> GetRequiredInterfaces()
        {
            return Type.EmptyTypes;
        }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            IMethodReturn methodReturn = getNext().Invoke(input, getNext);

            if (methodReturn.Exception != null)
            {
                this.logger.Trace(
                    "Finished {0} with exception {1}: {2}",
                    input.MethodBase.ToString(),
                    methodReturn.Exception.GetType().Name,
                    methodReturn.Exception.Message);
            }

            return methodReturn;
        }

        public bool WillExecute
        {
            get { return true; }
        }
    }
}
