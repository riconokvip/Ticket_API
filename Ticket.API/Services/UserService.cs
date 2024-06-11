using EFCore.BulkExtensions;

namespace Ticket.API.Services
{
    public interface IUserService
    {
        /// <summary>
        /// Lấy danh sách người dùng
        /// </summary>
        /// <param name="model">Bộ lọc</param>
        /// <returns></returns>
        Task<ListUserResponseModel> GetUsers(UserRequestModel model);

        /// <summary>
        /// Tạo người dùng mới
        /// </summary>
        /// <param name="model">Dữ liệu tạo mới</param>
        /// <param name="action">Id người thực hiện</param>
        /// <returns></returns>
        Task CreateUser(UserCreateMapRequestModel model, string action);
    }

    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserRepo _repo;
        private readonly IMapper _mapper;
        private readonly string _name = "Người dùng";

        public UserService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _repo = new UserRepo(context);
            _mapper = mapper;
        }

        public async Task<ListUserResponseModel> GetUsers(UserRequestModel model)
        {
            var text = string.IsNullOrEmpty(model.TextSearch) ? null : model.TextSearch.ToLower().Trim();
            var skip = (model.PageIndex - 1) * model.PageSize;

            var query = _context.Users
                    .Where(
                        _ => 
                        _.IsAdmin == false && 
                        _.IsDeleted == false
                    )
                    .Select(_ => new UserResponseModel
                    {
                        Id = _.Id,
                        Email = _.Email,
                        WorkName = _.WorkName,
                        Telegram = _.Telegram,
                        Level = _.Level,
                        Phone = _.Phone,
                        CreatedAt = _.CreatedAt,
                        LockoutViolationEnabled = _.LockoutViolationEnabled
                    });

            if (text != null)
            {
                query = query.Where(
                        _ =>
                        _.WorkName.ToLower().Contains(text) ||
                        _.Telegram.ToLower().Contains(text) ||
                        _.Email.ToLower().Contains(text) ||
                        _.Level.ToLower().Contains(text)
                    );
            }

            var data = await query
                            .OrderByDescending(_ => _.CreatedAt)
                            .Skip(skip)
                            .Take(model.PageSize)
                            .ToListAsync();
            var total = await query.CountAsync();

            return new ListUserResponseModel
            {
                Users = data,
                Pagination = new PaginationResponse
                {
                    PageIndex = model.PageIndex,
                    PageSize = model.PageSize,
                    Total = total
                }
            };
        }

        public async Task CreateUser(UserCreateMapRequestModel model, string action)
        {
            var user = await _context.Users
                    .Where(
                        _ => _.Email.ToLower() == model.Email.ToLower() ||
                        _.Telegram.ToLower() == model.Telegram.ToLower() ||
                        _.WorkName.ToLower() == model.WorkName.ToLower()
                    ).FirstOrDefaultAsync();

            if (user != null)
            {
                if (user.Email.ToLower() == model.Email.ToLower())
                    throw new BaseException(ErrorCodes.BAD_REQUEST, HttpCodes.BAD_REQUEST, $"{_name}: tài khoản email đã được sử dụng");

                if (user.Telegram.ToLower() == model.Telegram.ToLower())
                    throw new BaseException(ErrorCodes.BAD_REQUEST, HttpCodes.BAD_REQUEST, $"{_name}: tài khoản telegram đã được sử dụng");

                if (user.WorkName.ToLower() == model.WorkName.ToLower())
                    throw new BaseException(ErrorCodes.BAD_REQUEST, HttpCodes.BAD_REQUEST, $"{_name}: tên công việc đã được sử dụng");
            }

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    user = _mapper.Map<UserEntities>(model);
                    user.Id = Guid.NewGuid().ToString();
                    user.IsAdmin = false;
                    user.PasswordHash = model.Password.Hash();
                    user.CreatedAt = ApplicationExtensions.NOW;
                    user.CreatedBy = action;
                    _context.BulkInsert([user]);

                    if (model.Permissions.Count > 0)
                    {
                        var permissions = _mapper.Map<List<UserPermissionEntities>>(model.Permissions);
                        permissions.ForEach(permission =>
                        {
                            permission.Id = Guid.NewGuid().ToString();
                            permission.UserId = user.Id;
                            permission.PermissionName = permission.Permission.GetEnumMemberValue() + " " + permission.Resource.GetEnumMemberValue();
                            permission.CreatedAt = ApplicationExtensions.NOW;
                            permission.CreatedBy = action;
                        });
                        _context.BulkInsert(permissions);
                    }

                    _context.BulkSaveChanges();
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
                finally
                {
                    transaction.Dispose();
                }
            }
        }
    }
}
