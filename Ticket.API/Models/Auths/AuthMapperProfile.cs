namespace Ticket.API.Models.Auths
{
    public class AuthMapperProfile : Profile
    {
        public AuthMapperProfile()
        {
            CreateMap<LoginRequestModel, LoginMapRequestModel>()
                .ForMember(dest => dest.Email, act => act.MapFrom(src => src.Email.Trim()))
                .ForMember(dest => dest.Password, act => act.MapFrom(src => src.Password.Trim()));

            CreateMap<UserEntities, InformationResponse>().ReverseMap();

            CreateMap<JwtResponse, UserTokenEntities>()
                .ForMember(dest => dest.AccessToken, act => act.MapFrom(src => src.Token.Hash()))
                .ForMember(dest => dest.AccessTokenExpiresAt, act => act.MapFrom(src => src.AccessTokenExpriesAt))
                .ForMember(dest => dest.RefreshToken, act => act.MapFrom(src => src.RefreshToken.Hash()))
                .ForMember(dest => dest.RefreshTokenExpiresAt, act => act.MapFrom(src => src.Expiration));
        }
    }
}
