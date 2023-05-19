using System.Runtime.Serialization;
using FluentValidation;
using MediatrBuilder.Interfaces;

namespace LocationService.Message.Contracts.States.v1.Requests;

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