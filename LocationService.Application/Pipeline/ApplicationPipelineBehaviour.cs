using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using LocationService.Application.Events.Publishers;
using LocationService.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocationService.Application.Pipeline;

public sealed class ApplicationPipelineBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly IOutboxPublisher _outboxPublisher;
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    private readonly ILogger<ApplicationPipelineBehaviour<TRequest, TResponse>> _logger;

    public ApplicationPipelineBehaviour(IOutboxPublisher outboxPublisher, IEnumerable<IValidator<TRequest>> validators, ILogger<ApplicationPipelineBehaviour<TRequest, TResponse>> logger)
    {
        _logger = logger;
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
        
        var failureMessage = string.Join(", ", failures.Select(failure => failure.ErrorMessage).ToArray());
        throw new System.ComponentModel.DataAnnotations.ValidationException(failureMessage);
    }
}