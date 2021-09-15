using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace BankApi.Tests.Web
{
    public class WebTestBase
    {
        public WebApplicationFactory<DummyWebApp.Startup> BuildWebApplicationFactory(Action<IServiceCollection> configServices = null, Action<IApplicationBuilder> configBuilder = null)
        {
            return new WebApplicationFactory<DummyWebApp.Startup>()
                .WithWebHostBuilder(config =>
                {
                    config.ConfigureServices(services => 
                    {
                        this.ConfigureServices(services);

                        if (configServices != null)
                            configServices(services);
                    });

                    config.Configure(app =>
                    {
                        this.Configure(app);

                        if (configBuilder != null)
                            configBuilder(app);
                    });
                });
        }

        protected virtual void ConfigureServices(IServiceCollection services)
        {
            var assembly = typeof(BankApi.Startup).Assembly;

            services.AddControllers().AddApplicationPart(assembly);

            services.AddHttpContextAccessor();
        }

        protected virtual void Configure(IApplicationBuilder app)
        {
            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
