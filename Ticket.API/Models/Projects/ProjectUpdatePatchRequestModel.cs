namespace Ticket.API.Models.Projects
{
    public class ProjectUpdateEstimateTimeRequestModel
    {
        /// <summary>
        /// Thời hạn
        /// </summary>
        public DateTime EstimateTime { get; set; }
    }

    public class ProjectUpdatePriorityRequestModel
    {
        /// <summary>
        /// Độ ưu tiên
        /// </summary>
        [EnumExtension<ProjectTaskPriorityEnums>("Độ ưu tiên")]
        public ProjectTaskPriorityEnums Priority { get; set; }
    }
}
