﻿namespace Ticket.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : BaseController
    {
        private readonly IAuthorizationService _authService;
        private readonly IProjectService _projectService;
        private readonly IMapper _mapper;

        public ProjectsController(IAuthorizationService authService, IProjectService projectService, IMapper mapper)
        {
            _authService = authService;
            _projectService = projectService;
            _mapper = mapper;
        }

        /// <summary>
        /// Lấy danh sách dự án
        /// </summary>
        /// <param name="workSpaceId">Id không gian công việc</param>
        /// <param name="model">Bộ lọc</param>
        /// <returns></returns>
        [HttpGet("{workSpaceId}")]
        public async Task<BaseResponseWithPagination<List<ProjectResponseModel>>> GetAllProject([FromRoute] string workSpaceId, [FromQuery] ProjectRequestModel model)
        {
            var result = await _authService.AuthorizeAsync(User, ApplicationPermissions.GetListProject);
            if (!result.Succeeded)
                throw new BaseException(ErrorCodes.FORBIDDEN, HttpCodes.FOR_BIDDEN, ErrorCodes.FORBIDDEN.GetEnumMemberValue());

            var res = await _projectService.GetProjects(model, workSpaceId);
            return SuccessWithPagination(res.Pagination, res.Projects);
        }

        /// <summary>
        /// Thêm mới dự án
        /// </summary>
        /// <param name="model">Dữ liệu thêm mới</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<BaseResponse> CreateNewProject([FromBody] ProjectCreateRequestModel model)
        {
            var result = await _authService.AuthorizeAsync(User, ApplicationPermissions.CreateProject);
            if (!result.Succeeded)
                throw new BaseException(ErrorCodes.FORBIDDEN, HttpCodes.FOR_BIDDEN, ErrorCodes.FORBIDDEN.GetEnumMemberValue());

            await _projectService.CreateProject(_mapper.Map<ProjectCreateMapRequestModel>(model), User.Identity.Name);
            return Success();
        }

        /// <summary>
        /// Cập nhật dự án
        /// </summary>
        /// <param name="projectId">Id dự án</param>
        /// <param name="model">Dữ liệu cập nhật</param>
        /// <returns></returns>
        [HttpPut("{projectId}")]
        public async Task<BaseResponse> UpdateExistProject([FromRoute] string projectId, [FromBody] ProjectUpdateRequestModel model)
        {
            var result = await _authService.AuthorizeAsync(User, ApplicationPermissions.UpdateProject);
            if (!result.Succeeded)
                throw new BaseException(ErrorCodes.FORBIDDEN, HttpCodes.FOR_BIDDEN, ErrorCodes.FORBIDDEN.GetEnumMemberValue());

            await _projectService.UpdateProject(_mapper.Map<ProjectUpdateMapRequestModel>(model), User.Identity.Name, projectId);
            return Success();
        }

        /// <summary>
        /// Cập nhật thời hạn dự án
        /// </summary>
        /// <param name="projectId">Id dự án</param>
        /// <param name="model">Dữ liệu cập nhật</param>
        /// <returns></returns>
        [HttpPatch("deadline/{projectId}")]
        public async Task<BaseResponse> UpdateExistProject([FromRoute] string projectId, [FromBody] ProjectUpdateEstimateTimeRequestModel model)
        {
            var result = await _authService.AuthorizeAsync(User, ApplicationPermissions.UpdateProject);
            if (!result.Succeeded)
                throw new BaseException(ErrorCodes.FORBIDDEN, HttpCodes.FOR_BIDDEN, ErrorCodes.FORBIDDEN.GetEnumMemberValue());

            await _projectService.UpdateEstimateTimeProject(model, User.Identity.Name, projectId);
            return Success();
        }

        /// <summary>
        /// Cập nhật độ ưu tiên dự án
        /// </summary>
        /// <param name="projectId">Id dự án</param>
        /// <param name="model">Dữ liệu cập nhật</param>
        /// <returns></returns>
        [HttpPatch("priority/{projectId}")]
        public async Task<BaseResponse> UpdateExistProject([FromRoute] string projectId, [FromBody] ProjectUpdatePriorityRequestModel model)
        {
            var result = await _authService.AuthorizeAsync(User, ApplicationPermissions.UpdateProject);
            if (!result.Succeeded)
                throw new BaseException(ErrorCodes.FORBIDDEN, HttpCodes.FOR_BIDDEN, ErrorCodes.FORBIDDEN.GetEnumMemberValue());

            await _projectService.UpdatePriorityProject(model, User.Identity.Name, projectId);
            return Success();
        }

        /// <summary>
        /// Xóa dự án
        /// </summary>
        /// <param name="projectId">Id dự án</param>
        /// <returns></returns>
        [HttpDelete("{projectId}")]
        public async Task<BaseResponse> DeleteExistProject([FromRoute] string projectId)
        {
            var result = await _authService.AuthorizeAsync(User, ApplicationPermissions.DeleteProject);
            if (!result.Succeeded)
                throw new BaseException(ErrorCodes.FORBIDDEN, HttpCodes.FOR_BIDDEN, ErrorCodes.FORBIDDEN.GetEnumMemberValue());

            await _projectService.DeleteProject(User.Identity.Name, projectId);
            return Success();
        }
    }
}
