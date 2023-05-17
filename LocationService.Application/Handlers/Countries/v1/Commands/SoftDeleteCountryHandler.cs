using System;
using System.Threading;
using System.Threading.Tasks;
using LocationService.Application.Interfaces;
using LocationService.Domain;
using LocationService.Message.Contracts.Countries.v1.Requests;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocationService.Application.Handlers.Countries.v1.Commands;

public class SoftDeleteCountryHandler : IRequestHandler<SoftDeleteCountry, string>
{
    private readonly ILogger<SoftDeleteCountryHandler> _logger;
    private readonly IRepository _repository;

    public SoftDeleteCountryHandler(ILogger<SoftDeleteCountryHandler> logger, IRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task<string> Handle(SoftDeleteCountry request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request.Id);

        var entity = await DisableCountry(request.Id);
        _logger.LogInformation("Country with id {CountryID} disabled successfully", entity?.Id);
        return entity?.Id;
    }

    private async Task<Country> DisableCountry(string countryId)
    {
        var entity = await _repository.GetAsSingleAsync<Country, string>(c => c.Id == countryId || c.Code == countryId);
        if (entity == null) return null;
        
        entity.Disabled = true;
        await _repository.UpdateAsync(entity);

        return entity;
    }
}
