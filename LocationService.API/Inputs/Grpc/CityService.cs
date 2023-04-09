using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using LocationService.API.Outputs;
using LocationService.API.Outputs.Base;
using LocationService.Message.DataTransfer.Cities.v1;
using MediatR;

namespace LocationService.API.Inputs.Grpc;

public class CityService : Message.DataTransfer.Cities.v1.CityService.CityServiceBase
{
    private readonly CityOutput _cityOutput;
    
    public CityService(IMediator mediator)
    {
        _cityOutput = new CityOutput(mediator, OutputType.Grpc);
    }

    public override async Task<CityCollection> GetAll(AllCitiesRequest request, ServerCallContext context) 
        => new() { Cities = { await _cityOutput.GetAllAsync<List<CityData>>(request.StateId) }};
    
    public override async Task<CityData> Get(CityIdRequest request, ServerCallContext context) 
        => await _cityOutput.GetAsync<CityData>(request.Id);

    public override async Task<CityData> Create(CityData request, ServerCallContext context) 
        => await _cityOutput.CreateAsync<CityData>(request);
    
    public override async Task<CityData> Update(CityData request, ServerCallContext context) 
        => await _cityOutput.UpdateAsync<CityData>(request);
    
    public override async Task<Empty> Disable(CityIdRequest request, ServerCallContext context)
    {
        await _cityOutput.DeleteAsync<CityData>(request.Id);
        return new Empty();
    }
}