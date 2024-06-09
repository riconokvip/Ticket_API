namespace Ticket.API.Models.Auths
{
    public class LoginRequestModel
    {
        /// <summary>
        /// Tài khoản email
        /// </summary>
        [EmailExtension("Tài khoản email")]
        public string Email { get; set; }

        /// <summary>
        /// Mật khẩu
        /// </summary>
        [StringExtension("Tài khoản email")]
        public string Password { get; set; }
    }

    public class LoginMapRequestModel
    {
        /// <summary>
        /// Tài khoản email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Mật khẩu
        /// </summary>
        public string Password { get; set; }
    }
}
