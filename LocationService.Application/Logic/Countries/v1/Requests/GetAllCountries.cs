using System.Collections.Generic;
using LocationService.Message.Definition.Protos.Countries.v1;
using MediatR;

namespace LocationService.Application.Logic.Countries.v1.Requests;

public class GetAllCountries : IRequest<List<CountryData>>
{
    
}
