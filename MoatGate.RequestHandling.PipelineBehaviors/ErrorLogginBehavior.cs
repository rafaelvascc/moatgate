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
        public ILoggerFactory _loggerFactory;

        public ErrorLogginBehavior(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var result = await next();
            if (result is ErrorResponse)
            {
                var logger = _loggerFactory.CreateLogger(typeof(TRequest).FullName);
                logger.LogError(result.Exception, string.Join('|', result.Messages));
            }
            return result;
        }
    }
}
