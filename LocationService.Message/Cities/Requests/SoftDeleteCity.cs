using System.Runtime.Serialization;
using FluentValidation;
using MediatrBuilder.Interfaces;

namespace LocationService.Message.Cities.Requests;

[DataContract]
public class SoftDeleteCity : ICommand<CityData>
{
    [DataMember(Order = 1)]
    public string Id { get; init; }
}

public class SoftDeleteCityValidator : AbstractValidator<SoftDeleteCity>
{
    public SoftDeleteCityValidator()
    {
        RuleFor(x => x).NotNull();
        RuleFor(x => x.Id)
            .NotNull().NotEmpty().WithMessage("Id is required")
            .MaximumLength(36).WithMessage("Id cannot exceed 36 characters");
    }
}