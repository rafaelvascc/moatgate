using System;

namespace MoatGate.RequestHandling
{
    public class ErrorResponse : Response
    {
        public ErrorResponse(Guid id, Exception ex = null, int statusCode = 500) : base(id)
        {
            StatusCode = statusCode;
            Exception = ex;
            HasError = true;            
        }
    }
}
