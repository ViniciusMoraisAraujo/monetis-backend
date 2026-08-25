# 📦 Cenários de Negócio — Parcelamentos, Autenticação e Configuração

> **Camada:** Domain / Application / API  
> **Cenários:** BZ-02, BZ-15, BZ-16  
> **Entidades:** Expense, User

---

## Índice

1. [BZ-02: Criar despesa parcelada (2 a 24x)](#bz-02-criar-despesa-parcelada-2-a-24x)
2. [BZ-15: Login com JWT](#bz-15-login-com-jwt)
3. [BZ-16: Alterar senha](#bz-16-alterar-senha)

---

## BZ-02: Criar despesa parcelada (2 a 24x)

### Descrição
Usuário cria uma despesa parcelada em múltiplas vezes no cartão de crédito. O sistema gera N despesas (uma para cada parcela) com o mesmo `InstallmentGroupId`.

### Fluxo Principal

```mermaid
sequenceDiagram
    participant C as Cliente
    participant API as API
    participant S as ExpenseService
    participant RG as UserResourceGuard
    participant R as ExpenseRepository
    participant UoW as UnitOfWork
    participant DB as SQL Server

    C->>API: POST /api/expenses/installments
    Note over C,API: { accountId, categoryId, totalAmount, description, firstDueDate, numberOfInstallments, creditCardId }

    API->>S: CreateInstallmentAsync(request)

    S->>RG: GetOwnedAccountAsync(accountId)
    S->>RG: GetVisibleCategoryAsync(categoryId)
    S->>RG: GetOwnedCardAsync(creditCardId)

    S->>S: Expense.CreateInstallment(accountId, categoryId, totalAmount, description, firstDueDate, 12, creditCardId)

    Note over S: Cria 12 despesas:
    Note over S: Parcela 1: amount = totalAmount/12 + ajuste
    Note over S: Parcela 2-12: amount = totalAmount/12
    Note over S: Todas com mesmo InstallmentGroupId
    Note over S: DueDates: mensais (+1 mês cada)

    loop Para cada parcela
        S->>R: expenseRepository.Create(installment)
    end

    S->>UoW: CommitAsync()
    UoW->>DB: SaveChanges()
    DB-->>UoW: success
    S-->>API: ExpenseResponse[]
    API-->>C: 200 OK
```

### Funcionamento Interno (Static Factory)

```
Expense.CreateInstallment(accountId, categoryId, totalAmount, description, firstDueDate, numberOfInstallments, creditCardId)
```

```csharp
// Lógica simplificada:
var installmentAmount = Math.Round(totalAmount / numberOfInstallments, 2);
var adjustment = totalAmount - (installmentAmount * numberOfInstallments);
var groupId = Guid.NewGuid();

for (int i = 0; i < numberOfInstallments; i++)
{
    var amount = (i == 0) ? installmentAmount + adjustment : installmentAmount;
    var dueDate = firstDueDate.AddMonths(i);
    var desc = $"{description} ({i + 1}/{numberOfInstallments})";

    var expense = new Expense(
        accountId, categoryId, amount, desc, dueDate,
        PaymentMethod.CreditCard, creditCardId,
        isInstallment: true,
        installmentNumber: i + 1,
        totalInstallments: numberOfInstallments,
        installmentGroupId: groupId
    );
    expenses.Add(expense);
}
```

### Exemplo Prático

| Parâmetro | Valor |
|-----------|-------|
| TotalAmount | R$ 1.200,00 |
| Parcelas | 12 |
| 1ª parcela | R$ 100,08 (ajuste de centavos) |
| 2ª a 12ª | R$ 100,00 cada |
| Total | R$ 1.200,00 ✅ |

### Regras de Negócio

| Regra | Exceção |
|-------|---------|
| Parcelas mínimas: 2 | `ExpenseInstallmentRangeException` ("Installments must be between 2 and 24") |
| Parcelas máximas: 24 | `ExpenseInstallmentRangeException` |
| TotalAmount > 0 | `ExpenseTotalAmountMustBePositiveException` |
| CreditCardId obrigatório | `ExpenseCreditCardRequiredForInstallmentsException` |
| Cartão deve pertencer ao usuário | `KeyNotFoundException` |

### Request de Exemplo

```json
POST /api/expenses/installments
{
  "accountId": "3fa85f64-...",
  "categoryId": "28e61ce8-...",
  "totalAmount": 1200.00,
  "description": "Curso Online",
  "firstDueDate": "2026-08-10T00:00:00Z",
  "numberOfInstallments": 12,
  "creditCardId": "guid-do-cartao"
}
```

### Response de Exemplo

```json
[
  {
    "id": "guid-1",
    "accountId": "...",
    "amount": 100.08,
    "description": "Curso Online (1/12)",
    "dueDate": "2026-08-10T00:00:00Z",
    "installmentNumber": 1,
    "totalInstallments": 12,
    "installmentGroupId": "guid-grupo",
    "paymentMethod": "CreditCard",
    "status": "Pending"
  },
  {
    "id": "guid-2",
    "amount": 100.00,
    "description": "Curso Online (2/12)",
    "dueDate": "2026-09-10T00:00:00Z",
    "installmentNumber": 2,
    "totalInstallments": 12,
    "installmentGroupId": "guid-grupo",
    ...
  }
  // ... 10 mais
]
```

### Características das Parcelas

| Característica | Descrição |
|---------------|-----------|
| `IsInstallment` | `true` |
| `InstallmentGroupId` | Mesmo Guid para todas parcelas do grupo |
| `Status` | `Pending` (não debitam da conta) |
| `PaymentMethod` | Sempre `CreditCard` |
| Atualização | Parcela individual NÃO pode ser atualizada (`InstallmentExpenseCannotBeUpdatedException`) |

---

## BZ-15: Login com JWT

### Descrição
Usuário autentica-se no sistema e recebe um token JWT para acessar endpoints protegidos.

### Fluxo Principal

```mermaid
sequenceDiagram
    participant C as Cliente
    participant API as API
    participant Auth as AuthController
    participant S as UserAuthService
    participant R as UserRepository
    participant PH as PasswordHasher
    participant TK as TokenService

    C->>API: POST /api/auth/login
    Note over C,API: { email, password }

    API->>Auth: LoginUserRequest
    Auth->>S: LoginAsync(request)

    S->>R: GetUserByEmailAsync(email)
    R-->>S: User (ou null)

    alt Usuário não encontrado
        S-->>Auth: UnauthorizedAccessException
        Auth-->>C: 401 Unauthorized
    end

    S->>PH: Verify(password, user.PasswordHash)
    alt Senha inválida
        S-->>Auth: UnauthorizedAccessException
        Auth-->>C: 401 Unauthorized
    end

    PH-->>S: true (válida)
    S->>TK: GenerateToken(user.Id, user.Email)
    TK-->>S: "eyJhbGciOiJIUzI1NiIs..."

    S-->>Auth: token
    Auth-->>C: 200 OK { token: "eyJ..." }
```

### Estrutura do Token JWT

```json
// Decodificado (header.payload.signature)
{
  "sub": "3fa85f64-5717-4562-b3fc-2c963f66afa6",  // userId
  "email": "joao@email.com",
  "jti": "guid-único",
  "exp": 1735689600,  // 2 dias
  "iss": "Monetis",
  "aud": "MonetisUsers"
}
```

| Claim | Valor | Descrição |
|-------|-------|-----------|
| `sub` | Guid (userId) | Identificador do usuário |
| `email` | string | Email do usuário |
| `jti` | Guid | ID único do token |
| `exp` | timestamp | Expira em 2 dias |
| `iss` | "Monetis" | Emissor |
| `aud` | "MonetisUsers" | Audiência |

### Request

```json
POST /api/auth/login
{
  "email": "joao@email.com",
  "password": "Senha@123"
}
```

### Response

```json
200 OK
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI..."
}
```

### Como Usar o Token

```
GET /api/accounts
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

---

## BZ-16: Alterar senha

### Descrição
Usuário autenticado altera sua senha. Requer a senha atual para verificação.

### Fluxo

```
POST /api/auth/change-password
Authorization: Bearer <token>
Body: { "currentPassword": "Senha@123", "newPassword": "Nova@456" }
```

> ⚠️ **Nota:** O endpoint `POST /api/auth/change-password` **não existe atualmente** no `AuthController`. O serviço `UserAuthService.ChangePasswordAsync()` está implementado, mas não exposto via controller.

### Regras de Validação (ChangePasswordRequest)

| Campo | Regra |
|-------|-------|
| `CurrentPassword` | Obrigatório, 8-128 chars |
| `NewPassword` | Obrigatório, 8-128 chars, deve conter: maiúscula, minúscula, número, especial |

### Fluxo do Serviço

```
1. Buscar usuário (GetUserByEmailAsync(userId.ToString())) ← 🐛 BUG
2. PasswordHasher.Verify(currentPassword, user.PasswordHash)
3. PasswordHasher.Hash(newPassword)
4. user.ChangePassword(newHash)
5. UserRepository.Update(user)
6. UnitOfWork.CommitAsync()
```

### 🐛 Bug Conhecido

O `UserAuthService.ChangePasswordAsync()` usa `GetUserByEmailAsync(userContextAccessor.UserId.ToString())` para buscar o usuário. Isso está **incorreto** porque:
- `UserId` é um `Guid` convertido para string
- `GetUserByEmailAsync` espera um email, não um ID

**Correção sugerida:** Usar `GetByIdAsync(userContextAccessor.UserId)` em vez de `GetUserByEmailAsync`.

---

## Mapa de Endpoints

| Cenário | Método | Endpoint | Controller |
|---------|--------|----------|------------|
| BZ-02 | `POST` | `/api/expenses/installments` | ExpensesController |
| BZ-15 | `POST` | `/api/auth/login` | AuthController |
| BZ-16 | — | (não exposto) | — |

---

> **Próximo:** [Voltar à Visão Geral](../00-visao-geral.md) *(ou avançar para Fase 4 quando disponível)*
