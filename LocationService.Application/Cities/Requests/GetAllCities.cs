using System.Collections.Generic;
using System.Runtime.Serialization;
using FluentValidation;
using LocationService.Application.Cities.Responses;
using MediatrBuilder.Interfaces;

namespace LocationService.Application.Cities.Requests;

[DataContract]
public class GetAllCities : IQuery<List<CityData>>
{
    [DataMember(Order = 1)]
    public string StateId { get; init; }
}

public class GetAllCitiesValidator : AbstractValidator<GetAllCities>
{
    public GetAllCitiesValidator()
    {
        RuleFor(x => x).NotNull();
        RuleFor(x => x.StateId)
            .NotNull().NotEmpty().WithMessage("State id is required")
            .MaximumLength(36).WithMessage("State id cannot exceed 36 characters");
    }
}