using BankApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApi.Tests.Features
{
    public class AccountOperationsContext
    {
        public BankDbContext DbContext { get; internal set; }
        public string AccountNumber { get; internal set; }
        public decimal CurrentBalance { get; internal set; }
        public IList<MoneyTransaction> Transactions { get; internal set; }
    }
}