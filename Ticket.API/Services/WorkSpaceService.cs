namespace Ticket.API.Services
{
    public interface IWorkSpaceService
    {
        /// <summary>
        /// Lấy danh sách không gian công việc
        /// </summary>
        /// <param name="model">Bộ lọc</param>
        /// <returns></returns>
        Task<ListWorkSpaceResponseModel> GetWorkSpaces(WorkSpaceRequestModel model);

        /// <summary>
        /// Thêm mới không gian làm việc
        /// </summary>
        /// <param name="model">Dữ liệu thêm mới</param>
        /// <param name="action">Người thực hiện</param>
        /// <returns></returns>
        Task CreateWorkSpace(WorkSpaceCreateMapRequestModel model, string action);

        /// <summary>
        /// Cập nhật không gian làm việc
        /// </summary>
        /// <param name="model">Dữ liệu cập nhật</param>
        /// <param name="action">Người thực hiện</param>
        /// <param name="workSpaceId">Id không gian công việc</param>
        /// <returns></returns>
        Task UpdateWorkSpace(WorkSpaceUpdateMapRequestModel model, string action, string workSpaceId);

        /// <summary>
        /// Xóa không gian làm việc
        /// </summary>
        /// <param name="workSpaceId">Id không gian làm việc</param>
        /// <param name="action">Người thực hiện</param>
        /// <returns></returns>
        Task DeleteWorkSpace(string workSpaceId, string action);
    }

    public class WorkSpaceService : IWorkSpaceService
    {
        private readonly ApplicationDbContext _context;
        private readonly WorkSpaceRepo _repo;
        private readonly IMapper _mapper;
        private readonly string _name = "Không gian công việc";

        public WorkSpaceService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _repo = new WorkSpaceRepo(context);
            _mapper = mapper;
        }

        public async Task<ListWorkSpaceResponseModel> GetWorkSpaces(WorkSpaceRequestModel model)
        {
            var workSpaces = _context.WorkSpaces.Where(_ => _.IsDeleted == false);
            var users = _context.Users.Where(_ => _.IsDeleted == false);

            var query = from w in workSpaces
                        join u in users on w.CreatedBy equals u.Id into obj
                        from user in obj.DefaultIfEmpty()
                        select new WorkSpaceResponseModel
                        {
                            Id = w.Id,
                            WorkSpaceName = w.WorkSpaceName,
                            WorkSpaceColor = w.WorkSpaceColor,
                            CreatedAt = w.CreatedAt,
                            CreatedUser = user == null ? null : new CreatedByResponseModel
                            {
                                UserId = user.Id,
                                WorkName = user.WorkName,
                                IsAdmin = user.IsAdmin,
                                Level = user.IsAdmin ? "Quản trị viên" : user.Level
                            }
                        };

            var text = string.IsNullOrEmpty(model.TextSearch) ? null : model.TextSearch.ToLower().Trim();
            var skip = (model.PageIndex - 1) * model.PageSize;

            if (text != null)
            {
                query = query.Where(_ => 
                    _.WorkSpaceName.ToLower().Contains(text) ||
                    _.CreatedUser.WorkName.ToLower().Contains(text)
                );
            }

            var data = await query
                            .OrderByDescending(_ => _.CreatedAt)
                            .Skip(skip)
                            .Take(model.PageSize)
                            .ToListAsync();
            var total = await query.CountAsync();

            return new ListWorkSpaceResponseModel
            {
                WorkSpaces = data,
                Pagination = new PaginationResponse
                {
                    PageIndex = model.PageIndex,
                    PageSize = model.PageSize,
                    Total = total
                }
            };
        }

        public async Task CreateWorkSpace(WorkSpaceCreateMapRequestModel model, string action)
        {
            var workSpace = await _context.WorkSpaces
                    .Where(_ => 
                        _.WorkSpaceName.ToLower().Contains(model.WorkSpaceName.ToLower()) && 
                        _.CreatedBy == action &&
                        _.IsDeleted == false)
                    .FirstOrDefaultAsync();

            if (workSpace != null)
                throw new BaseException(ErrorCodes.CONFLICT, HttpCodes.CONFLICT, $"{_name} đã tồn tại");

            var entity = _mapper.Map<WorkSpaceEntities>(model);
            entity.Id = Guid.NewGuid().ToString();
            await _repo.Insert(entity, action);
        }

        public async Task UpdateWorkSpace(WorkSpaceUpdateMapRequestModel model, string action, string workSpaceId)
        {
            var workSpace = await _context.WorkSpaces
                    .Where(_ => 
                        _.Id == workSpaceId &&
                        _.IsDeleted == false)
                    .FirstOrDefaultAsync();

            if (workSpace != null)
                throw new BaseException(ErrorCodes.CONFLICT, HttpCodes.CONFLICT, $"{_name} chưa tồn tại");

            var workSpaceDup = await _context.WorkSpaces
                    .Where(_ => 
                        _.Id != workSpaceId &&
                        _.WorkSpaceName.ToLower().Contains(model.WorkSpaceName.ToLower()) && 
                        _.CreatedBy == action &&
                        _.IsDeleted == false)
                    .FirstOrDefaultAsync();

            if (workSpaceDup != null)
                throw new BaseException(ErrorCodes.CONFLICT, HttpCodes.CONFLICT, $"{_name} đã tồn tại");

            var entity = _mapper.Map<WorkSpaceEntities>(model);
            entity.Id = workSpaceId;
            await _repo.Update(entity, action);
        }

        public async Task DeleteWorkSpace(string workSpaceId, string action)
        {
            var workSpace = await _context.WorkSpaces
                    .Where(_ =>
                        _.Id == workSpaceId &&
                        _.IsDeleted == false)
                    .FirstOrDefaultAsync();

            if (workSpace == null)
                throw new BaseException(ErrorCodes.NOT_FOUND, HttpCodes.NOT_FOUND, $"{_name} không tồn tại");

            await _repo.Delete(workSpace, action);
        }
    }
}
