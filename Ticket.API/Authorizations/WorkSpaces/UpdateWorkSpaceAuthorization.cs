namespace Ticket.API.Authorizations
{
    public class UpdateWorkSpaceRequirement : IAuthorizationRequirement { }

    public class UpdateWorkSpaceAuthorization : AuthorizationHandler<UpdateWorkSpaceRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            UpdateWorkSpaceRequirement requirement)
        {
            if (context.User.HasClaim(ClaimTypes.Role, ApplicationRoles.Admin))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            if (context.User.HasClaim(ClaimTypes.Role, PermissionEnums.Update.FastToString() + ResourceEnums.WorkSpace.FastToString()))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
