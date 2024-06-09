namespace Ticket.API.Services
{
    public interface IWorkSpaceMemberService
    {
        /// <summary>
        /// Lấy danh sách thành viên trong không gian làm việc
        /// </summary>
        /// <param name="workSpaceId">Id không gian công việc</param>
        /// <param name="model">Bộ lọc</param>
        /// <returns></returns>
        Task<ListWorkSpaceMemberResponseModel> GetWorkSpaceMembers(WorkSpaceMemberRequestModel model, string workSpaceId);

        /// <summary>
        /// Thêm thành viên vào không gian làm việc
        /// </summary>
        /// <param name="model">Dữ liệu thêm mới</param>
        /// <param name="action">Người thực hiện</param>
        /// <returns></returns>
        Task AddMemberInWorkSpace(WorkSpaceAddMemberRequestModel model, string action);

        /// <summary>
        /// Xóa thành viên khỏi không gian làm việc
        /// </summary>
        /// <param name="model">Dữ liệu xóa</param>
        /// <param name="action">Người thực hiện</param>
        /// <returns></returns>
        Task RemoveMemberInWorkSpace(WorkSpaceRemoveMemberRequestModel model, string action);
    }

    public class WorkSpaceMemberService : IWorkSpaceMemberService
    {
        private readonly ApplicationDbContext _context;
        private readonly WorkSpaceMemberRepo _repo;
        private readonly IMapper _mapper;
        private readonly string _name = "Thành viên";

        public WorkSpaceMemberService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _repo = new WorkSpaceMemberRepo(context);
            _mapper = mapper;
        }

        public async Task<ListWorkSpaceMemberResponseModel> GetWorkSpaceMembers(WorkSpaceMemberRequestModel model, string workSpaceId)
        {
            var project = await _context.WorkSpaces
                    .Where(_ =>
                        _.Id == workSpaceId &&
                        _.IsDeleted == false)
                    .FirstOrDefaultAsync();

            if (project == null)
                throw new BaseException(ErrorCodes.NOT_FOUND, HttpCodes.NOT_FOUND, $"Không gian làm việc không tồn tại");

            var workSpaceMembers = _context.WorkSpaceMembers.Where(_ => _.WorkSpaceId == workSpaceId && _.IsDeleted == false);
            var users = _context.Users.Where(_ => _.IsDeleted == false);

            var query = from w in workSpaceMembers
                        join u in users on w.MemberId equals u.Id
                        select new WorkSpaceMemberResponseModel
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

            return new ListWorkSpaceMemberResponseModel
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

        public async Task AddMemberInWorkSpace(WorkSpaceAddMemberRequestModel model, string action)
        {
            var member = await _context.WorkSpaceMembers
                    .Where(_ =>
                        _.WorkSpaceId == model.WorkSpaceId &&
                        _.MemberId == model.MemberId &&
                        _.IsDeleted == false)
                    .FirstOrDefaultAsync();

            if (member != null)
                throw new BaseException(ErrorCodes.CONFLICT, HttpCodes.CONFLICT, $"{_name} đã tồn tại trong không gian công việc");

            var entity = _mapper.Map<WorkSpaceMemberEntities>(model);
            entity.Id = Guid.NewGuid().ToString();
            await _repo.Insert(entity, action);
        }

        public async Task RemoveMemberInWorkSpace(WorkSpaceRemoveMemberRequestModel model, string action)
        {
            var member = await _context.WorkSpaceMembers
                    .Where(_ =>
                        _.WorkSpaceId == model.WorkSpaceId &&
                        _.MemberId == model.MemberId &&
                        _.IsDeleted == false)
                    .FirstOrDefaultAsync();

            if (member != null)
                throw new BaseException(ErrorCodes.NOT_FOUND, HttpCodes.NOT_FOUND, $"{_name} chưa tồn tại trong không gian công việc");

            await _repo.Delete(member, action);
        }
    }
}
