namespace Ticket.API.Models.WorkSpaces
{
    public class WorkSpaceMapperProfile : Profile
    {
        public WorkSpaceMapperProfile()
        {
            CreateMap<WorkSpaceCreateRequestModel, WorkSpaceCreateMapRequestModel>()
                .ForMember(dest => dest.WorkSpaceName, act => act.MapFrom(src => src.WorkSpaceName.Trim()))
                .ForMember(dest => dest.WorkSpaceColor, act => act.MapFrom(src => src.WorkSpaceColor.Trim()));

            CreateMap<WorkSpaceCreateMapRequestModel, WorkSpaceEntities>().ReverseMap();

            CreateMap<WorkSpaceUpdateRequestModel, WorkSpaceUpdateMapRequestModel>()
                .ForMember(dest => dest.WorkSpaceName, act => act.MapFrom(src => src.WorkSpaceName.Trim()))
                .ForMember(dest => dest.WorkSpaceColor, act => act.MapFrom(src => src.WorkSpaceColor.Trim()));

            CreateMap<WorkSpaceUpdateMapRequestModel, WorkSpaceEntities>().ReverseMap();
        }
    }
}
