namespace Ticket.API.Controllers
{
    [Authorize(Roles = ApplicationRoles.Admin)]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UsersController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        /// <summary>
        /// Lấy danh sách nguời dùng
        /// </summary>
        /// <param name="model">Bộ lọc</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<BaseResponseWithPagination<List<UserResponseModel>>> GetAllUser([FromQuery] UserRequestModel model)
        {
            var res = await _userService.GetUsers(model);
            return SuccessWithPagination(res.Pagination, res.Users);
        }

        /// <summary>
        /// Thêm mới người dùng
        /// </summary>
        /// <param name="model">Dữ liệu thêm mới</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<BaseResponse> CreateNewUser([FromBody] UserCreateRequestModel model)
        {
            await _userService.CreateUser(_mapper.Map<UserCreateMapRequestModel>(model), User.Identity.Name);
            return Success();
        }
    }
}
