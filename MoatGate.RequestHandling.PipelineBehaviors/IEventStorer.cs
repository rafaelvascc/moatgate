using System.Threading.Tasks;

namespace MoatGate.RequestHandling.PipelineBehaviors
{
    public interface IEventStorer
    {
        Task AddAsync(Event @event);
    }
}
