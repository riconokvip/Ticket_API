namespace Ticket.API.Authorizations
{
    public class CreateProjectRequirement : IAuthorizationRequirement { }

    public class CreateProjectAuthorization : AuthorizationHandler<CreateProjectRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            CreateProjectRequirement requirement)
        {
            if (context.User.HasClaim(ClaimTypes.Role, ApplicationRoles.Admin))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            if (context.User.HasClaim(ClaimTypes.Role, PermissionEnums.Create.FastToString() + ResourceEnums.Project.FastToString()))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
