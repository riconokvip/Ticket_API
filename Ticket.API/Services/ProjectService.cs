namespace Ticket.API.Services
{
    public interface IProjectService
    {
        /// <summary>
        /// Lấy danh sách dự án
        /// </summary>
        /// <param name="workSpaceId">Id không gian công việc</param>
        /// <param name="model">Bộ lọc</param>
        /// <returns></returns>
        Task<ListProjectResponseModel> GetProjects(ProjectRequestModel model, string workSpaceId);

        /// <summary>
        /// Thêm mới dự án
        /// </summary>
        /// <param name="model">Dữ liệu thêm mới</param>
        /// <param name="action">Người thực hiện</param>
        /// <returns></returns>
        Task CreateProject(ProjectCreateMapRequestModel model, string action);

        /// <summary>
        /// Cập nhật dự án
        /// </summary>
        /// <param name="model">Dữ liệu cập nhật</param>
        /// <param name="action">Người thực hiện</param>
        /// <param name="projectId">Id dự án</param>
        /// <returns></returns>
        Task UpdateProject(ProjectUpdateMapRequestModel model, string action, string projectId);

        /// <summary>
        /// Xóa dự án
        /// </summary>
        /// <param name="projectId">Id dự án</param>
        /// <param name="action">Người thực hiện</param>
        /// <returns></returns>
        Task DeleteProject(string projectId, string action);
    }

    public class ProjectService : IProjectService
    {
        private readonly ApplicationDbContext _context;
        private readonly ProjectRepo _repo;
        private readonly IMapper _mapper;
        private readonly string _name = "Dự án";

        public ProjectService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _repo = new ProjectRepo(context);
            _mapper = mapper;
        }

        public async Task<ListProjectResponseModel> GetProjects(ProjectRequestModel model, string workSpaceId)
        {
            var projects = _context.Projects.Where(_ => _.WorkSpaceId == workSpaceId && _.IsDeleted == false);
            var users = _context.Users.Where(_ => _.IsDeleted == false);

            var query = from p in projects
                        join u in users on p.CreatedBy equals u.Id into obj
                        from user in obj.DefaultIfEmpty()
                        select new ProjectResponseModel
                        {
                            Id = p.Id,
                            ProjectName = p.ProjectName,
                            ProjectPriority = p.ProjectPriority,
                            CompleteTasks = 0,
                            Tasks = 10,
                            EstimateTime = p.EstimateTime,
                            CreatedAt = p.CreatedAt,
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
                    _.ProjectName.ToLower().Contains(text) ||
                    _.CreatedUser.WorkName.ToLower().Contains(text)
                );
            }

            var data = await query
                            .OrderByDescending(_ => _.CreatedAt)
                            .Skip(skip)
                            .Take(model.PageSize)
                            .ToListAsync();
            var total = await query.CountAsync();

            return new ListProjectResponseModel
            {
                Projects = data,
                Pagination = new PaginationResponse
                {
                    PageIndex = model.PageIndex,
                    PageSize = model.PageSize,
                    Total = total
                }
            };
        }

        public async Task CreateProject(ProjectCreateMapRequestModel model, string action)
        {
            var project = await _context.Projects
                    .Where(_ =>
                        _.ProjectName.ToLower().Contains(model.ProjectName.ToLower()) &&
                        _.CreatedBy == action &&
                        _.IsDeleted == false)
                    .FirstOrDefaultAsync();

            if (project != null)
                throw new BaseException(ErrorCodes.CONFLICT, HttpCodes.CONFLICT, $"{_name} đã tồn tại");

            var workSpace = await _context.WorkSpaces
                    .Where(_ =>
                        _.Id == model.WorkSpaceId &&
                        _.IsDeleted == false)
                    .FirstOrDefaultAsync();

            if (workSpace == null)
                throw new BaseException(ErrorCodes.CONFLICT, HttpCodes.CONFLICT, $"Không gian công việc không tồn tại");

            var entity = _mapper.Map<ProjectEntities>(model);
            entity.Id = Guid.NewGuid().ToString();
            await _repo.Insert(entity, action);
        }

        public async Task UpdateProject(ProjectUpdateMapRequestModel model, string action, string projectId)
        {
            var project = await _context.Projects
                    .Where(_ =>
                        _.Id == projectId &&
                        _.IsDeleted == false)
                    .FirstOrDefaultAsync();

            if (project != null)
                throw new BaseException(ErrorCodes.CONFLICT, HttpCodes.CONFLICT, $"{_name} chưa tồn tại");

            var projectDup = await _context.Projects
                    .Where(_ =>
                        _.Id != projectId &&
                        _.ProjectName.ToLower().Contains(model.ProjectName.ToLower()) &&
                        _.CreatedBy == action &&
                        _.IsDeleted == false)
                    .FirstOrDefaultAsync();

            if (projectDup != null)
                throw new BaseException(ErrorCodes.CONFLICT, HttpCodes.CONFLICT, $"{_name} đã tồn tại");

            var entity = _mapper.Map<ProjectEntities>(model);
            entity.Id = projectId;
            entity.WorkSpaceId = project.WorkSpaceId;
            await _repo.Update(entity, action);
        }

        public async Task DeleteProject(string projectId, string action)
        {
            var project = await _context.Projects
                    .Where(_ =>
                        _.Id == projectId &&
                        _.IsDeleted == false)
                    .FirstOrDefaultAsync();

            if (project == null)
                throw new BaseException(ErrorCodes.NOT_FOUND, HttpCodes.NOT_FOUND, $"{_name} không tồn tại");

            await _repo.Delete(project, action);
        }
    }
}
