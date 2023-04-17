using FluentValidation;
using LocationService.Application.Interfaces;

namespace LocationService.Application.Handlers.Cities.v1.Requests;

public class DeleteCity : ICommand<string>
{
    public string Id { get; init; }
}

public class DeleteCityValidator : AbstractValidator<DeleteCity>
{
    public DeleteCityValidator()
    {
        RuleFor(x => x).NotNull();
        RuleFor(x => x.Id)
            .NotNull().NotEmpty().WithMessage("Id is required")
            .MaximumLength(36).WithMessage("Id cannot exceed 36 characters");
    }
}
