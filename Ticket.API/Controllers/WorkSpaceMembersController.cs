namespace Ticket.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class WorkSpaceMembersController : BaseController
    {
        private readonly IAuthorizationService _authService;
        private readonly IWorkSpaceMemberService _workSpaceMemberService;

        public WorkSpaceMembersController(IAuthorizationService authService, IWorkSpaceMemberService workSpaceMemberService)
        {
            _authService = authService;
            _workSpaceMemberService = workSpaceMemberService;
        }

        /// <summary>
        /// Lấy danh sách thành viên trong không gian công việc
        /// </summary>
        /// <param name="model">Bộ lọc</param>
        /// <returns></returns>
        [HttpGet("{workSpaceId}")]
        public async Task<BaseResponseWithPagination<List<WorkSpaceMemberResponseModel>>> GetAllMemberInWorkSpace(
            [FromRoute] string workSpaceId,
            [FromQuery] WorkSpaceMemberRequestModel model)
        {
            var result = await _authService.AuthorizeAsync(User, ApplicationPermissions.GetListWorkSpaceMember);
            if (!result.Succeeded)
                throw new BaseException(ErrorCodes.FORBIDDEN, HttpCodes.FOR_BIDDEN, ErrorCodes.FORBIDDEN.GetEnumMemberValue());

            var res = await _workSpaceMemberService.GetWorkSpaceMembers(model, workSpaceId);
            return SuccessWithPagination(res.Pagination, res.Members);
        }

        /// <summary>
        /// Thêm thành viên vào không gian công việc
        /// </summary>
        /// <param name="model">Dữ liệu thêm mới</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<BaseResponse> AddNewMemberInWorkSpace([FromBody] WorkSpaceAddMemberRequestModel model)
        {
            var result = await _authService.AuthorizeAsync(User, ApplicationPermissions.AddWorkSpaceMember);
            if (!result.Succeeded)
                throw new BaseException(ErrorCodes.FORBIDDEN, HttpCodes.FOR_BIDDEN, ErrorCodes.FORBIDDEN.GetEnumMemberValue());

            await _workSpaceMemberService.AddMemberInWorkSpace(model, User.Identity.Name);
            return Success();
        }

        /// <summary>
        /// Xóa thành viên khỏi không gian công việc
        /// </summary>
        /// <param name="model">Dữ liệu xóa</param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<BaseResponse> DeleteExistMemberInWorkSpace([FromBody] WorkSpaceRemoveMemberRequestModel model)
        {
            var result = await _authService.AuthorizeAsync(User, ApplicationPermissions.RemoveWorkSpaceMember);
            if (!result.Succeeded)
                throw new BaseException(ErrorCodes.FORBIDDEN, HttpCodes.FOR_BIDDEN, ErrorCodes.FORBIDDEN.GetEnumMemberValue());

            await _workSpaceMemberService.RemoveMemberInWorkSpace(model, User.Identity.Name);
            return Success();
        }
    }
}
