using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using LocationService.Message.Common;

namespace LocationService.Message.States;

[ServiceContract]
public interface IStateService
{
    [OperationContract] Task<List<StateData>> GetAll(StringWrapper countryId);
    [OperationContract] Task<StateData> Get(StringWrapper id);
    [OperationContract] Task<StateData> Create(StateData data);
    [OperationContract] Task<StateData> Update(StateData data);
    [OperationContract] Task Disable(StringWrapper id);
    [OperationContract] Task Delete(StringWrapper id);
}