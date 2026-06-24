using FluentValidation;
using LeaveManagement.Domain.Contracts.Services;

namespace LeaveManagement.Application.Features.Leaves.Commands.CreateLeave;

public class CreateLeaveValidator : AbstractValidator<CreateLeaveCommand>
{
    public CreateLeaveValidator(ITimeProvider timeProvider)
    {
        RuleFor(command => command.EmployeeId).NotNull().GreaterThan(0);

        RuleFor(command => command.StartDate)
            .NotNull()
            .GreaterThan(timeProvider.Today)
            .WithMessage("La date debut des congés doit être ultérieur à celle d'aujourd'hui");

        RuleFor(command => command.EndDate)
            .NotNull()
            .GreaterThan(timeProvider.Today)
            .WithMessage("La date fin des congés doit être ultérieur à celle d'aujourd'hui");
    }
}
