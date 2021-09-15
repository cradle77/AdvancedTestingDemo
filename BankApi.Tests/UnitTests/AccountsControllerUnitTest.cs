using BankApi.Controllers;
using BankApi.Services;
using BankApi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace BankApi.Tests.UnitTests
{
    public class AccountsControllerUnitTest
    {
        [Fact]
        public async Task Accounts_WhenOwnerDifferent_Returns403()
        {
            var httpContext = new DefaultHttpContext() { User = new ClaimsPrincipal() };
            var balance = new AccountBalance() { Owner = "theOwner" };

            var accountService = new Mock<IAccountService>();
            accountService
                .Setup(x => x.GetBalanceAsync(It.IsAny<string>()))
                .ReturnsAsync(balance);

            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.Setup(x => x.HttpContext)
                .Returns(httpContext);

            var authorizationService = new Mock<IAuthorizationService>();
            authorizationService
                .Setup(x => x.AuthorizeAsync(httpContext.User, balance, "SameOwnerPolicy"))
                .ReturnsAsync(AuthorizationResult.Failed());

            var controller = new AccountsController(accountService.Object, authorizationService.Object, httpContextAccessor.Object);

            var result = await controller.GetBalanceAsync("123");

            Assert.IsType<ForbidResult>(result);
            authorizationService.Verify(x => x.AuthorizeAsync(httpContext.User, balance, "SameOwnerPolicy"), Times.Once);
        }
    }
}
