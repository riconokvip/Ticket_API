namespace Ticket.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class WorkSpaceMembersController : BaseController
    {
        private readonly IWorkSpaceMemberService _workSpaceMemberService;

        public WorkSpaceMembersController(IWorkSpaceMemberService workSpaceMemberService)
        {
            _workSpaceMemberService = workSpaceMemberService;
        }

        /// <summary>
        /// Lấy danh sách thành viên trong không gian công việc
        /// </summary>
        /// <param name="model">Bộ lọc</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<BaseResponseWithPagination<List<WorkSpaceMemberResponseModel>>> GetAllMemberInWorkSpace([FromQuery] WorkSpaceMemberRequestModel model)
        {
            var res = await _workSpaceMemberService.GetWorkSpaceMembers(model);
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
            await _workSpaceMemberService.RemoveMemberInWorkSpace(model, User.Identity.Name);
            return Success();
        }
    }
}
