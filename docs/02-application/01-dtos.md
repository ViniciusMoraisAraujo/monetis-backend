# 📦 DTOs — Data Transfer Objects

> **Camada:** Application (`src/Monetis.Application/DTOs/`)  
> **Propósito:** Contratos de entrada (Requests) e saída (Responses) da API

---

## Índice

1. [UserDtos](#1-userdtos)
2. [AccountDtos](#2-accountdtos)
3. [CardDtos](#3-carddtos)
4. [CategoryDtos](#4-categorydtos)
5. [ExpenseDtos](#5-expensedtos)
6. [IncomeDtos](#6-incomedtos)
7. [TransferDtos](#7-transferdtos)
8. [SubscriptionDtos](#8-subscriptiondtos)

---

## 1. UserDtos

**Arquivo:** `src/Monetis.Application/DTOs/UserDtos.cs`

### UserResponse

Dados de saída do usuário.

```csharp
public record UserResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Email
);
```

| Campo | Tipo | Descrição |
|-------|------|-----------|
| `Id` | `Guid` | ID do usuário |
| `FirstName` | `string` | Nome |
| `LastName` | `string` | Sobrenome |
| `Email` | `string` | Email (lowercase) |

### CreateUserRequest

Dados para criar um novo usuário.

```csharp
public record CreateUserRequest(
    string FirstName,
    string LastName,
    string Email,
    string Password
);
```

| Campo | Tipo | Descrição |
|-------|------|-----------|
| `FirstName` | `string` | Nome (2-50 chars) |
| `LastName` | `string` | Sobrenome (2-50 chars) |
| `Email` | `string` | Email válido |
| `Password` | `string` | Senha (8-128 chars, com maiúscula, minúscula, número, especial) |

### UpdateUserRequest

Dados para atualizar um usuário.

```csharp
public record UpdateUserRequest(
    string FirstName,
    string LastName,
    string Email
);
```

### LoginUserRequest

Dados para login.

```csharp
public record LoginUserRequest(
    string Email,
    string Password
);
```

### ChangePasswordRequest

Dados para alterar senha (requer autenticação).

```csharp
public record ChangePasswordRequest(
    string CurrentPassword,
    string NewPassword
);
```

| Campo | Tipo | Descrição |
|-------|------|-----------|
| `CurrentPassword` | `string` | Senha atual (para verificação) |
| `NewPassword` | `string` | Nova senha (8-128 chars, com maiúscula, minúscula, número, especial) |

---

## 2. AccountDtos

**Arquivo:** `src/Monetis.Application/DTOs/AccountDtos.cs`

### AccountResponse

```csharp
public record AccountResponse(
    Guid Id,
    string Name,
    Guid UserId,
    AccountType Type,
    decimal Balance
);
```

| Campo | Tipo | Descrição |
|-------|------|-----------|
| `Id` | `Guid` | ID da conta |
| `Name` | `string` | Nome da conta |
| `UserId` | `Guid` | ID do usuário dono |
| `Type` | `AccountType` | Tipo: Checking, Saving, CreditCard |
| `Balance` | `decimal` | Saldo atual |

### CreateAccountRequest

```csharp
public record CreateAccountRequest(
    string Name,
    AccountType Type
);
```

### UpdateAccountRequest

```csharp
public record UpdateAccountRequest(
    string Name
);
```

---

## 3. CardDtos

**Arquivo:** `src/Monetis.Application/DTOs/CardDtos.cs`

### CardResponse

```csharp
public record CardResponse(
    Guid Id,
    string Name,
    Guid UserId
);
```

### CreateCardRequest

```csharp
public record CreateCardRequest(
    string Name
);
```

### UpdateCardRequest

```csharp
public record UpdateCardRequest(
    string Name
);
```

---

## 4. CategoryDtos

**Arquivo:** `src/Monetis.Application/DTOs/CategoryDtos.cs`

### CategoryResponse

```csharp
public record CategoryResponse(
    Guid Id,
    string Name,
    Guid UserId,
    string Icon
);
```

> ⚠️ `UserId` pode ser `Guid.Empty` (0000...) para categorias do sistema.

### CreateCategoryRequest

```csharp
public record CreateCategoryRequest(
    string Name,
    string Icon
);
```

### UpdateCategoryRequest

```csharp
public record UpdateCategoryRequest(
    string Name,
    string Icon
);
```

---

## 5. ExpenseDtos

**Arquivo:** `src/Monetis.Application/DTOs/ExpenseDtos.cs`

### ExpenseResponse

```csharp
public record ExpenseResponse(
    Guid Id,
    Guid AccountId,
    Guid CategoryId,
    TransactionStatus Status,
    DateTime? PaidAt,
    decimal Amount,
    string Description,
    DateTime DueDate,
    PaymentMethod PaymentMethod,
    int? InstallmentNumber,
    int? TotalInstallments,
    Guid? InstallmentGroupId,
    Guid? CreditCardId
);
```

| Campo | Tipo | Descrição |
|-------|------|-----------|
| `Id` | `Guid` | ID da despesa |
| `AccountId` | `Guid` | Conta de origem |
| `CategoryId` | `Guid` | Categoria |
| `Status` | `TransactionStatus` | Pending, Paid, Overdue |
| `PaidAt` | `DateTime?` | Data de pagamento (null se pendente) |
| `Amount` | `decimal` | Valor |
| `Description` | `string` | Descrição |
| `DueDate` | `DateTime` | Data de vencimento |
| `PaymentMethod` | `PaymentMethod` | Forma de pagamento |
| `InstallmentNumber` | `int?` | Nº da parcela (se for parcelado) |
| `TotalInstallments` | `int?` | Total de parcelas |
| `InstallmentGroupId` | `Guid?` | Grupo de parcelas |
| `CreditCardId` | `Guid?` | Cartão de crédito |

### CreateExpenseRequest

```csharp
public record CreateExpenseRequest(
    Guid AccountId,
    Guid CategoryId,
    decimal Amount,
    string Description,
    DateTime DueDate,
    PaymentMethod PaymentMethod,
    Guid? CreditCardId
);
```

### CreateInstallmentRequest

```csharp
public record CreateInstallmentRequest(
    Guid AccountId,
    Guid CategoryId,
    decimal TotalAmount,
    string Description,
    DateTime FirstDueDate,
    int NumberOfInstallments,
    Guid? CreditCardId
);
```

### PayExpenseRequest

```csharp
public record PayExpenseRequest(
    DateTime PaidAt,
    Guid? AccountId
);
```

> `AccountId` é opcional. Se não informado, usa a conta original da despesa.

### UpdateExpenseRequest

```csharp
public record UpdateExpenseRequest(
    Guid CategoryId,
    decimal Amount,
    string Description,
    DateTime DueDate,
    bool? IsUpdatingGroup
);
```

### UpdateInstallmentGroupRequest

```csharp
public record UpdateInstallmentGroupRequest(
    Guid NewAccountId,
    Guid CategoryId,
    decimal NewTotalAmount,
    string Description,
    DateTime FirstDueDate,
    PaymentMethod PaymentMethod,
    Guid CreditCardId
);
```

> Usado para atualizar **todo um grupo de parcelas** de uma vez.

---

## 6. IncomeDtos

**Arquivo:** `src/Monetis.Application/DTOs/IncomeDtos.cs`

### IncomeResponse

```csharp
public record IncomeResponse(
    Guid Id,
    Guid AccountId,
    Guid CategoryId,
    decimal Amount,
    string Description,
    DateTime ReceivedAt
);
```

| Campo | Tipo | Descrição |
|-------|------|-----------|
| `Id` | `Guid` | ID da receita |
| `AccountId` | `Guid` | Conta de destino |
| `CategoryId` | `Guid` | Categoria |
| `Amount` | `decimal` | Valor |
| `Description` | `string` | Descrição |
| `ReceivedAt` | `DateTime` | Data de recebimento |

### CreateIncomeRequest

```csharp
public record CreateIncomeRequest(
    Guid AccountId,
    Guid CategoryId,
    decimal Amount,
    string Description,
    DateTime ReceivedAt
);
```

### UpdateIncomeRequest

```csharp
public record UpdateIncomeRequest(
    Guid CategoryId,
    decimal Amount,
    string Description,
    DateTime ReceivedAt
);
```

---

## 7. TransferDtos

**Arquivo:** `src/Monetis.Application/DTOs/TransferDtos.cs`

### TransferResponse

```csharp
public record TransferResponse(
    Guid Id,
    Guid AccountId,
    Guid DestinationAccountId,
    decimal Amount,
    string Description,
    DateTime TransferredAt
);
```

| Campo | Tipo | Descrição |
|-------|------|-----------|
| `Id` | `Guid` | ID da transferência |
| `AccountId` | `Guid` | Conta de origem |
| `DestinationAccountId` | `Guid` | Conta de destino |
| `Amount` | `decimal` | Valor transferido |
| `Description` | `string` | Descrição |
| `TransferredAt` | `DateTime` | Data da transferência |

### CreateTransferRequest

```csharp
public record CreateTransferRequest(
    Guid AccountId,
    Guid DestinationAccountId,
    decimal Amount,
    string Description,
    DateTime TransferredAt
);
```

### UpdateTransferRequest

```csharp
public record UpdateTransferRequest(
    decimal Amount,
    string Description
);
```

---

## 8. SubscriptionDtos

**Arquivo:** `src/Monetis.Application/DTOs/SubscriptionDtos.cs`

### SubscriptionResponse

```csharp
public record SubscriptionResponse(
    Guid Id,
    decimal Amount,
    string Description,
    Frequency Frequency,
    DateTime NextDueDate,
    bool IsActive
);
```

| Campo | Tipo | Descrição |
|-------|------|-----------|
| `Id` | `Guid` | ID da assinatura |
| `Amount` | `decimal` | Valor da recorrência |
| `Description` | `string` | Descrição |
| `Frequency` | `Frequency` | Periodicidade |
| `NextDueDate` | `DateTime` | Próximo vencimento |
| `IsActive` | `bool` | Está ativa? |

> ⚠️ O `SubscriptionResponse` **não inclui** `AccountId`, `CategoryId`, `PaymentMethod` ou `CardId`.

### CreateSubscriptionRequest

```csharp
public record CreateSubscriptionRequest(
    Guid AccountId,
    Guid CategoryId,
    decimal Amount,
    string Description,
    Frequency Frequency,
    DateTime NextDueDate,
    PaymentMethod PaymentMethod
);
```

> ⚠️ Não inclui `CardId` — para pagamento em crédito, o cardId não é enviado neste DTO, porém a entidade `Subscription` requer `CardId` se `PaymentMethod == CreditCard`.

### UpdateSubscriptionRequest

```csharp
public record UpdateSubscriptionRequest(
    decimal Amount,
    string Description,
    Frequency Frequency,
    DateTime NextDueDate,
    bool IsActive
);
```

> ⚠️ O DTO de update **não permite alterar** `AccountId`, `CategoryId`, `PaymentMethod` ou `CardId`.

---

## Mapa de Relacionamento: DTOs ⇔ Entidades

| DTO Request | Entidade Criada | Serviço |
|------------|----------------|---------|
| `CreateUserRequest` | `User` | `UserService.CreateAsync` |
| `LoginUserRequest` | — (valida credenciais) | `UserAuthService.LoginAsync` |
| `ChangePasswordRequest` | — (altera hash) | `UserAuthService.ChangePasswordAsync` |
| `UpdateUserRequest` | — (atualiza `User`) | `UserService.UpdateAsync` |
| `CreateAccountRequest` | `Account` | `AccountService.CreateAsync` |
| `UpdateAccountRequest` | — (atualiza `Account`) | `AccountService.UpdateAsync` |
| `CreateCardRequest` | `Card` | `CardService.CreateAsync` |
| `UpdateCardRequest` | — (atualiza `Card`) | `CardService.UpdateAsync` |
| `CreateCategoryRequest` | `Category` | `CategoryService.CreateAsync` |
| `UpdateCategoryRequest` | — (atualiza `Category`) | `CategoryService.UpdateAsync` |
| `CreateExpenseRequest` | `Expense` | `ExpenseService.CreateExpenseAsync` |
| `CreateInstallmentRequest` | `Expense[]` (N) | `ExpenseService.CreateInstallmentAsync` |
| `PayExpenseRequest` | — (altera status) | `ExpenseService.PayExpenseAsync` |
| `UpdateExpenseRequest` | — (atualiza `Expense`) | `ExpenseService.UpdateExpenseAsync` |
| `CreateIncomeRequest` | `Income` | `IncomeService.CreateAsync` |
| `UpdateIncomeRequest` | — (atualiza `Income`) | `IncomeService.UpdateAsync` |
| `CreateTransferRequest` | `Transfer` | `TransferService.CreateAsync` |
| `UpdateTransferRequest` | — (atualiza `Transfer`) | `TransferService.UpdateAsync` |
| `CreateSubscriptionRequest` | `Subscription` + `Expense` (1ª) | `SubscriptionService.CreateAsync` |
| `UpdateSubscriptionRequest` | — (atualiza `Subscription`) | `SubscriptionService.UpdateAsync` |

---

> **Próximo:** [Serviços](02-servicos.md)
