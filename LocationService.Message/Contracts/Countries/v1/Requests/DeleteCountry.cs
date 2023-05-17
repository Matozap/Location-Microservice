using System.Runtime.Serialization;
using FluentValidation;
using LocationService.Message.Contracts.Common.Interfaces;

namespace LocationService.Message.Contracts.Countries.v1.Requests;

[DataContract]
public class DeleteCountry : ICommand<string>
{
    [DataMember(Order = 1)]
    public string Id { get; init; }
}

public class DeleteCountryValidator : AbstractValidator<DeleteCountry>
{
    public DeleteCountryValidator()
    {
        RuleFor(x => x).NotNull();
        RuleFor(x => x.Id)
            .NotNull().NotEmpty().WithMessage("Id is required")
            .MaximumLength(36).WithMessage("Id cannot exceed 36 characters");
    }
}