namespace Ticket.API.Models
{
    public class UserPermission
    {
        /// <summary>
        /// Tài nguyên
        /// </summary>
        public ResourceEnums Resource { get; set; }

        /// <summary>
        /// Quyền
        /// </summary>
        public PermissionEnums Permission { get; set; }
    }
}
