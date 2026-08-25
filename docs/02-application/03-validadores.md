# ✅ Validadores (FluentValidation)

> **Camada:** Application (`src/Monetis.Application/Validators/`)  
> **Propósito:** Validar dados de entrada antes de processar no serviço  
> **Framework:** FluentValidation 11.x

---

## Índice

1. [Visão Geral](#1-visão-geral)
2. [User Validators](#2-user-validators)
3. [Account Validators](#3-account-validators)
4. [Category Validators](#4-category-validators)
5. [Expense Validators](#5-expense-validators)
6. [Income Validators](#6-income-validators)
7. [Transfer Validators](#7-transfer-validators)
8. [Subscription Validators](#8-subscription-validators)

---

## 1. Visão Geral

### Como funciona

Os validators são registrados automaticamente via Assembly scanning:

```csharp
// Monetis.Application/DependencyInjection.cs
services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
```

Isso registra todos os `AbstractValidator<>` do assembly no DI container. Eles são invocados automaticamente pelo ASP.NET Core quando um `[FromBody]` é recebido (se configurado), ou podem ser usados manualmente nos serviços.

### Lista Completa de Validators

| Validator | DTO Validado | Total de Regras |
|-----------|-------------|----------------|
| `CreateUserRequestValidator` | `CreateUserRequest` | 10 |
| `UpdateUserRequestValidator` | `UpdateUserRequest` | 6 |
| `LoginUserRequestValidator` | `LoginUserRequest` | 6 |
| `CreateAccountRequestValidator` | `CreateAccountRequest` | 3 |
| `UpdateAccountRequestValidator` | `UpdateAccountRequest` | 3 |
| `CreateCategoryRequestValidator` | `CreateCategoryRequest` | 4 |
| `UpdateCategoryRequestValidator` | `UpdateCategoryRequest` | 4 |
| `CreateExpenseRequestValidator` | `CreateExpenseRequest` | 8 |
| `CreateInstallmentRequestValidator` | `CreateInstallmentRequest` | 7 |
| `PayExpenseRequestValidator` | `PayExpenseRequest` | 2 |
| `UpdateExpenseRequestValidator` | `UpdateExpenseRequest` | 5 |
| `UpdateInstallmentGroupRequestValidator` | `UpdateInstallmentGroupRequest` | 7 |
| `CreateIncomeRequestValidator` | `CreateIncomeRequest` | 6 |
| `UpdateIncomeRequestValidator` | `UpdateIncomeRequest` | 5 |
| `CreateTransferRequestValidator` | `CreateTransferRequest` | 7 |
| `UpdateTransferRequestValidator` | `UpdateTransferRequest` | 2 |
| `CreateSubscriptionRequestValidator` | `CreateSubscriptionRequest` | 7 |
| `UpdateSubscriptionRequestValidator` | `UpdateSubscriptionRequest` | 5 |

**Total: 18 validators**

---

## 2. User Validators

### CreateUserRequestValidator

**Valida:** `CreateUserRequest`

| Campo | Regra | Mensagem de Erro |
|-------|-------|------------------|
| `FirstName` | `NotEmpty` | "First name is required" |
| `FirstName` | `MaximumLength(50)` | "First name cannot exceed 50 characters" |
| `FirstName` | `Matches(@"^[a-zA-Z\s\-_]+$")` | "First name can only contain letters, spaces, hyphens, and underscores" |
| `LastName` | `NotEmpty` | "Last name is required" |
| `LastName` | `MaximumLength(50)` | "Last name cannot exceed 50 characters" |
| `LastName` | `Matches(@"^[a-zA-Z\s\-_]+$")` | "Last name can only contain letters, spaces, hyphens, and underscores" |
| `Email` | `NotEmpty` | "Email is required" |
| `Email` | `EmailAddress()` | "Invalid email format" |
| `Email` | `MaximumLength(100)` | "Email cannot exceed 100 characters" |
| `Password` | `NotEmpty` | "Password is required" |
| `Password` | `MinimumLength(8)` | "Password must be at least 8 characters long" |
| `Password` | `MaximumLength(128)` | "Password cannot exceed 128 characters" |
| `Password` | `Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]+$")` | "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character" |

### UpdateUserRequestValidator

Mesmas regras de `FirstName`, `LastName` e `Email` do `CreateUserRequestValidator`, sem a validação de `Password`.

### LoginUserRequestValidator

Mesmas regras de `Email` e `Password` do `CreateUserRequestValidator`.

---

## 3. Account Validators

### CreateAccountRequestValidator

| Campo | Regra | Mensagem |
|-------|-------|----------|
| `Name` | `NotEmpty` | "Account name is required" |
| `Name` | `MaximumLength(100)` | "Account name cannot exceed 100 characters" |
| `Name` | `Matches(@"^[a-zA-ZÀ-ÿ\s]+$")` | "Account name can only contain letters and spaces" |
| `Type` | `IsInEnum` | "Invalid account type" |

### UpdateAccountRequestValidator

Mesmas regras de `Name` do `CreateAccountRequestValidator`.

---

## 4. Category Validators

### CreateCategoryRequestValidator

| Campo | Regra | Mensagem |
|-------|-------|----------|
| `Name` | `NotEmpty` | "Category name is required" |
| `Name` | `MaximumLength(50)` | "Category name cannot exceed 50 characters" |
| `Name` | `Matches(@"^[a-zA-ZÀ-ÿ\s]+$")` | "Category name can only contain letters and spaces" |
| `Icon` | `NotEmpty` | "Icon is required" |
| `Icon` | `MaximumLength(15)` | — |
| `Icon` | `Matches(@"^[\p{L}\p{N}\p{P}\p{S}\p{Cs}]+$")` | "Icon must be a valid character or emoji" |

### UpdateCategoryRequestValidator

Mesmas regras do `CreateCategoryRequestValidator`.

---

## 5. Expense Validators

### CreateExpenseRequestValidator

| Campo | Regra | Mensagem |
|-------|-------|----------|
| `AccountId` | `NotEmpty` | "Account is required" |
| `CategoryId` | `NotEmpty` | "Category is required" |
| `Amount` | `GreaterThan(0)` | "Amount must be greater than zero" |
| `Amount` | `LessThanOrEqualTo(9999999999)` | "Amount cannot exceed 9.999.999.999" |
| `Description` | `MaximumLength(200)` | "Description cannot exceed 200 characters" |
| `Description` | `NotEmpty` | "Description is required" |
| `DueDate` | `NotEmpty` | "Due date is required" |
| `DueDate` | `Must(x >= UtcNow.AddYears(-1))` | "Due date is too far in the past" |
| `PaymentMethod` | `IsInEnum` | "Invalid payment method" |
| `CreditCardId` (condicional) | `NotEmpty` *se `PaymentMethod == CreditCard`* | "Credit card is required for credit card payments" |
| `CreditCardId` (condicional) | Deve ser null *se `PaymentMethod != CreditCard`* | "Credit card should only be set for credit card payments" |

### CreateInstallmentRequestValidator

| Campo | Regra | Mensagem |
|-------|-------|----------|
| `AccountId` | `NotEmpty` | "Account is required" |
| `CategoryId` | `NotEmpty` | "Category is required" |
| `TotalAmount` | `GreaterThan(0)` | "Total amount must be greater than zero" |
| `TotalAmount` | `LessThanOrEqualTo(9999999999)` | "Total amount cannot exceed 9.999.999.999" |
| `Description` | `MaximumLength(200)` | "Description cannot exceed 200 characters" |
| `Description` | `NotEmpty` | "Description is required" |
| `FirstDueDate` | `NotEmpty` | "First due date is required" |
| `NumberOfInstallments` | `InclusiveBetween(2, 24)` | "Installments must be between 2 and 24" |
| `CreditCardId` | `NotEmpty` | "Credit card is required for installments" |

### PayExpenseRequestValidator

| Campo | Regra | Mensagem |
|-------|-------|----------|
| `PaidAt` | `NotEmpty` | "Payment date is required" |
| `PaidAt` | `Must(x <= UtcNow)` | "Payment date cannot be in the future" |

### UpdateExpenseRequestValidator

| Campo | Regra | Mensagem |
|-------|-------|----------|
| `CategoryId` | `NotEmpty` | "Category is required" |
| `Amount` | `GreaterThan(0)` | "Amount must be greater than zero" |
| `Amount` | `LessThanOrEqualTo(9999999999)` | "Amount cannot exceed 9.999.999.999" |
| `Description` | `MaximumLength(200)` | "Description cannot exceed 200 characters" |
| `Description` | `NotEmpty` | "Description is required" |
| `DueDate` | `NotEmpty` | "Due date is required" |
| `DueDate` | `Must(x >= UtcNow.Date.AddYears(-1))` | "Due date is too far in the past" |

### UpdateInstallmentGroupRequestValidator

| Campo | Regra | Mensagem |
|-------|-------|----------|
| `NewAccountId` | `NotEmpty` | "Account is required" |
| `CategoryId` | `NotEmpty` | "Category is required" |
| `NewTotalAmount` | `GreaterThan(0)` | "Total amount must be greater than zero" |
| `NewTotalAmount` | `LessThanOrEqualTo(9999999999)` | "Total amount cannot exceed 9.999.999.999" |
| `Description` | `MaximumLength(200)` | "Description cannot exceed 200 characters" |
| `Description` | `NotEmpty` | "Description is required" |
| `FirstDueDate` | `NotEmpty` | "First due date is required" |
| `PaymentMethod` | `Equal(PaymentMethod.CreditCard)` | "Installment groups must use credit card payment method" |
| `CreditCardId` | `NotEmpty` | "Credit card is required" |

---

## 6. Income Validators

### CreateIncomeRequestValidator

| Campo | Regra | Mensagem |
|-------|-------|----------|
| `AccountId` | `NotEmpty` | "Account is required" |
| `CategoryId` | `NotEmpty` | "Category is required" |
| `Amount` | `GreaterThan(0)` | "Amount must be greater than zero" |
| `Amount` | `LessThanOrEqualTo(9999999999)` | "Amount cannot exceed 9.999.999.999" |
| `Description` | `MaximumLength(200)` | "Description cannot exceed 200 characters" |
| `Description` | `NotEmpty` | "Description is required" |
| `ReceivedAt` | `NotEmpty` | "Received date is required" |
| `ReceivedAt` | `Must(x <= UtcNow)` | "Received date cannot be in the future" |

### UpdateIncomeRequestValidator

Mesmas regras do `CreateIncomeRequestValidator`.

---

## 7. Transfer Validators

### CreateTransferRequestValidator

| Campo | Regra | Mensagem |
|-------|-------|----------|
| `AccountId` | `NotEmpty` | "Source account is required" |
| `DestinationAccountId` | `NotEmpty` | "Destination account is required" |
| `DestinationAccountId` | `NotEqual(x => x.AccountId)` | "Destination account must be different from source account" |
| `Amount` | `GreaterThan(0)` | "Amount must be greater than zero" |
| `Amount` | `LessThanOrEqualTo(9999999999)` | "Amount cannot exceed 9.999.999.999" |
| `Description` | `NotEmpty` | "Description is required" |
| `Description` | `MaximumLength(200)` | "Description cannot exceed 200 characters" |
| `TransferredAt` | `NotEmpty` | "Transfer date is required" |
| `TransferredAt` | `Must(x <= UtcNow)` | "Transfer date cannot be in the future" |

### UpdateTransferRequestValidator

| Campo | Regra | Mensagem |
|-------|-------|----------|
| `Amount` | `GreaterThan(0)` | "Amount must be greater than zero" |
| `Amount` | `LessThanOrEqualTo(9999999999)` | "Amount cannot exceed 9.999.999.999" |
| `Description` | `NotEmpty` | "Description is required" |
| `Description` | `MaximumLength(200)` | "Description cannot exceed 200 characters" |

---

## 8. Subscription Validators

### CreateSubscriptionRequestValidator

| Campo | Regra | Mensagem |
|-------|-------|----------|
| `AccountId` | `NotEmpty` | "Account is required" |
| `CategoryId` | `NotEmpty` | "Category is required" |
| `Amount` | `GreaterThan(0)` | "Amount must be greater than zero" |
| `Amount` | `LessThanOrEqualTo(9999999999)` | "Amount cannot exceed 9.999.999.999" |
| `Description` | `NotEmpty` | "Description is required" |
| `Description` | `MaximumLength(100)` | "Description cannot exceed 100 characters" |
| `Description` | `Matches(@"^[a-zA-ZÀ-ÿ\s0-9\-_]+$")` | "Description can only contain letters and spaces" |
| `Frequency` | `IsInEnum` | "Invalid frequency" |
| `NextDueDate` | `NotEmpty` | "Next due date is required" |
| `NextDueDate` | `Must(x >= UtcNow)` | "Next due date cannot be in the past" |

### UpdateSubscriptionRequestValidator

Mesmas regras do `CreateSubscriptionRequestValidator`.

---

## Comparação: Validação no Domínio vs. Application

| Aspecto | Domínio (Entity) | Application (Validator) |
|---------|-----------------|------------------------|
| **Onde** | Nas entidades (construtores/métodos) | Nos DTOs (FluentValidation) |
| **Quando** | Durante a criação/alteração da entidade | Antes de chegar ao serviço |
| **Exceções** | `DomainException` (400 BUSINESS_ERROR) | `ValidationException` (400) |
| **Exemplo** | `Account` valida nome ≤ 25 chars | `CreateAccountRequestValidator` valida nome ≤ 100 chars |
| **Propósito** | Invariantes de negócio | Validação de entrada (defesa em profundidade) |

> ⚠️ Note a **discrepância**: o domínio de `Account` limita nome a 25 caracteres, mas o validator permite até 100. Quem dá a palavra final é o domínio.

---

> **Próximo:** [Exceções de Domínio](../01-domain/03-excecoes.md)
