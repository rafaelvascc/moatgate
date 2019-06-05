using System;

namespace MoatGate.RequestHandling.Exceptions
{
    public class AuthenticationException : RestResquestException
    {
        public AuthenticationException(string errorMessage, int statusCode, string responseContent = null, Exception innerException = null)
            : base(errorMessage, statusCode, responseContent, innerException)
        {
        }
    }
}
