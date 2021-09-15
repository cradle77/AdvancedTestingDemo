using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace BankApi.Tests.Utils
{
    public class DummyAuthenticationHandler : AuthenticationHandler<DummyAuthenticationOptions>
    {
        public DummyAuthenticationHandler(
            IOptionsMonitor<DummyAuthenticationOptions> options,
            ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected async override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            this.Options.TokenValue = this.Context.Request.Headers[HeaderNames.Authorization]
                .ToString()
                .Replace("Bearer ", string.Empty);

            return this.Options.GetResult();
        }
    }
}
