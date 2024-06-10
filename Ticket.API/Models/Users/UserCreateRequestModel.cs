namespace Ticket.API.Models.Users
{
    public class UserCreateRequestModel
    {
        /// <summary>
        /// Tên công việc/ tên đầy đủ
        /// </summary>
        [StringExtension("Tên công việc", maxSize: 50)]
        public string WorkName { get; set; }

        /// <summary>
        /// Tài khoản telegram
        /// </summary>
        [StringExtension("Tài khoản telegram", maxSize: 50)]
        public string Telegram { get; set; }

        /// <summary>
        /// Tài khoản email
        /// </summary>
        [EmailExtension("Tài khoản email")]
        public string Email { get; set; }

        /// <summary>
        /// Chức vụ
        /// </summary>
        [StringExtension("Chức vụ", maxSize: 50)]
        public string Level { get; set; }

        /// <summary>
        /// Mật khẩu
        /// </summary>
        [StringExtension("Mật khẩu")]
        [DefaultValue("okvip123")]
        public string Password { get; set; } = "okvip123";

        /// <summary>
        /// Quyền
        /// </summary>
        public List<UserPermission> Permissions { get; set; }
    }

    public class UserCreateMapRequestModel
    {
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
        /// Chức vụ
        /// </summary>
        public string Level { get; set; }

        /// <summary>
        /// Mật khẩu
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Quyền
        /// </summary>
        public List<UserPermission> Permissions { get; set; }
    }
}
