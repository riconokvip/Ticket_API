namespace Ticket.API.Models.Projects
{
    public class ProjectMapperProfile : Profile
    {
        public ProjectMapperProfile()
        {
            CreateMap<ProjectCreateRequestModel, ProjectCreateMapRequestModel>()
                .ForMember(dest => dest.ProjectName, act => act.MapFrom(src => src.ProjectName.Trim()))
                .ReverseMap();

            CreateMap<ProjectCreateMapRequestModel, ProjectEntities>().ReverseMap();

            CreateMap<ProjectUpdateRequestModel, ProjectUpdateMapRequestModel>()
                .ForMember(dest => dest.ProjectName, act => act.MapFrom(src => src.ProjectName.Trim()))
                .ReverseMap();

            CreateMap<ProjectUpdateMapRequestModel, ProjectEntities>().ReverseMap();
        }
    }
}
