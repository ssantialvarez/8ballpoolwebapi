using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace _8BallPool.Data
{
    public class _8BallPoolContextFactory : IDesignTimeDbContextFactory<_8BallPoolContext>
    {
        public _8BallPoolContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Development.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<_8BallPoolContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            optionsBuilder.UseNpgsql(connectionString);

            return new _8BallPoolContext(optionsBuilder.Options);
        }
    }
}