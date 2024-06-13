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
        Task<ListUserResponseModel> GetEsUsers(UserRequestModel model);

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
        private readonly IEsRepo<EsUsers> _esRepo;
        private readonly IDistributedCacheService _cacheService;
        private readonly IMapper _mapper;
        private readonly string _name = "Người dùng";

        public UserService(
            ApplicationDbContext context,
            IEsRepo<EsUsers> esRepo,
            IDistributedCacheService cacheService, 
            IMapper mapper)
        {
            _context = context;
            _repo = new UserRepo(context);
            _esRepo = esRepo;
            _cacheService = cacheService;
            _mapper = mapper;
        }

        public async Task<ListUserResponseModel> GetEsUsers(UserRequestModel model)
        {
            // Get cache
            var resUserKey = CacheSettings.ResKey(RedisKeys.Users(model.PageIndex, model.PageSize, text));
            var resUsers = _cacheService.TryGetValue<ListUserResponseModel>(resUserKey, out var _resUsers);
            if (resUsers)
                return _resUsers;

            var text = string.IsNullOrEmpty(model.TextSearch) ? "" : model.TextSearch.ToLower().Trim();
            var skip = (model.PageIndex - 1) * model.PageSize;

            // Query redis
            var esUserKey = CacheSettings.EsKey(RedisKeys.EsUsers);
            var esUsers = _cacheService.TryGetValue<List<UserResponseModel>>(esUserKey, out var _esUsers);

            // Query elastic
            var esQuery = await _esRepo.Client()
                                .SearchAsync<EsUsers>(s => 
                                    s.Index(typeof(EsUsers).Name.ToLower())
                                    .From(skip)
                                    .Take(model.PageSize)
                                    .Query(q => q.Match(m => m.Query(text)))
                                );
            var esCount = await _esRepo.Client()
                                .SearchAsync<EsUsers>(s =>
                                    s.Index(typeof(EsUsers).Name.ToLower())
                                    .Query(q => q.Match(m => m.Query(text)))
                                );
            var esDatas = _mapper.Map<List<UserResponseModel>>(esQuery.Documents.ToList());

            // Set cache
            var data = new ListUserResponseModel
            {
                Users = esUsers ? _esUsers.Concat(esDatas).ToList() : esDatas,
                Pagination = new PaginationResponse
                {
                    PageIndex = model.PageIndex,
                    PageSize = model.PageSize,
                    Total = esCount.Total
                }
            };
            await _cacheService.SetAsync(resUserKey, data, CacheSettings.CACHE_OPTION);

            return data;
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
                            permission.PermissionName = permission.Permission.GetEnumMemberValue() + " " + permission.Resource.GetEnumMemberValue().ToLower();
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
