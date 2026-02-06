using Microsoft.AspNetCore.Authorization;

namespace BackendAuthAssignment.Authorization;

public class RegistrationCompleteHandler : AuthorizationHandler<RegistrationCompleteRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RegistrationCompleteRequirement requirement)
    {
        var reg = context.User.FindFirst("reg")?.Value;
        if (reg == "1")
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
