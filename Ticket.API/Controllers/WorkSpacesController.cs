namespace Ticket.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class WorkSpacesController : BaseController
    {
        private readonly IAuthorizationService _authService;
        private readonly IWorkSpaceService _workSpaceService;
        private readonly IMapper _mapper;

        public WorkSpacesController(IAuthorizationService authService, IWorkSpaceService workSpaceService, IMapper mapper)
        {
            _authService = authService;
            _workSpaceService = workSpaceService;
            _mapper = mapper;
        }

        /// <summary>
        /// Lấy danh sách không gian công việc
        /// </summary>
        /// <param name="model">Bộ lọc</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<BaseResponseWithPagination<List<WorkSpaceResponseModel>>> GetAllWorkSpace([FromQuery] WorkSpaceRequestModel model)
        {
            var result = await _authService.AuthorizeAsync(User, ApplicationPermissions.GetListWorkSpace);
            if (!result.Succeeded)
                throw new BaseException(ErrorCodes.FORBIDDEN, HttpCodes.FOR_BIDDEN, ErrorCodes.FORBIDDEN.GetEnumMemberValue());

            var res = await _workSpaceService.GetWorkSpaces(model);
            return SuccessWithPagination(res.Pagination, res.WorkSpaces);
        }

        /// <summary>
        /// Thêm mới không gian công việc
        /// </summary>
        /// <param name="model">Dữ liệu thêm mới</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<BaseResponse> CreateNewWorkSpace([FromBody] WorkSpaceCreateRequestModel model)
        {
            var result = await _authService.AuthorizeAsync(User, ApplicationPermissions.CreateWorkSpace);
            if (!result.Succeeded)
                throw new BaseException(ErrorCodes.FORBIDDEN, HttpCodes.FOR_BIDDEN, ErrorCodes.FORBIDDEN.GetEnumMemberValue());

            await _workSpaceService.CreateWorkSpace(_mapper.Map<WorkSpaceCreateMapRequestModel>(model), User.Identity.Name);
            return Success();
        }

        /// <summary>
        /// Cập nhật không gian công việc
        /// </summary>
        /// <param name="workSpaceId">Id không gian công việc</param>
        /// <param name="model">Dữ liệu cập nhật</param>
        /// <returns></returns>
        [HttpPut("{workSpaceId}")]
        public async Task<BaseResponse> UpdateExistWorkSpace([FromRoute] string workSpaceId, [FromBody] WorkSpaceUpdateRequestModel model)
        {
            var result = await _authService.AuthorizeAsync(User, ApplicationPermissions.UpdateWorkSpace);
            if (!result.Succeeded)
                throw new BaseException(ErrorCodes.FORBIDDEN, HttpCodes.FOR_BIDDEN, ErrorCodes.FORBIDDEN.GetEnumMemberValue());

            await _workSpaceService.UpdateWorkSpace(_mapper.Map<WorkSpaceUpdateMapRequestModel>(model), User.Identity.Name, workSpaceId);
            return Success();
        }

        /// <summary>
        /// Xóa không gian công việc
        /// </summary>
        /// <param name="workSpaceId">Id không gian công việc</param>
        /// <returns></returns>
        [HttpDelete("{workSpaceId}")]
        public async Task<BaseResponse> DeleteExistWorkSpace([FromRoute] string workSpaceId)
        {
            var result = await _authService.AuthorizeAsync(User, ApplicationPermissions.DeleteWorkSpace);
            if (!result.Succeeded)
                throw new BaseException(ErrorCodes.FORBIDDEN, HttpCodes.FOR_BIDDEN, ErrorCodes.FORBIDDEN.GetEnumMemberValue());

            await _workSpaceService.DeleteWorkSpace(workSpaceId, User.Identity.Name);
            return Success();
        }
    }
}
