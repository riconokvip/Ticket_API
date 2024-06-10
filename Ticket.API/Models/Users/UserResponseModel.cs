namespace Ticket.API.Models.Users
{
    public class ListUserResponseModel
    {
        /// <summary>
        /// Danh sách người dùng
        /// </summary>
        public List<UserResponseModel> Users { get; set; }

        /// <summary>
        /// Phân trang
        /// </summary>
        public PaginationResponse Pagination { get; set; }
    }

    public class UserResponseModel
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
        public bool LockoutViolationEnabled { get; set; } = false;

        /// <summary>
        /// Trạng thái
        /// </summary>
        public string LockStatus => LockoutViolationEnabled ? "Khóa" : "Kích hoạt";

        /// <summary>
        /// Thời gian tạo
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
