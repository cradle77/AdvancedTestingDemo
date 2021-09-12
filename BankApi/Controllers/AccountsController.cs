using BankApi.Services;
using BankApi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BankApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountsController : ControllerBase
    {
        private IAccountService _accounts;

        public AccountsController(IAccountService accounts)
        {
            _accounts = accounts ?? throw new System.ArgumentNullException(nameof(accounts));
        }

        [HttpGet("{accountNumber}/balance")]
        public async Task<IActionResult> GetBalanceAsync(string accountNumber)
        {
            var balance = await _accounts.GetBalanceAsync(accountNumber);

            return this.Ok(new AccountBalance
            {
                AccountNumber = accountNumber,
                CurrentBalance = balance
            });
        }

        [HttpGet("{accountNumber}/statement")]
        public async Task<IActionResult> GetStatementAsync(string accountNumber)
        {
            var statement = await _accounts.GetStatementAsync(accountNumber);

            return this.Ok(statement);
        }
    }
}
