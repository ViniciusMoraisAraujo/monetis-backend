using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Monetis.Application.Abstractions.Security;
using Monetis.Domain.Entities;
using Monetis.Domain.Entities.Transactions;

namespace Monetis.Infrastructure.Persistence.Contexts;

public class MonetisDataContext(DbContextOptions<MonetisDataContext> options,
    IUserContextAccessor? userContextAccessor = null) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Card> Cards { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Expense> Expenses { get; set; }
    public DbSet<Income> Incomes { get; set; }
    public DbSet<Transfer> Transfers { get; set; }
    
    internal Guid CurrentUserId =>
        userContextAccessor?.IsResolved == true ? userContextAccessor.UserId : Guid.Empty;

    internal bool IsUserAuthenticated => 
        userContextAccessor?.IsResolved == true && userContextAccessor.UserId != Guid.Empty;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        SeedData.Seed(modelBuilder);

        modelBuilder.ApplyMultiTenantFilters(this);
        
        modelBuilder.Entity<Category>().HasQueryFilter(c => 
            c.UserId == null || (IsUserAuthenticated && c.UserId == CurrentUserId));
    }
}
