using System.Collections.Generic;
using System.Threading.Tasks;
using LocationService.API.Outputs;
using LocationService.API.Outputs.Base;
using LocationService.Message.Definition.Protos.Common;
using LocationService.Message.Definition.Protos.Countries.v1;
using MediatR;

namespace LocationService.API.Inputs.Grpc;

public class CountryService : ICountryService
{
    private readonly CountryOutput _countryOutput;
    
    public CountryService(IMediator mediator)
    {
        _countryOutput = new CountryOutput(mediator, OutputType.Grpc);
    }

    public async Task<List<CountryData>> GetAll(StringWrapper id) => await _countryOutput.GetAllAsync<List<CountryData>>();
    
    public async Task<CountryData> Get(StringWrapper id) => await _countryOutput.GetAsync<CountryData>(id.Value);
    
    public async Task<CountryData> Create(CountryData data) => await _countryOutput.CreateAsync<CountryData>(data);
    
    public async Task<CountryData> Update(CountryData data) => await _countryOutput.UpdateAsync<CountryData>(data);
    
    public async Task Disable(StringWrapper id) => await _countryOutput.DisableAsync<CountryData>(id.Value);

    public async Task Delete(StringWrapper id) => await _countryOutput.DeleteAsync<CountryData>(id.Value);
}