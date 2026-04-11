using AutoMapper;
using TaskManagerAPI.Models;
using TaskManagerAPI.Models.Dtos;

namespace TaskManagerAPI.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //TaskItemStatus
            CreateMap<TaskItemStatus, TaskItemStatusDto>();
            CreateMap<TaskItemStatusDto, TaskItemStatus>();
            CreateMap<CreateTaskItemStatusDto, TaskItemStatus>();
        }
    }
}
