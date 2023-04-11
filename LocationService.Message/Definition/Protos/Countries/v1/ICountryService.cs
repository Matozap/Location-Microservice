using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using LocationService.Message.Definition.Protos.Common;

namespace LocationService.Message.Definition.Protos.Countries.v1;

[ServiceContract]
public interface ICountryService
{
    [OperationContract] Task<List<CountryData>> GetAll(StringWrapper id);
    [OperationContract] Task<CountryData> Get(StringWrapper id);
    [OperationContract] Task<CountryData> Create(CountryData data);
    [OperationContract] Task<CountryData> Update(CountryData data);
    [OperationContract] Task Disable(StringWrapper id);
    [OperationContract] Task Delete(StringWrapper id);
}