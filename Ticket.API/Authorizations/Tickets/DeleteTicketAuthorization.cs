namespace Ticket.API.Authorizations
{
    public class DeleteTicketRequirement : IAuthorizationRequirement { }

    public class DeleteTicketAuthorization : AuthorizationHandler<DeleteTicketRequirement, string>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            DeleteTicketRequirement requirement,
            string fromUserId)
        {
            if (context.User.HasClaim(ClaimTypes.Role, ApplicationRoles.Admin))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            if (context.User.Identity.Name == fromUserId || context.User.HasClaim(ClaimTypes.Role, PermissionEnums.Delete.FastToString() + ResourceEnums.Ticket.FastToString()))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
