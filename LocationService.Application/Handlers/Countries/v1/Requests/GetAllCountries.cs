using System.Collections.Generic;
using LocationService.Application.Interfaces;
using LocationService.Message.Contracts.Countries.v1;

namespace LocationService.Application.Handlers.Countries.v1.Requests;

public class GetAllCountries : IQuery<List<CountryData>>
{
    
}
