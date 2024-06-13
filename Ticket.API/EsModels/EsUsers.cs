namespace Ticket.API.EsModels
{
    public class EsUsers
    {
        public string Id { get; set; }

        /// <summary>
        /// Tên công việc/ tên đầy đủ
        /// </summary>
        public string WorkName { get; set; }

        /// <summary>
        /// Tài khoản telegram
        /// </summary>
        public string Telegram { get; set; }

        /// <summary>
        /// Tài khoản email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Số điện thoại
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Chức vụ
        /// </summary>
        public string Level { get; set; }

        /// <summary>
        /// Trạng thái khóa khi vi phạm
        /// </summary>
        public bool LockoutViolationEnabled { get; set; }

        /// <summary>
        /// Thời gian tạo
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Danh sách tài nguyên được truy cập
        /// </summary>
        public List<UserPermissionResponse> Roles { get; set; }
    }
}
