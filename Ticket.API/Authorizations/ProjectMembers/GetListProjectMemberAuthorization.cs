namespace Ticket.API.Authorizations.ProjectMembers
{
    public class GetListProjectMemberRequirement : IAuthorizationRequirement { }

    public class GetListProjectMemberAuthorization : AuthorizationHandler<GetListProjectMemberRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            GetListProjectMemberRequirement requirement)
        {
            if (context.User.HasClaim(ClaimTypes.Role, ApplicationRoles.Admin))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            if (context.User.HasClaim(ClaimTypes.Role, PermissionEnums.ReadList.FastToString() + ResourceEnums.ProjectMember.FastToString()))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
