﻿namespace Ticket.API.Models.WorkSpaces
{
    public class ListWorkSpaceResponseModel
    {
        /// <summary>
        /// Danh sách không gian công việc
        /// </summary>
        public List<WorkSpaceResponseModel> WorkSpaces { get; set; }

        /// <summary>
        /// Phân trang
        /// </summary>
        public PaginationResponse Pagination { get; set; }
    }

    public class WorkSpaceResponseModel
    {
        public string Id { get; set; }

        /// <summary>
        /// Tên không gian công việc
        /// </summary>
        public string WorkSpaceName { get; set; }

        /// <summary>
        /// Màu đại diện
        /// </summary>
        public string WorkSpaceColor { get; set; }

        /// <summary>
        /// Thời gian tạo
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Thành viên
        /// </summary>
        public List<RefUserResponseModel> Members { get; set; } = [];

        /// <summary>
        /// Số lượng thành viên
        /// </summary>
        public long TotalMember => Members.Count;

        /// <summary>
        /// Người tạo
        /// </summary>
        public CreatedByResponseModel CreatedUser { get; set; }
    }
}
