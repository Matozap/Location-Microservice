using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using LocationService.Message.Contracts.Common;

namespace LocationService.Message.Contracts.Cities.v1;

[ServiceContract]
public interface ICityService
{
    [OperationContract] Task<List<CityData>> GetAll(StringWrapper stateId);
    [OperationContract] Task<CityData> Get(StringWrapper id);
    [OperationContract] Task<CityData> Create(CityData data);
    [OperationContract] Task<CityData> Update(CityData data);
    [OperationContract] Task Disable(StringWrapper id);
    [OperationContract] Task Delete(StringWrapper id);
}