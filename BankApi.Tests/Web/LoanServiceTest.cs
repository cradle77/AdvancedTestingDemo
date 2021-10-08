using BankApi.Services;
using BankApi.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace BankApi.Tests.Web
{
    public class LoanServiceTest : WebTestBase
    {
        [Fact]
        public async Task CheckCreditScoreAsync_WhenTestPassed_ReturnsTrue()
        {
            string username = "marco";
            string token = "testToken";
            string receivedHeader = null;

            var creditCheckClient = this.BuildWebApplicationFactory(
                configBuilder: app =>
                {
                    app.Map($"/check", builder =>
                    {
                        builder.Use((ctx, next) =>
                        {
                            receivedHeader = ctx.Request.Headers["Authorization"];

                            ctx.Response.StatusCode = StatusCodes.Status200OK;
                            ctx.Response.ContentType = "application/json";
                            var resp = JsonConvert.SerializeObject(new CreditScoreResponse()
                            {
                                Username = username,
                                Passed = true
                            });

                            return ctx.Response.WriteAsync(resp);
                        });
                    });
                })
                .CreateClient();

            var factory = new Mock<IHttpClientFactory>();
            factory.Setup(x => x.CreateClient("CreditScoreWebService")).Returns(creditCheckClient);

            var httpContext = this.BuildHttpContext(token);
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

            var service = new LoanService(factory.Object, contextAccessor.Object);

            var result = await service.CheckCreditScoreAsync(username);

            Assert.True(result);
            Assert.Equal($"Bearer {token}", receivedHeader);
        }

        private HttpContext BuildHttpContext(string tokenValue)
        {
            var authenticationService = new Mock<IAuthenticationService>();

            var authenticateResult = new TestAuthenticateResult();
            authenticateResult.Properties.Items.Add(".Token.access_token", tokenValue);

            authenticationService.Setup(x => x.AuthenticateAsync(It.IsAny<HttpContext>(), It.IsAny<string>()))
                .ReturnsAsync(authenticateResult);

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IAuthenticationService>(authenticationService.Object);

            var result = new DefaultHttpContext();
            result.RequestServices = serviceCollection.BuildServiceProvider();

            return result;
        }

        private class TestAuthenticateResult : AuthenticateResult
        {
            public TestAuthenticateResult()
            {
                this.Properties = new AuthenticationProperties();
            }
        }
    }
}
