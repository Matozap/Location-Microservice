using FluentValidation;
using LocationService.Application.Interfaces;

namespace LocationService.Application.Handlers.Countries.v1.Requests;

public class SoftDeleteCountry : ICommand<string>
{
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