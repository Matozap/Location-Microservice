using System.Runtime.Serialization;
using FluentValidation;
using LocationService.Message.Contracts.Common.Interfaces;

namespace LocationService.Message.Contracts.States.v1.Requests;

[DataContract]
public class GetStateById : IQuery<object>
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
