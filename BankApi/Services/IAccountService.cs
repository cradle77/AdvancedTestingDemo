using BankApi.Model;
using BankApi.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankApi.Services
{
    public interface IAccountService
    {
        Task<AccountBalance> GetBalanceAsync(string accountNumber);

        Task DepositAsync(string accountNumber, decimal amount, string description);
        
        Task WithdrawAsync(string accountNumber, decimal amount, string description);

        Task<IList<MoneyTransaction>> GetStatementAsync(string accountNumber);
    }
}
