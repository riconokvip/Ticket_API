namespace Ticket.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectMembersController : BaseController
    {
        private readonly IAuthorizationService _authService;
        private readonly IProjectMemberService _projectMemberService;

        public ProjectMembersController(IAuthorizationService authService, IProjectMemberService projectMemberService)
        {
            _authService = authService;
            _projectMemberService = projectMemberService;
        }

        /// <summary>
        /// Lấy danh sách thành viên trong dự án
        /// </summary>
        /// <param name="model">Bộ lọc</param>
        /// <returns></returns>
        [HttpGet("{projectId}")]
        public async Task<BaseResponseWithPagination<List<ProjectMemberResponseModel>>> GetAllMemberInProject(
            [FromRoute] string projectId,
            [FromQuery] ProjectMemberRequestModel model)
        {
            var result = await _authService.AuthorizeAsync(User, ApplicationPermissions.GetListProjectMember);
            if (!result.Succeeded)
                throw new BaseException(ErrorCodes.FORBIDDEN, HttpCodes.FOR_BIDDEN, ErrorCodes.FORBIDDEN.GetEnumMemberValue());

            var res = await _projectMemberService.GetProjectMembers(model, projectId);
            return SuccessWithPagination(res.Pagination, res.Members);
        }

        /// <summary>
        /// Thêm thành viên vào dự án
        /// </summary>
        /// <param name="model">Dữ liệu thêm mới</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<BaseResponse> AddNewMemberInProject([FromBody] ProjectAddMemberRequestModel model)
        {
            var result = await _authService.AuthorizeAsync(User, ApplicationPermissions.AddProjectMember);
            if (!result.Succeeded)
                throw new BaseException(ErrorCodes.FORBIDDEN, HttpCodes.FOR_BIDDEN, ErrorCodes.FORBIDDEN.GetEnumMemberValue());

            await _projectMemberService.AddMemberInProject(model, User.Identity.Name);
            return Success();
        }

        /// <summary>
        /// Xóa thành viên khỏi dự án
        /// </summary>
        /// <param name="model">Dữ liệu xóa</param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<BaseResponse> DeleteExistMemberInProject([FromBody] ProjectRemoveMemberRequestModel model)
        {
            var result = await _authService.AuthorizeAsync(User, ApplicationPermissions.RemoveProjectMember);
            if (!result.Succeeded)
                throw new BaseException(ErrorCodes.FORBIDDEN, HttpCodes.FOR_BIDDEN, ErrorCodes.FORBIDDEN.GetEnumMemberValue());

            await _projectMemberService.RemoveMemberInProject(model, User.Identity.Name);
            return Success();
        }
    }
}
