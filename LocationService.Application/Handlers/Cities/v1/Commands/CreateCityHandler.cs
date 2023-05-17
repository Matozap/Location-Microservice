using System;
using System.Threading;
using System.Threading.Tasks;
using LocationService.Application.Interfaces;
using LocationService.Domain;
using LocationService.Message.Contracts.Cities.v1;
using LocationService.Message.Contracts.Cities.v1.Requests;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocationService.Application.Handlers.Cities.v1.Commands;

public class CreateCityHandler : IRequestHandler<CreateCity, CityData>
{
    private readonly ILogger<CreateCityHandler> _logger;
    private readonly IRepository _repository;

    public CreateCityHandler(ILogger<CreateCityHandler> logger, IRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task<CityData> Handle(CreateCity request, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(request.Details?.Name);
        
        var resultEntity = await CreateCity(request.Details);
        if (resultEntity == null) return null;
        
        _logger.LogInformation("City with id {CityID} created successfully", resultEntity.Id);
        var resultDto = resultEntity.Adapt<City, CityData>();

        return resultDto;
    }

    private async Task<City> CreateCity(CityData city)
    {
        if (await _repository.GetAsSingleAsync<City, string>(e => e.Name == city.Name && e.StateId == city.StateId) != null)
        {
            return null;
        }
        
        var entity = city.Adapt<CityData, City>();
        entity.LastUpdateUserId ??= "system";
        entity.LastUpdateDate = DateTime.Now;
        
        return await _repository.AddAsync(entity);
    }
}
