using FluentValidation;
using LocationService.Application.Interfaces;
using LocationService.Message.Contracts.States.v1;

namespace LocationService.Application.Handlers.States.v1.Requests;

public class CreateState : ICommand<StateData>
{
    public StateData Details { get; init; }
}

public class CreateStateValidator : AbstractValidator<CreateState>
{
    public CreateStateValidator()
    {
        RuleFor(x => x.Details).NotNull();
        RuleFor(x => x.Details.Name)
            .NotNull().NotEmpty().WithMessage("Name is required")
            .MaximumLength(200).WithMessage("Name cannot exceed 200 characters");
        RuleFor(x => x.Details.Code)
            .NotNull().NotEmpty().WithMessage("Code is required")
            .MaximumLength(36).WithMessage("Code cannot exceed 36 characters");
        RuleFor(x => x.Details.CountryId)
            .NotNull().NotEmpty().WithMessage("Country id is required")
            .MaximumLength(36).WithMessage("Country id cannot exceed 36 characters");
    }
}