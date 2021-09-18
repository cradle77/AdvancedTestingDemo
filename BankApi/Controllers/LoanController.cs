using BankApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BankApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LoanController : ControllerBase
    {
        private ILoanService _loans;

        public LoanController(ILoanService loans)
        {
            _loans = loans;
        }

        [HttpPost]
        public async Task<IActionResult> OpenNewLoan()
        {
            var canProceed = await _loans.CheckCreditScoreAsync(this.User.Identity.Name);

            if (!canProceed)
            {
                return this.BadRequest();
            }

            return this.Ok();
        }
    }
}
