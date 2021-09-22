using BankApi.Security;
using BankApi.Services;
using BankApi.Tests.Utils;
using BankApi.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace BankApi.Tests.Web
{
    public class AccountControllerTest : WebTestBase
    {
        private Mock<IAccountService> _accountService;

        protected override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);

            services.AddAccountSecurity();

            _accountService = new Mock<IAccountService>();

            services.AddSingleton<IAccountService>(_accountService.Object);
        }

        [Fact]
        public async Task Accounts_WhenUserIsCorrect_ReturnsBalance()
        {
            var client = this.BuildWebApplicationFactory(
                configServices: services =>
                {
                    services.ConfigureAuthenticatedUser(userName: "Marco");
                })
                .CreateClient();

            _accountService.Setup(x => x.GetBalanceAsync(It.IsAny<string>()))
                .ReturnsAsync(new AccountBalance()
                {
                    Owner = "Marco",
                    CurrentBalance = 100
                });

            var result = await client.GetFromJsonAsync<AccountBalance>("/api/accounts/AC123/balance");

            Assert.Equal("Marco", result.Owner);
            Assert.Equal(100, result.CurrentBalance);
        }

        [Fact]
        public async Task Accounts_WhenUserIsDifferent_Returns403()
        {
            var client = this.BuildWebApplicationFactory(
                configServices: services =>
                {
                    services.ConfigureAuthenticatedUser(userName: "Marco");
                })
                .CreateClient();

            _accountService.Setup(x => x.GetBalanceAsync(It.IsAny<string>()))
                .ReturnsAsync(new AccountBalance()
                {
                    Owner = "Jason"
                });

            var response = await client.GetAsync("/api/accounts/AC123/balance");

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task Accounts_WhenUserNotAuthenticated_Returns401()
        {
            var client = this.BuildWebApplicationFactory(
                configServices: services =>
                {
                    services.ConfigureAnonymousUser();
                })
                .CreateClient();

            var response = await client.GetAsync("/api/accounts/AC123/balance");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
