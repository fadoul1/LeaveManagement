using FluentValidation;

namespace LeaveManagement.Application.Features.Employees.Commands.CreateEmployee;

public class CreateEmployeeValidator : AbstractValidator<CreateEmployeeCommand>
{
    public CreateEmployeeValidator()
    {
        RuleFor(command => command.Email).NotEmpty().EmailAddress();
        RuleFor(command => command.FirstName).NotEmpty().MinimumLength(2);
        RuleFor(command => command.LastName).NotEmpty().MinimumLength(2);
        RuleFor(command => command.PhoneNumber).NotEmpty().MinimumLength(8);
    }
}

