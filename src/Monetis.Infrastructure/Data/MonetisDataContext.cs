using Microsoft.EntityFrameworkCore;
using Monetis.Domain.Entities;
using Monetis.Infrastructure.Mappings;

namespace Monetis.Infrastructure.Data;

public class MonetisDataContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<Transaction> Transactions { get; set; }

    public MonetisDataContext(DbContextOptions<MonetisDataContext> options)  : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserMap());
        modelBuilder.ApplyConfiguration(new AccountMap());
        modelBuilder.ApplyConfiguration(new CategoryMap());
        modelBuilder.ApplyConfiguration(new SubscriptionMap());
        modelBuilder.ApplyConfiguration(new TransactionMap());
    }
}