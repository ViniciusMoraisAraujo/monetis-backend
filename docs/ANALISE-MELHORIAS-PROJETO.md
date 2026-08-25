# 🔬 Análise Completa de Melhorias — Monetis

> **Data:** Agosto 2026
> **Escopo:** Análise de TODO o código fonte + documentação existente
> **Classificação:** Bugs → Segurança → Arquitetura → Features → Over-Engineering

---

## 📊 Resumo Executivo

| Categoria | Itens | Esforço Total | Impacto |
|-----------|:-----:|:-------------:|:-------:|
| 🐛 Bugs Críticos | 6 | ~2h | 🔴 |
| 🔒 Segurança | 8 | ~8h | 🔴 |
| 🏗️ Arquitetura | 10 | ~15h | 🟡 |
| ✨ Features Ausentes | 8 | ~20h | 🟡 |
| 🔧 Melhorias de Código | 12 | ~10h | 🟡 |
| 🚀 Over-Engineering | 10 | ~40h | 🟢 |

---

## 🐛 1. BUGS CRÍTICOS (Corrigir HOJE)

### BUG-001: `UnitOfWork.Dispose()` lança `NotImplementedException`

**Arquivo:** `src/Monetis.Infrastructure/Persistence/UnitOfWork.cs:13`
**Severidade:** 🔴 Crash em runtime

```csharp
public void Dispose()
{
    throw new NotImplementedException(); // ❌ Crash se alguém usar `using`
}
```

**Impacto:** Qualquer código que use `using (var uow = ...)` vai crashar. O `IUnitOfWork` herda `IDisposable`, então isso é esperado pelo framework.

**Correção:**
```csharp
public void Dispose()
{
    monetisDataContext?.Dispose();
    GC.SuppressFinalize(this);
}
```

---

### BUG-002: `ChangePasswordAsync` busca por email usando GUID

**Arquivo:** `src/Monetis.Application/Services/UserServices/UserAuthService.cs:29`
**Severidade:** 🔴 Feature completamente quebrada

```csharp
var user = await userRepository.GetUserByEmailAsync(
    userContextAccessor.UserId.ToString(), // ❌ GUID como email!
    cancellationToken)
    ?? throw new UnauthorizedAccessException();
```

**Impacto:** Nenhum usuário consegue trocar senha. O `GetUserByEmailAsync` recebe `"3fa85f64-..."` (um GUID) e busca por email — nunca encontra.

**Correção:**
```csharp
var user = await userRepository.GetByIdAsync(
    userContextAccessor.UserId, cancellationToken)
    ?? throw new UnauthorizedAccessException();
```

---

### BUG-003: `UserResourceGuard` não verifica ownership

**Arquivo:** `src/Monetis.Application/Services/UserResourceGuard.cs:26-40`
**Severidade:** 🔴 Violação de multi-tenancy

```csharp
public async Task<Account> GetOwnedAccountAsync(Guid accountId, ...)
{
    var account = await accountRepository.GetByIdAsync(accountId, cancellationToken);
    if (account == null)
        throw new KeyNotFoundException(...);
    return account; // ❌ Nunca verifica account.UserId == CurrentUserId
}
```

**Impacto:** Usuário A pode acessar contas de Usuário B se o query filter do EF Core for contornado (ex: `IgnoreQueryFilters`).

**Correção:**
```csharp
public async Task<Account> GetOwnedAccountAsync(Guid accountId, ...)
{
    var account = await accountRepository.GetByIdAsync(accountId, cancellationToken);
    if (account == null)
        throw new KeyNotFoundException(...);
    if (account.UserId != CurrentUserId)
        throw new KeyNotFoundException(...);
    return account;
}
// Mesmo para GetOwnedCardAsync
```

---

### BUG-004: `SubscriptionService.UpdateAsync` ignora campos do request

**Arquivo:** `src/Monetis.Application/Services/SubscriptionService.cs:67-75`
**Severidade:** 🔴 Update não funciona

```csharp
subscription.Update(
    amount: request.Amount,
    description: request.Description,
    frequency: request.Frequency,
    nextDueDate: request.NextDueDate,
    isActive: request.IsActive,
    paymentMethod: subscription.PaymentMethod,  // ❌ valor ATUAL
    accountId: subscription.AccountId,           // ❌ valor ATUAL
    endDate: subscription.EndDate,               // ❌ valor ATUAL
    creditCardId: subscription.CardId);          // ❌ valor ATUAL
```

**Impacto:** Usuário não pode alterar conta, cartão, método de pagamento ou data de término de uma assinatura.

**Correção:** Adicionar campos ao `UpdateSubscriptionRequest` DTO e propagar no service.

---

### BUG-005: `catch(Exception)` genérico mascara erros 500 como 400

**Arquivo:** `src/Monetis.API/Controllers/ExpensesController.cs` (4 métodos: Create, CreateInstallments, Pay, Update)
**Severidade:** 🟡 Debugging difícil

```csharp
catch (Exception ex)
{
    return BadRequest(ex.Message); // ❌ Erro 500 vira 400
}
```

**Impacto:** Erros de banco de dados, rede ou infraestrutura são mascarados como erros de validação.

**Correção:** Remover `catch (Exception)` — o `ExceptionMiddleware` já captura globalmente.

---

### BUG-006: `UnauthorizedAccessException` retorna 500 em vez de 401

**Arquivo:** `src/Monetis.API/Middlewares/ExceptionMiddleware.cs:33-46`
**Severidade:** 🟡 Login falha com 500

```csharp
var (statusCode, message, errorcode) = exception switch
{
    DomainException => (400, exception.Message, "BUSINESS_ERROR"),
    ArgumentException => (400, "Bad Request", "04X0"),
    KeyNotFoundException => (404, "Not Found", "04X4"),
    _ => (500, "Internal error", "07X0") // ❌ UnauthorizedAccessException cai aqui
};
```

**Correção:**
```csharp
var (statusCode, message, errorcode) = exception switch
{
    DomainException => (400, exception.Message, "BUSINESS_ERROR"),
    ArgumentException => (400, "Bad Request", "04X0"),
    KeyNotFoundException => (404, "Not Found", "04X4"),
    UnauthorizedAccessException => (401, exception.Message, "UNAUTHORIZED"),
    _ => (500, "Internal error", "07X0")
};
```

---

## 🔒 2. SEGURANÇA

### SEC-001: `GET /api/users` expõe dados de todos os usuários

**Arquivo:** `src/Monetis.API/Controllers/UsersController.cs:23-28`
**Impacto:** Vazamento de emails para ataques de força bruta (LGPD)

**Solução:** Remover endpoint `GetAll` ou restringir a `[Authorize(Roles = "Admin")]`.

---

### SEC-002: `GET /api/users/{id}` sem verificação de ownership

**Arquivo:** `src/Monetis.API/Controllers/UsersController.cs:13-20`
**Impacto:** Qualquer usuário lê dados de outro usuário

**Solução:** Adicionar `if (id != UserId) return Forbid();`

---

### SEC-003: Sem bloqueio de conta após tentativas falhas

**Arquivo:** `src/Monetis.Application/Services/UserServices/UserAuthService.cs:11-22`
**Impacto:** Ataque de força bruta ilimitado

**Solução:** Adicionar `FailedLoginAttempts` + `LockedUntil` na entidade `User`.

---

### SEC-004: Token JWT expira em 2 dias sem refresh/revogação

**Arquivo:** `src/Monetis.Infrastructure/Security/TokenService.cs:33`
**Impacto:** Token roubado válido por 48h

**Solução:** Reduzir para 1h + implementar refresh tokens.

---

### SEC-005: `AllowedHosts: "*"` permite qualquer origem

**Arquivo:** `src/Monetis.API/appsettings.json`
**Impacto:** Superfície de ataque desnecessária

**Solução:** Restringir a domínios específicos via variável de ambiente.

---

### SEC-006: JWT Key sem validação de comprimento mínimo

**Arquivo:** `src/Monetis.Infrastructure/Security/TokenService.cs:17-18`
**Impacto:** Chave fraca pode ser usada

**Solução:** Adicionar `if (jwtKey.Length < 32) throw ...`

---

### SEC-007: `IgnoreQueryFilters` sem documentação de intencionalidade

**Arquivo:** `src/Monetis.Infrastructure/Persistence/Repositories/ExpenseRepository.cs:32`
**Impacto:** Método público pode ser chamado incorretamente

**Solução:** Adicionar comentário XML detalhando por que é intencional.

---

### SEC-008: DTOs de resposta expõem `UserId`

**Arquivos:** `AccountDtos.cs`, `CardDtos.cs`, `CategoryDtos.cs`
**Impacto:** Vazamento de GUIDs de usuários

**Solução:** Remover `UserId` dos DTOs de resposta.

---

## 🏗️ 3. ARQUITETURA

### ARC-001: `UserContext` registrado DUAS vezes

**Arquivo:** `src/Monetis.Infrastructure/DependencyInjection.cs:21` E `src/Monetis.API/Program.cs:17`

```csharp
// Infrastructure/DependencyInjection.cs
services.AddScoped<UserContext>(); // 1ª vez

// Program.cs
builder.Services.AddScoped<UserContext>(); // 2ª vez (redundante)
```

**Impacto:** Funciona, mas é confuso e viola Single Responsibility.

**Solução:** Remover de `Program.cs`, manter apenas no `Infrastructure`.

---

### ARC-002: `ExpenseQueryService` não registrado no DI

**Arquivo:** `src/Monetis.Application/Abstractions/Persistence/IExpenseQueryService.cs` existe, mas `ExpenseQueryService` não está no `DependencyInjection.cs` da Infrastructure.

**Impacto:** `ExpenseService` depende de `IExpenseQueryService` — causa `InvalidOperationException` em runtime.

**Solução:** Adicionar `services.AddScoped<IExpenseQueryService, ExpenseQueryService>();` no `Infrastructure/DependencyInjection.cs`.

---

### ARC-003: `ExpenseService` usa `throw new Exception("Expense not found")`

**Arquivo:** `src/Monetis.Application/Services/ExpenseService.cs:68,93`

```csharp
if (expense == null) throw new Exception("Expense not found"); // ❌ Genérico
```

**Impacto:** `ExceptionMiddleware` retorna 500 em vez de 404.

**Solução:** Trocar por `throw new KeyNotFoundException(...)`.

---

### ARC-004: Services não usam `accountRepository.Update()` após mutações

**Arquivo:** `src/Monetis.Application/Services/AccountService.cs:43`

```csharp
public async Task UpdateAsync(Guid id, UpdateAccountRequest updateDto, ...)
{
    var account = await userResourceGuard.GetOwnedAccountAsync(id, cancellationToken);
    account.Update(updateDto.Name);
    await unitOfWork.CommitAsync(cancellationToken); // ❌ Falta accountRepository.Update(account)
}
```

**Impacto:** O EF Core detecta mudanças via ChangeTracker, então funciona por acidente. Mas é uma anti-pattern — explícito > implícito.

**Solução:** Adicionar `accountRepository.Update(account);` antes do `CommitAsync`.

---

### ARC-005: `IncomesController` e `SubscriptionsController` não têm `[ApiController]` + `[Route]`

**Arquivo:** `src/Monetis.API/Controllers/IncomesController.cs`, `SubscriptionsController.cs`

```csharp
[Authorize]
public class IncomesController : ApiControllerBase // ❌ Sem [ApiController] e [Route]
```

**Impacto:** Herda `[Route("api/[controller]")]` do base, mas `[ApiController]` não é herdado — pode causar problemas com validação automática.

**Solução:** Adicionar `[ApiController]` e `[Route("api/[controller]")]` explicitamente.

---

### ARC-006: `IncomesController.Update` retorna 200 em vez de 204

**Arquivo:** `src/Monetis.API/Controllers/IncomesController.cs:54`

```csharp
public async Task<IActionResult> Update(...)
{
    try
    {
        await incomeService.UpdateAsync(id, request, cancellationToken);
        return Ok(); // ❌ Deveria ser NoContent()
    }
```

**Impacto:** Inconsistência — outros updates retornam 204.

**Solução:** Trocar `return Ok()` por `return NoContent()`.

---

### ARC-007: `Category.Update()` não valida name/icon

**Arquivo:** `src/Monetis.Domain/Entities/Category.cs:41-45`

```csharp
public void Update(string name, string icon)
{
    Name = name;  // ❌ Sem validação!
    Icon = icon;
}
```

**Impacto:** Categoria pode ser atualizada com nome vazio ou nulo.

**Solução:** Chamar `ValidateCategory(name, icon)` antes de atribuir.

---

### ARC-008: `Card.Update()` não valida name

**Arquivo:** `src/Monetis.Domain/Entities/Card.cs:19-22`

```csharp
public void Update(string name)
{
    Name = name;  // ❌ Sem validação!
}
```

**Solução:** Chamar `ValidateName(name)` antes de atribuir.

---

### ARC-009: `AuthController` não herda de `ApiControllerBase`

**Arquivo:** `src/Monetis.API/Controllers/AuthController.cs:11`

```csharp
public class AuthController : ControllerBase // ❌ Não herda ApiControllerBase
```

**Impacto:** Não tem acesso a `UserId` helper, mas é intencional (login é anônimo). Porém, se no futuro precisar de auth no controller, vai precisar mudar.

---

### ARC-010: `Transfer.Update` no service ajusta saldos ANTES de validar

**Arquivo:** `src/Monetis.Application/Services/TransferService.cs:50-60`

```csharp
// Ajusta saldos
if (amountDelta > 0) transfer.Account.Withdraw(amountDelta);
// ...
transfer.Update(updateDto.Amount, updateDto.Description); // Valida aqui
await unitOfWork.CommitAsync();
```

**Impacto:** Se `transfer.Update()` lançar exceção (ex: transferência cancelada), os saldos já foram alterados mas não serão commitados (porque não há transaction explícita). Funciona, mas é confuso.

**Solução:** Chamar `transfer.Update()` primeiro (validação), depois ajustar saldos.

---

## ✨ 4. FEATURES AUSENTES

### FEAT-001: Endpoint `POST /api/auth/change-password`

**Status:** Serviço implementado, mas sem endpoint no controller.

**Solução:** Adicionar action no `AuthController`:
```csharp
[Authorize]
[HttpPost("change-password")]
public async Task<IActionResult> ChangePassword(ChangePasswordRequest request, ...)
{
    await userAuthService.ChangePasswordAsync(request, cancellationToken);
    return NoContent();
}
```

---

### FEAT-002: Endpoint `GET /api/expenses/period` com filtro por data

**Status:** `ExpenseRepository.GetByPeriodAsync` existe mas não é exposto.

**Solução:** Adicionar endpoint e service method.

---

### FEAT-003: Paginação em endpoints de lista

**Status:** Todos os `GetAll` retornam lista completa.

**Solução:** Criar `PagedResult<T>` e aplicar em todos os endpoints.

---

### FEAT-004: Health Check endpoint

**Status:** Nenhum `/health` configurado.

**Solução:**
```csharp
builder.Services.AddHealthChecks()
    .AddDbContextCheck<MonetisDataContext>();
app.MapHealthChecks("/health");
```

---

### FEAT-005: OpenAPI com Security Scheme JWT

**Status:** Swagger existe mas sem scheme de autenticação.

**Solução:** Configurar `AddSecurityDefinition` no `AddSwaggerGen`.

---

### FEAT-006: `[ProducesResponseType]` nos controllers

**Status:** Nenhum controller tem documentação de status code.

**Solução:** Adicionar attributes:
```csharp
[HttpGet("{id:guid}")]
[ProducesResponseType(typeof(AccountResponse), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<ActionResult<AccountResponse>> GetById(...)
```

---

### FEAT-007: Soft Delete

**Status:** Delete é físico.

**Solução:** Adicionar `IsDeleted` + `DeletedAt` em `BaseEntity` + query filter global.

---

### FEAT-008: `SubscriptionResponse` incompleto

**Status:** Não retorna `AccountId`, `CategoryId`, `PaymentMethod`, `CardId`.

**Solução:** Expandir DTO e mapeamento.

---

## 🔧 5. MELHORIAS DE CÓDIGO

### CODE-001: Mapeamento DTO repetido em todos os services

**Problema:** Cada service tem seu próprio `MapToResponse`/`MapToDto` com lógica similar.

**Solução:** Criar `MappingProfile` com AutoMapper ou mapeadores manuais centralizados.

---

### CODE-002: `ExpenseService.CreateInstallmentAsync` valida cardId com `ArgumentException`

**Arquivo:** `src/Monetis.Application/Services/ExpenseService.cs:58`

```csharp
if (!request.CreditCardId.HasValue)
    throw new ArgumentException("Credit card is required for installments");
    // ❌ Deveria ser DomainException ou validação no validator
```

**Solução:** Mover para validator FluentValidation.

---

### CODE-003: `IncomesController` não tem `[ApiController]`

**Arquivo:** `src/Monetis.API/Controllers/IncomesController.cs`

```csharp
[Authorize]
public class IncomesController : ApiControllerBase // Falta [ApiController]
```

**Solução:** Adicionar `[ApiController]` e `[Route("api/[controller]")]`.

---

### CODE-004: `RateLimitingMiddleware` não está registrado no pipeline

**Arquivo:** `src/Monetis.API/Middlewares/RateLimitingMiddleware.cs`

**Problema:** Existe no código mas não é usado. Código morto.

**Solução:** Remover ou registrar. Recomendação: remover (o nativo do ASP.NET Core já cobre).

---

### CODE-005: `ExpenseService` não injeta `IExpenseQueryService`

**Arquivo:** `src/Monetis.Application/Services/ExpenseService.cs:7-8`

```csharp
public class ExpenseService(IExpenseRepository expenseRepository,
    IUnitOfWork unitOfWork,
    IUserResourceGuard userResourceGuard) : IExpenseService
// ❌ Falta IExpenseQueryService
```

**Solução:** Adicionar no construtor se for usar, ou remover a interface `IExpenseQueryService` se não for necessária.

---

### CODE-006: `Category.CreateSystemCategory` hardcodes `CreatedAt`

**Arquivo:** `src/Monetis.Domain/Entities/Category.cs:31`

```csharp
CreatedAt = new DateTime(2026, 3, 13), // ❌ Hardcoded
```

**Solução:** Usar `DateTime.UtcNow` ou tornar o parâmetro configurável.

---

### CODE-007: `Expense.CreateInstallment` Description truncada silenciosamente

**Problema:** Se `description + " (1/12)"` exceder 100 chars, a exception `ExpenseDescriptionInvalidException` é lançada para TODAS as parcelas.

**Solução:** Truncar a descrição antes de adicionar o sufixo, ou validar o tamanho com o sufixo.

---

### CODE-008: `TransferService.UpdateAsync` duplica lógica de ajuste

**Arquivo:** `src/Monetis.Application/Services/TransferService.cs:47-58`

```csharp
if (amountDelta > 0)
    transfer.Account.Withdraw(amountDelta);
else if (amountDelta < 0)
    transfer.Account.Deposit(Math.Abs(amountDelta));

if (amountDelta > 0)  // ❌ Mesmo if repetido
    transfer.DestinationAccount.Deposit(amountDelta);
else if (amountDelta < 0)
    transfer.DestinationAccount.Withdraw(Math.Abs(amountDelta));
```

**Solução:** Simplificar com um método helper ou lógica mais limpa.

---

### CODE-009: `TokenService` cria `JwtSecurityTokenHandler` toda chamada

**Arquivo:** `src/Monetis.Infrastructure/Security/TokenService.cs:38`

```csharp
var tokenHandler = new JwtSecurityTokenHandler(); // ❌ Criado a cada chamada
```

**Solução:** Tornar estático ou injetar como singleton.

---

### CODE-010: `BaseRepository.DeleteAsync` não lança exceção se não encontrar

**Arquivo:** `src/Monetis.Infrastructure/Persistence/Repositories/BaseRepository.cs:33-40`

```csharp
public async Task DeleteAsync(Guid id, ...)
{
    var entity = await context.Set<T>().FirstOrDefaultAsync(x => x.Id == id, ...);
    if (entity != null)
        context.Set<T>().Remove(entity);
    // ❌ Silenciosamente ignora se não existe
}
```

**Solução:** Lançar `KeyNotFoundException` se não encontrar, ou retornar `bool`.

---

### CODE-011: `UserOwnedEntity.SetUser` permite `Guid.Empty`

**Arquivo:** `src/Monetis.Domain/Entities/UserOwnedEntity.cs:16`

```csharp
public void SetUser(Guid userId)
{
    if (UserId != Guid.Empty) // ❌ Permite setar com Guid.Empty
        throw new UserOwnedEntityUserAlreadySetException();
    UserId = userId;
}
```

**Solução:** Adicionar `if (userId == Guid.Empty) throw new ...`

---

### CODE-012: `Account.Name` validator permite 100 chars, domínio limita a 25

**Problema:** FluentValidation permite 100, domínio lança exceção com 25.

**Solução:** Padronizar para 25 em ambos.

---

## 🚀 6. OVER-ENGINEERING (Melhorias Avançadas)

### OE-001: CQRS com MediatR

**O que:** Separar Queries de Commands usando CQRS.

**Por que:** O projeto já tem `ExpenseQueryService` separado — expandir esse padrão para todos os services.

**Implementação:**
```
Application/
├── Commands/
│   ├── Accounts/
│   │   ├── CreateAccountCommand.cs
│   │   └── CreateAccountHandler.cs
│   └── Expenses/
│       └── ...
├── Queries/
│   ├── Accounts/
│   │   ├── GetAccountByIdQuery.cs
│   │   └── GetAccountByIdHandler.cs
│   └── Expenses/
│       └── ...
```

---

### OE-002: Domain Events

**O que:** Disparar eventos quando algo acontece no domínio.

**Por que:** Desacoplar side effects (ex: "conta ficou negativa", "despesa vencida").

**Implementação:**
```csharp
// Na entidade
public class Account : UserOwnedEntity
{
    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void Withdraw(decimal amount)
    {
        ValidateAmountPositive(amount);
        Balance -= amount;
        if (Balance < 0)
            _domainEvents.Add(new AccountBecameNegativeEvent(Id, UserId, Balance));
    }
}
```

---

### OE-003: Result Pattern (eliminar exceções)

**O que:** Substituir exceptions por `Result<T>` para fluxos esperados.

**Por que:** Exceptions para controle de fluxo são lentas e poluem logs.

**Implementação:**
```csharp
public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? Error { get; }

    public static Result<T> Success(T value) => new(true, value, null);
    public static Result<T> Failure(string error) => new(false, default, error);
}

// No service
public async Task<Result<ExpenseResponse>> CreateExpenseAsync(...)
{
    var account = await _repo.GetByIdAsync(request.AccountId);
    if (account == null)
        return Result<ExpenseResponse>.Failure("Account not found");
    // ...
}
```

---

### OE-004: Outbox Pattern para Transactions

**O que:** Garantir que mutations no banco e eventos sejam atômicos.

**Por que:** Hoje, `TransferService.CreateAsync` faz withdraw + deposit + create sem transaction explícita.

**Implementação:**
```csharp
// Tabela OutboxMessage
public class OutboxMessage
{
    public Guid Id { get; set; }
    public string EventType { get; set; }
    public string Payload { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool Processed { get; set; }
}

// Background service processa outbox
```

---

### OE-005: Event Sourcing para Transações Financeiras

**O que:** Cada transação financeira gera um evento imutável.

**Por que:** Auditoria completa, ability de reconstruir saldos históricos.

**Complexidade:** Alta — requer nova entidade `FinancialEvent` e replayer.

---

### OE-006: Idempotency Keys para POSTs

**O que:** Cada POST pode receber um `Idempotency-Key` header.

**Por que:** Evitar duplicação em retry de rede.

**Implementação:**
```csharp
// Middleware
public class IdempotencyMiddleware
{
    private readonly IMemoryCache _cache;

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Method == "POST" &&
            context.Request.Headers.TryGetValue("Idempotency-Key", out var key))
        {
            if (_cache.TryGetValue(key, out var cached))
            {
                // Retornar resposta cacheada
                return;
            }
            // Processar e cachear resposta
        }
    }
}
```

---

### OE-007: Audit Log com SaveChangesInterceptor

**O que:** Rastrear quem criou/modificou cada registro.

**Implementação:**
```csharp
public class AuditInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result, CancellationToken ct = default)
    {
        foreach (var entry in eventData.Context!.ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Modified)
                entry.Entity.UpdatedAt = DateTime.UtcNow;
        }
        return base.SavingChangesAsync(eventData, result, ct);
    }
}
```

---

### OE-008: CQRS Read Model com projetos separados

**O que:** Projetos separados para leitura e escrita.

**Por que:** Escalar reads e writes independentemente.

**Estrutura:**
```
src/
├── Monetis.Domain/           # Core
├── Monetis.Application/      # Write side
├── Monetis.ReadModel/        # Read side (projeções)
├── Monetis.Infrastructure/
│   ├── Write/                # Write DB context
│   └── Read/                 # Read DB context (projeto)
└── Monetis.API/
```

---

### OE-009: Feature Flags

**O que:** Toggle de features sem deploy.

**Por que:** Permitir rollout gradual de novas funcionalidades.

**Implementação:** Usar `Microsoft.FeatureManagement`:
```csharp
builder.Services.AddFeatureManagement();
app.MapGet("/api/dashboard", [FeatureGate("Dashboard")] () => { ... });
```

---

### OE-010: Distributed Tracing com OpenTelemetry

**O que:** Rastrear requisições entre services.

**Por que:** Em produção, entender fluxos de requisições.

**Implementação:**
```csharp
builder.Services.AddOpenTelemetry()
    .WithTracing(b => b
        .AddAspNetCoreInstrumentation()
        .AddEntityFrameworkCoreInstrumentation()
        .AddOtlpExporter());
```

---

## 📋 Plano de Ação Sugerido

### Fase 1 — Hotfix (1 dia)
1. Corrigir `UnitOfWork.Dispose()` (BUG-001)
2. Corrigir `ChangePasswordAsync` (BUG-002)
3. Corrigir `UserResourceGuard` ownership (BUG-003)
4. Mapear `UnauthorizedAccessException` → 401 (BUG-006)
5. Remover `catch(Exception)` dos controllers (BUG-005)
6. Adicionar `ExpenseQueryService` ao DI (ARC-002)

### Fase 2 — Segurança (2-3 dias)
7. Remover `GET /api/users` (SEC-001)
8. Adicionar ownership check no `UsersController` (SEC-002)
9. Implementar account lockout (SEC-003)
10. Reduzir expiração JWT + refresh tokens (SEC-004)
11. Validar comprimento JWT key (SEC-006)
12. Adicionar comment no `IgnoreQueryFilters` (SEC-007)

### Fase 3 — Qualidade (1 semana)
13. Corrigir `Category.Update()` validação (ARC-007)
14. Corrigir `Card.Update()` validação (ARC-008)
15. Adicionar `[ApiController]` nos controllers faltantes (ARC-005)
16. Padronizar `Account.Name` validator (CODE-012)
17. Corrigir `ExpenseService` `throw new Exception` (ARC-003)
18. Adicionar `[ProducesResponseType]` (FEAT-006)
19. Adicionar Health Check (FEAT-004)
20. Expandir `SubscriptionResponse` (FEAT-008)

### Fase 4 — Features (2-3 semanas)
21. Endpoint change-password (FEAT-001)
22. Endpoint expenses/period (FEAT-002)
23. Paginação (FEAT-003)
24. OpenAPI security scheme (FEAT-005)
25. Soft Delete (FEAT-007)

### Fase 5 — Over-Engineering (1-2 meses)
26. CQRS + MediatR (OE-001)
27. Domain Events (OE-002)
28. Result Pattern (OE-003)
29. Audit Log (OE-007)
30. OpenTelemetry (OE-010)

---

> **Documento gerado em:** Agosto 2026
> **Baseado em:** Análise completa do código fonte do Monetis Backend
