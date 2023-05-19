using System.Runtime.Serialization;
using FluentValidation;
using MediatrBuilder.Interfaces;

namespace LocationService.Message.Contracts.Countries.v1.Requests;

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