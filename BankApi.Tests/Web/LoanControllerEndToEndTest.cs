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
using System.Net.Http.Headers;
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
            string token = "testToken";
            string receivedHeader = null;

            int requestCount = 0;

            var creditCheckClient = this.BuildWebApplicationFactory(
                configBuilder: app =>
                {
                    app.Map("/check", builder =>
                    {
                        builder.Use((ctx, next) =>
                        {
                            receivedHeader = ctx.Request.Headers["Authorization"];
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

            var request = new HttpRequestMessage(HttpMethod.Post, "api/loan");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Content = JsonContent.Create(new object());

            var response = await bankApiClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            Assert.Equal(1, requestCount);
            Assert.Equal($"Bearer {token}", receivedHeader);
        }

        [Fact]
        public async Task OpenNewLoan_WhenCheckFails_ReturnsBadRequest()
        {
            string username = "marco";
            string token = "testToken";
            string receivedHeader = null;

            int requestCount = 0;

            var creditCheckClient = this.BuildWebApplicationFactory(
                configBuilder: app =>
                {
                    app.Map("/check", builder =>
                    {
                        builder.Use((ctx, next) =>
                        {
                            receivedHeader = ctx.Request.Headers["Authorization"];
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

            var request = new HttpRequestMessage(HttpMethod.Post, "api/loan");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Content = JsonContent.Create(new object());

            var response = await bankApiClient.SendAsync(request);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal($"Bearer {token}", receivedHeader);
            Assert.Equal(1, requestCount);
        }
    }
}