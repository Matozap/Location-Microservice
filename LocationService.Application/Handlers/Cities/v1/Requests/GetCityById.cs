using FluentValidation;
using LocationService.Application.Interfaces;
using LocationService.Message.Contracts.Cities.v1;

namespace LocationService.Application.Handlers.Cities.v1.Requests;

public class GetCityById : IQuery<CityData>
{
    public string Id { get; init; }
}

public class GetCityByIdValidator : AbstractValidator<GetCityById>
{
    public GetCityByIdValidator()
    {
        RuleFor(x => x).NotNull();
        RuleFor(x => x.Id)
            .NotNull().NotEmpty().WithMessage("Id is required")
            .MaximumLength(36).WithMessage("Id cannot exceed 36 characters");
    }
}