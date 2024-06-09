namespace Ticket.API.Models.Projects
{
    public class ProjectCreateRequestModel
    {
        /// <summary>
        /// Tên dự án
        /// </summary>
        [StringExtension("Tên dự án")]
        public string ProjectName { get; set; }

        /// <summary>
        /// Id không gian công việc
        /// </summary>
        [StringExtension("Id không gian công việc", maxSize: 100)]
        public string WorkSpaceId { get; set; }

        /// <summary>
        /// Độ ưu tiên dự án
        /// </summary>
        public ProjectTaskPriorityEnums? ProjectPriority { get; set; }

        /// <summary>
        /// Thời gian kết thúc
        /// </summary>
        public DateTime? EstimateTime { get; set; }
    }

    public class ProjectCreateMapRequestModel
    {
        /// <summary>
        /// Tên dự án
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// Id không gian công việc
        /// </summary>
        public string WorkSpaceId { get; set; }

        /// <summary>
        /// Độ ưu tiên dự án
        /// </summary>
        public ProjectTaskPriorityEnums? ProjectPriority { get; set; }

        /// <summary>
        /// Thời gian kết thúc
        /// </summary>
        public DateTime? EstimateTime { get; set; }
    }
}
