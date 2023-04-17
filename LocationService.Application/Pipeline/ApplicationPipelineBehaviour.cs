using System.Threading;
using System.Threading.Tasks;
using LocationService.Application.Events.Publishers;
using LocationService.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocationService.Application.Pipeline;

public class ApplicationPipelineBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly IOutboxPublisher _outboxPublisher;
    private readonly ILogger<ApplicationPipelineBehaviour<TRequest, TResponse>> _logger;

    public ApplicationPipelineBehaviour(IOutboxPublisher outboxPublisher, ILogger<ApplicationPipelineBehaviour<TRequest, TResponse>> logger)
    {
        _logger = logger;
        _outboxPublisher = outboxPublisher;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var response = await next();
        
        if (request is ICommand<TResponse>)
        {
            await _outboxPublisher.PublishOutboxAsync();
        }
        
        return response;
    }
}