using System;
using System.Collections.Generic;
using System.Text;

namespace MoatGate.RequestHandling
{
    public class Event
    {
        public Event()
        {
            Id = Guid.NewGuid();
            EventDate = DateTime.UtcNow;
        }

        public Guid Id { get; set; }
        public DateTime EventDate { get; set; }
        public string EventType { get; set; }
        public string PayloadType { get; set; }
        public string PayloadJson { get; set; }
        public Dictionary<string, object> Meta { get; set; } = new Dictionary<string, object>();
    }
}
