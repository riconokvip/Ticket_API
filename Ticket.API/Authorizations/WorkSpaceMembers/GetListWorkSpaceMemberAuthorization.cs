namespace Ticket.API.Authorizations
{
    public class GetListWorkSpaceMemberRequirement : IAuthorizationRequirement { }

    public class GetListWorkSpaceMemberAuthorization : AuthorizationHandler<GetListWorkSpaceMemberRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            GetListWorkSpaceMemberRequirement requirement)
        {
            if (context.User.HasClaim(ClaimTypes.Role, ApplicationRoles.Admin))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            if (context.User.HasClaim(ClaimTypes.Role, PermissionEnums.ReadList.FastToString() + ResourceEnums.WorkSpaceMember.FastToString()))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
