using BankApi.Services;
using BankApi.ViewModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
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

            var creditCheckClient = this.BuildWebApplicationFactory(
                configBuilder: app =>
                {
                    app.Map($"/check/{username}", builder =>
                    {
                        builder.Use((ctx, next) =>
                        {
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

            var service = new LoanService(factory.Object);

            var result = await service.CheckCreditScoreAsync(username);

            Assert.True(result);
        }
    }
}
