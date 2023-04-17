using FluentValidation;
using LocationService.Application.Interfaces;

namespace LocationService.Application.Handlers.States.v1.Requests;

public class DeleteState : ICommand<string>
{
    public string Id { get; init; }
}

public class DeleteStateValidator : AbstractValidator<DeleteState>
{
    public DeleteStateValidator()
    {
        RuleFor(x => x).NotNull();
        RuleFor(x => x.Id)
            .NotNull().NotEmpty().WithMessage("Id is required")
            .MaximumLength(36).WithMessage("Id cannot exceed 36 characters");
    }
}
