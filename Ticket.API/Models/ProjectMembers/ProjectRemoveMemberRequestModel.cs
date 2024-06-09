namespace Ticket.API.Models.ProjectMembers
{
    public class ProjectRemoveMemberRequestModel
    {
        /// <summary>
        /// Id người dùng
        /// </summary>
        [StringExtension("Id người dùng", maxSize: 100)]
        public string MemberId { get; set; }

        /// <summary>
        /// Id dự án
        /// </summary>
        [StringExtension("Id dự án", maxSize: 100)]
        public string ProjectId { get; set; }
    }
}
