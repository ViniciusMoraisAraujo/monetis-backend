using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Monetis.Application.Interfaces;
using Monetis.Application.Services;
using System.Reflection;
using Monetis.Application.Services.UserServices;

namespace Monetis.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUserAuthService, UserAuthService>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<ICardService, CardService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ISubscriptionService, SubscriptionService>();
        services.AddScoped<ITransferService, TransferService>();
        services.AddScoped<IExpenseService, ExpenseService>();
        services.AddScoped<IIncomeService, IncomeService>();

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        

        return services;
    }
}
