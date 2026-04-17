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
        services.AddScoped<IUserAuthService, UserAuthServiceService>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ISubscriptionService, SubscriptionService>();

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        

        return services;
    }
}