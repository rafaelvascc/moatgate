using System;

namespace MoatGate.RequestHandling
{
    public interface IAuthenticatedRequest
    {
        Guid UserId { get; set; }
    }
}
