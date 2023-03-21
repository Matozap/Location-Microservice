using System;
using System.Threading;
using System.Threading.Tasks;
using LocationService.Application.Interfaces;
using LocationService.Message.DataTransfer.States.v1;
using LocationService.Message.Definition;
using LocationService.Message.Definition.States.Events.v1;
using LocationService.Message.Definition.States.Requests.v1;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocationService.Application.Logic.States.Commands.v1;

public class CreateStateHandler : IRequestHandler<CreateState, object>
{
    private readonly ILogger<CreateStateHandler> _logger;
    private readonly IRepository _repository;
    private readonly IEventBus _eventBus;

    public CreateStateHandler(ILogger<CreateStateHandler> logger, IRepository repository, IEventBus eventBus)
    {
        _logger = logger;
        _repository = repository;
        _eventBus = eventBus;
    }

    public async Task<object> Handle(CreateState request, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(request.LocationDetails?.Name);
        
        var resultEntity = await CreateState(request.LocationDetails);
        if (resultEntity == null) return null;
        
        _logger.LogInformation("State with id {StateID} created successfully", resultEntity.Id);
        var resultDto = resultEntity.Adapt<Domain.State, StateData>();
            
        _ = _eventBus.Publish(new StateEvent { LocationDetails = request.LocationDetails, Action = EventAction.StateCreate});

        return resultDto;
    }

    private async Task<Domain.State> CreateState(StateData state)
    {
        if (await _repository.GetStateAsync(e => e.Code == state.Code && e.CountryId == state.CountryId) != null)
        {
            return null;
        }
        
        var entity = state.Adapt<StateData, Domain.State>();
        entity.LastUpdateUserId ??= "system";
        entity.LastUpdateDate = DateTime.Now;
        
        return await _repository.AddAsync(entity);
    }
}
