using FluentValidation;
using LocationService.Application.Interfaces;
using LocationService.Message.Contracts.Countries.v1;

namespace LocationService.Application.Handlers.Countries.v1.Requests;

public class UpdateCountry : ICommand<CountryData>
{
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