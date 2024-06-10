namespace Ticket.API.Authorizations
{
    public class CreateTicketRequirement : IAuthorizationRequirement { }

    public class CreateTicketAuthorization : AuthorizationHandler<CreateTicketRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            CreateTicketRequirement requirement)
        {
            if (context.User.HasClaim(ClaimTypes.Role, ApplicationRoles.Admin))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            if (context.User.HasClaim(ClaimTypes.Role, PermissionEnums.Create.FastToString() + ResourceEnums.Ticket.FastToString()))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
