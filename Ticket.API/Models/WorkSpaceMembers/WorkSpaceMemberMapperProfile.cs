namespace Ticket.API.Models.WorkSpaceMembers
{
    public class WorkSpaceMemberMapperProfile : Profile
    {
        public WorkSpaceMemberMapperProfile()
        {
            CreateMap<WorkSpaceAddMemberRequestModel, WorkSpaceMemberEntities>().ReverseMap();
        }
    }
}
