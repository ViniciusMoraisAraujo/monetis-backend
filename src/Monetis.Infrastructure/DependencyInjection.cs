using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Monetis.Application.Interfaces;
using Monetis.Domain.Entities.Transactions;
using Monetis.Domain.Interfaces;
using Monetis.Infrastructure.Contexts;
using Monetis.Infrastructure.Persistence.Repositories;
using Monetis.Infrastructure.Security;

namespace Monetis.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<MonetisDataContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("MonetisConnection")));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<ICardRepository, CardRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<MonetisDataContext>());
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IExpenseRepository, ExpenseRepository>();
        services.AddScoped<ITokenService, TokenService>();
        
        
        
        return services;
    }
}
