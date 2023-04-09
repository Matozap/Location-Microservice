using System.Collections.Generic;
using LocationService.Message.DataTransfer.Countries.v1;
using MediatR;

namespace LocationService.Message.Definition.Countries.Requests.v1;

public class GetAllCountries : BaseMessage, IRequest<List<CountryData>>
{
    
}
