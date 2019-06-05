using System;
using System.Collections.Generic;
using Utf8Json;

namespace MoatGate.RequestHandling.Exceptions
{
    public class RestRequestUserCreationException : RestResquestException
    {
        public IList<string> Errors { get; private set; } = new List<string>();

        public RestRequestUserCreationException(string errorMessage, int statusCode, string responseContent = null, Exception innerException = null)
            : base(errorMessage, statusCode, responseContent, innerException)
        {
            if (!string.IsNullOrWhiteSpace(responseContent))
            {
                try
                {
                    Errors = JsonSerializer.Deserialize<IList<string>>(responseContent);
                }
                catch
                {
                    Errors = new List<string> { "Unspecified error" };
                }
            }
        }
    }
}
