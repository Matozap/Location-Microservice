using FluentValidation;
using LocationService.Application.Interfaces;

namespace LocationService.Application.Handlers.Cities.v1.Requests;

public class SoftDeleteCity : ICommand<string>
{
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