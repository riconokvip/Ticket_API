namespace Ticket.API.Models.Users
{
    public class UserMapperProfile : Profile
    {
        public UserMapperProfile()
        {
            CreateMap<UserCreateRequestModel, UserCreateMapRequestModel>()
                .ForMember(dest => dest.WorkName, act => act.MapFrom(src => src.WorkName.Trim()))
                .ForMember(dest => dest.Telegram, act => act.MapFrom(src => src.Telegram.Trim()))
                .ForMember(dest => dest.Email, act => act.MapFrom(src => src.Email.Trim()))
                .ForMember(dest => dest.Level, act => act.MapFrom(src => src.Level.Trim()))
                .ForMember(dest => dest.Password, act => act.MapFrom(src => src.Password.Trim()))
                .ReverseMap();

            CreateMap<UserCreateMapRequestModel, UserEntities>().ReverseMap();

            CreateMap<UserPermission, UserPermissionEntities>()
                .ForMember(dest => dest.Claim, act => act.MapFrom(src => src.Permission.FastToString() + src.Resource.FastToString()))
                .ReverseMap();

            CreateMap<EsUsers, UserResponseModel>().ReverseMap();
        }
    }
}
