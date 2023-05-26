using System.Runtime.Serialization;
using FluentValidation;
using MediatrBuilder.Interfaces;

namespace LocationService.Message.Countries.Requests;

[DataContract]
public class DeleteCountry : ICommand<CountryData>
{
    [DataMember(Order = 1)]
    public string Id { get; init; }
}

public class DeleteCountryValidator : AbstractValidator<DeleteCountry>
{
    public DeleteCountryValidator()
    {
        RuleFor(x => x).NotNull();
        RuleFor(x => x.Id)
            .NotNull().NotEmpty().WithMessage("Id is required")
            .MaximumLength(36).WithMessage("Id cannot exceed 36 characters");
    }
}