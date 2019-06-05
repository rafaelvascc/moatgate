using System;

namespace MoatGate.RequestHandling
{
    public class AuthenticatedRequest<T> : Request, IAuthenticatedRequest
    {
        public Guid UserId { get; set; }
    }
}
