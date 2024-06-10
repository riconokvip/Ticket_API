namespace Ticket.API.Authorizations
{
    public class UpdateTicketRequirement : IAuthorizationRequirement { }

    public class UpdateTicketAuthorization : AuthorizationHandler<UpdateTicketRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            UpdateTicketRequirement requirement)
        {
            if (context.User.HasClaim(ClaimTypes.Role, ApplicationRoles.Admin))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            if (context.User.HasClaim(ClaimTypes.Role, PermissionEnums.Update.FastToString() + ResourceEnums.Ticket.FastToString()))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
