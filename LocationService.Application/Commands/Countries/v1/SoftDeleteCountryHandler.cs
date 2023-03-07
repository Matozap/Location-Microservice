using System;
using System.Threading;
using System.Threading.Tasks;
using LocationService.Application.Interfaces;
using LocationService.Message.DTO.Countries.v1;
using LocationService.Message.Messaging.Event;
using LocationService.Message.Messaging.Event.v1;
using LocationService.Message.Messaging.Request.Countries.v1;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocationService.Application.Commands.Countries.v1;

public class SoftDeleteCountryHandler : IRequestHandler<SoftDeleteCountry, object>
{
    private readonly ILogger<SoftDeleteCountryHandler> _logger;
    private readonly ILocationRepository _repository;
    private readonly IMediator _mediator;
    private readonly IEventBus _eventBus;

    public SoftDeleteCountryHandler(ILogger<SoftDeleteCountryHandler> logger, ILocationRepository repository, IMediator mediator, IEventBus eventBus)
    {
        _logger = logger;
        _repository = repository;
        _mediator = mediator;
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
        var query = new GetCountryById
        {
            Id = countryId
        };
        var readResult = await _mediator.Send(query);
        var resultDto = (CountryData)readResult;
            
        if(resultDto != null)
        {
            var result = resultDto.Adapt<CountryData, Domain.Country>();
            result.Disabled = true;
            await _repository.UpdateAsync(result);
            _logger.LogInformation("Country with id {CountryId} was soft deleted", countryId);
        }
    }
}
