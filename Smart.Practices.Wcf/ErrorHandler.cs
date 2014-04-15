using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace Smart.Practices.Wcf
{
    /// <summary>
    /// 服务器错误处理。
    /// </summary>
    class ErrorHandler : IErrorHandler
    {
        public bool HandleError(Exception error)
        {
            if (error is FaultException)
                return true;

            return true;
        }

        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
            if (error is FaultException)
                return;

            FaultException faultException = new FaultException(error.Message);

            MessageFault message = faultException.CreateMessageFault();

            fault = Message.CreateMessage(version, message, "");
        }
    }
}
