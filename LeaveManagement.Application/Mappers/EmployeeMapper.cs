using AutoMapper;
using LeaveManagement.Application.Features.Employees.Commands.CreateEmployee;
using LeaveManagement.Application.Features.Employees.Commands.UpdateEmployee;
using LeaveManagement.Application.Responses;
using LeaveManagement.Domain.Entities;

namespace LeaveManagement.Application.Mappers;

public static class EmployeeMapper
{
    public static Employee ToEmployee(this CreateEmployeeCommand createEmployeeCommand)
    {
        var mapper = BaseMapper.CreateMapper<CreateEmployeeCommand,Employee>();
        return mapper.Map<Employee>(createEmployeeCommand);
    }

    public static Employee ToEmployee(this UpdateEmployeeCommand updateEmployeeCommand)
    {
        var configuration = new MapperConfiguration(config =>
            config.CreateMap<UpdateEmployeeCommand, Employee>()
            .ForMember(dest =>
                dest.Id,
                opt => opt.MapFrom(src => src.EmployeeId)
            )
        );

        var mapper = new Mapper(configuration);
        return mapper.Map<Employee>(updateEmployeeCommand);
    }

    public static EmployeeResponse ToEmployeeResponse(this Employee employee)
    {

        var configuration = new MapperConfiguration(config =>
            config.CreateMap<Employee, EmployeeResponse>()
            .ForMember(dest =>
                dest.EmployeeId,
                opt => opt.MapFrom(src => src.Id)
            )
        );

        var mapper = new Mapper(configuration);
        return mapper.Map<EmployeeResponse>(employee);
    }
}
