using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using LocationService.Application.Events.Publishers;
using LocationService.Application.Interfaces;
using MediatR;

namespace LocationService.Application.Pipeline;

public sealed class ApplicationPipelineBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly IOutboxPublisher _outboxPublisher;
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ApplicationPipelineBehaviour(IOutboxPublisher outboxPublisher, IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
        _outboxPublisher = outboxPublisher;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        ApplyValidators(request);
        var response = await next();
        
        if (request is ICommand<TResponse>)
        {
            await _outboxPublisher.PublishOutboxAsync();
        }
        
        return response;
    }

    private void ApplyValidators(TRequest request)
    {
        var context = new ValidationContext<TRequest>(request);
        var failures = _validators
            .Select(validator => validator.Validate(context))
            .SelectMany(result => result.Errors)
            .Where(failure => failure != null)
            .ToList();

        if (!failures.Any()) return;
        
        throw new ValidationException(failures);
    }
}