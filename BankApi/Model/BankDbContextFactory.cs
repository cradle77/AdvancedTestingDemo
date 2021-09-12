using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace BankApi.Model
{
    public class BankDbContextFactory : IDesignTimeDbContextFactory<BankDbContext>
    {
        public BankDbContext CreateDbContext(string[] args)
        {
            // Build config
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            // Get connection string
            var connectionString = config.GetConnectionString(nameof(BankDbContext));

            var optionsBuilder = new DbContextOptionsBuilder<BankDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new BankDbContext(optionsBuilder.Options);
        }
    }
}
