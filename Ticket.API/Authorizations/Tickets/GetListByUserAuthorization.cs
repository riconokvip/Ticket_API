namespace Ticket.API.Authorizations
{
    public class GetListByUserTicketRequirement : IAuthorizationRequirement { }

    public class GetListByUserTicketAuthorization : AuthorizationHandler<GetListByUserTicketRequirement, string>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            GetListByUserTicketRequirement requirement,
            string fromUserId)
        {
            if (context.User.HasClaim(ClaimTypes.Role, ApplicationRoles.Admin))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            if (context.User.Identity.Name == fromUserId)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
