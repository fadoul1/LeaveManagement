using AutoMapper;

namespace LeaveManagement.Application.Mappers;

public static class BaseMapper
{
    public static Mapper CreateMapper<T, K>()
    {
        var configuration = new MapperConfiguration(config =>
            config.CreateMap<T, K>()
        );

        return new Mapper(configuration);
    }
}
