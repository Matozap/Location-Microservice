using System.Collections.Generic;
using System.Runtime.Serialization;
using FluentValidation;
using LocationService.Application.Countries.Responses;
using MediatrBuilder.Interfaces;

namespace LocationService.Application.Countries.Requests;

[DataContract]
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
