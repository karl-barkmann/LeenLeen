﻿namespace Microsoft.Practices.ServiceLocation
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class ActivationException : Exception
    {
        public ActivationException()
        {

        }

        public ActivationException(string message) : base(message)
        {
        }

        protected ActivationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ActivationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

