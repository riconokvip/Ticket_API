namespace Ticket.API.Authorizations
{
    public class RemoveWorkSpaceMemberRequirement : IAuthorizationRequirement { }

    public class RemoveWorkSpaceMemberAuthorization : AuthorizationHandler<RemoveWorkSpaceMemberRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            RemoveWorkSpaceMemberRequirement requirement)
        {
            if (context.User.HasClaim(ClaimTypes.Role, ApplicationRoles.Admin))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            if (context.User.HasClaim(ClaimTypes.Role, PermissionEnums.Delete.FastToString() + ResourceEnums.WorkSpaceMember.FastToString()))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
