using Utf8Json;

namespace MoatGate.RequestHandling
{
    public class Command : Request, ICommand
    {
        public Event ToEvent()
        {
            return new Event()
            {
                EventType = this.GetType().FullName,
                Meta = Meta
            };
        }
    }

    public class Command<T> : Request<T>, ICommand
    {
        public Event ToEvent()
        {
            return new Event()
            {
                EventType = this.GetType().FullName,
                PayloadJson = JsonSerializer.ToJsonString(Payload),
                Meta = Meta
            };
        }
    }
}
