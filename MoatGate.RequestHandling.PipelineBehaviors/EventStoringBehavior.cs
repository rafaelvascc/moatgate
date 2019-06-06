using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace MoatGate.RequestHandling.PipelineBehaviors
{
    //TODO: Implement as CQRS event store
    public class EventStoringBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : Request
        where TResponse : Response
    {
        private readonly IEventStorer _storer;

        public EventStoringBehavior(IEventStorer storer)
        {
            _storer = storer;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if (request is ICommand)
            {
                await _storer.AddAsync(((ICommand)request).ToEvent());
            }
            return await next();
        }
    }    
}
