namespace Ticket.API.Models.WorkSpaces
{
    public class WorkSpaceCreateRequestModel
    {
        /// <summary>
        /// Tên không gian công việc
        /// </summary>
        [StringExtension("Tên không gian công việc")]
        public string WorkSpaceName { get; set; }

        /// <summary>
        /// Màu đại diện
        /// </summary>
        [StringExtension("Mã màu đại diện")]
        public string WorkSpaceColor { get; set; }
    }

    public class WorkSpaceCreateMapRequestModel
    {
        /// <summary>
        /// Tên không gian công việc
        /// </summary>
        public string WorkSpaceName { get; set; }

        /// <summary>
        /// Màu đại diện
        /// </summary>
        public string WorkSpaceColor { get; set; }
    }
}
