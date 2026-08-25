# 🔄 Cenários de Negócio — Transferências e Ajustes de Saldo

> **Camada:** Domain / Application  
> **Cenários:** BZ-09, BZ-10, BZ-17  
> **Entidades:** Transfer, Account

---

## Índice

1. [BZ-09: Transferir entre contas do mesmo usuário](#bz-09-transferir-entre-contas-do-mesmo-usuário)
2. [BZ-10: Cancelar transferência no mesmo dia](#bz-10-cancelar-transferência-no-mesmo-dia)
3. [BZ-17: Ajustar saldo de conta manualmente](#bz-17-ajustar-saldo-de-conta-manualmente)

---

## BZ-09: Transferir entre contas do mesmo usuário

### Descrição
Usuário transfere um valor entre duas contas de sua propriedade. A transferência é executada imediatamente.

### Fluxo Principal

```mermaid
sequenceDiagram
    participant C as Cliente
    participant API as API
    participant S as TransferService
    participant RG as UserResourceGuard
    participant T as Transfer (Entity)
    participant R as TransferRepository
    participant UoW as UnitOfWork
    participant DB as SQL Server

    C->>API: POST /api/transfers
    Note over C,API: { accountId, destinationAccountId, amount, description, transferredAt }

    API->>S: CreateAsync(request)

    S->>RG: GetOwnedAccountAsync(originAccountId)
    RG-->>S: Account (Origem)

    S->>RG: GetOwnedAccountAsync(destinationAccountId)
    RG-->>S: Account (Destino)

    S->>S: Contas pertencem ao mesmo user?<br/>(validado no construtor)

    S->>T: new Transfer(origin, destination, amount, description, transferredAt)
    Note over T: Construtor VALIDA + EXECUTA:
    Note over T: ✓ Contas diferentes
    Note over T: ✓ Mesmo userId
    Note over T: ✓ Saldo suficiente
    Note over T: ✓ Data não futura
    Note over T: ⚡ origin.Withdraw(amount)
    Note over T: ⚡ destination.Deposit(amount)

    S->>R: transferRepository.Create(transfer)
    S->>UoW: CommitAsync()
    UoW->>DB: SaveChanges()
    DB-->>UoW: success
    S-->>API: TransferResponse
    API-->>C: 201 Created
```

### Regras de Negócio

| Regra | Validação | Exceção |
|-------|-----------|---------|
| Conta origem ≠ conta destino | Construtor Transfer | `TransferAccountsMustBeDifferentException` |
| Ambas contas do mesmo usuário | Construtor Transfer | `TransferAccountsMustBelongToSameUserException` |
| Saldo origem ≥ amount | Construtor Transfer | `TransferInsufficientFundsException(balance, amount)` |
| Amount > 0 | Construtor Transfer | `TransferAmountMustBePositiveException` |
| Data não futura (tolerância 5 min) | Construtor Transfer | `TransferDateInFutureException` |

### Efeitos Colaterais

| Conta | Operação | Saldo |
|-------|----------|-------|
| Origem | `Withdraw(amount)` | ↓ Reduz |
| Destino | `Deposit(amount)` | ↑ Aumenta |

> ⚠️ **Importante:** A movimentação financeira acontece **dentro do construtor** da entidade `Transfer`, não no service. Isso garante que a transferência é atômica com a criação da entidade.

### Atualizar transferência (PUT)

```
PUT /api/transfers/{id}
Body: { "amount": 600.00, "description": "Atualizado" }
```

**Comportamento especial:** Se o valor mudar, os saldos de origem e destino são ajustados pela diferença.

| Situação | Efeito |
|----------|--------|
| Novo valor > valor original | Origem: saque adicional; Destino: depósito adicional |
| Novo valor < valor original | Origem: depósito do excesso; Destino: saque do excesso |

### Exceções

| Exceção | Quando |
|---------|--------|
| `InvalidOperationException` | Tentar atualizar transferência cancelada |

---

## BZ-10: Cancelar transferência no mesmo dia

### Descrição
Usuário desfaz uma transferência realizada no mesmo dia. Os valores são estornados.

### Fluxo Principal

```
DELETE /api/transfers/{id}
```

```mermaid
sequenceDiagram
    participant C as Cliente
    participant API as API
    participant S as TransferService
    participant R as TransferRepository
    participant T as Transfer (Entity)
    participant UoW as UnitOfWork
    participant DB as SQL Server

    C->>API: DELETE /api/transfers/{id}
    API->>S: DeleteAsync(id)

    S->>R: GetByIdWithAccountsAsync(id)
    R-->>S: Transfer + Accounts (via Include)

    S->>T: transfer.Cancel(originAccount, UtcNow)
    Note over T: Valida:
    Note over T: ✓ Não está cancelada
    Note over T: ✓ Mesmo dia
    Note over T: ✓ Conta confere
    Note over T: ⚡ destination.Withdraw(amount) (estorno)
    Note over T: ⚡ origin.Deposit(amount) (estorno)
    Note over T: IsCancelled = true

    S->>UoW: CommitAsync()
    UoW->>DB: SaveChanges()
    API-->>C: 204 No Content
```

### Regras de Cancelamento

| Regra | Exceção |
|-------|---------|
| Só pode cancelar no mesmo dia | `TransferCancellationSameDayOnlyException` |
| Não pode cancelar duas vezes | `TransferAlreadyCancelledException` |
| Conta de origem deve ser a mesma da transferência | `TransferOriginAccountMismatchException` |

### Efeitos Colaterais (Estorno)

| Conta | Operação | Saldo |
|-------|----------|-------|
| Destino | `Withdraw(amount)` | ↓ Reduz (estorno) |
| Origem | `Deposit(amount)` | ↑ Aumenta (estorno) |

> ⚠️ O cancelamento reverte **exatamente** a transferência original, movendo o valor de volta.

---

## BZ-17: Ajustar saldo de conta manualmente

### Descrição
Usuário ajusta manualmente o saldo de uma conta, informando uma razão para o ajuste.

### Fluxo

```
account.AdjustBalance(newBalance, reason)
```

1. `account.AdjustBalance(1500.00, "Correção de saldo bancário")`
2. Valida que `reason` tem no mínimo 5 caracteres
3. Define `Balance = newBalance` diretamente

### Regras

| Regra | Exceção |
|-------|---------|
| Razão deve ter ≥ 5 caracteres | `AccountAdjustmentReasonInvalidException` |

### Exemplo de Uso

```
Saldo atual: 1000.00
AdjustBalance(1500.00, "Correção de saldo após extrato bancário")
Saldo após: 1500.00
```

> ⚠️ **Nota:** Este método existe na entidade `Account` mas **não possui endpoint REST dedicado**. É usado internamente pelo sistema ou pode ser exposto futuramente.

---

## Mapa de Endpoints

| Cenário | Método | Endpoint | Controller |
|---------|--------|----------|------------|
| BZ-09 | `POST` | `/api/transfers` | TransfersController |
| BZ-09 (update) | `PUT` | `/api/transfers/{id}` | TransfersController |
| BZ-10 | `DELETE` | `/api/transfers/{id}` | TransfersController |
| BZ-17 | — | (não exposto) | — |

---

> **Próximo:** [Assinaturas](03-assinaturas.md)
