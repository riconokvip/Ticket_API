namespace Ticket.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : BaseController
    {
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;

        public AuthController(IAuthService authService, IMapper mapper)
        {
            _authService = authService;
            _mapper = mapper;
        }

        /// <summary>
        /// Đăng nhập, cấp accessToken và refreshToken
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<BaseResponse<LoginResponseModel>> Login([FromBody] LoginRequestModel model)
        {
            var loginModel = _mapper.Map<LoginMapRequestModel>(model);
            var res = await _authService.Login(loginModel.Email, loginModel.Password);
            return Success(res);
        }
    }
}
