using System;
using System.Threading;
using System.Threading.Tasks;
using LocationService.Application.Interfaces;
using LocationService.Message.DataTransfer.Countries.v1;
using LocationService.Message.Definition;
using LocationService.Message.Definition.Countries.Events.v1;
using LocationService.Message.Definition.Countries.Requests.v1;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocationService.Application.Logic.Countries.Commands.v1;

public class DeleteCountryHandler : IRequestHandler<DeleteCountry, object>
{
    private readonly ILogger<DeleteCountryHandler> _logger;
    private readonly ILocationRepository _repository;
    private readonly IEventBus _eventBus;

    public DeleteCountryHandler(ILogger<DeleteCountryHandler> logger, ILocationRepository repository, IEventBus eventBus)
    {
        _logger = logger;
        _repository = repository;
        _eventBus = eventBus;
    }

    public async Task<object> Handle(DeleteCountry request, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(request.Id);

        await DeleteCountryAsync(request.Id);

        _ = _eventBus.Publish(new CountryEvent { LocationDetails = new CountryData {Id = request.Id}, Action = EventAction.CountryDelete});

        return request.Id;
    }

    private async Task DeleteCountryAsync(string countryId)
    {
        var entity = await _repository.GetCountryAsync(c => c.Id == countryId || c.Code == countryId);
            
        if(entity != null)
        {                
            await _repository.DeleteAsync(entity);
            _logger.LogInformation("Country with id {CountryId} was completely deleted", countryId);
        }
    }
}
