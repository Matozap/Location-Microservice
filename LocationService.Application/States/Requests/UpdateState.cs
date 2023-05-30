using System.Runtime.Serialization;
using FluentValidation;
using LocationService.Application.States.Responses;
using MediatrBuilder.Interfaces;

namespace LocationService.Application.States.Requests;

[DataContract]
public class UpdateState : ICommand<StateData>
{
    [DataMember(Order = 1)]
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