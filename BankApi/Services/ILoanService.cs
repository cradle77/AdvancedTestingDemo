using BankApi.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace BankApi.Services
{
    public interface ILoanService
    {
        public Task<bool> CheckCreditScoreAsync(string username);
    }

    internal class LoanService : ILoanService
    {
        private IHttpClientFactory _factory;
        private IHttpContextAccessor _contextAccessor;

        public LoanService(IHttpClientFactory factory, IHttpContextAccessor contextAccessor)
        {
            _factory = factory;
            _contextAccessor = contextAccessor;
        }

        public async Task<bool> CheckCreditScoreAsync(string username)
        {
            var accessToken = await _contextAccessor.HttpContext.GetTokenAsync("access_token");

            var client = _factory.CreateClient("CreditScoreWebService");

            // assumption: the remote service is protected with the same Azure AD Application
            var request = new HttpRequestMessage(HttpMethod.Get, "/check");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<CreditScoreResponse>();

            return result.Passed;
        }
    }

    internal static class LoanExtensions
    {
        public static IServiceCollection AddLoanService(this IServiceCollection services)
        {
            services.AddHttpClient("CreditScoreWebService", client => 
            {
                client.BaseAddress = new Uri("https://mycreditcheck.com");
            });

            services.TryAddTransient<ILoanService, LoanService>();

            return services;
        }
    }
        
}
