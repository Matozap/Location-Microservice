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

public class SoftDeleteCountryHandler : IRequestHandler<SoftDeleteCountry, object>
{
    private readonly ILogger<SoftDeleteCountryHandler> _logger;
    private readonly ILocationRepository _repository;
    private readonly IEventBus _eventBus;

    public SoftDeleteCountryHandler(ILogger<SoftDeleteCountryHandler> logger, ILocationRepository repository, IEventBus eventBus)
    {
        _logger = logger;
        _repository = repository;
        _eventBus = eventBus;
    }

    public async Task<object> Handle(SoftDeleteCountry request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request.Id);

        await UpdateCountry(request.Id);

        _ = _eventBus.Publish(new CountryEvent { LocationDetails = new CountryData { Id = request.Id }, Action = EventAction.CountryDelete});

        return request.Id;
    }

    private async Task UpdateCountry(string countryId)
    {
        var entity = await _repository.GetCountryAsync(c => c.Id == countryId || c.Code == countryId);
            
        if(entity != null)
        {
            entity.Disabled = true;
            await _repository.UpdateAsync(entity);
            _logger.LogInformation("Country with id {CountryId} was soft deleted", countryId);
        }
    }
}
