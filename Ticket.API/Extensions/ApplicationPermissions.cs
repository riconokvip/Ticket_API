namespace Ticket.API.Extensions
{
    public class ApplicationPermissions
    {
        /* --- Ticket policies --- */
        public const string GetListTicket = "GetListTicket";
        public const string GetListTicketByUser = "GetListTicketByUser";
        public const string CreateTicket = "CreateTicket";
        public const string UpdateTicket = "UpdateTicket";
        public const string DeleteTicket = "DeleteTicket";

        /* --- WorkSpace policies --- */
        public const string GetListWorkSpace = "GetListWorkSpace";
        public const string CreateWorkSpace = "CreateWorkSpace";
        public const string UpdateWorkSpace = "UpdateWorkSpace";
        public const string DeleteWorkSpace = "DeleteWorkSpace";

        /* --- Project policies --- */
        public const string GetListProject = "GetListProject";
        public const string CreateProject = "CreateProject";
        public const string UpdateProject = "UpdateProject";
        public const string DeleteProject = "DeleteProject";

        /* --- WorkSpaceMember policies --- */
        public const string GetListWorkSpaceMember = "GetListWorkSpaceMember";
        public const string AddWorkSpaceMember = "AddWorkSpaceMember";
        public const string RemoveWorkSpaceMember = "RemoveWorkSpaceMember";

        /* --- ProjectMember policies --- */
        public const string GetListProjectMember = "GetListProjectMember";
        public const string AddProjectMember = "AddProjectMember";
        public const string RemoveProjectMember = "RemoveProjectMember";
    }
}
