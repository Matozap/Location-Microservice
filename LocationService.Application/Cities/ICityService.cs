using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using LocationService.Application.Cities.Responses;
using LocationService.Application.Common;

namespace LocationService.Application.Cities;

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