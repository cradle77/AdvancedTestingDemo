using BankApi.Services;
using BankApi.ViewModels;
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

        // to call it, get a token via:
        // az account get-access-token --resource api://advancedtestdemo/bankapi

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

        [HttpPost("{accountNumber}/transactions/deposit")]
        public async Task<IActionResult> DepositAsync(string accountNumber, [FromBody] TransactionViewModel transationDetails)
        {
            await _accounts.DepositAsync(accountNumber, transationDetails.Amount, transationDetails.Description);

            return new StatusCodeResult(StatusCodes.Status204NoContent);
        }

        [HttpPost("{accountNumber}/transactions/withdrawal")]
        public async Task<IActionResult> WithdrawAsync(string accountNumber, [FromBody] TransactionViewModel transationDetails)
        {
            var user = _contextAccessor.HttpContext.User;

            var result = await _accounts.GetBalanceAsync(accountNumber);

            var authorizationResult = await _authorization
                .AuthorizeAsync(user, result, "SameOwnerPolicy");

            if (authorizationResult.Succeeded) 
            {
                await _accounts.WithdrawAsync(accountNumber, transationDetails.Amount, transationDetails.Description);

                return new StatusCodeResult(StatusCodes.Status204NoContent);
            }
            else
                return new ForbidResult();
        }
    }
}
