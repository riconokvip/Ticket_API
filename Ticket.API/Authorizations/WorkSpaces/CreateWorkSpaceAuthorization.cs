namespace Ticket.API.Authorizations
{
    public class CreateWorkSpaceRequirement : IAuthorizationRequirement { }

    public class CreateWorkSpaceAuthorization : AuthorizationHandler<CreateWorkSpaceRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            CreateWorkSpaceRequirement requirement)
        {
            if (context.User.HasClaim(ClaimTypes.Role, ApplicationRoles.Admin))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            if (context.User.HasClaim(ClaimTypes.Role, PermissionEnums.Create.FastToString() + ResourceEnums.WorkSpace.FastToString()))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
