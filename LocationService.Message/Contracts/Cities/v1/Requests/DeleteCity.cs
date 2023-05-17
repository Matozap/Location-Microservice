using System.Runtime.Serialization;
using FluentValidation;
using LocationService.Message.Contracts.Common.Interfaces;

namespace LocationService.Message.Contracts.Cities.v1.Requests;

[DataContract]
public class DeleteCity : ICommand<string>
{
    [DataMember(Order = 1)]
    public string Id { get; init; }
}

public class DeleteCityValidator : AbstractValidator<DeleteCity>
{
    public DeleteCityValidator()
    {
        RuleFor(x => x).NotNull();
        RuleFor(x => x.Id)
            .NotNull().NotEmpty().WithMessage("Id is required")
            .MaximumLength(36).WithMessage("Id cannot exceed 36 characters");
    }
}
