namespace Ticket.API.Models.Tickets
{
    public class TicketCreateRequestModel
    {
        /// <summary>
        /// Id người yêu cầu
        /// </summary>
        [StringExtension("Người yêu cầu", maxSize: 100)]
        public string FromUserId { get; set; }

        /// <summary>
        /// Nội dung yêu cầu
        /// </summary>
        [StringExtension("Nội dung yêu cầu", maxSize: 400)]
        public string TicketContent { get; set; }
    }

    public class TicketCreateMapRequestModel
    {
        /// <summary>
        /// Id người yêu cầu
        /// </summary>
        public string FromUserId { get; set; }

        /// <summary>
        /// Nội dung yêu cầu
        /// </summary>
        public string TicketContent { get; set; }
    }
}
