using FluentValidation;
using LocationService.Application.Interfaces;
using LocationService.Message.Contracts.States.v1;

namespace LocationService.Application.Handlers.States.v1.Requests;

public class UpdateState : ICommand<StateData>
{
    public StateData Details { get; init; }
}

public class UpdateStateValidator : AbstractValidator<UpdateState>
{
    public UpdateStateValidator()
    {
        RuleFor(x => x.Details).NotNull();
        RuleFor(x => x.Details.Id)
            .NotNull().NotEmpty().WithMessage("Id is required")
            .MaximumLength(36).WithMessage("Id cannot exceed 36 characters");
    }
}