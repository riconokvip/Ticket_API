namespace Ticket.API.Models.Tickets
{
    public class TicketUpdateContentRequestModel
    {
        /// <summary>
        /// Nội dung yêu cầu
        /// </summary>
        [StringExtension("Nội dung yêu cầu", maxSize: 400)]
        public string TicketContent { get; set; }
    }

    public class TicketUpdateStatusRequestModel
    {
        /// <summary>
        /// Trạng thái
        /// </summary>
        [EnumExtension<TicketEnums>("Trạng thái")]
        public TicketEnums TicketStatus { get; set; }
    }
}
