using System;

namespace F1Solutions.Naati.Common.Contracts.Dal
{
    [Serializable]
    public class WebServiceException : Exception
    {
        public WebServiceException(string message)
            : base(message) { }

        public WebServiceException(string message, Exception innerException)
            : base(message, innerException) { }

        public WebServiceException()
        {
            
        }
    }
}
