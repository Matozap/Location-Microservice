using System.Runtime.Serialization;
using FluentValidation;
using LocationService.Message.Contracts.Common.Interfaces;

namespace LocationService.Message.Contracts.Countries.v1.Requests;

[DataContract]
public class UpdateCountry : ICommand<CountryData>
{
    [DataMember(Order = 1)]
    public CountryData Details { get; init; }
}

public class UpdateCountryValidator : AbstractValidator<UpdateCountry>
{
    public UpdateCountryValidator()
    {
        RuleFor(x => x.Details).NotNull();
        RuleFor(x => x.Details.Id)
            .NotNull().NotEmpty().WithMessage("Id is required")
            .MaximumLength(36).WithMessage("Id cannot exceed 36 characters");
    }
}