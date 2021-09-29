﻿using BankApi.Model;
using BankApi.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using Xunit;

namespace BankApi.Tests.Features
{
    [Binding]
    public class AccountOperationsSteps
    {
        // try to call the service
        /*
           $token = az account get-access-token --resource api://advancedtestdemo/bankapi | ConvertFrom-Json
           Invoke-WebRequest -Uri https://localhost:5001/api/Accounts/AC0001/balance `
              -Headers @{ "Authorization" = "Bearer $($token.accessToken)"}
        */

        private AccountOperationsContext _context;

        private AccountService _service;
        private Mock<IDateProvider> _dateProvider;

        public AccountOperationsSteps(AccountOperationsContext context)
        {
            _context = context;

            var options = new DbContextOptionsBuilder<BankDbContext>()
                .UseInMemoryDatabase("AccountOperations")
                .Options;

            _context.DbContext = new BankDbContext(options);

            _context.DbContext.Database.EnsureDeleted();

            _dateProvider = new Mock<IDateProvider>();

            _service = new AccountService(_context.DbContext, _dateProvider.Object);
        }

        [Given(@"an account '(.*)' exists with a balance of (.*)")]
        public async Task GivenAnAccountExistsWithABalanceOf(string accountNumber, int initialBalance)
        {
            _context.AccountNumber = accountNumber;

            var account = new Account()
            {
                Balance = initialBalance,
                Number = accountNumber
            };

            _context.DbContext.Accounts.Add(account);

            await _context.DbContext.SaveChangesAsync();
        }

        [Given(@"I've made a deposit of (.*)")]
        public async Task GivenIVeMadeADepositOf(int amount)
        {
            await _service.DepositAsync(_context.AccountNumber, amount, "a deposit");
        }

        [Given(@"I've made a withdrawal of (.*)")]
        public async Task GivenIVeMadeAWithdrawalOf(int amount)
        {
            await _service.WithdrawAsync(_context.AccountNumber, amount, "a withdrawal");
        }

        [Given(@"I've made the following transactions")]
        public async Task GivenIVeMadeTheFollowingTransactions(Table table)
        {
            var transactions = table.CreateSet<(TransactionType type, decimal amount)>();

            foreach (var transaction in transactions)
            {
                if (transaction.type == TransactionType.Deposit)
                {
                    await _service.DepositAsync(_context.AccountNumber, transaction.amount, "a deposit");
                }
                else
                {
                    await _service.WithdrawAsync(_context.AccountNumber, transaction.amount, "a withdrawal");

                }
            }
        }

        [When(@"I check the account balance")]
        public async Task WhenICheckTheAccountBalance()
        {
            var result = await _service.GetBalanceAsync(_context.AccountNumber);

            _context.CurrentBalance = result.CurrentBalance;
        }


        [When(@"I get the account statement")]
        public async Task WhenIGetTheAccountStatement()
        {
            var transactions = await _service.GetStatementAsync(_context.AccountNumber);

            _context.Transactions = transactions;
        }

        [Then(@"the balance should be (.*)")]
        public void ThenTheBalanceShouldBe(int expectedBalance)
        {
            Assert.Equal(expectedBalance, _context.CurrentBalance);
        }

        [Then(@"I get the following transactions back")]
        public void ThenIGetTheFollowingTransactionsBack(Table table)
        {
            table.CompareToSet(_context.Transactions);
        }

    }
}