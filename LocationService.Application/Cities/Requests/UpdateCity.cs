using System.Runtime.Serialization;
using FluentValidation;
using LocationService.Application.Cities.Responses;
using MediatrBuilder.Interfaces;

namespace LocationService.Application.Cities.Requests;

[DataContract]
public class UpdateCity : ICommand<CityData>
{
    [DataMember(Order = 1)]
    public CityData Details { get; init; }
}

public class UpdateCityValidator : AbstractValidator<UpdateCity>
{
    public UpdateCityValidator()
    {
        RuleFor(x => x.Details).NotNull();
        RuleFor(x => x.Details.Id)
            .NotNull().NotEmpty().WithMessage("Id is required")
            .MaximumLength(36).WithMessage("Id cannot exceed 36 characters");
    }
}