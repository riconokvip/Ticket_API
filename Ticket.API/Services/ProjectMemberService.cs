namespace Ticket.API.Services
{
    public interface IProjectMemberService
    {
        /// <summary>
        /// Lấy danh sách thành viên trong dự án
        /// </summary>
        /// <param name="projectId">Id dự án</param>
        /// <param name="model">Bộ lọc</param>
        /// <returns></returns>
        Task<ListProjectMemberResponseModel> GetProjectMembers(ProjectMemberRequestModel model, string projectId);

        /// <summary>
        /// Thêm thành viên vào dự án
        /// </summary>
        /// <param name="model">Dữ liệu thêm mới</param>
        /// <param name="action">Người thực hiện</param>
        /// <returns></returns>
        Task AddMemberInProject(ProjectAddMemberRequestModel model, string action);

        /// <summary>
        /// Xóa thành viên khỏi dự án
        /// </summary>
        /// <param name="model">Dữ liệu xóa</param>
        /// <param name="action">Người thực hiện</param>
        /// <returns></returns>
        Task RemoveMemberInProject(ProjectRemoveMemberRequestModel model, string action);
    }

    public class ProjectMemberService : IProjectMemberService
    {
        private readonly ApplicationDbContext _context;
        private readonly ProjectMemberRepo _repo;
        private readonly IMapper _mapper;
        private readonly string _name = "Thành viên";

        public ProjectMemberService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _repo = new ProjectMemberRepo(context);
            _mapper = mapper;
        }

        public async Task<ListProjectMemberResponseModel> GetProjectMembers(ProjectMemberRequestModel model, string projectId)
        {
            var project = await _context.Projects
                    .Where(_ =>
                        _.Id == projectId &&
                        _.IsDeleted == false)
                    .FirstOrDefaultAsync();

            if (project == null)
                throw new BaseException(ErrorCodes.NOT_FOUND, HttpCodes.NOT_FOUND, $"Dự án không tồn tại");

            var projectMembers = _context.ProjectMembers.Where(_ => _.ProjectId == projectId && _.IsDeleted == false);
            var users = _context.Users.Where(_ => _.IsDeleted == false);

            var query = from w in projectMembers
                        join u in users on w.MemberId equals u.Id
                        select new ProjectMemberResponseModel
                        {
                            Id = w.Id,
                            Member = u == null ? null : new RefUserResponseModel
                            {
                                UserId = u.Id,
                                WorkName = u.WorkName,
                                IsAdmin = u.IsAdmin,
                                Level = u.IsAdmin ? "Quản trị viên" : u.Level
                            },
                            MemberType = w.MemberType
                        };

            var text = string.IsNullOrEmpty(model.TextSearch) ? null : model.TextSearch.ToLower().Trim();
            var skip = (model.PageIndex - 1) * model.PageSize;

            if (text != null)
            {
                query = query.Where(_ =>
                    _.Member.WorkName.ToLower().Contains(text)
                );
            }

            var data = await query
                            .OrderBy(_ => _.MemberType)
                            .Skip(skip)
                            .Take(model.PageSize)
                            .ToListAsync();
            var total = await query.CountAsync();

            return new ListProjectMemberResponseModel
            {
                Members = data,
                Pagination = new PaginationResponse
                {
                    PageIndex = model.PageIndex,
                    PageSize = model.PageSize,
                    Total = total
                }
            };
        }

        public async Task AddMemberInProject(ProjectAddMemberRequestModel model, string action)
        {
            var member = await _context.ProjectMembers
                    .Where(_ =>
                        _.ProjectId == model.ProjectId &&
                        _.MemberId == model.MemberId &&
                        _.IsDeleted == false)
                    .FirstOrDefaultAsync();

            if (member != null)
                throw new BaseException(ErrorCodes.CONFLICT, HttpCodes.CONFLICT, $"{_name} đã tồn tại trong dự án");

            var entity = _mapper.Map<ProjectMemberEntities>(model);
            entity.Id = Guid.NewGuid().ToString();
            await _repo.Insert(entity, action);
        }

        public async Task RemoveMemberInProject(ProjectRemoveMemberRequestModel model, string action)
        {
            var member = await _context.ProjectMembers
                    .Where(_ =>
                        _.ProjectId == model.ProjectId &&
                        _.MemberId == model.MemberId &&
                        _.IsDeleted == false)
                    .FirstOrDefaultAsync();

            if (member != null)
                throw new BaseException(ErrorCodes.NOT_FOUND, HttpCodes.NOT_FOUND, $"{_name} chưa tồn tại trong dự án");

            await _repo.Delete(member, action);
        }
    }
}
