namespace Ticket.API.Authorizations
{
    public class GetListWorkSpaceRequirement : IAuthorizationRequirement { }

    public class GetListWorkSpaceAuthorization : AuthorizationHandler<GetListWorkSpaceRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            GetListWorkSpaceRequirement requirement)
        {
            if (context.User.HasClaim(ClaimTypes.Role, ApplicationRoles.Admin))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            if (context.User.HasClaim(ClaimTypes.Role, PermissionEnums.ReadList.FastToString() + ResourceEnums.WorkSpace.FastToString()))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
