namespace Ticket.API.Services
{
    public interface ITicketService
    {
        /// <summary>
        /// Lấy danh sách yêu cầu
        /// </summary>
        /// <param name="model">Bộ lọc</param>
        /// <returns></returns>
        Task<ListTicketResponseModel> GetTickets(TicketRequestModel model);

        /// <summary>
        /// Lấy danh sách yêu cầu theo người yêu cầu
        /// </summary>
        /// <param name="fromUserId">Id người dùng</param>
        /// <param name="model">Bộ lọc</param>
        /// <returns></returns>
        Task<ListTicketResponseModel> GetTicketsByUserId(TicketRequestModel model, string fromUserId);

        /// <summary>
        /// Tạo yêu cầu mới
        /// </summary>
        /// <param name="model">Dữ liệu thêm mới</param>
        /// <param name="action">Người thực hiện</param>
        /// <returns></returns>
        Task CreateTicket(TicketCreateMapRequestModel model, string action);

        /// <summary>
        /// Cập nhật nội dung yêu cầu
        /// </summary>
        /// <param name="model">Dữ liệu cập nhật</param>
        /// <param name="action">Người thực hiện</param>
        /// <param name="ticketId">Id yêu cầu</param>
        /// <returns></returns>
        Task UpdateContentTicket(TicketUpdateContentRequestModel model, string action, string ticketId);

        /// <summary>
        /// Cập nhật trạng thái yêu cầu
        /// </summary>
        /// <param name="model">Dữ liệu cập nhật</param>
        /// <param name="action">Người thực hiện</param>
        /// <param name="ticketId">Id yêu cầu</param>
        /// <returns></returns>
        Task UpdateStatusTicket(TicketUpdateStatusRequestModel model, string action, string ticketId);

        /// <summary>
        /// Xóa yêu cầu
        /// </summary>
        /// <param name="ticketId">Id yêu cầu</param>
        /// <param name="action">Người thực hiện</param>
        /// <returns></returns>
        Task DeleteTicket(string ticketId, string action);

        /// <summary>
        /// Kiểm tra yêu cầu tồn tại
        /// </summary>
        /// <param name="ticketId">Id yêu cầu</param>
        /// <returns></returns>
        Task<TicketEntities> CheckExistTicket(string ticketId);
    }

    public class TicketService : ITicketService
    {
        private readonly ApplicationDbContext _context;
        private readonly TicketRepo _repo;
        private readonly IMapper _mapper;
        private readonly string _name = "Yêu cầu";

        public TicketService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _repo = new TicketRepo(context);
            _mapper = mapper;
        }

        public async Task<ListTicketResponseModel> GetTickets(TicketRequestModel model)
        {
            var tickets = _context.Tickets.Where(_ => _.IsDeleted == false);
            var users = _context.Users.Where(_ => _.IsDeleted == false);

            var query = from t in tickets
                        join u1 in users on t.CreatedBy equals u1.Id into obj1
                        join u2 in users on t.FromUserId equals u2.Id into obj2
                        from user1 in obj1.DefaultIfEmpty()
                        from user2 in obj2.DefaultIfEmpty()
                        select new TicketResponseModel
                        {
                            Id = t.Id,
                            FromUser = user2 == null ? null : new RefUserResponseModel
                            {
                                UserId = user2.Id,
                                WorkName = user2.WorkName,
                                IsAdmin = user2.IsAdmin,
                                Level = user2.IsAdmin ? "Quản trị viên" : user2.Level
                            },
                            TicketContent = t.TicketContent,
                            TicketStatus = t.TicketStatus,
                            UpdatedUser = user1 == null ? null : new CreatedByResponseModel
                            {
                                UserId = user1.Id,
                                WorkName = user1.WorkName,
                                IsAdmin = user1.IsAdmin,
                                Level = user1.IsAdmin ? "Quản trị viên" : user1.Level
                            }
                        };
            var text = string.IsNullOrEmpty(model.TextSearch) ? null : model.TextSearch.ToLower().Trim();
            var skip = (model.PageIndex - 1) * model.PageSize;

            if (text != null)
            {
                query = query.Where(_ =>
                    _.TicketContent.ToLower().Contains(text) ||
                    _.FromUser.WorkName.ToLower().Contains(text)
                );
            }

            var data = await query
                            .OrderByDescending(_ => _.CreatedAt)
                            .Skip(skip)
                            .Take(model.PageSize)
                            .ToListAsync();
            var total = await query.CountAsync();

            return new ListTicketResponseModel
            {
                Tickets = data,
                Pagination = new PaginationResponse
                {
                    PageIndex = model.PageIndex,
                    PageSize = model.PageSize,
                    Total = total
                }
            };
        }

        public async Task<ListTicketResponseModel> GetTicketsByUserId(TicketRequestModel model, string fromUserId)
        {
            var tickets = _context.Tickets.Where(_ => _.FromUserId == fromUserId && _.IsDeleted == false);
            var users = _context.Users.Where(_ => _.IsDeleted == false);

            var query = from t in tickets
                        join u1 in users on t.CreatedBy equals u1.Id into obj1
                        join u2 in users on t.FromUserId equals u2.Id into obj2
                        from user1 in obj1.DefaultIfEmpty()
                        from user2 in obj2.DefaultIfEmpty()
                        select new TicketResponseModel
                        {
                            Id = t.Id,
                            FromUser = user2 == null ? null : new RefUserResponseModel
                            {
                                UserId = user2.Id,
                                WorkName = user2.WorkName,
                                IsAdmin = user2.IsAdmin,
                                Level = user2.IsAdmin ? "Quản trị viên" : user2.Level
                            },
                            TicketContent = t.TicketContent,
                            TicketStatus = t.TicketStatus,
                            UpdatedUser = user1 == null ? null : new CreatedByResponseModel
                            {
                                UserId = user1.Id,
                                WorkName = user1.WorkName,
                                IsAdmin = user1.IsAdmin,
                                Level = user1.IsAdmin ? "Quản trị viên" : user1.Level
                            }
                        };
            var text = string.IsNullOrEmpty(model.TextSearch) ? null : model.TextSearch.ToLower().Trim();
            var skip = (model.PageIndex - 1) * model.PageSize;

            if (text != null)
            {
                query = query.Where(_ =>
                    _.TicketContent.ToLower().Contains(text) ||
                    _.FromUser.WorkName.ToLower().Contains(text)
                );
            }

            var data = await query
                            .OrderByDescending(_ => _.CreatedAt)
                            .Skip(skip)
                            .Take(model.PageSize)
                            .ToListAsync();
            var total = await query.CountAsync();

            return new ListTicketResponseModel
            {
                Tickets = data,
                Pagination = new PaginationResponse
                {
                    PageIndex = model.PageIndex,
                    PageSize = model.PageSize,
                    Total = total
                }
            };
        }

        public async Task CreateTicket(TicketCreateMapRequestModel model, string action)
        {
            var ticket = await _context.Tickets
                    .Where(_ =>
                        _.TicketContent.ToLower().Contains(model.TicketContent.ToLower()) &&
                        _.FromUserId == model.FromUserId &&
                        _.IsDeleted == false)
                    .FirstOrDefaultAsync();

            if (ticket != null)
                throw new BaseException(ErrorCodes.CONFLICT, HttpCodes.CONFLICT, $"{_name} đã tồn tại");

            var entity = _mapper.Map<TicketEntities>(model);
            entity.Id = Guid.NewGuid().ToString();
            await _repo.Insert(entity, action);
        }

        public async Task UpdateContentTicket(TicketUpdateContentRequestModel model, string action, string ticketId)
        {
            var ticket = await _context.Tickets
                    .Where(_ =>
                        _.Id == ticketId &&
                        _.IsDeleted == false)
                    .FirstOrDefaultAsync();

            if (ticket == null)
                throw new BaseException(ErrorCodes.NOT_FOUND, HttpCodes.NOT_FOUND, $"{_name} chưa tồn tại");

            ticket.TicketContent = model.TicketContent.Trim();
            await _repo.Update(ticket, action);
        }

        public async Task UpdateStatusTicket(TicketUpdateStatusRequestModel model, string action, string ticketId)
        {
            var ticket = await _context.Tickets
                    .Where(_ =>
                        _.Id == ticketId &&
                        _.IsDeleted == false)
                    .FirstOrDefaultAsync();

            if (ticket == null)
                throw new BaseException(ErrorCodes.NOT_FOUND, HttpCodes.NOT_FOUND, $"{_name} chưa tồn tại");

            ticket.TicketStatus = model.TicketStatus;
            await _repo.Update(ticket, action);
        }

        public async Task DeleteTicket(string ticketId, string action)
        {
            var ticket = await _context.Tickets
                    .Where(_ =>
                        _.Id == ticketId &&
                        _.IsDeleted == false)
                    .FirstOrDefaultAsync();

            if (ticket == null)
                throw new BaseException(ErrorCodes.NOT_FOUND, HttpCodes.NOT_FOUND, $"{_name} chưa tồn tại");

            await _repo.Delete(ticket, action);
        }

        public async Task<TicketEntities> CheckExistTicket(string ticketId)
        {
            var ticket = await _context.Tickets
                   .Where(_ =>
                       _.Id == ticketId &&
                       _.IsDeleted == false)
                   .FirstOrDefaultAsync();

            if (ticket == null)
                throw new BaseException(ErrorCodes.NOT_FOUND, HttpCodes.NOT_FOUND, $"{_name} chưa tồn tại");

            return ticket;
        }
    }
}
