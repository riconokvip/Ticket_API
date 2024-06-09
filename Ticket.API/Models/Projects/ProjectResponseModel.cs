namespace Ticket.API.Models.Projects
{
    public class ListProjectResponseModel
    {
        /// <summary>
        /// Danh sách dự án
        /// </summary>
        public List<ProjectResponseModel> Projects { get; set; }

        /// <summary>
        /// Phân trang
        /// </summary>
        public PaginationResponse Pagination { get; set; }
    }

    public class ProjectResponseModel
    {
        public string Id { get; set; }

        /// <summary>
        /// Tên dự án
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// Số lượng task hoàn thành
        /// </summary>
        public int CompleteTasks { get; set; }

        /// <summary>
        /// Số lượng task
        /// </summary>
        public int Tasks { get; set; }

        /// <summary>
        /// Tiến độ dự án
        /// </summary>
        public decimal ProjectProgress => CompleteTasks / Tasks * 100;

        /// <summary>
        /// Độ ưu tiên dự án
        /// </summary>
        public ProjectTaskPriorityEnums? ProjectPriority { get; set; }

        /// <summary>
        /// Thời gian tạo
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Thời gian kết thúc
        /// </summary>
        public DateTime? EstimateTime { get; set; }

        /// <summary>
        /// Người tạo
        /// </summary>
        public CreatedByResponseModel CreatedUser { get; set; }
    }
}
