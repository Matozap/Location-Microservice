using System;
using System.Threading;
using System.Threading.Tasks;
using LocationService.Application.Interfaces;
using LocationService.Domain;
using LocationService.Message.DataTransfer.Countries.v1;
using LocationService.Message.Definition.Countries.Requests.v1;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocationService.Application.Logic.Countries.Commands.v1;

public class CreateCountryHandler : IRequestHandler<CreateCountry, object>
{
    private readonly ILogger<CreateCountryHandler> _logger;
    private readonly IRepository _repository;

    public CreateCountryHandler(ILogger<CreateCountryHandler> logger, IRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task<object> Handle(CreateCountry request, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(request.LocationDetails?.Name);
        
        var resultEntity = await CreateCountry(request.LocationDetails);
        if (resultEntity == null) return null;
            
        _logger.LogInformation("Country with id {CountryID} created successfully", resultEntity.Id);
        return resultEntity.Adapt<Country, CountryData>();
    }

    private async Task<Country> CreateCountry(CountryData country)
    {
        if (await _repository.GetAsSingleAsync<Country,string>(e => e.Code == country.Code || e.Name == country.Name) != null)
        {
            return null;
        }
        
        var entity = country.Adapt<CountryData, Country>();
        entity.LastUpdateUserId ??= "system";
        entity.LastUpdateDate = DateTime.Now;
        
        return await _repository.AddAsync(entity);
    }
}
