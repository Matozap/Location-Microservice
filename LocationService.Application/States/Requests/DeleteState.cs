using System.Runtime.Serialization;
using FluentValidation;
using LocationService.Application.States.Responses;
using MediatrBuilder.Interfaces;

namespace LocationService.Application.States.Requests;

[DataContract]
public class DeleteState : ICommand<StateData>
{
    [DataMember(Order = 1)]
    public string Id { get; init; }
}

public class DeleteStateValidator : AbstractValidator<DeleteState>
{
    public DeleteStateValidator()
    {
        RuleFor(x => x).NotNull();
        RuleFor(x => x.Id)
            .NotNull().NotEmpty().WithMessage("Id is required")
            .MaximumLength(36).WithMessage("Id cannot exceed 36 characters");
    }
}
