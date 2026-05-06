using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Monetis.Infrastructure.Contexts;

public class MonetisDataContextFactory : IDesignTimeDbContextFactory<MonetisDataContext>
{
    public MonetisDataContext CreateDbContext(string[] args)
    {
        var basePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "Monetis.API");

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddJsonFile("appsettings.json", optional: true)
            .Build();

        var builder = new DbContextOptionsBuilder<MonetisDataContext>();

        var connectionString = configuration.GetConnectionString("MonetisConnection");
        builder.UseSqlServer(connectionString);
        
        return new MonetisDataContext(builder.Options, null);
    }
}