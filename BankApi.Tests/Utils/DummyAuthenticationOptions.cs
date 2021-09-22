using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace BankApi.Tests.Utils
{
    public class DummyAuthenticationOptions : AuthenticationSchemeOptions
    {
        public IEnumerable<Claim> Claims { get; set; }

        public string TokenValue { get; set; }

        public AuthenticateResult GetResult()
        {
            if (this.Claims == null || !this.Claims.Any())
            {
                // anonymous
                return AuthenticateResult.NoResult();
            }

            var identity = new ClaimsIdentity(this.Claims, "dummy");
            var principal = new ClaimsPrincipal(identity);

            var properties = new AuthenticationProperties();
            properties.SetString(".Token.access_token", this.TokenValue);

            var ticket = new AuthenticationTicket(principal, properties, "dummy");

            return AuthenticateResult.Success(ticket);
        }
    }
}