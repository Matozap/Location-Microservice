using FluentValidation;
using LocationService.Application.Interfaces;

namespace LocationService.Application.Handlers.Countries.v1.Requests;

public class DeleteCountry : ICommand<string>
{
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