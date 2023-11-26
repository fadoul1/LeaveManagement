using FluentValidation;
using LeaveManagement.Application.Mappers;
using LeaveManagement.Application.Responses;
using LeaveManagement.Domain.Contracts.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LeaveManagement.Application.Features.Leaves.Commands.CreateLeave;

public class CreateLeaveCommandHandler : IRequestHandler<CreateLeaveCommand, LeaveResponse>
{
    private readonly string _className = nameof(CreateLeaveCommandHandler);
    private readonly ILeaveRepository _leaveRepository;
    private readonly IValidator<CreateLeaveCommand> _createLeaveCommandValidator;
    private readonly ILogger<CreateLeaveCommandHandler> _logger;

    public CreateLeaveCommandHandler(
        ILeaveRepository leaveRepository, 
        IValidator<CreateLeaveCommand> createLeaveCommandValidator,
        ILogger<CreateLeaveCommandHandler> logger)
    {
        _leaveRepository = leaveRepository;
        _createLeaveCommandValidator = createLeaveCommandValidator;
        _logger = logger;
    }

    public async Task<LeaveResponse> Handle(CreateLeaveCommand request, CancellationToken cancellationToken)
    { 
        var methodeName = nameof(Handle);
        try
        {
            _logger.LogInformation($"[{_className}][{methodeName}] Validation of the entries");

            var result = await _createLeaveCommandValidator.ValidateAsync(request);
            if (!result.IsValid)
            {
                var resultErrors = result.Errors;
                string errors = string.Empty;

                foreach (var error in resultErrors)
                {
                    errors += $"Property {error.PropertyName} failed Validation. Error was: {error.ErrorMessage} \n";
                }
                _logger.LogInformation($"[{_className}][{methodeName}] Entries not valid, with errors: {errors}");

                return new LeaveResponse
                {
                    Success = false,
                    ValidationErrors = errors
                };
            }

            _logger.LogInformation($"[{_className}][{methodeName}] Successfully validated entries");

            _logger.LogInformation($"[{_className}][{methodeName}] Creation of the leave");
            var leave = request.ToLeave();
            var leaveCreated = await _leaveRepository.CreateAsync(leave);

            _logger.LogInformation($"[{_className}][{methodeName}] Successfully created the leave");
            
            return leaveCreated.ToLeaveResponse();
        }
        catch (Exception ex)
        {
            _logger.LogError($"[{_className}][{methodeName}] Exception: {ex}, Message: {ex.Message}");
            return new LeaveResponse
            {
                Success = false
            };
        }
    }
}
