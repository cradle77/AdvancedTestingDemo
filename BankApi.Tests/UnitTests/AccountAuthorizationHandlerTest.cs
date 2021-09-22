using BankApi.Security;
using BankApi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace BankApi.Tests.UnitTests
{
    public class AccountAuthorizationHandlerTest
    {
        [Fact]
        public async Task Account_WithSameName_IsAuthorized()
        {
            var requirements = new[] { new SameOwnerRequirement() };

            var principal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.Name, "theOwner")
            }));

            var balance = new AccountBalance() { Owner = "theOwner" };

            var context = new AuthorizationHandlerContext(requirements, principal, balance);

            var handler = new AccountAuthorizationHandler();

            await handler.HandleAsync(context);

            Assert.True(context.HasSucceeded);
        }

        [Fact]
        public async Task Account_WithDifferentName_IsForbidden()
        {
            var requirements = new[] { new SameOwnerRequirement() };

            var principal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.Name, "theOwner")
            }));

            var balance = new AccountBalance() { Owner = "differentName" };

            var context = new AuthorizationHandlerContext(requirements, principal, balance);

            var handler = new AccountAuthorizationHandler();

            await handler.HandleAsync(context);

            Assert.False(context.HasSucceeded);
        }
    }
}
