using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace MoatGate.RequestHandling.PipelineBehaviors
{
    //TODO: implement
    public class ErrorLogginBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : Request
        where TResponse : Response
    {
        public ILogger _logger;

        public ErrorLogginBehavior(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger("MoatGate");
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var result = await next();
            if (result is ErrorResponse)
            {
                _logger.LogDebug(result.Exception, string.Join('|', result.Messages));
            }
            return result;
        }
    }
}
