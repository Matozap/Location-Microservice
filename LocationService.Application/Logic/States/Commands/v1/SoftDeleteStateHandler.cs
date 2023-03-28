using System;
using System.Threading;
using System.Threading.Tasks;
using LocationService.Application.Interfaces;
using LocationService.Domain;
using LocationService.Message.Definition.States.Requests.v1;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocationService.Application.Logic.States.Commands.v1;

public class SoftDeleteStateHandler : IRequestHandler<SoftDeleteState, object>
{
    private readonly ILogger<SoftDeleteStateHandler> _logger;
    private readonly IRepository _repository;

    public SoftDeleteStateHandler(ILogger<SoftDeleteStateHandler> logger, IRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task<object> Handle(SoftDeleteState request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request.Id);
        
        var entity = await DisableState(request.Id);
        _logger.LogInformation("State with id {StateId} disabled successfully", entity?.Id);
        return entity?.Id;
    }

    private async Task<State> DisableState(string stateId)
    {
        var entity = await _repository.GetAsSingleAsync<State, string>(c => c.Id == stateId || c.Code == stateId);
        if (entity == null) return null;
        
        entity.Disabled = true;
        await _repository.UpdateAsync(entity);

        return entity;
    }
}
