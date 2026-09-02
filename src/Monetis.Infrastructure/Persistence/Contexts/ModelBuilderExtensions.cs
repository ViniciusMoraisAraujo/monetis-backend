using System.Reflection;

using Microsoft.EntityFrameworkCore;

using Monetis.Domain.Entities;

namespace Monetis.Infrastructure.Persistence.Contexts;

public static class ModelBuilderExtensions
{
    private static readonly MethodInfo SetFilterMethod =
        typeof(ModelBuilderExtensions)
            .GetMethod(nameof(SetQueryFilterForUserOwnedEntity), BindingFlags.Public | BindingFlags.Static)
            ?? throw new InvalidOperationException($"Método '{nameof(SetQueryFilterForUserOwnedEntity)}' não foi encontrado.");
    public static void ApplyMultiTenantFilters(this ModelBuilder modelBuilder, MonetisDataContext context)
    {
        var userOwnedEntityType = modelBuilder.Model.GetEntityTypes()
            .Select(e => e.ClrType)
            .Where(t => typeof(UserOwnedEntity).IsAssignableFrom(t));

        foreach (var entityType in userOwnedEntityType)
        {
            var genericMethod = SetFilterMethod.MakeGenericMethod(entityType);
            genericMethod.Invoke(null, [modelBuilder, context]);
        }
    }

    public static void SetQueryFilterForUserOwnedEntity<T>(
        ModelBuilder modelBuilder,
        MonetisDataContext context)
        where T : UserOwnedEntity
    {
        modelBuilder.Entity<T>().HasQueryFilter(e =>
            context.IsUserAuthenticated && e.UserId == context.CurrentUserId);
    }
}
