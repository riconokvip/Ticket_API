namespace Ticket.API.Models.Tickets
{
    public class ListTicketResponseModel
    {
        /// <summary>
        /// Danh sách yêu cầu
        /// </summary>
        public List<TicketResponseModel> Tickets { get; set; }

        /// <summary>
        /// Phân trang
        /// </summary>
        public PaginationResponse Pagination { get; set; }
    }

    public class TicketResponseModel
    {
        public string Id { get; set; }

        /// <summary>
        /// Người tạo
        /// </summary>
        public RefUserResponseModel FromUser { get; set; }

        /// <summary>
        /// Nội dung yêu cầu
        /// </summary>
        public string TicketContent { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public TicketEnums TicketStatus { get; set; }

        /// <summary>
        /// Tên trạng thái
        /// </summary>
        public string TicketStatusName => TicketStatus.GetEnumMemberValue();

        /// <summary>
        /// Người cập nhật
        /// </summary>
        public CreatedByResponseModel UpdatedUser { get; set; }

        /// <summary>
        /// Thời gian tạo
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
