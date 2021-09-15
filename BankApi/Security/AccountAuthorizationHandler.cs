using BankApi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace BankApi.Security
{
    public class AccountAuthorizationHandler : AuthorizationHandler<SameOwnerRequirement, AccountBalance>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, 
            SameOwnerRequirement requirement, AccountBalance resource)
        {
            if (context.User.Identity?.Name == resource.Owner)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }

    public class SameOwnerRequirement : IAuthorizationRequirement { }
}
