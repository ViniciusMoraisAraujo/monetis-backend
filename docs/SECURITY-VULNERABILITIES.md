# 🔒 Vulnerabilidades de Segurança — Monetis Backend

> **Auditoria:** Julho 2026
> **Total de falhas:** 17 (2 críticas, 7 médias, 7 baixas, 1 informativa)
> **Baseado em:** Análise de código fonte + documentação em `docs/`

---

## Sumário

- [🔴 Críticas (Ação Imediata)](#-críticas-ação-imediata)
  - [C-01: UserResourceGuard sem verificação de ownership](#c-01-userresourceguard-sem-verificação-de-ownership)
  - [C-02: ChangePasswordAsync busca por email usando GUID](#c-02-changepasswordasync-busca-por-email-usando-guid)
- [🟡 Médias](#-médias)
  - [M-01: GET /api/users expõe dados de todos os usuários](#m-01-get-apiusers-expõe-dados-de-todos-os-usuários)
  - [M-02: GET /api/users/{id} sem verificação de ownership](#m-02-get-apiusersid-sem-verificação-de-ownership)
  - [M-03: Sem bloqueio de conta após múltiplas tentativas de login](#m-03-sem-bloqueio-de-conta-após-múltiplas-tentativas-de-login)
  - [M-04: Token JWT sem refresh/revogação e expiração longa (2 dias)](#m-04-token-jwt-sem-refreshrevogação-e-expiração-longa-2-dias)
  - [M-05: IgnoreQueryFilters sem justificativa e sem verificação de segurança](#m-05-ignorequeryfilters-sem-justificativa-e-sem-verificação-de-segurança)
  - [M-06: CORS configurado como AllowedHosts: "*"](#m-06-cors-configurado-como-allowedhosts-)
  - [M-07: UnauthorizedAccessException tratado como 500 em vez de 401](#m-07-unauthorizedaccessexception-tratado-como-500-em-vez-de-401)
- [🟢 Baixas / Melhorias](#-baixas--melhorias)
  - [L-01: Senha sem validação de formato na entidade de domínio User](#l-01-senha-sem-validação-de-formato-na-entidade-de-domínio-user)
  - [L-02: catch(Exception) genérico mascara erros de infraestrutura como 400](#l-02-catchexception-genérico-mascara-erros-de-infraestrutura-como-400)
  - [L-03: JWT Key sem validação de força mínima](#l-03-jwt-key-sem-validação-de-força-mínima)
  - [L-04: Expiração de token muito longa (2 dias)](#l-04-expiração-de-token-muito-longa-2-dias)
  - [L-05: Sem verificação de email no cadastro](#l-05-sem-verificação-de-email-no-cadastro)
  - [L-06: DTOs de resposta expõem UserId](#l-06-dtos-de-resposta-expõem-userid)
  - [L-07: ExceptionMiddleware captura tudo como 500](#l-07-exceptionmiddleware-captura-tudo-como-500)
- [📋 Matriz de Prioridades](#-matriz-de-prioridades)
- [🎯 Plano de Correção por Fase](#-plano-de-correção-por-fase)

---

## 🔴 Críticas (Ação Imediata)

---

### C-01: UserResourceGuard sem verificação de ownership

| Campo | Valor |
|-------|-------|
| **Severidade** | 🔴 **Crítica — Violação de multi-tenancy** |
| **Arquivo** | `src/Monetis.Application/Services/UserResourceGuard.cs` |
| **Linhas** | 26–40 |
| **Já documentado em** | `docs/BUGS-FIX-PLAN.md` (B-008) |

#### Descrição

Os métodos `GetOwnedAccountAsync()` e `GetOwnedCardAsync()` verificam apenas se a entidade **existe** no banco de dados, mas **NUNCA verificam** se `entity.UserId == CurrentUserId`. Isso significa que um usuário pode acessar contas e cartões de **qualquer outro usuário** se o query filter do EF Core for contornado por qualquer motivo.

#### Código Vulnerável

```csharp
// Linhas 26–33: GetOwnedAccountAsync — NÃO verifica ownership!
public async Task<Account> GetOwnedAccountAsync(Guid accountId, CancellationToken cancellationToken = default)
{
    var account = await accountRepository.GetByIdAsync(accountId, cancellationToken);
    if (account == null)
        throw new KeyNotFoundException($"Account with id {accountId} not found.");

    return account;  // ❌ Deveria verificar account.UserId != CurrentUserId
}

// Linhas 35–40: GetOwnedCardAsync — mesmo problema!
public async Task<Card> GetOwnedCardAsync(Guid cardId, CancellationToken cancellationToken = default)
{
    var card = await cardRepository.GetByIdAsync(cardId, cancellationToken);
    if (card == null)
        throw new KeyNotFoundException($"Card with id {cardId} not found.");

    return card;  // ❌ Deveria verificar card.UserId != CurrentUserId
}
```

#### Impacto

- Usuário A pode acessar contas bancárias e cartões de crédito do Usuário B
- Violação completa do isolamento multi-tenant
- Exposição de dados financeiros sensíveis (saldos, nomes de contas)

#### Como Corrigir

Adicionar verificação `entity.UserId != CurrentUserId` em ambos os métodos:

```csharp
public async Task<Account> GetOwnedAccountAsync(Guid accountId, CancellationToken cancellationToken = default)
{
    var account = await accountRepository.GetByIdAsync(accountId, cancellationToken);
    if (account == null)
        throw new KeyNotFoundException($"Account with id {accountId} not found.");

    if (account.UserId != CurrentUserId)                              // ✅ NOVO
        throw new KeyNotFoundException($"Account with id {accountId} not found.");  // ✅ NOVO

    return account;
}

public async Task<Card> GetOwnedCardAsync(Guid cardId, CancellationToken cancellationToken = default)
{
    var card = await cardRepository.GetByIdAsync(cardId, cancellationToken);
    if (card == null)
        throw new KeyNotFoundException($"Card with id {cardId} not found.");

    if (card.UserId != CurrentUserId)                                 // ✅ NOVO
        throw new KeyNotFoundException($"Card with id {cardId} not found.");        // ✅ NOVO

    return card;
}
```

**Esforço:** ~15 minutos | **Arquivos afetados:** 1

---

### C-02: ChangePasswordAsync busca por email usando GUID

| Campo | Valor |
|-------|-------|
| **Severidade** | 🔴 **Crítica — Funcionalidade de segurança quebrada** |
| **Arquivo** | `src/Monetis.Application/Services/UserServices/UserAuthService.cs` |
| **Linha** | 29 |
| **Já documentado em** | `docs/BUGS-FIX-PLAN.md` (B-002) |

#### Descrição

O método `ChangePasswordAsync` usa `GetUserByEmailAsync()` passando o `UserId` (um GUID) convertido para string como se fosse um email. O repositório sempre retorna `null`, e o método lança `UnauthorizedAccessException`, impedindo qualquer usuário de trocar sua senha.

#### Código Vulnerável

```csharp
// Linha 29: ❌ Passa GUID como email!
var user = await userRepository.GetUserByEmailAsync(
    userContextAccessor.UserId.ToString(), cancellationToken)  // "3fa85f64-5717-4562-b3fc-2c963f66afa6" não é email!
    ?? throw new UnauthorizedAccessException();
```

#### Impacto

- Nenhum usuário consegue trocar sua senha
- Se a senha atual for comprometida, o usuário não tem como se proteger
- Frustração de UX — feature de segurança essencial quebrada

#### Como Corrigir

Substituir `GetUserByEmailAsync` por `GetByIdAsync`:

```csharp
// ✅ Correto: busca por ID
var user = await userRepository.GetByIdAsync(
    userContextAccessor.UserId, cancellationToken)
    ?? throw new UnauthorizedAccessException();
```

**Esforço:** ~5 minutos | **Arquivos afetados:** 1

---

## 🟡 Médias

---

### M-01: GET /api/users expõe dados de todos os usuários

| Campo | Valor |
|-------|-------|
| **Severidade** | 🟡 **Média — Privacidade de dados (LGPD)** |
| **Arquivo** | `src/Monetis.API/Controllers/UsersController.cs` |
| **Linhas** | 23–28 |
| **Já documentado em** | *Novo — não consta no BUGS-FIX-PLAN.md* |

#### Descrição

Qualquer usuário autenticado pode listar **todos os usuários do sistema**, incluindo seus nomes e emails. Isso viola o princípio de最小 privilégio e expõe dados pessoais protegidos pela LGPD.

#### Código Vulnerável

```csharp
// Linhas 23–28
[HttpGet]
public async Task<ActionResult<IEnumerable<UserResponse>>> GetAll(CancellationToken cancellationToken)
{
    var users = await userService.GetAllAsync(cancellationToken);
    return Ok(users);  // ❌ Retorna dados de TODOS os usuários!
}
```

#### Impacto

- Vazamento de emails válidos para ataques de força bruta
- Violação de privacidade (LGPD — Lei Geral de Proteção de Dados)
- Usuário consegue enumerar todos os usuários da plataforma

#### Como Corrigir

**Opção A (Recomendada):** Remover o endpoint — não há caso de uso legítimo para listar todos os usuários:

```csharp
// Remover completamente o método GetAll
```

**Opção B:** Restringir a administradores (se houver role de admin no futuro):

```csharp
[HttpGet]
[Authorize(Roles = "Admin")]
public async Task<ActionResult<IEnumerable<UserResponse>>> GetAll(CancellationToken cancellationToken)
{
    var users = await userService.GetAllAsync(cancellationToken);
    return Ok(users);
}
```

**Esforço:** ~5 minutos | **Arquivos afetados:** 1

---

### M-02: GET /api/users/{id} sem verificação de ownership

| Campo | Valor |
|-------|-------|
| **Severidade** | 🟡 **Média — Privacidade de dados** |
| **Arquivo** | `src/Monetis.API/Controllers/UsersController.cs` |
| **Linhas** | 13–20 |
| **Já documentado em** | *Novo* |

#### Descrição

Qualquer usuário autenticado pode ler dados de **qualquer outro usuário** apenas sabendo seu GUID. O endpoint não verifica se o usuário logado é o proprietário do recurso solicitado.

#### Código Vulnerável

```csharp
// Linhas 13–20
[HttpGet("{id}")]
public async Task<ActionResult<UserResponse>> GetById(Guid id, CancellationToken cancellationToken)
{
    var user = await userService.GetByIdAsync(id, cancellationToken);
    if (user == null)
        return NotFound("User not found.");

    return Ok(user);  // ❌ Sem verificação: usuário A lê dados do usuário B
}
```

#### Impacto

- Qualquer usuário autenticado pode ler nome, sobrenome e email de qualquer outro usuário
- Facilita ataques de engenharia social e phishing

#### Como Corrigir

**Opção A (Recomendada):** Restringir para que o usuário só leia seus próprios dados:

```csharp
[HttpGet("{id}")]
public async Task<ActionResult<UserResponse>> GetById(Guid id, CancellationToken cancellationToken)
{
    if (id != UserId)  // ✅ Verifica se é o próprio usuário
        return Forbid();

    var user = await userService.GetByIdAsync(id, cancellationToken);
    if (user == null)
        return NotFound("User not found.");

    return Ok(user);
}
```

**Nota:** `UserId` é herdado de `ApiControllerBase`.

**Esforço:** ~10 minutos | **Arquivos afetados:** 1

---

### M-03: Sem bloqueio de conta após múltiplas tentativas de login

| Campo | Valor |
|-------|-------|
| **Severidade** | 🟡 **Média — Força bruta de senha** |
| **Arquivo** | `src/Monetis.Application/Services/UserServices/UserAuthService.cs` |
| **Linhas** | 11–22 |
| **Já documentado em** | *Novo* |

#### Descrição

Não há mecanismo de **account lockout**, **delay progressivo** ou **registro de tentativas falhas**. Um atacante pode tentar senhas indefinidamente sem qualquer consequência.

#### Código Vulnerável

```csharp
// Linhas 11–22
public async Task<string> LoginAsync(LoginUserRequest loginDto, CancellationToken cancellationToken = default)
{
    var user = await userRepository.GetUserByEmailAsync(loginDto.Email, cancellationToken);

    if (user == null)
        throw new UnauthorizedAccessException("Invalid credentials.");

    var passwordIsValid = passwordHasher.Verify(loginDto.Password, user.PasswordHash);
    if (!passwordIsValid)
        throw new UnauthorizedAccessException("Invalid credentials.");  // ❌ Sem contagem de tentativas!

    var token = tokenService.GenerateToken(user.Id, user.Email);
    return token;
}
```

#### Impacto

- Ataque de força bruta ilimitado contra senhas de usuários
- Sem rate limiting específico para o endpoint de login (apenas global de 100 req/min)
- Dicionários de senhas comuns podem ser testados rapidamente

#### Como Corrigir

**Passo 1:** Adicionar campos de bloqueio na entidade `User`:

```csharp
// src/Monetis.Domain/Entities/User.cs
public int FailedLoginAttempts { get; private set; }
public DateTime? LockedUntil { get; private set; }

public void RecordFailedLogin()
{
    FailedLoginAttempts++;
    if (FailedLoginAttempts >= 5)
        LockedUntil = DateTime.UtcNow.AddMinutes(15);
}

public void ResetFailedLogin()
{
    FailedLoginAttempts = 0;
    LockedUntil = null;
}

public bool IsLocked => LockedUntil.HasValue && LockedUntil > DateTime.UtcNow;
```

**Passo 2:** Adicionar verificação no `UserAuthService.LoginAsync`:

```csharp
public async Task<string> LoginAsync(LoginUserRequest loginDto, CancellationToken cancellationToken = default)
{
    var user = await userRepository.GetUserByEmailAsync(loginDto.Email, cancellationToken);

    if (user == null)
        throw new UnauthorizedAccessException("Invalid credentials.");

    if (user.IsLocked)                                          // ✅ NOVO
        throw new UnauthorizedAccessException("Account is temporarily locked.");  // ✅ NOVO

    var passwordIsValid = passwordHasher.Verify(loginDto.Password, user.PasswordHash);
    if (!passwordIsValid)
    {
        user.RecordFailedLogin();                                // ✅ NOVO
        userRepository.Update(user);
        await unitOfWork.CommitAsync(cancellationToken);         // ✅ NOVO
        throw new UnauthorizedAccessException("Invalid credentials.");
    }

    user.ResetFailedLogin();                                     // ✅ NOVO
    userRepository.Update(user);
    await unitOfWork.CommitAsync(cancellationToken);             // ✅ NOVO

    var token = tokenService.GenerateToken(user.Id, user.Email);
    return token;
}
```

**Passo 3:** Adicionar migração para os novos campos.

**Esforço:** ~2–3 horas | **Arquivos afetados:** `User.cs`, `UserAuthService.cs`, migração

---

### M-04: Token JWT sem refresh/revogação e expiração longa (2 dias)

| Campo | Valor |
|-------|-------|
| **Severidade** | 🟡 **Média — Sessão não gerenciável** |
| **Arquivo** | `src/Monetis.Infrastructure/Security/TokenService.cs` |
| **Linha** | 33 |
| **Já documentado em** | *Novo* |

#### Descrição

O token JWT:
1. Expira em **2 dias** (muito longo para um token sem revogação)
2. **Não há** endpoint de refresh token
3. **Não há** como revogar tokens individualmente
4. Troca de senha **não invalida** tokens existentes

#### Código Vulnerável

```csharp
// Linha 33 — TokenService.cs
Expires = DateTime.UtcNow.AddDays(2),  // ❌ 2 dias sem possibilidade de revogação
```

#### Impacto

- Token roubado continua válido por até 2 dias
- Usuário não consegue invalidar sessões ativas ao trocar a senha
- Sem mecanismo de "log out de todos os dispositivos"

#### Como Corrigir

**Opção A (Recomendada — refresh tokens):**
1. Reduzir expiração do access token para 15–60 minutos
2. Criar tabela `RefreshTokens`:
   ```csharp
   public class RefreshToken
   {
       public Guid Id { get; set; }
       public Guid UserId { get; set; }
       public string Token { get; set; }
       public DateTime ExpiresAt { get; set; }
       public bool IsRevoked { get; set; }
       public DateTime CreatedAt { get; set; }
   }
   ```
3. Criar endpoint `POST /api/auth/refresh`
4. Invalidar refresh tokens na troca de senha

**Opção B (Simples — reduzir expiração):**

```csharp
// TokenService.cs — Linha 33
Expires = DateTime.UtcNow.AddHours(1),  // ✅ Reduzido para 1 hora
```

**Esforço:** 4–6 horas (refresh tokens) / 5 minutos (reduzir expiração)

---

### M-05: IgnoreQueryFilters sem justificativa e sem verificação de segurança

| Campo | Valor |
|-------|-------|
| **Severidade** | 🟡 **Média — Risco de segurança em operação global** |
| **Arquivo** | `src/Monetis.Infrastructure/Persistence/Repositories/ExpenseRepository.cs` |
| **Linha** | 30–35 |
| **Já documentado em** | `docs/BUGS-FIX-PLAN.md` (B-006) |

#### Descrição

O método `GetOverdueAsync` usa `.IgnoreQueryFilters()`, que desativa os filtros multi-tenant. Embora seja intencional para o `BackgroundService` (que roda sem contexto de usuário), não há documentação explicando o motivo, e o método público pode ser chamado por outros serviços sem a devida precaução.

#### Código Vulnerável

```csharp
// Linhas 30–35
public async Task<IEnumerable<Expense>> GetOverdueAsync(CancellationToken cancellationToken = default)
{
    return await context.Set<Expense>()
        .IgnoreQueryFilters()  // ❌ Desativa isolamento multi-tenant — sem justificativa!
        .Where(x => x.DueDate < DateTime.UtcNow && x.Status == TransactionStatus.Pending)
        .ToListAsync(cancellationToken);
}
```

#### Impacto

- Se um serviço de aplicação chamar `GetOverdueAsync`, ele vai receber despesas de **todos os usuários**
- A operação atual (marcar como Overdue) é segura, mas o método pode ser usado incorretamente no futuro
- Código sem documentação é propenso a erros

#### Como Corrigir

Adicionar comentário justificando o uso de `IgnoreQueryFilters`:

```csharp
/// <summary>
/// Obtém todas as despesas vencidas de TODOS os usuários para processamento em lote.
/// 
/// ⚠️ ATENÇÃO: O uso de IgnoreQueryFilters() é INTENCIONAL porque:
/// 1. Este método é chamado exclusivamente pelo OverDueExpenseProcessorService
///    (BackgroundService), que executa sem um contexto de usuário autenticado.
/// 2. O processamento apenas altera o Status das despesas para "Overdue",
///    sem expor dados sensíveis ou permitir escrita não autorizada.
/// 3. A segurança é garantida porque a operação é idempotente e não
///    retorna dados para o cliente HTTP.
/// 
/// 🚫 NÃO use este método em endpoints de API ou serviços de aplicação
///    que operam no contexto de um usuário autenticado.
/// </summary>
public async Task<IEnumerable<Expense>> GetOverdueAsync(CancellationToken cancellationToken = default)
{
    return await context.Set<Expense>()
        .IgnoreQueryFilters()
        .Where(x => x.DueDate < DateTime.UtcNow && x.Status == TransactionStatus.Pending)
        .ToListAsync(cancellationToken);
}
```

**Esforço:** ~5 minutos | **Arquivos afetados:** 1

---

### M-06: CORS configurado como AllowedHosts: "*"

| Campo | Valor |
|-------|-------|
| **Severidade** | 🟡 **Média — Acesso de origens não autorizadas** |
| **Arquivo** | `src/Monetis.API/appsettings.json` |
| **Linha** | 7 |
| **Já documentado em** | *Novo* |

#### Descrição

A configuração `AllowedHosts: "*"` permite que qualquer domínio acesse a API, abrindo portas para ataques de CSRF e outros abusos de origem cruzada.

#### Código Vulnerável

```json
{
  "Logging": { ... },
  "AllowedHosts": "*"    // ❌ Permite qualquer origem
}
```

#### Impacto

- Qualquer site pode fazer requisições para a API
- Aumenta a superfície de ataque
- Riscos de CSRF (embora mitigados por JWT Bearer que não é automático como cookies)

#### Como Corrigir

**Opção A (Recomendada para produção):** Especificar origens legítimas via variável de ambiente:

```json
{
  "Logging": { ... },
  "AllowedHosts": "localhost:3000,meuapp.com"  // ✅ Restrito
}
```

**Opção B (Se usar CORS do ASP.NET Core):**
Adicionar configuração CORS no `Program.cs`:

```csharp
// Program.cs
builder.Services.AddCors(options =>
{
    options.AddPolicy("MonetisCors", policy =>
    {
        policy.WithOrigins("https://meuapp.com", "http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// ...
app.UseCors("MonetisCors");
```

**Esforço:** ~5 minutos | **Arquivos afetados:** `appsettings.json`, `Program.cs`

---

### M-07: UnauthorizedAccessException tratado como 500 em vez de 401

| Campo | Valor |
|-------|-------|
| **Severidade** | 🟡 **Média — Vazamento de informação / mau comportamento HTTP** |
| **Arquivo** | `src/Monetis.API/Middlewares/ExceptionMiddleware.cs` |
| **Linhas** | 33–46 |
| **Já documentado em** | *Novo* |

#### Descrição

O `ExceptionMiddleware` não mapeia `UnauthorizedAccessException` para HTTP 401. Em vez disso, ela cai no `catch-all` que retorna **500 Internal Server Error**.

#### Código Vulnerável

```csharp
// Linhas 37–46 — Não trata UnauthorizedAccessException!
var (statusCode, message, errorcode) = exception switch
{
    DomainException => (StatusCodes.Status400BadRequest, exception.Message, "BUSINESS_ERROR"), 
    ArgumentException => (StatusCodes.Status400BadRequest, "Bad Request", "04X0"),
    KeyNotFoundException => (StatusCodes.Status404NotFound, "Not Found", "04X4"),
    _ => (StatusCodes.Status500InternalServerError, "Internal error", "07X0")
    // ❌ UnauthorizedAccessException cai aqui como 500!
};
```

#### Impacto

- Tentativas de login inválidas retornam 500 em vez de 401
- Cliente não consegue distinguir entre "não autenticado" e "erro do servidor"
- Dificulta debugging e tratamento de erros no frontend

#### Como Corrigir

Adicionar mapeamento para `UnauthorizedAccessException`:

```csharp
var (statusCode, message, errorcode) = exception switch
{
    DomainException => (StatusCodes.Status400BadRequest, exception.Message, "BUSINESS_ERROR"), 
    ArgumentException => (StatusCodes.Status400BadRequest, "Bad Request", "04X0"),
    KeyNotFoundException => (StatusCodes.Status404NotFound, "Not Found", "04X4"),
    UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, exception.Message, "UNAUTHORIZED"),  // ✅ NOVO
    _ => (StatusCodes.Status500InternalServerError, "Internal error", "07X0")
};
```

**Esforço:** ~5 minutos | **Arquivos afetados:** 1

---

## 🟢 Baixas / Melhorias

---

### L-01: Senha sem validação de formato na entidade de domínio User

| Campo | Valor |
|-------|-------|
| **Severidade** | 🟢 **Baixa — Defesa em profundidade** |
| **Arquivo** | `src/Monetis.Domain/Entities/User.cs` |
| **Linhas** | 20–29 |
| **Já documentado em** | *Novo* |

#### Descrição

A entidade `User` valida `firstName`, `lastName` e `email` no construtor, mas **não valida a senha** (apenas verifica se o hash não é nulo). A validação de formato da senha só existe no FluentValidation (`UserValidator.cs`), que pode ser contornada se o serviço for chamado sem validação.

#### Código Vulnerável

```csharp
// Linhas 20–29 — User.cs
public User(string firstName, string lastName, string email, string passwordHash)
{
    Validate(firstName, lastName, email);  // ✅ Valida nome e email
    
    if (string.IsNullOrWhiteSpace(passwordHash))
        throw new UserPasswordRequiredException();  // ✅ Só verifica se não é nulo

    FirstName = firstName.Trim();
    LastName = lastName.Trim();
    Email = email.Trim().ToLowerInvariant();
    PasswordHash = passwordHash;  // ❌ Não valida formato/força da senha
}
```

#### Como Corrigir

Adicionar validação de senha na entidade (defesa em profundidade):

```csharp
private static readonly Regex PasswordRegex = new(
    @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
    RegexOptions.Compiled);

public User(string firstName, string lastName, string email, string passwordHash)
{
    Validate(firstName, lastName, email);
    ValidatePassword(passwordHash);  // ✅ NOVO
    
    // ...
}

private void ValidatePassword(string passwordHash)
{
    if (string.IsNullOrWhiteSpace(passwordHash))
        throw new UserPasswordRequiredException();
    // A validação de formato é feita no validator (FluentValidation)
    // A entidade recebe o hash, não a senha em texto — validar hash é suficiente
}
```

> **Nota:** Como a entidade recebe o hash (não a senha em texto), a validação real de formato da senha pertence ao FluentValidation. Esta melhoria é documentar que a decisão é **intencional** (defesa em profundidade está no validator).

**Esforço:** ~15 minutos (documentação) | **Arquivos afetados:** 1

---

### L-02: catch(Exception) genérico mascara erros de infraestrutura como 400

| Campo | Valor |
|-------|-------|
| **Severidade** | 🟢 **Baixa — Debugging/Monitoramento** |
| **Arquivo** | `src/Monetis.API/Controllers/ExpensesController.cs` |
| **Linhas** | Várias (4 métodos) |
| **Já documentado em** | `docs/BUGS-FIX-PLAN.md` (B-007) |

#### Descrição

Quatro métodos no `ExpensesController` têm `catch (Exception)` genérico que retorna `400 Bad Request` com a mensagem da exceção. Erros de infraestrutura (banco de dados, rede) são mascarados como erros de validação.

#### Código Vulnerável

```csharp
catch (Exception ex)
{
    return BadRequest(ex.Message);  // ❌ Erro 500 vira 400
}
```

#### Como Corrigir

Remover `catch (Exception)` de todos os métodos. O `ExceptionMiddleware` já captura exceções não tratadas globalmente e retorna o status code apropriado.

```csharp
// DE:
catch (ArgumentException ex) { return BadRequest(ex.Message); }
catch (Exception ex) { return BadRequest(ex.Message); }  // ❌ Remover

// PARA:
catch (ArgumentException ex) { return BadRequest(ex.Message); }
// Exception genérica removida — será capturada pelo ExceptionMiddleware como 500
```

**Esforço:** ~15 minutos | **Arquivos afetados:** 1

---

### L-03: JWT Key sem validação de força mínima

| Campo | Valor |
|-------|-------|
| **Severidade** | 🟢 **Baixa — Configuração segura** |
| **Arquivo** | `src/Monetis.Infrastructure/Security/TokenService.cs` |
| **Linhas** | 17–18 |
| **Já documentado em** | *Novo* |

#### Descrição

A única validação da chave JWT é se ela não é nula/vazia. Não há verificação de comprimento mínimo (recomendado: 32+ caracteres).

#### Código Vulnerável

```csharp
// Linhas 17–18
if (string.IsNullOrWhiteSpace(jwtKey))
    throw new InvalidOperationException("JWT key is not configured.");
// ❌ Não verifica comprimento mínimo!
```

#### Como Corrigir

Adicionar validação de comprimento mínimo:

```csharp
if (string.IsNullOrWhiteSpace(jwtKey))
    throw new InvalidOperationException("JWT key is not configured.");

if (jwtKey.Length < 32)  // ✅ NOVO
    throw new InvalidOperationException("JWT key must be at least 32 characters long.");
```

**Esforço:** ~5 minutos | **Arquivos afetados:** 1

---

### L-04: Expiração de token muito longa (2 dias)

| Campo | Valor |
|-------|-------|
| **Severidade** | 🟢 **Baixa — Prática recomendada** |
| **Arquivo** | `src/Monetis.Infrastructure/Security/TokenService.cs` |
| **Linha** | 33 |
| **Já documentado em** | *Novo* (relacionado a M-04) |

#### Descrição

O token expira em 2 dias. Para um app financeiro que lida com dados sensíveis, o recomendado é 15–60 minutos com refresh tokens.

#### Código Vulnerável

```csharp
Expires = DateTime.UtcNow.AddDays(2),  // ❌ 2 dias
```

#### Como Corrigir

```csharp
Expires = DateTime.UtcNow.AddHours(1),  // ✅ 1 hora (com refresh token)
```

**Esforço:** ~5 minutos | **Arquivos afetados:** 1

---

### L-05: Sem verificação de email no cadastro

| Campo | Valor |
|-------|-------|
| **Severidade** | 🟢 **Baixa — Melhoria de segurança** |
| **Arquivo** | `src/Monetis.Application/Services/UserServices/UserService.cs` |
| **Já documentado em** | *Novo* |

#### Descrição

O endpoint `POST /api/users` (`[AllowAnonymous]`) cria usuários sem qualquer verificação de email (confirmação de cadastro). Um atacante pode criar contas com emails falsos.

#### Impacto

- Criação de contas falsas sem verificação
- Possível abuso do sistema (spam, scraping)
- Impossibilidade de recuperação de senha (sem email confirmado)

#### Como Corrigir

Implementar fluxo de confirmação de email:
1. Campo `EmailConfirmed` (bool) na entidade `User`
2. Na criação, gerar token de confirmação e enviar email
3. Endpoint `GET /api/auth/confirm-email?token=...`
4. Bloquear login até email ser confirmado

**Esforço:** ~4–6 horas | **Arquivos afetados:** Múltiplos

---

### L-06: DTOs de resposta expõem UserId

| Campo | Valor |
|-------|-------|
| **Severidade** | 🟢 **Baixa — Vazamento de informação** |
| **Arquivos** | `AccountDtos.cs`, `CardDtos.cs`, `CategoryDtos.cs` |
| **Já documentado em** | *Novo* |

#### Descrição

Os DTOs de resposta expõem o `UserId` do proprietário para o cliente. Embora útil para o frontend, isso vaza informações sobre a estrutura de dados e GUIDs de usuários.

#### Código Vulnerável

```csharp
// AccountDtos.cs
public record AccountResponse(Guid Id, string Name, Guid UserId, AccountType Type, decimal Balance);
//                                        ^^^^^^ ❌ Expondo UserId

// CardDtos.cs
public record CardResponse(Guid Id, string Name, Guid UserId);
//                                        ^^^^^^ ❌ Expondo UserId

// CategoryDtos.cs
public record CategoryResponse(Guid Id, string Name, Guid UserId, string Icon);
//                                        ^^^^^^ ❌ Expondo UserId
```

#### Como Corrigir

Remover `UserId` dos responses, já que o cliente já sabe que está autenticado como aquele usuário (o token JWT contém essa informação):

```csharp
public record AccountResponse(Guid Id, string Name, AccountType Type, decimal Balance);
//                                        ^^^^^^ ✅ Removido UserId
```

**Esforço:** ~30 minutos | **Arquivos afetados:** DTOs + Services que mapeiam as respostas

---

### L-07: ExceptionMiddleware captura tudo como 500

| Campo | Valor |
|-------|-------|
| **Severidade** | 🟢 **Baixa — Prática recomendada** |
| **Arquivo** | `src/Monetis.API/Middlewares/ExceptionMiddleware.cs` |
| **Linhas** | 18–20 |
| **Já documentado em** | *Novo* |

#### Descrição

O segundo `catch` captura **todas** as exceções não tratadas. Algumas exceções como `SecurityException`, `AuthenticationException`, `InvalidOperationException` deveriam ter mapeamentos específicos.

#### Código Vulnerável

```csharp
// Linhas 18–20
catch (Exception e)
{
    logger.LogError(e, e.Message);
    await HandleExceptionAsync(context, e);  // ❌ Captura TUDO
}
```

#### Como Corrigir

Adicionar mais mapeamentos no switch do `HandleExceptionAsync`:

```csharp
var (statusCode, message, errorcode) = exception switch
{
    DomainException => (StatusCodes.Status400BadRequest, exception.Message, "BUSINESS_ERROR"),
    ArgumentException => (StatusCodes.Status400BadRequest, "Bad Request", "04X0"),
    KeyNotFoundException => (StatusCodes.Status404NotFound, "Not Found", "04X4"),
    UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, exception.Message, "UNAUTHORIZED"),
    InvalidOperationException => (StatusCodes.Status400BadRequest, "Invalid Operation", "INVALID_OP"),  // ✅ NOVO
    NotImplementedException => (StatusCodes.Status501NotImplemented, "Not Implemented", "NOT_IMPL"),   // ✅ NOVO
    _ => (StatusCodes.Status500InternalServerError, "Internal error", "07X0")
};
```

**Esforço:** ~10 minutos | **Arquivos afetados:** 1

---

## 📋 Matriz de Prioridades

| ID | Falha | Severidade | Esforço | Já Documentado? | Prioridade |
|:--:|-------|:----------:|:-------:|:----------------:|:----------:|
| C-01 | UserResourceGuard sem ownership | 🔴 Crítica | 15 min | BUGS-FIX-PLAN.md (B-008) | 🥇 **HOJE** |
| C-02 | ChangePasswordAsync busca por email GUID | 🔴 Crítica | 5 min | BUGS-FIX-PLAN.md (B-002) | 🥇 **HOJE** |
| M-01 | GET /api/users expõe todos os usuários | 🟡 Média | 5 min | Novo | 🥇 **HOJE** |
| M-02 | GET /api/users/{id} sem ownership | 🟡 Média | 10 min | Novo | 🥇 **HOJE** |
| M-03 | Sem bloqueio de conta (brute force) | 🟡 Média | 2–3h | Novo | 🥈 Esta semana |
| M-04 | Token sem refresh/revogação | 🟡 Média | 4–6h | Novo | 🥈 Esta semana |
| M-05 | IgnoreQueryFilters sem justificativa | 🟡 Média | 5 min | BUGS-FIX-PLAN.md (B-006) | 🥈 Esta semana |
| M-06 | CORS AllowedHosts: "*" | 🟡 Média | 5 min | Novo | 🥈 Esta semana |
| M-07 | UnauthorizedAccessException → 500 | 🟡 Média | 5 min | Novo | 🥈 Esta semana |
| L-01 | Senha sem validação no domínio | 🟢 Baixa | 15 min | Novo | 🥉 Próximo mês |
| L-02 | catch(Exception) mascara erros | 🟢 Baixa | 15 min | BUGS-FIX-PLAN.md (B-007) | 🥉 Próximo mês |
| L-03 | JWT Key sem validação mínima | 🟢 Baixa | 5 min | Novo | 🥉 Próximo mês |
| L-04 | Expiração longa do token | 🟢 Baixa | 5 min | Novo | 🥉 Próximo mês |
| L-05 | Sem verificação de email | 🟢 Baixa | 4–6h | Novo | 🥉 Futuro |
| L-06 | DTOs expõem UserId | 🟢 Baixa | 30 min | Novo | 🥉 Futuro |
| L-07 | ExceptionMiddleware genérico | 🟢 Baixa | 10 min | Novo | 🥉 Futuro |

---

## 🎯 Plano de Correção por Fase

### 🚨 Fase 0 — Hotfix (1 hora)

| Ordem | ID | O que fazer | Arquivo |
|:-----:|:--:|-------------|---------|
| 1 | C-01 | Adicionar `if (entity.UserId != CurrentUserId)` | `UserResourceGuard.cs` |
| 2 | C-02 | Trocar `GetUserByEmailAsync` por `GetByIdAsync` | `UserAuthService.cs` |
| 3 | M-01 | Remover ou restringir `GET /api/users` | `UsersController.cs` |
| 4 | M-02 | Adicionar verificação `if (id != UserId) return Forbid()` | `UsersController.cs` |
| 5 | M-06 | Restringir `AllowedHosts` no `appsettings.json` | `appsettings.json` |
| 6 | M-07 | Mapear `UnauthorizedAccessException` para 401 | `ExceptionMiddleware.cs` |
| 7 | M-05 | Adicionar comentário no `IgnoreQueryFilters` | `ExpenseRepository.cs` |

### ⚡ Fase 1 — Melhorias de Autenticação (1 dia)

| Ordem | ID | O que fazer | Esforço |
|:-----:|:--:|-------------|:-------:|
| 1 | M-03 | Implementar bloqueio de conta (FailedLoginAttempts + LockedUntil) | 2–3h |
| 2 | M-04 | Reduzir expiração do token + implementar refresh tokens | 4–6h |
| 3 | L-03 | Validar comprimento mínimo da JWT Key | 5 min |

### 🛠️ Fase 2 — Limpeza de Código (1 hora)

| Ordem | ID | O que fazer | Esforço |
|:-----:|:--:|-------------|:-------:|
| 1 | L-02 | Remover `catch(Exception)` do `ExpensesController` | 15 min |
| 2 | L-07 | Adicionar mais mapeamentos no ExceptionMiddleware | 10 min |
| 3 | L-06 | Remover UserId dos DTOs de resposta | 30 min |
| 4 | L-04 | Reduzir expiração do token | 5 min |

### 🏗️ Fase 3 — Futuro

| Ordem | ID | O que fazer | Esforço |
|:-----:|:--:|-------------|:-------:|
| 1 | L-05 | Fluxo de confirmação de email | 4–6h |
| 2 | L-01 | Documentar decisão de validação de senha | 15 min |

---

> **Documento gerado em:** Julho 2026
> **Baseado em:** Análise do código fonte do Monetis Backend
