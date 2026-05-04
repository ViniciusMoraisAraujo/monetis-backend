using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Monetis.Domain.Entities;
using Monetis.Domain.Interfaces;
using Monetis.Infrastructure.Persistence;
using Monetis.Infrastructure.Persistence.Configurations;

namespace Monetis.Infrastructure.Contexts;

public class MonetisDataContext(DbContextOptions<MonetisDataContext> options, UserContext userContext) : DbContext(options), IUnitOfWork
{
    public DbSet<User> Users { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Card> Cards { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<Transaction> Transactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new AccountConfiguration());
        modelBuilder.ApplyConfiguration(new CardConfiguration());
        modelBuilder.ApplyConfiguration(new CategoryConfiguration());
        modelBuilder.ApplyConfiguration(new SubscriptionConfiguration());
        modelBuilder.ApplyConfiguration(new TransactionConfiguration());
        modelBuilder.ApplyConfiguration(new ExpenseConfiguration());
        modelBuilder.ApplyConfiguration(new IncomeConfiguration());
        modelBuilder.ApplyConfiguration(new TransferConfiguration());
        SeedData.Seed(modelBuilder);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (!typeof(UserOwnedEntity).IsAssignableFrom(entityType.ClrType)) continue;
            
            var method = GetType()
                .GetMethod(nameof(ApplyUserFilter), BindingFlags.NonPublic | BindingFlags.Instance)!
                .MakeGenericMethod(entityType.ClrType);

            method.Invoke(this, [modelBuilder]);
            
            modelBuilder.Entity<Category>().HasQueryFilter(c =>
                c.UserId == null ||
                c.UserId == userContext.UserId);
        }
    }
    
    private void ApplyUserFilter<T>(ModelBuilder builder) where T : UserOwnedEntity
    {
        builder.Entity<T>().HasQueryFilter(e => e.UserId == userContext.UserId);
    }

    public async Task<bool> CommitAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<UserOwnedEntity>()
                     .Where(e => e.State == EntityState.Added))
        {
            entry.Entity.SetUser(userContext.UserId);
        }
        return await base.SaveChangesAsync(cancellationToken) > 0;
    }
}
