namespace Ticket.API.Authorizations
{
    public class AddWorkSpaceMemberRequirement : IAuthorizationRequirement { }

    public class AddWorkSpaceMemberAuthorization : AuthorizationHandler<AddWorkSpaceMemberRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            AddWorkSpaceMemberRequirement requirement)
        {
            if (context.User.HasClaim(ClaimTypes.Role, ApplicationRoles.Admin))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            if (context.User.HasClaim(ClaimTypes.Role, PermissionEnums.Create.FastToString() + ResourceEnums.WorkSpaceMember.FastToString()))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
