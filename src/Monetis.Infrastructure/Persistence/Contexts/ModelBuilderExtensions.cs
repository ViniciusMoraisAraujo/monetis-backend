using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Monetis.Domain.Entities;

namespace Monetis.Infrastructure.Persistence.Contexts;

public static class ModelBuilderExtensions
{
    public static void ApplyMultiTenantFilters(this ModelBuilder modelBuilder, MonetisDataContext context)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (!typeof(UserOwnedEntity).IsAssignableFrom(entityType.ClrType))
                continue;

            var method = typeof(ModelBuilderExtensions)
                             .GetMethod(
                                 nameof(SetQueryFilterForUserOwnedEntity),
                                 BindingFlags.NonPublic | BindingFlags.Static)
                             ?.MakeGenericMethod(entityType.ClrType)
                         ?? throw new InvalidOperationException(
                             $"Método '{nameof(SetQueryFilterForUserOwnedEntity)}' não encontrado via reflection.");

            method.Invoke(null, new object[] { modelBuilder, context });
        }
    }

    private static void SetQueryFilterForUserOwnedEntity<T>(
        ModelBuilder modelBuilder,
        MonetisDataContext context)
        where T : UserOwnedEntity
    {
        modelBuilder.Entity<T>().HasQueryFilter(e => 
            context.IsUserAuthenticated && e.UserId == context.CurrentUserId);
    }
}