using System.Collections.Generic;

namespace BankApi.Model
{
    public class Account
    {
        public int Id { get; set; }

        public string Number { get; set; }
        
        public string Owner { get; set; }

        public decimal Balance { get; set; }

        public virtual IList<MoneyTransaction> Transactions { get; set; }

        public Account()
        {
            this.Transactions = new List<MoneyTransaction>();
        }
    }
}
