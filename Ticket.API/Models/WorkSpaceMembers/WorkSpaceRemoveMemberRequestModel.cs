namespace Ticket.API.Models.WorkSpaceMembers
{
    public class WorkSpaceRemoveMemberRequestModel
    {
        /// <summary>
        /// Id người dùng
        /// </summary>
        [StringExtension("Id người dùng", maxSize: 100)]
        public string MemberId { get; set; }

        /// <summary>
        /// Id không gian công việc
        /// </summary>
        [StringExtension("Id không gian công việc", maxSize: 100)]
        public string WorkSpaceId { get; set; }
    }
}
