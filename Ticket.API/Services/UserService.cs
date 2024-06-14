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
        /// Đồng bộ người dùng
        /// </summary>
        /// <param name="esUsers">Danh sách người dùng</param>
        /// <returns></returns>
        Task SyncUsers(IEnumerable<EsUsers> esUsers);

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
        private readonly string _index = typeof(EsUsers).Name.ToLower();

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

        public async Task<ListUserResponseModel> GetUsers(UserRequestModel model)
        {
            var text = string.IsNullOrEmpty(model.TextSearch) ? "" : model.TextSearch.ToLower().Trim();
            var skip = (model.PageIndex - 1) * model.PageSize;

            var esUserKey = EsKeys.Users(model.PageIndex, model.PageSize, text);
            var syncUserKey = SyncKeys.Users;

            // Response data
            var data = new ListUserResponseModel
            {
                Users = new List<UserResponseModel>(),
                Pagination = new PaginationResponse
                {
                    PageIndex = model.PageIndex,
                    PageSize = model.PageSize,
                    Total = 0
                }
            };

            // Query elastic
            var esUsers = _cacheService.TryGetValue<EsUserCaches>(esUserKey, out var _esUsers);
            if (!esUsers)
            {
                var esQuery = await _esRepo.Client()
                                .SearchAsync<EsUsers>(s =>
                                    s.Index(_index)
                                    .From(skip)
                                    .Take(model.PageSize)
                                    .Sort(s => s.Descending(d => d.CreatedAt))
                                    .Query(q => q.Match(m => m.Query(text)))
                                );
                var esCountQuery = await _esRepo.Client()
                                .SearchAsync<EsUsers>(s =>
                                    s.Index(_index)
                                    .Query(q => q.Match(m => m.Query(text)))
                                );
                _esUsers = new EsUserCaches
                {
                    EsUsers = esQuery.Documents.ToList(),
                    Total = esCountQuery.Total
                };
                await _cacheService.SetAsync(esUserKey, _esUsers, RedisSettings.CACHE_OPTION);
            }
            data.Users.AddRange(_mapper.Map<List<UserResponseModel>>(_esUsers.EsUsers));
            data.Pagination.Total += _esUsers.Total;

            // Query redis
            var syncUsers = _cacheService.TryGetValue<List<EsUsers>>(syncUserKey, out var _syncUsers);
            if (syncUsers)
            {
                data.Users.AddRange(_mapper.Map<List<UserResponseModel>>(_syncUsers));
                data.Pagination.Total += _syncUsers.Count;
            }

            return data;
        }

        public async Task SyncUsers(IEnumerable<EsUsers> esUsers)
        {
            await _esRepo.AddOrUpdateBulk(esUsers);
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
                    var syncUser = _mapper.Map<EsUsers>(user);

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
                        syncUser.Roles = _mapper.Map<List<EsUserPermissions>>(permissions);
                    }

                    _context.BulkSaveChanges();

                    // Add sync
                    var syncUserKey = SyncKeys.Users;
                    var syncUsers = _cacheService.TryGetValue<List<EsUsers>>(syncUserKey, out var _syncUsers);
                    if (syncUsers)
                    {
                        _syncUsers.Add(syncUser);
                    }
                    else
                        _syncUsers = new List<EsUsers>{ syncUser };

                    await _cacheService.SetAsync(syncUserKey, _syncUsers);

                    // transaction commit
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
