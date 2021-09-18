using BankApi.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Net.Http;
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

        public LoanService(IHttpClientFactory factory)
        {
            _factory = factory;
        }

        public async Task<bool> CheckCreditScoreAsync(string username)
        {
            var client = _factory.CreateClient("CreditScoreWebService");

            var result = await client.GetFromJsonAsync<CreditScoreResponse>($"/check/{username}");

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
