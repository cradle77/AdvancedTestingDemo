using BankApi.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankApi.Services
{
    internal class AccountService : IAccountService
    {
        private readonly BankDbContext _context;
        private readonly IDateProvider _dateProvider;

        public AccountService(BankDbContext context, IDateProvider dateProvider)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dateProvider = dateProvider ?? throw new ArgumentNullException(nameof(dateProvider));
        }

        public async Task<decimal> GetBalanceAsync(string accountNumber)
        {
            var account = await _context.Accounts
                .SingleAsync(x => x.Number == accountNumber);

            return account.Balance;
        }

        public async Task<IList<MoneyTransaction>> GetStatementAsync(string accountNumber)
        {
            var account = await _context.Accounts
                .SingleAsync(x => x.Number == accountNumber);

            return account.Transactions;
        }

        public async Task DepositAsync(string accountNumber, decimal amount, string description)
        {
            if (amount < 0)
                throw new ArgumentException(nameof(amount));

            var account = await _context.Accounts
                .SingleAsync(x => x.Number == accountNumber);

            account.Transactions.Add(new MoneyTransaction() 
            {
                Amount = amount,
                Description = description,
                Type = TransactionType.Deposit,
                Date = _dateProvider.Today,
                Balance = account.Balance + amount
            });

            account.Balance += amount;

            await _context.SaveChangesAsync();
        }

        public async Task WithdrawAsync(string accountNumber, decimal amount, string description)
        {
            if (amount < 0)
                throw new ArgumentException(nameof(amount));

            var account = await _context.Accounts
                .SingleAsync(x => x.Number == accountNumber);

            if (account.Balance < amount)
                throw new InvalidOperationException("Insufficient funds");

            account.Transactions.Add(new MoneyTransaction()
            {
                Amount = amount,
                Description = description,
                Type = TransactionType.Withdrawal,
                Date = _dateProvider.Today,
                Balance = account.Balance - amount
            });

            account.Balance -= amount;

            await _context.SaveChangesAsync();
        }
    }
}
