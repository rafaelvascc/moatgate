using System.Threading.Tasks;

namespace MoatGate.RequestHandling
{
    public interface IEventStorer
    {
        Task AddAsync(Event @event);
    }
}
