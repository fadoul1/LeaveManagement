using LeaveManagement.Application.Mappers;
using LeaveManagement.Application.Responses;
using LeaveManagement.Domain.Contracts.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LeaveManagement.Application.Features.Leaves.Queries.GetLeavesByEmployeeId;

public class GetLeavesByEmployeeIdQueryHandler : IRequestHandler<GetLeavesByEmployeeIdQuery, List<LeaveResponse>>
{
    private readonly string _className = nameof(GetLeavesByEmployeeIdQueryHandler);
    private readonly ILeaveRepository _leaveRepository;
    private readonly ILogger<GetLeavesByEmployeeIdQueryHandler> _logger;

    public GetLeavesByEmployeeIdQueryHandler(
        ILeaveRepository leaveRepository,
        ILogger<GetLeavesByEmployeeIdQueryHandler> logger)
    {
        _leaveRepository = leaveRepository;
        _logger = logger;
    }

    public async Task<List<LeaveResponse>> Handle(GetLeavesByEmployeeIdQuery request, CancellationToken cancellationToken)
    {
        var methodeName = nameof(Handle);
        try
        {
            _logger.LogInformation($"[{_className}][{methodeName}] Retriving user with id: {request.EmployeeId} leaves from the database");

            var leaves = await _leaveRepository.GetEmployeeLeaveAsync(request.EmployeeId);
            var leavesResponseList = leaves.
                Select(e => e.ToLeaveResponse())
                .ToList();

            _logger.LogInformation($"[{_className}][{methodeName}] Successfully retrieved user with id: {request.EmployeeId} leaves from the database");

            return leavesResponseList;
        }
        catch (Exception ex)
        {
            _logger.LogError($"[{_className}][{methodeName}] Exception: {ex}, Message: {ex.Message}");
            return [];
        }
    }
}