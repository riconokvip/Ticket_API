namespace Ticket.API.Models.Auths
{
    public class LoginResponseModel
    {
        /// <summary>
        /// Access token
        /// </summary>
        public JwtResponse Jwt { get; set; }

        /// <summary>
        /// Thông tin người đăng nhập
        /// </summary>
        public InformationResponse Info { get; set; }

        /// <summary>
        /// Danh sách tài nguyên được truy cập
        /// </summary>
        public List<UserPermissionResponse> Roles { get; set; } = [];
    }

    public class InformationResponse
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
        /// Quản trị viên
        /// </summary>
        public bool IsAdmin { get; set; }
    }
}
