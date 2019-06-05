using System;
using System.Collections.Generic;

namespace MoatGate.RequestHandling
{
    public class Request : MediatR.IRequest<Response>
    {
        public Request()
        {
            Id = new Guid();
            RequestDate = DateTime.UtcNow;
        }

        /// <summary>
        /// A request id that should be used as an idempontency code
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Any other necessary information for the Request
        /// </summary>
        public Dictionary<string, object> Meta { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// Gets the request date and time
        /// </summary>
        public DateTime RequestDate { get; }
    }

    /// <summary>
    /// Represents a request made to MoatGate to process and return a response.
    /// </summary>
    /// <typeparam name="T">The type of the Request payload</typeparam>
    public class Request<T> : Request
    {
        public Request() : base() { }

        /// <summary>
        /// The payload of the request
        /// </summary>
        public T Payload { get; set; }
    }
}
