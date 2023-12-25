using LeaveManagement.Application.Features.Employees.Commands.CreateEmployee;
using LeaveManagement.Application.Features.Employees.Commands.DeleteEmployee;
using LeaveManagement.Application.Features.Employees.Commands.UpdateEmployee;
using LeaveManagement.Application.Features.Employees.Queries.GetEmployeeById;
using LeaveManagement.Application.Features.Employees.Queries.GetEmployeesList;
using LeaveManagement.Application.Features.Leaves.Queries.GetLeavesByEmployeeId;
using LeaveManagement.Application.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LeaveManagement.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EmployeeController : ControllerBase
{
    private readonly IMediator _mediator;

    public EmployeeController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet(Name = "GetAllEmployees")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<List<EmployeeResponse>>> GetAllEmployees()
    {
        var employees = await _mediator.Send(new GetEmployeesListQuery());
        return Ok(employees);
    }

    [HttpGet("{employeeId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<EmployeeResponse>> GetEmployeeById(long employeeId)
    {
        var employeeResponse = await _mediator.Send(new GetEmployeeByIdQuery
        {
            EmployeeId = employeeId
        });

        return Ok(employeeResponse);
    }

    [HttpPost(Name = "AddEmployee")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<EmployeeResponse>> Create([FromBody] CreateEmployeeCommand createEmployeeCommand)
    {
        var employeeResponse = await _mediator.Send(createEmployeeCommand);
        return Created(string.Empty, employeeResponse);
    }
   
    [HttpPut(Name = "UpdateEmployee")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<EmployeeResponse>> Update([FromBody] UpdateEmployeeCommand updateEmployeeCommand)
    {
        var updatedEmployee = await _mediator.Send(updateEmployeeCommand);
        return Ok(updatedEmployee);
    }

    [HttpDelete("{employeeId}", Name = "DeleteEmployee")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult> Delete(long employeeId)
    {
        await _mediator.Send(new DeleteEmployeeCommand {
            EmployeeId = employeeId
        });
        return Ok();
    }
}
