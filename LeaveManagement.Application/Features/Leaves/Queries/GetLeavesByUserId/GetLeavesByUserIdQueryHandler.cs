using LeaveManagement.Application.Mappers;
using LeaveManagement.Application.Responses;
using LeaveManagement.Domain.Contracts.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LeaveManagement.Application.Features.Leaves.Queries.GetLeavesByUserId;

public class GetLeavesByUserIdQueryHandler : IRequestHandler<GetLeavesByUserIdQuery, List<LeaveResponse>>
{
    private readonly string _className = nameof(GetLeavesByUserIdQueryHandler);
    private readonly ILeaveRepository _leaveRepository;
    private readonly ILogger<GetLeavesByUserIdQueryHandler> _logger;

    public GetLeavesByUserIdQueryHandler(
        ILeaveRepository leaveRepository,
        ILogger<GetLeavesByUserIdQueryHandler> logger)
    {
        _leaveRepository = leaveRepository;
        _logger = logger;
    }

    public async Task<List<LeaveResponse>> Handle(GetLeavesByUserIdQuery request, CancellationToken cancellationToken)
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