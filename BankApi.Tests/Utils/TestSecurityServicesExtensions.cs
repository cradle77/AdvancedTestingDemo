using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Security.Claims;

namespace BankApi.Tests.Utils
{
    static class TestSecurityServicesExtensions
    {
        public static void ConfigureAnonymousUser(this IServiceCollection services)
        {
            services.AddAuthentication("dummy")
                .AddScheme<DummyAuthenticationOptions, DummyAuthenticationHandler>("dummy", configureOptions => { });
        }

        public static void ConfigureAuthenticatedUser(this IServiceCollection services, string userName = "someUser", string tokenValue = null)
        {
            services.AddAuthentication("dummy")
                .AddScheme<DummyAuthenticationOptions, DummyAuthenticationHandler>("dummy", configureOptions =>
                {
                    configureOptions.Claims = new List<Claim>()
                    {
                        new Claim("name", userName),
                        new Claim(ClaimTypes.Email, "dummy")
                    };

                    configureOptions.TokenValue = tokenValue;
                });
        }

        public static void ConfigureDummyPolicy(this IServiceCollection services, string policyName)
        {
            services.AddAuthorization(config =>
            {
                config.AddPolicy(policyName, p => p.RequireAuthenticatedUser());
            });
        }
    }
}
