using Microsoft.EntityFrameworkCore;
using Monetis.Domain.Entities;
using Monetis.Domain.Enums;

namespace Monetis.Infrastructure.Persistence;

public static class SeedData
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>().HasData(
            Category.CreateSystemCategory(
                Guid.Parse("28e61ce8-8149-4c81-a570-c0085eefa121"),
                "Alimentação",
                TransactionType.Expense,
                "🍽️"
            ),
            Category.CreateSystemCategory(
                Guid.Parse("e29e79d5-7844-491f-a9bd-9e744890555b"),
                "Transporte",
                TransactionType.Expense,
                "🚗"),
            Category.CreateSystemCategory(
                Guid.Parse("96d53840-9752-4f2b-a522-c7357cdc4986"),
                "Salário",
                TransactionType.Income,
                "💰")
            );
    }
}