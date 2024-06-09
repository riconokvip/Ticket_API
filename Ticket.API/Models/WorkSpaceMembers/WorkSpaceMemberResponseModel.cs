namespace Ticket.API.Models.WorkSpaceMembers
{
    public class ListWorkSpaceMemberResponseModel
    {
        /// <summary>
        /// Danh sách thành viên
        /// </summary>
        public List<WorkSpaceMemberResponseModel> Members { get; set; }

        /// <summary>
        /// Phân trang
        /// </summary>
        public PaginationResponse Pagination { get; set; }
    }

    public class WorkSpaceMemberResponseModel
    {
        public string Id { get; set; }

        /// <summary>
        /// Thành viên
        /// </summary>
        public RefUserResponseModel Member { get; set; }

        /// <summary>
        /// Loại thành viên
        /// </summary>
        public WorkSpaceMemberEnums MemberType { get; set; }

        /// <summary>
        /// Tên loại thành viên
        /// </summary>
        public string MemberTypeName => MemberType.GetEnumMemberValue();
    }
}
