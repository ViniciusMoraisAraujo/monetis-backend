using Microsoft.EntityFrameworkCore;
using Monetis.Domain.Entities;
using Monetis.Infrastructure.Persistence;
using Monetis.Infrastructure.Persistence.Configurations;

namespace Monetis.Infrastructure.Contexts;

public class MonetisDataContext(DbContextOptions<MonetisDataContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<Transaction> Transactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserMap());
        modelBuilder.ApplyConfiguration(new AccountMap());
        modelBuilder.ApplyConfiguration(new CategoryMap());
        modelBuilder.ApplyConfiguration(new SubscriptionMap());
        modelBuilder.ApplyConfiguration(new TransactionMap());
        SeedData.Seed(modelBuilder);
    }
}