using System.Collections.Generic;
using FluentValidation;
using LocationService.Application.Interfaces;
using LocationService.Message.Contracts.Cities.v1;

namespace LocationService.Application.Handlers.Cities.v1.Requests;

public class GetAllCities : IQuery<List<CityData>>
{
    public string StateId { get; init; }
}

public class GetAllCitiesValidator : AbstractValidator<GetAllCities>
{
    public GetAllCitiesValidator()
    {
        RuleFor(x => x).NotNull();
        RuleFor(x => x.StateId)
            .NotNull().NotEmpty().WithMessage("State id is required")
            .MaximumLength(36).WithMessage("State id cannot exceed 36 characters");
    }
}