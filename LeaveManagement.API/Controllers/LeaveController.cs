using LeaveManagement.Application.Features.Employees.Commands.CreateEmployee;
using LeaveManagement.Application.Features.Leaves.Commands.CreateLeave;
using LeaveManagement.Application.Features.Leaves.Queries.GetLeavesByUserId;
using LeaveManagement.Application.Features.Leaves.Queries.GetLeavesList;
using LeaveManagement.Application.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LeaveManagement.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LeaveController : ControllerBase
{
    private readonly IMediator _mediator;

    public LeaveController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet(Name = "GetAllLeaves")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<List<LeaveResponse>>> GetAllLeaves()
    {
        var leaves = await _mediator.Send(new GetLeavesListQuery());
        return Ok(leaves);
    }

    [HttpPost(Name = "AddLeave")]
    public async Task<ActionResult<LeaveResponse>> Create([FromBody] CreateLeaveCommand createLeaveCommand)
    {
        var leaveResponse = await _mediator.Send(createLeaveCommand);
        return Ok(leaveResponse);
    }

    [HttpGet("{employeeId}", Name = "GetUserLeaves")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<List<LeaveResponse>>> GetUserLeaves(long employeeId)
    {
        var leaves = await _mediator.Send(new GetLeavesByUserIdQuery {
            EmployeeId = employeeId
        });
        return Ok(leaves);
    }
}
