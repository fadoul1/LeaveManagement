using AutoMapper;
using LeaveManagement.Application.Features.Leaves.Commands.CreateLeave;
using LeaveManagement.Application.Responses;
using LeaveManagement.Domain.Entities;
using LeaveManagement.Domain.Helpers;

namespace LeaveManagement.Application.Mappers;

public static class LeaveMapper
{
    public static Leave ToLeave(this CreateLeaveCommand createLeaveCommand)
    {
        var configuration = new MapperConfiguration(config =>
            config.CreateMap<CreateLeaveCommand, Leave>()
            .ForMember(dest => 
                dest.Employee, 
                opt => opt.Ignore()
            )
        );

        var mapper = new Mapper(configuration);
        return mapper.Map<Leave>(createLeaveCommand);
    }

    public static LeaveResponse ToLeaveResponse(this Leave leave)
    {
        var configuration = new MapperConfiguration(config =>
            config.CreateMap<Leave, LeaveResponse>()
            .ForMember(dest => 
                dest.LeaveId,
                opt => opt.MapFrom(src => src.Id))
            .ForMember(dest =>
                dest.StartDate,
                opt => opt.MapFrom(src => src.StartDate.ToString("dd/MM/yyyy")))
            .ForMember(dest =>
                dest.EndDate,
                opt => opt.MapFrom(src => src.EndDate.ToString("dd/MM/yyyy")))
            .ForMember(dest =>
                dest.Status,
                opt => opt.MapFrom(src => EnumHelper.GetDescription(src.Status)))
            .ForMember(dest =>
                dest.Type,
                opt => opt.MapFrom(src => EnumHelper.GetDescription(src.Type)))           
            .ForMember(dest =>
                dest.EmployeeName,
                opt => opt.MapFrom(src =>
                    src.Employee != null
                        ? $"{src.Employee.FirstName} {src.Employee.LastName}"
                        : string.Empty))
        );

        var mapper = new Mapper(configuration);
        return mapper.Map<LeaveResponse>(leave);
    }
}
