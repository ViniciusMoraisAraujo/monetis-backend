# 🔄 Migrations — Evolução do Banco de Dados

> **Camada:** Infrastructure (`src/Monetis.Infrastructure/Migrations/`)  
> **Ferramenta:** Entity Framework Core Migrations  
> **Provedor:** Microsoft.EntityFrameworkCore.SqlServer

---

## Índice

1. [Visão Geral](#1-visão-geral)
2. [Migration Inicial](#2-migration-inicial)
3. [Comandos de Migração](#3-comandos-de-migração)
4. [ModelSnapshot](#4-modelsnapshot)
5. [Design-Time Factory](#5-design-time-factory)
6. [Estratégia de Migrações](#6-estratégia-de-migrações)

---

## 1. Visão Geral

O projeto utiliza o sistema de **migrations** do Entity Framework Core para versionar o esquema do banco de dados. As migrations são armazenadas em arquivos C# na pasta `Migrations/`.

### Localização das Migrations

```
src/Monetis.Infrastructure/Migrations/
├── 20260507163326_InitialCreate.cs           # Migration
├── 20260507163326_InitialCreate.Designer.cs  # Código gerado (designer)
└── MonetisDataContextModelSnapshot.cs        # Snapshot do modelo atual
```

### Tabelas Criadas

A migration inicial `20260507163326_InitialCreate` cria as seguintes tabelas:

| Tabela | Descrição |
|--------|-----------|
| `Users` | Usuários do sistema |
| `Accounts` | Contas financeiras |
| `Cards` | Cartões de crédito |
| `Categories` | Categorias (sistema + personalizadas) |
| `Subscriptions` | Assinaturas recorrentes |
| `Transactions` | Tabela TPH para todas as transações |

---

## 2. Migration Inicial

**Arquivo:** `20260507163326_InitialCreate.cs`

### Estrutura da Tabela `Users`

```csharp
migrationBuilder.CreateTable(
    name: "Users",
    columns: table => new
    {
        Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
        FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
        LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
        Email = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
        PasswordHash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
        CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false)
    },
    constraints: table => table.PrimaryKey("PK_Users", x => x.Id));

migrationBuilder.CreateIndex(
    name: "IX_Users_Email",
    table: "Users",
    column: "Email",
    unique: true);
```

### Estrutura da Tabela `Accounts`

```csharp
migrationBuilder.CreateTable(
    name: "Accounts",
    columns: table => new
    {
        Id = table.Column<Guid>(nullable: false),
        Name = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
        Type = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
        Balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
        UserId = table.Column<Guid>(nullable: false),
        CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false)
    },
    constraints: table =>
    {
        table.PrimaryKey("PK_Accounts", x => x.Id);
        table.ForeignKey(
            name: "FK_Accounts_Users_UserId",
            column: x => x.UserId,
            principalTable: "Users",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    });

migrationBuilder.CreateIndex(
    name: "IX_Accounts_UserId",
    table: "Accounts",
    column: "UserId");
```

### Estrutura da Tabela `Transactions` (TPH)

A tabela `Transactions` contém colunas de todas as subclasses (`Expense`, `Income`, `Transfer`) e usa o campo `Discriminator` para identificar o tipo.

```csharp
migrationBuilder.CreateTable(
    name: "Transactions",
    columns: table => new
    {
        Id = table.Column<Guid>(nullable: false),
        AccountId = table.Column<Guid>(nullable: false),
        Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
        Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
        CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
        UserId = table.Column<Guid>(nullable: false),

        // Discriminator TPH
        Discriminator = table.Column<string>(type: "nvarchar(max)", nullable: false),

        // Campos específicos de Expense
        CategoryId = table.Column<Guid>(nullable: true),
        DueDate = table.Column<DateTime>(nullable: true),
        Status = table.Column<string>(nullable: true),
        PaidAt = table.Column<DateTime>(nullable: true),
        PaymentMethod = table.Column<string>(nullable: true),
        CreditCardId = table.Column<Guid>(nullable: true),
        SubscriptionId = table.Column<Guid>(nullable: true),

        // Campos específicos de Installment (Expense)
        IsInstallment = table.Column<bool>(nullable: true),
        InstallmentNumber = table.Column<int>(nullable: true),
        TotalInstallments = table.Column<int>(nullable: true),
        InstallmentGroupId = table.Column<Guid>(nullable: true),

        // Campos específicos de Income
        ReceivedAt = table.Column<DateTime>(nullable: true),

        // Campos específicos de Transfer
        DestinationAccountId = table.Column<Guid>(nullable: true),
        TransferredAt = table.Column<DateTime>(nullable: true),
        IsCancelled = table.Column<bool>(nullable: true)
    },
    constraints: table =>
    {
        table.PrimaryKey("PK_Transactions", x => x.Id);
        // Foreign keys...
    });
```

### Seed Data

A migration inicial inclui dados seed das categorias padrão, configurados via `SeedData.Seed()` chamado no `OnModelCreating`. O EF Core converte esses dados em comandos `INSERT` na migration gerada:

```csharp
// Inserido via SeedData.Seed(modelBuilder)
modelBuilder.Entity<Category>().HasData(
    new Category { Id = Guid.Parse("28e61ce8-..."), Name = "Alimentação", Icon = "🍽️" },
    new Category { Id = Guid.Parse("e29e79d5-..."), Name = "Transporte", Icon = "🚗" },
    new Category { Id = Guid.Parse("96d53840-..."), Name = "Salário", Icon = "💰" }
);
```

---

## 3. Comandos de Migração

### Criar Nova Migration

```bash
# No diretório src/Monetis.Infrastructure/
dotnet ef migrations add NomeDaMigration
```

### Aplicar Migrations ao Banco

```bash
# A partir do diretório src/Monetis.Infrastructure/
dotnet ef database update

# Ou especificando o projeto de startup (API):
dotnet ef database update --startup-project ../Monetis.API
```

### Remover Última Migration

```bash
dotnet ef migrations remove
```

### Gerar Script SQL

```bash
dotnet ef migrations script -o script.sql
```

> ⚠️ A factory `MonetisDataContextFactory` assume que está sendo executada a partir do diretório do projeto `Monetis.Infrastructure`. O `basePath` aponta para `../Monetis.API` para ler o `appsettings.json`.

---

## 4. ModelSnapshot

**Arquivo:** `MonetisDataContextModelSnapshot.cs`

O snapshot é gerado automaticamente pelo EF Core e reflete o estado atual do modelo. É usado para comparar com o modelo atual ao gerar novas migrations.

O snapshot atual inclui todas as entidades e suas configurações:
- `AccountEntityType`
- `CardEntityType`
- `CategoryEntityType`
- `ExpenseEntityType`
- `IncomeEntityType`
- `SubscriptionEntityType`
- `TransactionEntityType`
- `TransferEntityType`
- `UserEntityType`

---

## 5. Design-Time Factory

**Arquivo:** `src/Monetis.Infrastructure/Persistence/Contexts/MonetisDataContextFactory.cs`

```csharp
public class MonetisDataContextFactory : IDesignTimeDbContextFactory<MonetisDataContext>
{
    public MonetisDataContext CreateDbContext(string[] args)
    {
        var basePath = Path.Combine(
            Directory.GetCurrentDirectory(), "..", "Monetis.API");

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddJsonFile("appsettings.json", optional: true)
            .Build();

        var builder = new DbContextOptionsBuilder<MonetisDataContext>();
        var connectionString = configuration.GetConnectionString("MonetisConnection");
        builder.UseSqlServer(connectionString);

        return new MonetisDataContext(builder.Options);
    }
}
```

A factory é necessária para que os comandos do EF Core saibam como instanciar o `DbContext` durante o design-time (criação de migrations). Ela:
1. Localiza o `appsettings.json` no projeto `Monetis.API`
2. Lê a connection string
3. Cria o contexto **sem** `IUserContextAccessor` (null)

---

## 6. Estratégia de Migrações

### Recomendações

| Prática | Descrição |
|---------|-----------|
| **Uma migration por sprint/feature** | Evite múltiplas migrations pequenas |
| **Nomes descritivos** | Ex: `AddCreditCardFields` ou `CreateExpenseIndexes` |
| **Review de migrations** | Verificar SQL gerado antes de aplicar |
| **Testar em staging** | Nunca aplicar migration diretamente em produção sem testar |
| **Backup antes de migrar** | Especialmente em produção |

### Histórico de Migrações

| Data | Migration | Descrição |
|------|-----------|-----------|
| 2026-05-07 | `20260507163326_InitialCreate` | Criação inicial de todas as tabelas |

---

> **Próximo:** [Voltar à Visão Geral](../00-visao-geral.md) *(ou avançar para Fase 5 quando disponível)*
