using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace BankApi.Security
{
    public static class SecurityExtensions
    {
        public static IServiceCollection AddAccountSecurity(this IServiceCollection services)
        {
            services.AddAuthorization(configure => 
            {
                configure.AddPolicy("SameOwnerPolicy", c => c.AddRequirements(new SameOwnerRequirement()));
            });

            services.AddSingleton<IAuthorizationHandler, AccountAuthorizationHandler>();

            return services;
        }
    }
}
