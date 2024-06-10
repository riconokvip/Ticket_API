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

    public class UserPermissionResponse
    {
        public string Id { get; set; }

        /// <summary>
        /// Tài nguyên
        /// </summary>
        public ResourceEnums Resource { get; set; }

        /// <summary>
        /// Quyền
        /// </summary>
        public PermissionEnums Permission { get; set; }

        /// <summary>
        /// Claim
        /// </summary>
        public string Claim { get; set; }

        /// <summary>
        /// Tên quyền
        /// </summary>
        public string PermissionName { get; set; }
    }
}
