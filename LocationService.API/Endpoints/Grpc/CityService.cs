using System.Collections.Generic;
using System.Threading.Tasks;
using LocationService.API.Outputs;
using LocationService.API.Outputs.Base;
using LocationService.Application.Cities;
using LocationService.Application.Cities.Responses;
using LocationService.Application.Common;
using MediatR;

namespace LocationService.API.Endpoints.Grpc;

public class CityService : ICityService
{
    private readonly CityOutput _cityOutput;
     
     public CityService(IMediator mediator)
     {
         _cityOutput = new CityOutput(mediator, OutputType.Grpc);
     }
    
    public async Task<List<CityData>> GetAll(StringWrapper stateId) => await _cityOutput.GetAllAsync<List<CityData>>(stateId.Value);
    
    public async Task<CityData> Get(StringWrapper id) => await _cityOutput.GetAsync<CityData>(id.Value);
    
    public async Task<CityData> Create(CityData data) => await _cityOutput.CreateAsync<CityData>(data);
    
    public async Task<CityData> Update(CityData data) => await _cityOutput.UpdateAsync<CityData>(data);
    
    public async Task Disable(StringWrapper id) => await _cityOutput.DisableAsync<CityData>(id.Value);

    public async Task Delete(StringWrapper id) => await _cityOutput.DeleteAsync<CityData>(id.Value);
}