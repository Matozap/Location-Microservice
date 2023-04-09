using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using LocationService.API.Outputs;
using LocationService.API.Outputs.Base;
using LocationService.Message.DataTransfer.Countries.v1;
using MediatR;

namespace LocationService.API.Inputs.Grpc;

public class CountryService : Message.DataTransfer.Countries.v1.CountryService.CountryServiceBase
{
    private readonly CountryOutput _countryOutput;
    
    public CountryService(IMediator mediator)
    {
        _countryOutput = new CountryOutput(mediator, OutputType.Grpc);
    }

    public override async Task<CountryCollection> GetAll(AllCountriesRequest request, ServerCallContext context) 
        => new() { Countries = { await _countryOutput.GetAllAsync<List<CountryData>>() }};
    
    public override async Task<CountryData> Get(CountryIdRequest request, ServerCallContext context) 
        => await _countryOutput.GetAsync<CountryData>(request.Id);

    public override async Task<CountryData> Create(CountryData request, ServerCallContext context) 
        => await _countryOutput.CreateAsync<CountryData>(request);
    
    public override async Task<CountryData> Update(CountryData request, ServerCallContext context) 
        => await _countryOutput.UpdateAsync<CountryData>(request);
    
    public override async Task<Empty> Disable(CountryIdRequest request, ServerCallContext context)
    {
        await _countryOutput.DeleteAsync<CountryData>(request.Id);
        return new Empty();
    }
}