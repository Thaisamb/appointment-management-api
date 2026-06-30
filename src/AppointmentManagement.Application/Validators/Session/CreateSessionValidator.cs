using AppointmentManagement.Application.Constants;
using AppointmentManagement.Application.DTOs.Session;
using AppointmentManagement.Domain.Enums;
using FluentValidation;

namespace AppointmentManagement.Application.Validators.Session;

public class CreateSessionValidator : AbstractValidator<CreateSessionDto>
{
    public CreateSessionValidator()
    {
        RuleFor(x => x.ClientId).GreaterThan(0).WithMessage(ValidationMessages.RequiredField);
        RuleFor(x => x.DateTime).GreaterThanOrEqualTo(new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1))
                                .WithMessage(ValidationMessages.PastMonthTime);
        RuleFor(x => x.Duration).InclusiveBetween(10, 240).WithMessage(ValidationMessages.RequiredField);
        When(x => x.Repetition != null && x.Repetition.Type != "none", () =>
        {
            RuleFor(x => x.Repetition!.EndDate)
                .NotNull().WithMessage(ValidationMessages.RequiredField)
                .GreaterThan(x => x.DateTime).WithMessage(ValidationMessages.RepetitionEndDateInvalid);

            When(x => x.Repetition!.Type == "weekly", () =>
                RuleFor(x => x.Repetition!.WeekDays)
                    .NotEmpty().WithMessage(ValidationMessages.WeekDaysRequired));

            When(x => x.Repetition!.Type == "biweekly", () =>
                RuleFor(x => x.Repetition!.WeekDays)
                    .NotEmpty()
                    .Must(d => d != null && d.Count == 1)
                    .WithMessage(ValidationMessages.WeekDaysRequired));

            When(x => x.Repetition!.Type == "monthly", () =>
            {
                RuleFor(x => x.Repetition!.WeekOfMonth)
                    .NotNull().WithMessage(ValidationMessages.RequiredField)
                    .InclusiveBetween(1, 5);
                RuleFor(x => x.Repetition!.DayOfWeek)
                    .NotNull().WithMessage(ValidationMessages.WeekDaysRequired);
            });
        });
    }
}