using System;
using System.Threading;
using System.Threading.Tasks;
using LocationService.Application.Handlers.States.v1.Requests;
using LocationService.Application.Interfaces;
using LocationService.Domain;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocationService.Application.Handlers.States.v1.Commands;

public class SoftDeleteStateHandler : IRequestHandler<SoftDeleteState, string>
{
    private readonly ILogger<SoftDeleteStateHandler> _logger;
    private readonly IRepository _repository;

    public SoftDeleteStateHandler(ILogger<SoftDeleteStateHandler> logger, IRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task<string> Handle(SoftDeleteState request, CancellationToken cancellationToken)
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
