using LeaveManagement.Application.Mappers;
using LeaveManagement.Application.Responses;
using LeaveManagement.Domain.Contracts.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LeaveManagement.Application.Features.Leaves.Queries.GetLeavesList;

public class GetLeavesListQuery : IRequest<List<LeaveResponse>>
{
}

public class GetLeavesListQueryHandler : IRequestHandler<GetLeavesListQuery, List<LeaveResponse>>
{
    private readonly string _className = nameof(GetLeavesListQueryHandler);
    private readonly ILeaveRepository _leaveRepository;
    private readonly ILogger<GetLeavesListQueryHandler> _logger;

    public GetLeavesListQueryHandler(
        ILeaveRepository leaveRepository,
        ILogger<GetLeavesListQueryHandler> logger)
    {
        _leaveRepository = leaveRepository;
        _logger = logger;
    }

    public async Task<List<LeaveResponse>> Handle(GetLeavesListQuery request, CancellationToken cancellationToken)
    {
        var methodeName = nameof(Handle);
        try
        {
            _logger.LogInformation($"[{_className}][{methodeName}] Retriving leaves from the database");

            var leaves = await _leaveRepository.GetAllAsync();
            var leavesResponseList = leaves.
                Select(e => e.ToLeaveResponse())
                .ToList();

            _logger.LogInformation($"[{_className}][{methodeName}] Successfully retrieved leaves from the database");

            return leavesResponseList;
        }
        catch (Exception ex)
        {
            _logger.LogError($"[{_className}][{methodeName}] Exception: {ex}, Message: {ex.Message}");
            return [];
        }
    }
}