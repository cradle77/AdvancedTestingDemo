using BankApi.Services;
using BankApi.Tests.Utils;
using BankApi.ViewModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace BankApi.Tests.Web
{
    public class LoanControllerEndToEndTest : WebTestBase
    {
        [Fact]
        public async Task OpenNewLoan_WhenCheckPasses_ReturnsOk()
        {
            string username = "marco";

            int requestCount = 0;

            var creditCheckClient = this.BuildWebApplicationFactory(
                configBuilder: app =>
                 {
                     app.Map($"/check/{username}", builder =>
                     {
                         builder.Use((ctx, next) =>
                         {
                             requestCount++;

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

            var bankApiClient = this.BuildWebApplicationFactory(
                configServices: services =>
                {
                    services.AddLoanService();

                    // let's configure the HttpClientFactory to return the creditCheckClient
                    services.RemoveAll<IHttpClientFactory>();

                    services.AddSingleton<IHttpClientFactory>(factory.Object);

                    services.ConfigureAuthenticatedUser(username);
                })
                .CreateClient();

            var response = await bankApiClient.PostAsync("api/loan", JsonContent.Create(new object()));
            response.EnsureSuccessStatusCode();
            
            Assert.Equal(1, requestCount);
        }

        [Fact]
        public async Task OpenNewLoan_WhenCheckFails_ReturnsBadRequest()
        {
            string username = "marco";

            int requestCount = 0;

            var creditCheckClient = this.BuildWebApplicationFactory(
                configBuilder: app =>
                {
                    app.Map($"/check/{username}", builder =>
                    {
                        builder.Use((ctx, next) =>
                        {
                            requestCount++;

                            ctx.Response.StatusCode = StatusCodes.Status200OK;
                            ctx.Response.ContentType = "application/json";
                            var resp = JsonConvert.SerializeObject(new CreditScoreResponse()
                            {
                                Username = username,
                                Passed = false
                            });

                            return ctx.Response.WriteAsync(resp);
                        });
                    });
                })
                .CreateClient();

            var factory = new Mock<IHttpClientFactory>();
            factory.Setup(x => x.CreateClient("CreditScoreWebService")).Returns(creditCheckClient);

            var bankApiClient = this.BuildWebApplicationFactory(
                configServices: services =>
                {
                    services.AddLoanService();

                    // let's configure the HttpClientFactory to return the creditCheckClient
                    services.RemoveAll<IHttpClientFactory>();

                    services.AddSingleton<IHttpClientFactory>(factory.Object);

                    services.ConfigureAuthenticatedUser(username);
                })
                .CreateClient();

            var response = await bankApiClient.PostAsync("api/loan", JsonContent.Create(new object()));

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal(1, requestCount);
        }
    }
}
