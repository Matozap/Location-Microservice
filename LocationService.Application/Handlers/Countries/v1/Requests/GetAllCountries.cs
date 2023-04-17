using System.Collections.Generic;
using FluentValidation;
using LocationService.Application.Interfaces;
using LocationService.Message.Contracts.Countries.v1;

namespace LocationService.Application.Handlers.Countries.v1.Requests;

public class GetAllCountries : IQuery<List<CountryData>>
{
    
}

public class GetAllCitiesValidator : AbstractValidator<GetAllCountries>
{
    public GetAllCitiesValidator()
    {
        RuleFor(x => x).NotNull();
    }
}
