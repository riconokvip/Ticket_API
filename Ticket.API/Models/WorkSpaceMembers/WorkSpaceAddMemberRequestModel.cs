namespace Ticket.API.Models.WorkSpaceMembers
{
    public class WorkSpaceAddMemberRequestModel
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

        /// <summary>
        /// Loại thành viên
        /// </summary>
        [EnumExtension<WorkSpaceMemberEnums>("Loại thành viên")]
        public WorkSpaceMemberEnums MemberType { get; set; } = WorkSpaceMemberEnums.Member;
    }
}
