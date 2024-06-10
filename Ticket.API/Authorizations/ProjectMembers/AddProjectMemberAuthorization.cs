namespace Ticket.API.Authorizations
{
    public class AddProjectMemberRequirement : IAuthorizationRequirement { }

    public class AddProjectMemberAuthorization : AuthorizationHandler<AddProjectMemberRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            AddProjectMemberRequirement requirement)
        {
            if (context.User.HasClaim(ClaimTypes.Role, ApplicationRoles.Admin))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            if (context.User.HasClaim(ClaimTypes.Role, PermissionEnums.Create.FastToString() + ResourceEnums.ProjectMember.FastToString()))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
