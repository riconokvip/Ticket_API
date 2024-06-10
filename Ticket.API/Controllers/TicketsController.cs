namespace Ticket.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TicketsController : BaseController
    {
        private readonly ITicketService _ticketService;
        private readonly IMapper _mapper;

        public TicketsController(ITicketService ticketService, IMapper mapper)
        {
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
            await _ticketService.DeleteTicket(User.Identity.Name, ticketId);
            return Success();
        }
    }
}
