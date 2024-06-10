namespace Ticket.API.Authorizations
{
    public class GetListTicketRequirement : IAuthorizationRequirement { }

    public class GetListTicketAuthorization : AuthorizationHandler<GetListTicketRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            GetListTicketRequirement requirement)
        {
            if (context.User.HasClaim(ClaimTypes.Role, ApplicationRoles.Admin))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            if (context.User.HasClaim(ClaimTypes.Role, PermissionEnums.ReadList.FastToString() + ResourceEnums.Ticket.FastToString()))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
