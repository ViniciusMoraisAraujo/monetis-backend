using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Monetis.Application.Interfaces;
using Monetis.Application.Services;
using System.Reflection;

namespace Monetis.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ITransactionService, TransactionService>();
        services.AddScoped<ISubscriptionService, SubscriptionService>();

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        

        return services;
    }
}