using System;

namespace BankApi.Services
{
    public interface IDateProvider
    {
        DateTime Today { get; }
    }

    internal class DateProvider : IDateProvider
    {
        public DateTime Today => DateTime.Today;
    }
}