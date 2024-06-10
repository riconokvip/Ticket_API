namespace Ticket.API.Authorizations.ProjectMembers
{
    public class RemoveProjectMemberRequirement : IAuthorizationRequirement { }

    public class RemoveProjectMemberAuthorization : AuthorizationHandler<RemoveProjectMemberRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            RemoveProjectMemberRequirement requirement)
        {
            if (context.User.HasClaim(ClaimTypes.Role, ApplicationRoles.Admin))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            if (context.User.HasClaim(ClaimTypes.Role, PermissionEnums.Delete.FastToString() + ResourceEnums.ProjectMember.FastToString()))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
