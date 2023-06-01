using System.Runtime.Serialization;
using FluentValidation;
using LocationService.Application.Countries.Responses;
using MediatrBuilder.Interfaces;

namespace LocationService.Application.Countries.Requests;

[DataContract]
public class SoftDeleteCountry : ICommand<CountryData>
{
    [DataMember(Order = 1)]
    public string Id { get; init; }
}

public class SoftDeleteCountryValidator : AbstractValidator<SoftDeleteCountry>
{
    public SoftDeleteCountryValidator()
    {
        RuleFor(x => x).NotNull();
        RuleFor(x => x.Id)
            .NotNull().NotEmpty().WithMessage("Id is required")
            .MaximumLength(36).WithMessage("Id cannot exceed 36 characters");
    }
}