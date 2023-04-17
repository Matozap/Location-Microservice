using FluentValidation;
using LocationService.Application.Interfaces;
using LocationService.Message.Contracts.Countries.v1;

namespace LocationService.Application.Handlers.Countries.v1.Requests;

public class GetCountryById : IQuery<CountryData>
{
    public string Id { get; init; }
}

public class GetCountryByIdValidator : AbstractValidator<GetCountryById>
{
    public GetCountryByIdValidator()
    {
        RuleFor(x => x).NotNull();
        RuleFor(x => x.Id)
            .NotNull().NotEmpty().WithMessage("Id is required")
            .MaximumLength(36).WithMessage("Id cannot exceed 36 characters");
    }
}
