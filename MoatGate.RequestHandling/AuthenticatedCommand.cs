using System;

namespace MoatGate.RequestHandling
{
    public class AuthenticatedCommand<T> : Command<T>, IAuthenticatedRequest
    {
        public Guid UserId { get; set; }
    }
}
