namespace Ticket.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectMembersController : BaseController
    {
        private readonly IProjectMemberService _projectMemberService;

        public ProjectMembersController(IProjectMemberService projectMemberService)
        {
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
            await _projectMemberService.RemoveMemberInProject(model, User.Identity.Name);
            return Success();
        }
    }
}
