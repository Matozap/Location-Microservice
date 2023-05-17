using System.Collections.Generic;
using System.Runtime.Serialization;
using FluentValidation;
using LocationService.Message.Contracts.Common.Interfaces;

namespace LocationService.Message.Contracts.Countries.v1.Requests;

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
