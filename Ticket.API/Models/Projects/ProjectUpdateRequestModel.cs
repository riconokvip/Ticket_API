namespace Ticket.API.Models.Projects
{
    public class ProjectUpdateRequestModel
    {
        /// <summary>
        /// Tên dự án
        /// </summary>
        [StringExtension("Tên dự án")]
        public string ProjectName { get; set; }

        /// <summary>
        /// Độ ưu tiên dự án
        /// </summary>
        public ProjectTaskPriorityEnums? ProjectPriority { get; set; }

        /// <summary>
        /// Thời gian kết thúc
        /// </summary>
        public DateTime? EstimateTime { get; set; }
    }

    public class ProjectUpdateMapRequestModel
    {
        /// <summary>
        /// Tên dự án
        /// </summary>
        public string ProjectName { get; set; }

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
