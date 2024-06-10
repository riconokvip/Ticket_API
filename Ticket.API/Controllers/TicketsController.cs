namespace Ticket.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TicketsController : BaseController
    {
        private readonly IAuthorizationService _authService;
        private readonly ITicketService _ticketService;
        private readonly IMapper _mapper;

        public TicketsController(IAuthorizationService authService, ITicketService ticketService, IMapper mapper)
        {
            _authService = authService;
            _ticketService = ticketService;
            _mapper = mapper;
        }

        /// <summary>
        /// Lấy danh sách yêu cầu của người dùng
        /// </summary>
        /// <param name="fromUserId">Id người dùng</param>
        /// <param name="model">Bộ lọc</param>
        /// <returns></returns>
        [HttpGet("{fromUserId}")]
        public async Task<BaseResponseWithPagination<List<TicketResponseModel>>> GetAllTicketByUserId(
            [FromRoute] string fromUserId, 
            [FromQuery] TicketRequestModel model)
        {
            var result = await _authService.AuthorizeAsync(User, fromUserId, ApplicationPermissions.GetListTicketByUser);
            if (!result.Succeeded)
                throw new BaseException(ErrorCodes.FORBIDDEN, HttpCodes.FOR_BIDDEN, ErrorCodes.FORBIDDEN.GetEnumMemberValue());

            var res = await _ticketService.GetTicketsByUserId(model, fromUserId);
            return SuccessWithPagination(res.Pagination, res.Tickets);
        }

        /// <summary>
        /// Lấy danh sách yêu cầu
        /// </summary>
        /// <param name="model">Bộ lọc</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<BaseResponseWithPagination<List<TicketResponseModel>>> GetAllTicket([FromQuery] TicketRequestModel model)
        {
            var result = await _authService.AuthorizeAsync(User, ApplicationPermissions.GetListTicket);
            if (!result.Succeeded)
                throw new BaseException(ErrorCodes.FORBIDDEN, HttpCodes.FOR_BIDDEN, ErrorCodes.FORBIDDEN.GetEnumMemberValue());

            var res = await _ticketService.GetTickets(model);
            return SuccessWithPagination(res.Pagination, res.Tickets);
        }

        /// <summary>
        /// Thêm mới yêu cầu
        /// </summary>
        /// <param name="model">Dữ liệu thêm mới</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<BaseResponse> CreateNewTicket([FromBody] TicketCreateRequestModel model)
        {
            var result = await _authService.AuthorizeAsync(User, ApplicationPermissions.CreateTicket);
            if (!result.Succeeded)
                throw new BaseException(ErrorCodes.FORBIDDEN, HttpCodes.FOR_BIDDEN, ErrorCodes.FORBIDDEN.GetEnumMemberValue());

            await _ticketService.CreateTicket(_mapper.Map<TicketCreateMapRequestModel>(model), User.Identity.Name);
            return Success();
        }

        /// <summary>
        /// Cập nhật nội dung yêu cầu
        /// </summary>
        /// <param name="ticketId">Id yêu cầu</param>
        /// <param name="model">Dữ liệu cập nhật</param>
        /// <returns></returns>
        [HttpPatch("content/{ticketId}")]
        public async Task<BaseResponse> UpdateExistTicket([FromRoute] string ticketId, [FromBody] TicketUpdateContentRequestModel model)
        {
            var result = await _authService.AuthorizeAsync(User, ApplicationPermissions.UpdateTicket);
            if (!result.Succeeded)
                throw new BaseException(ErrorCodes.FORBIDDEN, HttpCodes.FOR_BIDDEN, ErrorCodes.FORBIDDEN.GetEnumMemberValue());

            await _ticketService.UpdateContentTicket(model, User.Identity.Name, ticketId);
            return Success();
        }

        /// <summary>
        /// Cập nhật trạng thái yêu cầu
        /// </summary>
        /// <param name="ticketId">Id yêu cầu</param>
        /// <param name="model">Dữ liệu cập nhật</param>
        /// <returns></returns>
        [HttpPatch("status/{ticketId}")]
        public async Task<BaseResponse> UpdateExistTicket([FromRoute] string ticketId, [FromBody] TicketUpdateStatusRequestModel model)
        {
            var result = await _authService.AuthorizeAsync(User, ApplicationPermissions.UpdateTicket);
            if (!result.Succeeded)
                throw new BaseException(ErrorCodes.FORBIDDEN, HttpCodes.FOR_BIDDEN, ErrorCodes.FORBIDDEN.GetEnumMemberValue());

            await _ticketService.UpdateStatusTicket(model, User.Identity.Name, ticketId);
            return Success();
        }

        /// <summary>
        /// Xóa yêu cầu
        /// </summary>
        /// <param name="ticketId">Id yêu cầu</param>
        /// <returns></returns>
        [HttpDelete("{ticketId}")]
        public async Task<BaseResponse> DeleteExistTicket([FromRoute] string ticketId)
        {
            var ticket = await _ticketService.CheckExistTicket(ticketId);

            var result = await _authService.AuthorizeAsync(User, ticket.FromUserId, ApplicationPermissions.DeleteTicket);
            if (!result.Succeeded)
                throw new BaseException(ErrorCodes.FORBIDDEN, HttpCodes.FOR_BIDDEN, ErrorCodes.FORBIDDEN.GetEnumMemberValue());

            await _ticketService.DeleteTicket(User.Identity.Name, ticketId);
            return Success();
        }
    }
}
