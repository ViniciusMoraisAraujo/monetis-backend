# 🐛 Plano de Correção de Bugs — Monetis

> **Data:** Julho 2026  
> **Bugs validados:** 11 (1 false positive removido)  
> **Esforço estimado total:** ~3 dias + 45 min  

---

## 📊 Estado da Validação

| ID | Bug | Severidade | Status | Fonte Validada |
|:--:|-----|:----------:|:------:|---------------|
| B-001 | `UnitOfWork.Dispose()` — NotImplementedException | 🔴 Crítica | ✅ Confirmado | `UnitOfWork.cs:13` |
| B-002 | `ChangePasswordAsync` — busca por email usando UserId | 🔴 Crítica | ✅ Confirmado | `UserAuthService.cs:29` |
| B-003 | Dupla camada de rate limiting | 🟡 Média | ✅ Confirmado | `RateLimitingMiddleware.cs` + `Program.cs` |
| B-004 | `SubscriptionService.UpdateAsync` não propaga campos | 🔴 Crítica | ✅ Confirmado | `SubscriptionService.cs:67-75` |
| B-005 | Inconsistência em data de vencimento | 🟡 Média | ✅ Confirmado | `Expense.cs:118` vs `ExpenseRepository.cs:32` |
| B-006 | `GetOverdueAsync` usa `IgnoreQueryFilters` | 🟡 Média | ✅ Confirmado | `ExpenseRepository.cs:32` |
| B-007 | `catch(Exception)` genérico mascara erros 500 como 400 | 🟡 Média | ✅ Confirmado | `ExpensesController.cs` (4 métodos) |
| B-008 | `UserResourceGuard` sem verificação de ownership | 🔴 Crítica | ✅ Confirmado | `UserResourceGuard.cs:26-40` |
| B-010 | Limite de descrição — domínio (200) vs coluna `nvarchar(100)` | 🟡 Média | ✅ Confirmado | `Transaction.cs:48` + `TransactionConfiguration.cs:35` |
| B-011 | Descrição de assinatura — 3 limites divergentes | 🟡 Média | ✅ Confirmado | `SubscriptionValidator.cs:27` + `Subscription.cs:163` + `SubscriptionConfiguration.cs:37` |
| B-012 | Expense — validator 200 vs domínio 100 | 🟢 Baixa | ✅ Confirmado | `ExpenseValidator.cs:26,118,71` + `Expense.cs:173` |
| ~~B-009~~ | ~~UsersController sem [Authorize]~~ | — | ❌ **False Positive** | `ApiControllerBase.cs` tem `[Authorize]` — ASP.NET Core herda atributos de classe base |

---

## 🟥 Fase 0 — HOTFIX: Correções Imediatas (Prioridade Máxima)

> **Duração estimada:** 1 dia  
> **Dependências:** Nenhuma  
> **Risco:** Crítico — bugs que quebram funcionalidades ou expõem dados

---

### B-008: UserResourceGuard sem validação de ownership

**Severidade:** 🔴 Crítica — Segurança  
**Onde:** `src/Monetis.Application/Services/UserResourceGuard.cs`  
**Quando:** Agora — vulnerabilidade ativa de multi-tenancy

**Problema:**
`GetOwnedAccountAsync` e `GetOwnedCardAsync` verificam apenas se a entidade **existe**, mas **nunca verificam** se `entity.UserId == CurrentUserId`. Um usuário pode acessar contas/cartões de outro usuário se o query filter do EF Core for contornado.

```csharp
// Código atual (BUG)
public async Task<Account> GetOwnedAccountAsync(Guid accountId, ...)
{
    var account = await accountRepository.GetByIdAsync(accountId, cancellationToken);
    if (account == null) throw new KeyNotFoundException(...);
    return account;  // ❌ NÃO verifica ownership!
}
```

**Como corrigir:**
```csharp
public async Task<Account> GetOwnedAccountAsync(Guid accountId, ...)
{
    var account = await accountRepository.GetByIdAsync(accountId, cancellationToken);
    if (account == null) throw new KeyNotFoundException(...);
    if (account.UserId != CurrentUserId)
        throw new KeyNotFoundException($"Account {accountId} not found.");
    return account;
}
```

**Mesma correção para `GetOwnedCardAsync`.**

**Arquivos afetados:**
- `UserResourceGuard.cs` — adicionar verificação `entity.UserId != CurrentUserId`

---

### B-002: ChangePasswordAsync — busca por email usando UserId

**Severidade:** 🔴 Crítica — Funcionalidade quebrada  
**Onde:** `src/Monetis.Application/Services/UserServices/UserAuthService.cs:29`  
**Quando:** Agora — qualquer tentativa de trocar senha falha

**Problema:**
```csharp
var user = await userRepository.GetUserByEmailAsync(
    userContextAccessor.UserId.ToString(), cancellationToken)  // ❌ GUID como email
    ?? throw new UnauthorizedAccessException();
```

`GetUserByEmailAsync` espera um **email**, mas recebe `UserId.ToString()` (um GUID tipo `"3f8a..."`). A busca sempre retorna `null`, causando `UnauthorizedAccessException`.

**Como corrigir:**
```csharp
var user = await userRepository.GetByIdAsync(
    userContextAccessor.UserId, cancellationToken)
    ?? throw new UnauthorizedAccessException();
```

**Arquivos afetados:**
- `UserAuthService.cs` — trocar `GetUserByEmailAsync` por `GetByIdAsync`

---

### B-001: UnitOfWork.Dispose() — NotImplementedException

**Severidade:** 🔴 Crítica — Crash em runtime  
**Onde:** `src/Monetis.Infrastructure/Persistence/UnitOfWork.cs:13`  
**Quando:** Agora — se alguém usar `using` ou `Dispose()` explícito

**Problema:**
```csharp
public void Dispose()
{
    throw new NotImplementedException();  // ❌ Crash!
}
```

**Como corrigir:**
```csharp
public void Dispose()
{
    monetisDataContext?.Dispose();
    GC.SuppressFinalize(this);
}
```

**Arquivos afetados:**
- `UnitOfWork.cs` — implementar `Dispose()` corretamente

---

### B-004: SubscriptionService.UpdateAsync não propaga campos do request

**Severidade:** 🔴 Crítica — Funcionalidade quebrada  
**Onde:** `src/Monetis.Application/Services/SubscriptionService.cs:67-75`  
**Quando:** Agora — usuário não pode alterar conta/cartão da assinatura

**Problema:**
```csharp
subscription.Update(
    paymentMethod: subscription.PaymentMethod,  // ❌ valor ATUAL
    accountId: subscription.AccountId,           // ❌ valor ATUAL
    endDate: subscription.EndDate,               // ❌ valor ATUAL
    creditCardId: subscription.CardId);          // ❌ valor ATUAL
```

**Como corrigir:**

1. Adicionar campos ao `UpdateSubscriptionRequest` DTO:
```csharp
public record UpdateSubscriptionRequest(
    decimal Amount,
    string Description,
    Frequency Frequency,
    DateTime NextDueDate,
    bool IsActive,
    PaymentMethod? PaymentMethod = null,
    Guid? AccountId = null,
    Guid? CategoryId = null,
    Guid? CardId = null,
    DateTime? EndDate = null
);
```

2. Atualizar `SubscriptionService.UpdateAsync` para passar os novos campos.

3. Atualizar `UpdateSubscriptionRequestValidator` para validar os novos campos opcionais.

**Arquivos afetados:**
- `SubscriptionDtos.cs` — adicionar campos ao DTO
- `SubscriptionService.cs` — passar novos campos
- `SubscriptionValidator.cs` — validar novos campos

---

## 🟧 Fase 1 — Fundação: Bugs Médios (1-2 dias)

> **Duração estimada:** 1-2 dias  
> **Depende de:** Fase 0  
> **Risco:** Moderado

---

### B-007: catch(Exception) genérico mascara erros 500 como 400

**Severidade:** 🟡 Média — Debugging/Monitoramento  
**Onde:** `src/Monetis.API/Controllers/ExpensesController.cs` (4 métodos: Create, CreateInstallments, Pay, Update)  
**Quando:** Após Fase 0

**Problema:**
```csharp
catch (Exception ex)
{
    return BadRequest(ex.Message);  // ❌ Erro 500 vira 400
}
```

Erros de infraestrutura (banco, rede) são mascarados como `400 Bad Request`.

**Como corrigir:**

Remover `catch (Exception)` de todos os métodos do `ExpensesController`. O `ExceptionMiddleware` já captura exceções não tratadas globalmente e retorna o status code apropriado.

```csharp
// DE:
catch (ArgumentException ex) { return BadRequest(ex.Message); }
catch (Exception ex) { return BadRequest(ex.Message); }

// PARA:
catch (ArgumentException ex) { return BadRequest(ex.Message); }
// Exception genérica removida — será capturada pelo ExceptionMiddleware
```

**Arquivos afetados:**
- `ExpensesController.cs` — remover `catch (Exception)` em 4 métodos

---

### B-003: Dupla camada de rate limiting

**Severidade:** 🟡 Média — UX confusa  
**Onde:** `RateLimitingMiddleware.cs` (custom) + `Program.cs:28-34` (nativo)  
**Quando:** Após Fase 0

**Problema:** Duas implementações ativas:

| Camada | Limite | Janela | Storage |
|--------|:------:|:------:|---------|
| Custom Middleware | 5 req | 10s | `IMemoryCache` |
| ASP.NET Core nativo | 100 req | 1 min | TokenBucket |

**Como corrigir:**

**Opção A (recomendada):** Remover o middleware custom `RateLimitingMiddleware.cs` e manter apenas o nativo (100 req/min). O middleware custom nem está registrado no pipeline atualmente.

**Opção B:** Manter o custom e remover o nativo, se o limite mais restrito for desejado.

**Arquivos afetados:**
- `RateLimitingMiddleware.cs` — remover ou manter documentado como não registrado
- `Program.cs` — sem alterações (nativo já é o único ativo)

---

### B-005: Inconsistência em data de vencimento

**Severidade:** 🟡 Média — Consistência  
**Onde:** `Expense.cs:118-122` + `ExpenseRepository.cs:32`  
**Quando:** Após Fase 0

**Problema:**

| Método | Comparação | Efeito |
|--------|-----------|--------|
| `Expense.MarkAsOverDue()` | `DueDate.Date < DateTime.UtcNow.Date` | Apenas **data** |
| `GetOverdueAsync()` | `x.DueDate < DateTime.UtcNow` | **Data + hora** |

Uma despesa com `DueDate = 2026-07-17 23:59:59` pode ser considerada vencida pela query mas não pelo método `MarkAsOverDue()`.

**Como corrigir:**

**Opção A:** Padronizar ambos para comparação de data:
```csharp
// GetOverdueAsync
.Where(x => x.DueDate.Date <= DateTime.UtcNow.Date && ...)

// MarkAsOverDue
if (Status == TransactionStatus.Pending && DueDate.Date <= DateTime.UtcNow.Date)
```

**Opção B:** Padronizar ambos para comparação completa (data + hora), usando `<=`:
```csharp
// GetOverdueAsync (já está assim)
.Where(x => x.DueDate < DateTime.UtcNow && ...)

// MarkAsOverDue
if (Status == TransactionStatus.Pending && DueDate < DateTime.UtcNow)
```

**Recomendação:** Opção A (comparação por data) — mais intuitiva para despesas com vencimento em data específica.

**Arquivos afetados:**
- `Expense.cs` — ajustar `MarkAsOverDue()`
- `ExpenseRepository.cs` — ajustar `GetOverdueAsync()`

---

### B-006: GetOverdueAsync usa IgnoreQueryFilters (documentar intencionalidade)

**Severidade:** 🟡 Média — Segurança/Manutenibilidade  
**Onde:** `ExpenseRepository.cs:32`  
**Quando:** Após Fase 0

**Problema:**
```csharp
return await context.Set<Expense>()
    .IgnoreQueryFilters()  // ❌ Desativa filtro multi-tenant
    .Where(x => x.DueDate < DateTime.UtcNow && x.Status == TransactionStatus.Pending)
    .ToListAsync(cancellationToken);
```

O `IgnoreQueryFilters()` é **intencional e necessário** pois o `BackgroundService` roda sem contexto de usuário. No entanto, não há justificativa documentada e operações de escrita pós-query não verificam segurança adicional.

**Como corrigir:**
```csharp
// Intencional: OverDueExpenseProcessorService executa globalmente
// sem um usuário autenticado. A query busca despesas vencidas de
// TODOS os usuários para processamento em lote.
// A segurança é garantida porque a operação só altera Status para Overdue,
// sem expor dados sensíveis ou permitir escrita não autorizada.
return await context.Set<Expense>()
    .IgnoreQueryFilters()
    .Where(x => x.DueDate < DateTime.UtcNow && x.Status == TransactionStatus.Pending)
    .ToListAsync(cancellationToken);
```

**Arquivos afetados:**
- `ExpenseRepository.cs` — adicionar comentário justificando

---

## 🟩 Fase 2 — Alinhamento de Limites de Descrição (Integridade de Dados)

> **Duração estimada:** ~45 min  
> **Depende de:** Nenhuma  
> **Risco:** Médio — descrições dentro da janela aceita podem causar falha de `INSERT` no SQL Server (truncation)

---

### B-010: Limite de descrição inconsistente — domínio (200) vs coluna `nvarchar(100)`

**Severidade:** 🟡 Média — Integridade de dados  
**Onde:** `Transaction.cs:48` + `TransactionConfiguration.cs:33-36` + migration `20260507163326_InitialCreate`  
**Quando:** Após Fase 0

**Problema:**

| Camada | Limite | Fonte |
|--------|:------:|-------|
| Domínio (`Transaction.ValidateDescription`) | 200 | `Transaction.cs:48` |
| EF Configuration (`HasMaxLength`) | 100 | `TransactionConfiguration.cs:35` |
| Banco (`Incomes`/`Transfers`/`Expenses`.Description) | `nvarchar(100)` | Migration |

Como o mapeamento é **TPC**, a coluna `Description` é configurada uma única vez na base (`TransactionConfiguration`) e aplicada às três tabelas. Uma **receita ou transferência** com descrição entre **100 e 200** caracteres passa na validação de domínio, mas o `INSERT` falha no SQL Server (`String or binary data would be truncated`).

**Como corrigir:**

**Opção A (recomendada — preserva os 200 do domínio):** ampliar a coluna para `nvarchar(200)`:
```csharp
// TransactionConfiguration.cs
builder.Property(x => x.Description)
    .IsRequired()
    .HasMaxLength(200)          // antes: 100
    .HasColumnType("nvarchar(200)") // antes: nvarchar(100)
```
Criar migration (`dotnet ef migrations add AlignTransactionDescriptionLength`) para ampliar as três colunas. O domínio de `Expense` (100, `Expense.cs:173`) continua valendo antes da persistência.

**Opção B (sem migration):** reduzir a validação de domínio para 100 em `Transaction.cs:48` — reduz o limite de Income/Transfer, contrariando a documentação existente (200).

**Arquivos afetados:**
- `TransactionConfiguration.cs` — `HasMaxLength(100)` → `200` (Opção A)
- Nova migration (Opção A)
- `Transaction.cs` — `ValidateDescription` (Opção B)

---

### B-011: Descrição de assinatura — 3 limites divergentes

**Severidade:** 🟡 Média — Integridade de dados  
**Onde:** `SubscriptionValidator.cs:27` + `Subscription.cs:163` + `SubscriptionConfiguration.cs:35-38`  
**Quando:** Após Fase 0

**Problema:**

| Camada | Limite | Fonte |
|--------|:------:|-------|
| FluentValidation (`MaximumLength`) | 100 | `SubscriptionValidator.cs:27,57` |
| Domínio (`Subscription`) | 200 | `Subscription.cs:163` |
| Banco (`Subscriptions.Description`) | `nvarchar(50)` | `SubscriptionConfiguration.cs:37` |

Uma descrição entre **51 e 100** caracteres passa no validator (≤100) e no domínio (≤200), mas a coluna é `nvarchar(50)` → falha de truncation no `INSERT`. Descrições entre 50 e 100 já quebram o cadastro.

**Como corrigir:**

**Opção A (recomendada — unificar em 100):** ampliar a coluna para `nvarchar(100)` e alinhar o domínio a 100:
- `SubscriptionConfiguration.cs` → `HasMaxLength(100)` / `HasColumnType("nvarchar(100)")`
- `Subscription.cs:163` → `description.Length > 100` (ou validar exatamente como o validator)
- Migration para alterar a coluna

**Opção B:** padronizar em 200 — ampliar coluna para `nvarchar(200)` e validator para `MaximumLength(200)`.

> ⚠️ O validator também aplica `Matches(@"^[a-zA-ZÀ-ÿ\s0-9\-_]+$")` — mantê-lo consistente com o limite escolhido.

**Arquivos afetados:**
- `SubscriptionConfiguration.cs` — limite da coluna
- `SubscriptionValidator.cs` — `MaximumLength`
- `Subscription.cs` — `ValidateDescription`
- Nova migration

---

### B-012: Expense — validator permite 200, domínio rejeita >100

**Severidade:** 🟢 Baixa — UX/consistência  
**Onde:** `ExpenseValidator.cs:26` (Create) + `ExpenseValidator.cs:118` (Update) + `ExpenseValidator.cs:71` (Installment) + `Expense.cs:173`  
**Quando:** Após Fase 2 (requer decisão do B-010)

**Problema:**

| Camada | Limite |
|--------|:------:|
| FluentValidation (`MaximumLength`) | 200 |
| Domínio (`Expense.ValidateExpense`) | 100 |

Os validators de despesa aceitam até **200** caracteres, mas a entidade lança `ExpenseDescriptionInvalidException` para descrições `> 100`. Um usuário com descrição entre 101 e 200 recebe um `400 BUSINESS_ERROR` (mensagem de domínio) em vez de um erro de validação claro do DTO.

**Detalhe adicional (parcelas):** o domínio concatena o sufixo `" (i/N)"` na descrição de cada parcela (`Expense.cs:81`). Uma descrição-base próxima de 100 pode estourar o limite **após** a concatenação.

**Como corrigir:**
```csharp
// ExpenseValidator.cs — em Create, Update e Installment
RuleFor(x => x.Description)
    .MaximumLength(100)   // antes: 200 — alinhar à entidade
    .WithMessage("Description cannot exceed 100 characters");
```
Para parcelas, considerar reservar espaço para o sufixo (ex.: limitar a base a ~90 caracteres) ou ajustar a validação do domínio.

**Arquivos afetados:**
- `ExpenseValidator.cs` — `MaximumLength(200)` → `100` nos 3 validators
- `ExpenseService.cs` (parcelas) — avaliar espaço para o sufixo `" (i/N)"`

---

## 🟢 B-009: Removido — False Positive

O bug B-009 (`UsersController` sem `[Authorize]`) foi **removido** após validação.

**Motivo:** Em ASP.NET Core, o atributo `[Authorize]` definido na classe base **é herdado** pelas classes derivadas e por todas as actions. O `ApiControllerBase` já possui `[Authorize(AuthenticationSchemes = "Bearer")]`, portanto:

| Action | Atributo | Protegido? |
|--------|----------|:----------:|
| `GetAll` | (herdado da base) | ✅ Sim |
| `GetById` | (herdado da base) | ✅ Sim |
| `Create` | `[AllowAnonymous]` | ✅ Correto (público) |
| `Update` | `[Authorize]` (redundante) | ✅ Sim |
| `Delete` | (herdado da base) | ✅ Sim |

---

## 📋 Resumo das Correções

| Fase | ID | Arquivo | Correção | Esforço |
|:----:|:--:|---------|----------|:-------:|
| 🟥 0 | B-008 | `UserResourceGuard.cs` | Adicionar verificação `entity.UserId != CurrentUserId` | 15 min |
| 🟥 0 | B-002 | `UserAuthService.cs` | Trocar `GetUserByEmailAsync` por `GetByIdAsync` | 5 min |
| 🟥 0 | B-001 | `UnitOfWork.cs` | Implementar `Dispose()` delegando ao DbContext | 5 min |
| 🟥 0 | B-004 | `SubscriptionDtos.cs` + `SubscriptionService.cs` | Adicionar campos ao DTO + propagar no service | 30 min |
| 🟧 1 | B-007 | `ExpensesController.cs` | Remover `catch (Exception)` genérico | 15 min |
| 🟧 1 | B-003 | `RateLimitingMiddleware.cs` | Remover middleware custom (não registrado) | 5 min |
| 🟧 1 | B-005 | `Expense.cs` + `ExpenseRepository.cs` | Padronizar comparação de data | 15 min |
| 🟧 1 | B-006 | `ExpenseRepository.cs` | Adicionar comentário justificando `IgnoreQueryFilters` | 5 min |
| 🟩 2 | B-010 | `TransactionConfiguration.cs` + migration | Ampliar `Description` para `nvarchar(200)` (ou reduzir domínio p/ 100) | 20 min |
| 🟩 2 | B-011 | `SubscriptionConfiguration.cs` + `SubscriptionValidator.cs` + `Subscription.cs` | Unificar limite (coluna/validator/domínio) | 20 min |
| 🟩 2 | B-012 | `ExpenseValidator.cs` | Alinhar `MaximumLength` para 100 | 5 min |
| | **Total** | | | **~2 horas** |
