using System.Runtime.Serialization;
using FluentValidation;
using LocationService.Message.Contracts.Common.Interfaces;

namespace LocationService.Message.Contracts.States.v1.Requests;

[DataContract]
public class GetAllStates : IQuery<object>
{
    [DataMember(Order = 1)]
    public string CountryId { get; init; }
}

public class GetAllStatesValidator : AbstractValidator<GetAllStates>
{
    public GetAllStatesValidator()
    {
        RuleFor(x => x).NotNull();
        RuleFor(x => x.CountryId)
            .NotNull().NotEmpty().WithMessage("Country id is required")
            .MaximumLength(36).WithMessage("Country id  cannot exceed 36 characters");
    }
}