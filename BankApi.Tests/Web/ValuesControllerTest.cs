using System.Threading.Tasks;
using Xunit;

namespace BankApi.Tests.Web
{
    public class ValuesControllerTest : WebTestBase
    {
        [Fact]
        public async Task ValuesController_WhenCalled_Returns200()
        {
            var factory = this.BuildWebApplicationFactory();

            var client = factory.CreateClient();

            var response = await client.GetAsync("/api/values");

            response.EnsureSuccessStatusCode();
        }
    }
}
