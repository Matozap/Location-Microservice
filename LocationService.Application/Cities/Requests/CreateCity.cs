using System.Runtime.Serialization;
using FluentValidation;
using LocationService.Application.Cities.Responses;
using MediatrBuilder.Interfaces;

namespace LocationService.Application.Cities.Requests;

[DataContract]
public class CreateCity : ICommand<CityData>
{
    [DataMember(Order = 1)]
    public CityData Details { get; init; }
}

public class CreateCityValidator : AbstractValidator<CreateCity>
{
    public CreateCityValidator()
    {
        RuleFor(x => x.Details).NotNull();
        RuleFor(x => x.Details.Name)
            .NotNull().NotEmpty().WithMessage("Name is required")
            .MaximumLength(200).WithMessage("Name cannot exceed 200 characters");
        RuleFor(x => x.Details.StateId)
            .NotNull().NotEmpty().WithMessage("State id is required")
            .MaximumLength(36).WithMessage("Id cannot exceed 36 characters");
    }
}