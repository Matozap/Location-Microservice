using System.Threading;
using System.Threading.Tasks;
using LocationService.Application.Interfaces;
using LocationService.Application.Logic.Cities.v1.Requests;
using LocationService.Domain;
using LocationService.Message.Definition.Protos.Cities.v1;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocationService.Application.Logic.Cities.v1.Commands;

public class UpdateCityHandler : IRequestHandler<UpdateCity, CityData>
{
    private readonly ILogger<UpdateCityHandler> _logger;
    private readonly IRepository _repository;

    public UpdateCityHandler(ILogger<UpdateCityHandler> logger, IRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task<CityData> Handle(UpdateCity request, CancellationToken cancellationToken)
    {
        var result = await UpdateCity(request.Details);
        _logger.LogInformation("City with id {CityID} updated successfully", request.Details.Id);
            
        return result;
    }

    private async Task<CityData> UpdateCity(CityData cityData)
    {
        var entity = await _repository.GetAsSingleAsync<City, string>(city => city.Id == cityData.Id);
        if (entity == null) return null;
        
        cityData.StateId = entity.StateId;
        var changes = cityData.Adapt(entity);
        
        await _repository.UpdateAsync(changes);
        return changes.Adapt<City, CityData>();
    }
}
