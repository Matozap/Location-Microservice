using FluentValidation;
using LocationService.Application.Interfaces;

namespace LocationService.Application.Handlers.States.v1.Requests;

public class GetStateById : IQuery<object>
{
    public string Id { get; init; }
}

public class GetStateByIdValidator : AbstractValidator<GetStateById>
{
    public GetStateByIdValidator()
    {
        RuleFor(x => x).NotNull();
        RuleFor(x => x.Id)
            .NotNull().NotEmpty().WithMessage("Id is required")
            .MaximumLength(36).WithMessage("Id cannot exceed 36 characters");
    }
}
