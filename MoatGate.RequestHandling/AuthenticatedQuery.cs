using System;

namespace MoatGate.RequestHandling
{
    public class AuthenticatedQuery : Query, IAuthenticatedRequest
    {
        public Guid UserId { get; set; }
    }

    public class AuthenticatedQuery<T> : Query<T>, IAuthenticatedRequest
    {
        public Guid UserId { get; set; }
    }
}
