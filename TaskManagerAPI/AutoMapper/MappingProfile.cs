using AutoMapper;
using TaskManagerAPI.Models;
using TaskManagerAPI.Models.Dtos;
using TaskManagerAPI.Models.Dtos.TaskItemDtos;
using TaskManagerAPI.Models.Dtos.TaskItemStatusDtos;

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

            //TaskItem
            CreateMap<TaskItem, TaskItemDto>();
            CreateMap<TaskItemDto, TaskItem>();
            CreateMap<CreateTaskItemDto, TaskItem>();
            CreateMap<UpdateTaskItemDto, TaskItem>()
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore());
        }
    }
}