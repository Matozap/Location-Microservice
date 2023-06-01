using System.Runtime.Serialization;
using FluentValidation;
using LocationService.Application.Cities.Responses;
using MediatrBuilder.Interfaces;

namespace LocationService.Application.Cities.Requests;

[DataContract]
public class GetCityById : IQuery<CityData>
{
    [DataMember(Order = 1)]
    public string Id { get; init; }
}

public class GetCityByIdValidator : AbstractValidator<GetCityById>
{
    public GetCityByIdValidator()
    {
        RuleFor(x => x).NotNull();
        RuleFor(x => x.Id)
            .NotNull().NotEmpty().WithMessage("Id is required")
            .MaximumLength(36).WithMessage("Id cannot exceed 36 characters");
    }
}