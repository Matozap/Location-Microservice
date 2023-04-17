using FluentValidation;
using LocationService.Application.Interfaces;

namespace LocationService.Application.Handlers.States.v1.Requests;

public class GetAllStates : IQuery<object>
{
    public string CountryId { get; init; }
}

public class GetAllStatesValidator : AbstractValidator<GetAllStates>
{
    public GetAllStatesValidator()
    {
        RuleFor(x => x).NotNull();
        RuleFor(x => x.CountryId)
            .NotNull().NotEmpty().WithMessage("Country id is required")
            .MaximumLength(36).WithMessage("Country id  cannot exceed 36 characters");
    }
}