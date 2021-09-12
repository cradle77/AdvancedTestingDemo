using System;

namespace BankApi.Model
{
    public class MoneyTransaction
    {
        public int Id { get; set; }

        public int AccountId { get; set; }

        public DateTime Date { get; set; }

        public TransactionType Type { get; set; }

        public string Description { get; set; }

        public decimal Amount { get; set; }
        
        public decimal Balance { get; set; }
    }
}