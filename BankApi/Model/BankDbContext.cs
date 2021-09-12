using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BankApi.Model
{
    public class BankDbContext : DbContext
    {
        public BankDbContext(DbContextOptions<BankDbContext> options) : base(options)
        {
        }

        public DbSet<Account> Accounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Account>()
                .HasIndex(x => x.Number)
                .IsUnique();

            modelBuilder.Entity<MoneyTransaction>()
                .Property(x => x.Type)
                .HasConversion(new EnumToStringConverter<TransactionType>());
        }
    }
}
