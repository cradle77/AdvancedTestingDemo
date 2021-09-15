using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Xunit;

namespace BankApi.Tests.Web
{
    public class ValuesControllerTest
    {
        [Fact]
        public async Task ValuesController_WhenCalled_Returns200()
        {
            var factory = new WebApplicationFactory<DummyWebApp.Startup>()
                .WithWebHostBuilder(config => 
                {
                    config.ConfigureServices(services => 
                    {
                        var assembly = typeof(BankApi.Startup).Assembly;

                        services.AddControllers().AddApplicationPart(assembly);
                    });
                });

            var client = factory.CreateClient();

            var response = await client.GetAsync("/api/values");

            response.EnsureSuccessStatusCode();
        }
    }
}
