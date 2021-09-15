using BankApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BankApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountsController
    {
        private IAccountService _accounts;
        private IAuthorizationService _authorization;
        private IHttpContextAccessor _contextAccessor;

        public AccountsController(IAccountService accounts, IAuthorizationService authorization, IHttpContextAccessor contextAccessor)
        {
            _accounts = accounts ?? throw new System.ArgumentNullException(nameof(accounts));
            _authorization = authorization ?? throw new System.ArgumentNullException(nameof(authorization));
            _contextAccessor = contextAccessor ?? throw new System.ArgumentNullException(nameof(contextAccessor));
        }

        [HttpGet("{accountNumber}/balance")]
        public async Task<IActionResult> GetBalanceAsync(string accountNumber)
        {
            var result = await _accounts.GetBalanceAsync(accountNumber);

            var user = _contextAccessor.HttpContext.User;

            var authorizationResult = await _authorization
                .AuthorizeAsync(user, result, "SameOwnerPolicy");

            if (authorizationResult.Succeeded)
                return new ObjectResult(result) { StatusCode = StatusCodes.Status200OK };
            else
                return new ForbidResult();
        }

        [HttpGet("{accountNumber}/statement")]
        public async Task<IActionResult> GetStatementAsync(string accountNumber)
        {
            var statement = await _accounts.GetStatementAsync(accountNumber);

            return new ObjectResult(statement) { StatusCode = StatusCodes.Status200OK };
        }
    }
}
