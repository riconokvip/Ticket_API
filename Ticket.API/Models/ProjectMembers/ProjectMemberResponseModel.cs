namespace Ticket.API.Models.ProjectMembers
{
    public class ListProjectMemberResponseModel
    {
        /// <summary>
        /// Danh sách thành viên
        /// </summary>
        public List<ProjectMemberResponseModel> Members { get; set; }

        /// <summary>
        /// Phân trang
        /// </summary>
        public PaginationResponse Pagination { get; set; }
    }

    public class ProjectMemberResponseModel
    {
        public string Id { get; set; }

        /// <summary>
        /// Thành viên
        /// </summary>
        public RefUserResponseModel Member { get; set; }

        /// <summary>
        /// Loại thành viên
        /// </summary>
        public ProjectMemberEnums MemberType { get; set; }

        /// <summary>
        /// Tên loại thành viên
        /// </summary>
        public string MemberTypeName => MemberType.GetEnumMemberValue();
    }
}
