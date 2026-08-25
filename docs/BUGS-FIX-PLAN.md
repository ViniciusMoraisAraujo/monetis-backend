# 🐛 Plano de Correção de Bugs — Monetis

> **Data:** Julho 2026  
> **Bugs validados:** 8 (1 false positive removido)  
> **Esforço estimado total:** ~3 dias  

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
| | **Total** | | | **~1.5 horas** |
