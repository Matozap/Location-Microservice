using System.Collections.Generic;
using LocationService.Message.Contracts.Countries.v1;
using MediatR;

namespace LocationService.Application.Logic.Countries.v1.Requests;

public class GetAllCountries : IRequest<List<CountryData>>
{
    
}
