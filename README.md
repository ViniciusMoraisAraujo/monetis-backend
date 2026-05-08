# 💰 Monetis — API

> API back-end da plataforma de finanças pessoais Monetis. Controle de receitas, despesas, assinaturas, transferências e relatórios mensais.

![Status](https://img.shields.io/badge/status-em%20desenvolvimento-yellow)
![License](https://img.shields.io/badge/license-MIT-blue)
![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)
![EF Core](https://img.shields.io/badge/EF%20Core-10.0-512BD4?logo=dotnet)

---

## 📋 Sobre o Projeto

O **Monetis** é uma plataforma full stack para gerenciamento de finanças pessoais. Este repositório contém o **back-end** da aplicação, responsável por toda a lógica de negócio, persistência de dados e exposição da API REST.

> 🚧 Projeto em desenvolvimento ativo — também utilizado como laboratório de aprendizado de arquitetura de software.

---

## ✨ Funcionalidades

- 📥 Lançamento de receitas e despesas
- 💳 Controle de despesas parceladas no cartão de crédito
- 🔁 Assinaturas recorrentes com geração automática de despesas
- 🔀 Transferências entre contas com reversão no mesmo dia
- 🏷️ Categorização de transações (categorias do sistema + personalizadas)
- 📊 Dashboard com gráficos e resumos *(em breve)*
- 📅 Relatório mensal detalhado *(em breve)*
- 🔐 Autenticação JWT com multi-tenancy por usuário

---

## 🛠️ Tecnologias

| Tecnologia | Versão | Descrição |
|---|---|---|
| ![.NET](https://img.shields.io/badge/.NET-512BD4?style=flat&logo=dotnet&logoColor=white) | 10.0 | Framework back-end |
| ![C#](https://img.shields.io/badge/C%23-239120?style=flat&logo=csharp&logoColor=white) | 13 | Linguagem principal |
| ![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?style=flat&logo=microsoftsqlserver&logoColor=white) | - | Banco de dados relacional |
| Entity Framework Core | 10.0 | ORM e migrations |
| FluentValidation | 11.9 | Validação de inputs |
| JWT Bearer | 10.0 | Autenticação |
| Scalar / Swagger | - | Documentação da API |

---

## 🏛️ Arquitetura

O projeto segue os princípios da **Clean Architecture**, separando responsabilidades em camadas com dependências que apontam sempre para o centro (domínio).

```
Monetis.API              → Controllers, Middlewares, Background Services
Monetis.Application      → Services, DTOs, Validators, Interfaces
Monetis.Infrastructure   → EF Core, Repositories, JWT, PasswordHasher
Monetis.Domain           → Entities, Enums, Exceptions, Interfaces
```

### Por que Clean Architecture?

Por se tratar de um sistema de finanças pessoais (SaaS), o domínio possui regras de negócio complexas — parcelamento, assinaturas recorrentes, transferências com reversão. A separação em camadas garante que essas regras vivam no `Domain`, independentes de banco de dados ou framework web, facilitando testes e evolução do sistema.

---

## 🎨 Design Patterns

### Repository Pattern
Cada agregado possui sua interface de repositório definida no `Domain` e implementada na `Infrastructure`. Isso desacopla a lógica de negócio da tecnologia de persistência.

```
IExpenseRepository (Domain)  ←  ExpenseRepository : BaseRepository<Expense> (Infrastructure)
```

O `BaseRepository<T>` centraliza as operações genéricas (`GetById`, `Create`, `Update`, `Delete`), evitando repetição.

### Unit of Work
O `MonetisDataContext` implementa `IUnitOfWork`, centralizando o `SaveChanges` em um único ponto. Operações que criam múltiplos objetos (ex: criar uma assinatura + sua primeira despesa) são persistidas atomicamente num único `CommitAsync`.

```csharp
subscriptionRepository.Create(subscription);
expenseRepository.Create(firstExpense);
await unitOfWork.CommitAsync(); // um único SaveChanges
```

> ⚠️ **Em progresso:** operações críticas como `TransferService.CreateAsync`, `ExpenseService.PayExpenseAsync` e `SubscriptionService.CreateAsync` estão recebendo `IDbContextTransaction` para garantir rollback em caso de falha parcial.

### Multi-tenancy
O Monetis é um SaaS — uma única instância da aplicação serve múltiplos usuários simultaneamente, onde cada usuário é um **tenant** isolado. A estratégia adotada é **shared database, shared schema**, com isolamento garantido via `UserId` como identificador do tenant.

Cada entidade que pertence a um usuário herda de `UserOwnedEntity`. O `MonetisDataContext` aplica automaticamente um **query filter global** por tenant em todas essas entidades, garantindo que nenhuma query retorne dados de outro usuário — sem necessidade de filtrar manualmente em cada consulta.

```csharp
// Aplicado automaticamente para todas as entidades UserOwnedEntity
builder.Entity<T>().HasQueryFilter(e => e.UserId == userContext.UserId);
```

O `UserId` (tenant) é extraído do JWT pelo `UserContextMiddleware`, populado no `UserContext` (scoped) e injetado no `DbContext` — tornando o isolamento entre tenants completamente transparente para os serviços.

### TPC (Table Per Concrete Type) — Herança de Transações
`Expense`, `Income` e `Transfer` herdam de `Transaction`, mas cada um possui sua própria tabela no banco (`Expenses`, `Incomes`, `Transfers`). Isso evita colunas nulas desnecessárias (problema do TPH) mantendo a hierarquia de domínio.

### Domain Exceptions
Regras de negócio inválidas lançam exceções de domínio tipadas (ex: `ExpenseAlreadyPaidException`, `TransferInsufficientFundsException`). O `ExceptionMiddleware` as intercepta e retorna respostas HTTP padronizadas com `ErrorCode`, sem vazar stack trace em produção.

### Background Service
`OverdueExpenseProcessorService` roda como `IHostedService`, processando despesas vencidas uma vez por dia sem bloquear o pipeline da API.

---

## 🚀 Como executar

### Pré-requisitos

- [.NET SDK 10+](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/pt-br/sql-server/sql-server-downloads)

### Instalação

```bash
git clone https://github.com/ViniciusMoraisAraujo/monetis.git
cd monetis
```

### Variáveis de ambiente

Crie `src/Monetis.API/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "MonetisConnection": "Server=localhost,1433;Database=MonetisDb;User Id=sa;Password=sua_senha;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Key": "sua_chave_secreta_base64",
    "Issuer": "MonetisIdentityServer",
    "Audience": "MonetisAPI"
  }
}
```

### Rodando

```bash
dotnet restore
dotnet ef database update --project src/Monetis.Infrastructure --startup-project src/Monetis.API
dotnet run --project src/Monetis.API
```

Documentação disponível em `http://localhost:5074/swagger` após iniciar.

---

## 📡 Endpoints principais

| Método | Rota | Descrição |
|---|---|---|
| POST | `/api/auth/login` | Autenticação — retorna JWT |
| POST | `/api/users` | Cadastro de usuário |
| GET/POST/PUT/DELETE | `/api/accounts` | Contas bancárias |
| GET/POST/PUT/DELETE | `/api/cards` | Cartões de crédito |
| GET/POST/PUT/DELETE | `/api/categories` | Categorias |
| GET/POST/PUT/DELETE | `/api/expenses` | Despesas |
| POST | `/api/expenses/installments` | Criar despesa parcelada |
| POST | `/api/expenses/{id}/pay` | Pagar despesa |
| GET/POST/PUT/DELETE | `/api/incomes` | Receitas |
| GET/POST/PUT/DELETE | `/api/subscriptions` | Assinaturas recorrentes |
| GET/POST/PUT/DELETE | `/api/transfers` | Transferências entre contas |

> Documentação completa via Swagger/Scalar após iniciar o servidor.

---

## 🗺️ Roadmap

- [x] Estrutura Clean Architecture
- [x] Autenticação JWT
- [x] Multi-tenancy por usuário via query filters globais no EF Core
- [x] CRUD de contas, cartões e categorias
- [x] Despesas simples e parceladas
- [x] Receitas
- [x] Assinaturas recorrentes
- [x] Transferências entre contas
- [x] Background service para despesas vencidas
- [x] Rate limiting
- [ ] Transações de banco (`IDbContextTransaction`) nas operações críticas
- [ ] Dashboard e relatório mensal
- [ ] Testes unitários
- [ ] Deploy

---

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.

---

<p align="center">Feito com ❤️ por <a href="https://github.com/ViniciusMoraisAraujo">ViniciusMoraisAraujo</a></p>