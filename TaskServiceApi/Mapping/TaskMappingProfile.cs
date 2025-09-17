using AutoMapper;
using C_Part1;
using TaskServiceApi.DTOs;

namespace TaskServiceApi.Mapping
{
    public sealed class TaskMappingProfile : Profile
    {
        public TaskMappingProfile()
        {
            CreateMap<TaskItem, TaskReadDto>();

            CreateMap<TaskCreateDto, TaskItem>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.IsCompleted, o => o.MapFrom(_ => false));

            CreateMap<TaskUpdateDto, TaskItem>()
                .ForMember(d => d.Id, o => o.Ignore()); // Id берём из маршрута
        }
    }
}
