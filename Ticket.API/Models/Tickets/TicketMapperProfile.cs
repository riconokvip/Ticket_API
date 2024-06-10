namespace Ticket.API.Models.Tickets
{
    public class TicketMapperProfile : Profile
    {
        public TicketMapperProfile()
        {
            CreateMap<TicketCreateRequestModel, TicketCreateMapRequestModel>()
                .ForMember(dest => dest.TicketContent, act => act.MapFrom(src => src.TicketContent.Trim()))
                .ReverseMap();
        }
    }
}
