namespace Ticket.API.Authorizations
{
    public class UpdateProjectRequirement : IAuthorizationRequirement { }

    public class UpdateProjectAuthorization : AuthorizationHandler<UpdateProjectRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            UpdateProjectRequirement requirement)
        {
            if (context.User.HasClaim(ClaimTypes.Role, ApplicationRoles.Admin))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            if (context.User.HasClaim(ClaimTypes.Role, PermissionEnums.Update.FastToString() + ResourceEnums.Project.FastToString()))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
