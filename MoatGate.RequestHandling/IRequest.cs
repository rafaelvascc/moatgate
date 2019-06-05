using System;
using System.Collections.Generic;

namespace MoatGate.RequestHandling
{
    public interface IRequest
    {
        Guid Id { get; set; }
        Dictionary<string, object> Meta { get; set; }
        DateTime RequestDate { get; }
    }

    public interface IRequest<T> : IRequest
    {
        T Payload { get; set; }
    }
}
