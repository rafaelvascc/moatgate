using System;

namespace MoatGate.RequestHandling.Exceptions
{
    public class RestResquestException : Exception
    {
        public string Content { get; private set; }

        public int StatusCode { get; private set; }

        public RestResquestException(string errorMessage, int statusCode, string responseContent = null, Exception innerException = null) : 
            base(errorMessage, innerException)
        {
            this.StatusCode = statusCode;
            this.Content = responseContent;
        }
    }
}
