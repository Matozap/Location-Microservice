using System.Runtime.Serialization;
using FluentValidation;
using MediatrBuilder.Interfaces;

namespace LocationService.Message.Contracts.States.v1.Requests;

[DataContract]
public class CreateState : ICommand<StateData>
{
    [DataMember(Order = 1)]
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