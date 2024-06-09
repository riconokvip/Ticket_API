namespace Ticket.API.Models
{
    public class RefUserResponseModel
    {
        /// <summary>
        /// Id người tạo
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Tên công việc
        /// </summary>
        public string WorkName { get; set; }

        /// <summary>
        /// Chức vụ
        /// </summary>
        public string Level { get; set; }

        /// <summary>
        /// Quản trị viên
        /// </summary>
        public bool IsAdmin { get; set; }
    }

    public class CreatedByResponseModel
    {
        /// <summary>
        /// Id người tạo
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Tên công việc
        /// </summary>
        public string WorkName { get; set; }

        /// <summary>
        /// Chức vụ
        /// </summary>
        public string Level { get; set; }

        /// <summary>
        /// Quản trị viên
        /// </summary>
        public bool IsAdmin { get; set; }
    }
}
