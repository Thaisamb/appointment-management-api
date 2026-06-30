using AppointmentManagement.Application.Constants;
using AppointmentManagement.Application.DTOs.Session;
using FluentValidation;

namespace AppointmentManagement.Application.Validators.Session;

public class UpdateSessionGroupValidator : AbstractValidator<UpdateSessionGroupDto>
{
    public UpdateSessionGroupValidator()
    {
        RuleFor(x => x.GroupId).NotEmpty().WithMessage(ValidationMessages.RequiredField);
        RuleFor(x => x.NewDateTime).GreaterThanOrEqualTo(new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1))
                            .WithMessage(ValidationMessages.PastMonthTime);
        RuleFor(x => x.Duration).InclusiveBetween(10, 240).WithMessage(ValidationMessages.RequiredField);
        RuleFor(x => x.PricePerSession).GreaterThan(0).WithMessage(ValidationMessages.InvalidPrice);
    }
}