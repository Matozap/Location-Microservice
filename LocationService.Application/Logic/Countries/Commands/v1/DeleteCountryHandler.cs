using System;
using System.Threading;
using System.Threading.Tasks;
using LocationService.Application.Interfaces;
using LocationService.Domain;
using LocationService.Message.DataTransfer.Countries.v1;
using LocationService.Message.Definition;
using LocationService.Message.Definition.Countries.Events.v1;
using LocationService.Message.Definition.Countries.Requests.v1;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocationService.Application.Logic.Countries.Commands.v1;

public class DeleteCountryHandler : IRequestHandler<DeleteCountry, object>
{
    private readonly ILogger<DeleteCountryHandler> _logger;
    private readonly IRepository _repository;
    private readonly IEventBus _eventBus;

    public DeleteCountryHandler(ILogger<DeleteCountryHandler> logger, IRepository repository, IEventBus eventBus)
    {
        _logger = logger;
        _repository = repository;
        _eventBus = eventBus;
    }

    public async Task<object> Handle(DeleteCountry request, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(request.Id);

        var entity = await DeleteCountryAsync(request.Id);

        if (entity != null)
        {
            var publishData = entity.Adapt<Country, CountryData>();
            publishData.States = null;
            _ = _eventBus.Publish(new CountryEvent { LocationDetails = publishData, Action = EventAction.CountryDelete });
        }

        return request.Id;
    }

    private async Task<Country> DeleteCountryAsync(string id)
    {
        var entity = await _repository.GetCountryAsync(c => c.Id == id || c.Code == id);
            
        if(entity != null)
        {                
            await _repository.DeleteAsync(entity);
            _logger.LogInformation("Country with id {CountryId} was completely deleted", id);
        }

        return entity;
    }
}
