namespace Ticket.API.Services
{
    public interface IAuthService
    {
        /// <summary>
        /// Đăng nhập
        /// </summary>
        /// <param name="email">Tài khoản email</param>
        /// <param name="password">Mật khẩu</param>
        /// <returns></returns>
        Task<LoginResponseModel> Login(string email, string password);
    }

    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserRepo _repo;
        private readonly IJwtHelper _jwtHelper;
        private readonly IMapper _mapper;
        private readonly string _name = "Tài khoản";

        public AuthService(
            ApplicationDbContext context,
            IJwtHelper jwtHelper,
            IMapper mapper
        )
        {
            _context = context;
            _repo = new UserRepo(context);
            _jwtHelper = jwtHelper;
            _mapper = mapper;
        }

        public async Task<LoginResponseModel> Login(string email, string password)
        {
            var user = await _context.Users
                        .Where(_ => _.Email.ToLower() == email.ToLower() && _.IsDeleted == false)
                        .FirstOrDefaultAsync();

            if (user == null)
                throw new BaseException(ErrorCodes.CONFLICT, HttpCodes.CONFLICT, $"{_name} không tồn tại");

            if (password.Verify(user.PasswordHash) == false)
                throw new BaseException(ErrorCodes.BAD_REQUEST, HttpCodes.BAD_REQUEST, $"{_name} sai mật khẩu");

            if (user.LockoutViolationEnabled == true)
                throw new BaseException(ErrorCodes.LOCKED, HttpCodes.LOCKED, $"{_name} đã bị khóa");

            var jwt = _jwtHelper.GenerateToken(user, [ApplicationRoles.Admin]);

            return new LoginResponseModel()
            {
                Jwt = jwt,
                Info = _mapper.Map<InformationResponse>(user)
            };
        }
    }
}
