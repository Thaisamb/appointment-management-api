using AppointmentManagement.Application.Constants;
using AppointmentManagement.Application.DTOs.Session;
using FluentValidation;

namespace AppointmentManagement.Application.Validators.Session;

public class UpdateSessionValidator : AbstractValidator<UpdateSessionDto>
{
    public UpdateSessionValidator()
    {
        RuleFor(x => x.DateTime).GreaterThanOrEqualTo(new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1))
                            .WithMessage(ValidationMessages.PastMonthTime);
        RuleFor(x => x.Duration).InclusiveBetween(10, 240).WithMessage(ValidationMessages.RequiredField);
        RuleFor(x => x.PricePerSession).GreaterThan(0).WithMessage(ValidationMessages.InvalidPrice);
    }
}