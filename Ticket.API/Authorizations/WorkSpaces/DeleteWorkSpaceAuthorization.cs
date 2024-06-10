namespace Ticket.API.Authorizations
{
    public class DeleteWorkSpaceRequirement : IAuthorizationRequirement { }

    public class DeleteWorkSpaceAuthorization : AuthorizationHandler<DeleteWorkSpaceRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            DeleteWorkSpaceRequirement requirement)
        {
            if (context.User.HasClaim(ClaimTypes.Role, ApplicationRoles.Admin))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            if (context.User.HasClaim(ClaimTypes.Role, PermissionEnums.Delete.FastToString() + ResourceEnums.WorkSpace.FastToString()))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
