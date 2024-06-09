namespace Ticket.API.Models.ProjectMembers
{
    public class ProjectMemberMapperProfile : Profile
    {
        public ProjectMemberMapperProfile()
        {
            CreateMap<ProjectAddMemberRequestModel, ProjectMemberEntities>().ReverseMap();
        }
    }
}
