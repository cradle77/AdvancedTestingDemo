﻿namespace BankApi.ViewModels
{
    public class AccountBalance
    {
        public string AccountNumber { get; set; }

        public string Owner { get; set; }

        public decimal CurrentBalance { get; set; }
    }
}
