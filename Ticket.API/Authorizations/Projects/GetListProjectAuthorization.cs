namespace Ticket.API.Authorizations
{
    public class GetListProjectRequirement : IAuthorizationRequirement { }

    public class GetListProjectAuthorization : AuthorizationHandler<GetListProjectRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            GetListProjectRequirement requirement)
        {
            if (context.User.HasClaim(ClaimTypes.Role, ApplicationRoles.Admin))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            if (context.User.HasClaim(ClaimTypes.Role, PermissionEnums.ReadList.FastToString() + ResourceEnums.Project.FastToString()))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
