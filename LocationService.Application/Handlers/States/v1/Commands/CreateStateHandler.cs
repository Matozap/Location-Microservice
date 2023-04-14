using System;
using System.Threading;
using System.Threading.Tasks;
using LocationService.Application.Handlers.States.v1.Requests;
using LocationService.Application.Interfaces;
using LocationService.Domain;
using LocationService.Message.Contracts.States.v1;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocationService.Application.Handlers.States.v1.Commands;

public class CreateStateHandler : IRequestHandler<CreateState, StateData>
{
    private readonly ILogger<CreateStateHandler> _logger;
    private readonly IRepository _repository;

    public CreateStateHandler(ILogger<CreateStateHandler> logger, IRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task<StateData> Handle(CreateState request, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(request.Details?.Name);
        
        var resultEntity = await CreateState(request.Details);
        if (resultEntity == null) return null;
        
        _logger.LogInformation("State with id {StateID} created successfully", resultEntity.Id);
        var resultDto = resultEntity.Adapt<State, StateData>();

        return resultDto;
    }

    private async Task<State> CreateState(StateData state)
    {
        if (await _repository.GetAsSingleAsync<State, string>(e => e.Code == state.Code && e.CountryId == state.CountryId) != null)
        {
            return null;
        }
        
        var entity = state.Adapt<StateData, State>();
        entity.LastUpdateUserId ??= "system";
        entity.LastUpdateDate = DateTime.Now;
        
        return await _repository.AddAsync(entity);
    }
}
