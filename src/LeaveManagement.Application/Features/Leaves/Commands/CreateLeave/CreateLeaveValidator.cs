using FluentValidation;

namespace LeaveManagement.Application.Features.Leaves.Commands.CreateLeave;

public class CreateLeaveValidator : AbstractValidator<CreateLeaveCommand>
{
    public CreateLeaveValidator()
    {
        RuleFor(command => command.EmployeeId)
            .NotNull()
            .GreaterThan(0);

        RuleFor(command => command.StartDate)
            .NotNull()
            .GreaterThan(DateTime.Today)
            .WithMessage("La date debut des congés doit être ultérieur à celle d'aujourd'hui");

        RuleFor(command => command.EndDate)
            .NotNull()
            .GreaterThan(DateTime.Today)
            .WithMessage("La date fin des congés doit être ultérieur à celle d'aujourd'hui");
    }
}