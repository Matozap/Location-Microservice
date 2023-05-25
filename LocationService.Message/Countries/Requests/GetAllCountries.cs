using System.Collections.Generic;
using System.Runtime.Serialization;
using FluentValidation;
using MediatrBuilder.Interfaces;

namespace LocationService.Message.Countries.Requests;

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
