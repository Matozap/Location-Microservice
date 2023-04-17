using FluentValidation;
using LocationService.Application.Interfaces;

namespace LocationService.Application.Handlers.States.v1.Requests;

public class SoftDeleteState : ICommand<string>
{
    public string Id { get; init; }
}

public class SoftDeleteStateValidator : AbstractValidator<SoftDeleteState>
{
    public SoftDeleteStateValidator()
    {
        RuleFor(x => x).NotNull();
        RuleFor(x => x.Id)
            .NotNull().NotEmpty().WithMessage("Id is required")
            .MaximumLength(36).WithMessage("Id cannot exceed 36 characters");
    }
}