using System.Runtime.Serialization;
using FluentValidation;
using MediatrBuilder.Interfaces;

namespace LocationService.Message.States.Requests;

[DataContract]
public class GetStateById : IQuery<StateData>
{
    [DataMember(Order = 1)]
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
