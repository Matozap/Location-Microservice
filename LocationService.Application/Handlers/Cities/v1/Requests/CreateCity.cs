using FluentValidation;
using LocationService.Application.Interfaces;
using LocationService.Message.Contracts.Cities.v1;

namespace LocationService.Application.Handlers.Cities.v1.Requests;

public class CreateCity : ICommand<CityData>
{
    public CityData Details { get; init; }
}

public class CreateCityValidator : AbstractValidator<CreateCity>
{
    public CreateCityValidator()
    {
        RuleFor(x => x.Details).NotNull();
        RuleFor(x => x.Details.Name)
            .NotNull().NotEmpty().WithMessage("Name is required")
            .MaximumLength(200).WithMessage("Name cannot exceed 200 characters");
        RuleFor(x => x.Details.StateId)
            .NotNull().NotEmpty().WithMessage("State id is required")
            .MaximumLength(36).WithMessage("Id cannot exceed 36 characters");
    }
}