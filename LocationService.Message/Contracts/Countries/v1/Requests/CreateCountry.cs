using System.Runtime.Serialization;
using FluentValidation;
using LocationService.Message.Contracts.Common.Interfaces;

namespace LocationService.Message.Contracts.Countries.v1.Requests;

[DataContract]
public class CreateCountry : ICommand<CountryData>
{
    [DataMember(Order = 1)]
    public CountryData Details { get; init; }
}

public class CreateCountryValidator : AbstractValidator<CreateCountry>
{
    public CreateCountryValidator()
    {
        RuleFor(x => x.Details).NotNull();
        RuleFor(x => x.Details.Name)
            .NotNull().NotEmpty().WithMessage("Name is required")
            .MaximumLength(200).WithMessage("Name cannot exceed 200 characters");
        RuleFor(x => x.Details.Code)
            .NotNull().NotEmpty().WithMessage("Code is required")
            .MaximumLength(36).WithMessage("Code cannot exceed 36 characters");
    }
}
