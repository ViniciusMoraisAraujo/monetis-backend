# 🎯 Cenários Principais de Teste

> **Propósito:** Mapear todos os cenários críticos de negócio que devem ser cobertos por testes  
> **Total de cenários:** 25+

---

## Índice

1. [Cenários de User/Auth](#1-cenários-de-userauth)
2. [Cenários de Account](#2-cenários-de-account)
3. [Cenários de Expense](#3-cenários-de-expense)
4. [Cenários de Income](#4-cenários-de-income)
5. [Cenários de Transfer](#5-cenários-de-transfer)
6. [Cenários de Subscription](#6-cenários-de-subscription)
7. [Cenários de Multi-tenancy](#7-cenários-de-multi-tenancy)
8. [Cenários de Segurança](#8-cenários-de-segurança)

---

## 1. Cenários de User/Auth

### UC-01: Criar usuário com sucesso
- **Entrada:** Dados válidos (nome, email, senha forte)
- **Esperado:** 201 Created, `UserResponse` com Id, dados retornados
- **Verificar:** Senha foi hasheada (não retornar plain text)

### UC-02: Criar usuário com email duplicado
- **Entrada:** Email já cadastrado
- **Esperado:** `UserAlreadyExistsException` → 400 "User with email ... already exists."

### UC-03: Criar usuário com dados inválidos (validator)
- **Entrada:** Nome vazio, email inválido, senha fraca
- **Esperado:** Erro de validação (400)
- **Testar cada campo individualmente**

### UC-04: Login com credenciais válidas
- **Entrada:** Email e senha corretos
- **Esperado:** 200 OK, token JWT retornado
- **Verificar:** Token é string não vazia, contém 3 partes (header.payload.signature)

### UC-05: Login com credenciais inválidas
- **Entrada:** Email ou senha incorretos
- **Esperado:** 401 Unauthorized, "Invalid credentials."

### UC-06: Alterar senha com sucesso
- **Entrada:** Senha atual correta + nova senha válida
- **Esperado:** 204 No Content
- **Verificar:** Login com senha antiga falha, login com senha nova funciona

### UC-07: Alterar senha com senha atual errada
- **Entrada:** Senha atual incorreta
- **Esperado:** 401 Unauthorized

### UC-08: Atualizar dados do usuário
- **Entrada:** Novo nome e email
- **Esperado:** 204, dados atualizados no GET

---

## 2. Cenários de Account

### UC-09: Criar conta
- **Entrada:** Nome válido + tipo Checking
- **Esperado:** 201, saldo = 0

### UC-10: Criar conta com nome inválido
- **Entrada:** Nome vazio
- **Esperado:** 400

### UC-11: Depositar e sacar
- **Entrada:** Deposit(100) → Withdraw(30)
- **Esperado:** Balance = 70

### UC-12: Depositar valor negativo
- **Entrada:** Deposit(-50)
- **Esperado:** `AccountAmountMustBePositiveException`

### UC-13: Conta pode ficar negativa
- **Entrada:** Saldo = 0, Withdraw(100)
- **Esperado:** Balance = -100, `IsNegative` = true

### UC-14: Ajustar saldo com razão inválida
- **Entrada:** AdjustBalance(100, "abc") (razão < 5 chars)
- **Esperado:** `AccountAdjustmentReasonInvalidException`

---

## 3. Cenários de Expense

### UC-15: Criar despesa à vista (Cash)
- **Entrada:** Criar Expense com PaymentMethod = Cash, valor 100
- **Esperado:** 
  - Status = Paid
  - Saldo da conta debitado em 100
  - PaidAt preenchido

### UC-16: Criar despesa a prazo (CreditCard)
- **Entrada:** Criar Expense com PaymentMethod = CreditCard, CreditCardId válido
- **Esperado:**
  - Status = Pending
  - Saldo da conta **não** debitado
  - CreditCardId preenchido

### UC-17: Criar despesa em crédito sem cartão
- **Entrada:** PaymentMethod = CreditCard, CreditCardId = null
- **Esperado:** `ExpenseCreditCardRequiredException` → 400

### UC-18: Pagar despesa pendente
- **Entrada:** Expense pendente → Pay(UtcNow, accountId)
- **Esperado:** Status = Paid, PaidAt preenchido

### UC-19: Pagar despesa em conta diferente
- **Entrada:** Expense da Conta A → Pay(UtcNow, Conta B)
- **Esperado:** AccountId alterado para Conta B, saldo da Conta B debitado

### UC-20: Pagar despesa já paga
- **Entrada:** Pay() em expense já paga
- **Esperado:** `ExpenseAlreadyPaidException`

### UC-21: Criar despesa parcelada (12x)
- **Entrada:** CreateInstallment(amount=1200, parcelas=12, cartão)
- **Esperado:**
  - 12 despesas criadas
  - Mesmo InstallmentGroupId
  - InstallmentNumber de 1 a 12
  - Cada parcela = 100.00 (ou 100.08 na primeira se houver ajuste)

### UC-22: Criar parcelas com número inválido
- **Entrada:** numberOfInstallments = 1 ou 25
- **Esperado:** `ExpenseInstallmentRangeException`

### UC-23: Atualizar despesa pendente
- **Entrada:** Update(categoryId, amount, description, dueDate)
- **Esperado:** Dados alterados com sucesso

### UC-24: Atualizar despesa paga
- **Entrada:** Update() em expense com Status = Paid
- **Esperado:** `PaidExpenseCannotBeUpdatedException`

### UC-25: Processar despesas vencidas
- **Entrada:** ProcessOverdueExpensesAsync()
- **Esperado:** Todas as Expenses com `Status=Pending && DueDate < today` viram `Overdue`

### UC-26: Despesa com DueDate muito antiga
- **Entrada:** DueDate < -1 ano
- **Esperado:** `ExpenseDueDateTooOldException`

---

## 4. Cenários de Income

### UC-27: Criar receita (já recebida)
- **Entrada:** CreateIncome (receivedAt ≤ today)
- **Esperado:**
  - 201 Created
  - Saldo da conta creditado
  - Status = Paid

### UC-28: Criar receita com data futura
- **Entrada:** CreateIncome com receivedAt > today (usa CreatePaid internamente)
- **Esperado:** `IncomeReceivedDateInFutureException`

### UC-29: Atualizar receita com valor diferente
- **Entrada:** Update amount de 1000 para 1500
- **Esperado:** Saldo da conta ajustado (+500)

### UC-30: Deletar receita
- **Entrada:** Delete income
- **Esperado:** Saldo da conta debitado (estorno)

---

## 5. Cenários de Transfer

### UC-31: Transferir entre contas do mesmo usuário
- **Entrada:** CreateTransfer(contaA → contaB, 500)
- **Esperado:**
  - Saldo contaA: -500
  - Saldo contaB: +500
  - Status: Paid (sempre paga na criação)

### UC-32: Transferir para a mesma conta
- **Entrada:** origem = destino
- **Esperado:** `TransferAccountsMustBeDifferentException`

### UC-33: Transferir com saldo insuficiente
- **Entrada:** amount > saldo da conta origem
- **Esperado:** `TransferInsufficientFundsException`

### UC-34: Transferir entre contas de usuários diferentes
- **Entrada:** Contas de usuários diferentes
- **Esperado:** `TransferAccountsMustBelongToSameUserException`

### UC-35: Cancelar transferência no mesmo dia
- **Entrada:** Cancel() no mesmo dia
- **Esperado:**
  - IsCancelled = true
  - Saldos estornados (contaA +500, contaB -500)

### UC-36: Cancelar transferência em dia diferente
- **Entrada:** Cancel() em dia diferente do TransferredAt
- **Esperado:** `TransferCancellationSameDayOnlyException`

### UC-37: Cancelar transferência já cancelada
- **Entrada:** Cancel() duas vezes
- **Esperado:** `TransferAlreadyCancelledException`

### UC-38: Atualizar transferência com valor maior
- **Entrada:** Update(amount=800, original=500)
- **Esperado:** Saldos ajustados (+300 na origem e destino)

---

## 6. Cenários de Subscription

### UC-39: Criar assinatura mensal
- **Entrada:** CreateSubscription(frequency=Monthly, nextDueDate=2026-08-15)
- **Esperado:**
  - 201 Created
  - 1ª despesa gerada com dueDate = 2026-08-15
  - NextDueDate da subscription avançado para 2026-09-15

### UC-40: Processar assinatura
- **Entrada:** Process() na data correta
- **Esperado:**
  - Nova Expense gerada
  - LastProcessedAt atualizado
  - NextDueDate avançado conforme frequência

### UC-41: Processar assinatura antes do vencimento
- **Entrada:** Process() antes de NextDueDate
- **Esperado:** `SubscriptionNotDueYetException`

### UC-42: Processar assinatura inativa
- **Entrada:** Cancel() → Process()
- **Esperado:** `SubscriptionInactiveProcessException`

### UC-43: Reativar assinatura
- **Entrada:** Cancel() → Reactivate()
- **Esperado:** IsActive = true, NextDueDate recalculado

### UC-44: Reativar assinatura após EndDate
- **Entrada:** Reactivate() após EndDate
- **Esperado:** `SubscriptionCannotReactivateAfterEndDateException`

### UC-45: Assinatura desativa automaticamente ao atingir EndDate
- **Entrada:** Subscription com EndDate = hoje, Process()
- **Esperado:** Após processar, IsActive = false

### UC-46: Criar assinatura com crédito sem cartão
- **Entrada:** PaymentMethod = CreditCard, sem CardId
- **Esperado:** `SubscriptionCardRequiredException`

---

## 7. Cenários de Multi-tenancy

### UC-47: Usuário A cria, Usuário B não vê
- **Entrada:** UserA cria Account "Minha Conta" → UserB lista accounts
- **Esperado:** UserB não vê a conta de UserA

### UC-48: Categorias do sistema são visíveis para todos
- **Entrada:** UserA e UserB listam categorias
- **Esperado:** Ambos veem as categorias de sistema (Alimentação, Transporte, Salário)

### UC-49: Categorias personalizadas são privadas
- **Entrada:** UserA cria categoria "Freela" → UserB lista
- **Esperado:** UserB não vê "Freela"

### UC-50: Transferência entre contas de usuários diferentes
- **Entrada:** UserA tenta transferir para conta de UserB
- **Esperado:** `TransferAccountsMustBelongToSameUserException`

---

## 8. Cenários de Segurança

### UC-51: Request sem token
- **Entrada:** GET /api/accounts sem header Authorization
- **Esperado:** 401 Unauthorized

### UC-52: Request com token inválido
- **Entrada:** Authorization: Bearer token_invalido
- **Esperado:** 401 Unauthorized

### UC-53: Rate limiting (middleware custom)
- **Entrada:** 6 requests em 10 segundos
- **Esperado:** 429 Too Many Requests no 6º request

### UC-54: Rate limiting (ASPNET Core)
- **Entrada:** 101 requests em 1 minuto
- **Esperado:** 429 Too Many Requests no 101º request

---

## Mapa: Cenários × Nível de Teste

| Cenário | Nível | Entidade Principal | Regra Crítica |
|---------|-------|-------------------|--------------|
| UC-01 | E2E | User | Criação de usuário |
| UC-04 | E2E | User | Login + JWT |
| UC-09 | Unit + E2E | Account | Criação com saldo 0 |
| UC-11 | Unit | Account | Depósito e saque |
| UC-15 | Unit + Integração | Expense | Pagamento à vista debita conta |
| UC-18 | Unit | Expense | Transição de status |
| UC-21 | Unit | Expense | Criação de parcelas |
| UC-25 | Integração | Expense | Processar vencidas |
| UC-27 | Unit + Integração | Income | Crédito na conta |
| UC-31 | Unit + Integração | Transfer | Movimentação entre contas |
| UC-35 | Unit | Transfer | Cancelamento + estorno |
| UC-39 | Unit + Integração | Subscription | Geração automática de despesa |
| UC-47 | Integração | Multi-tenancy | Isolamento de dados |
| UC-51 | E2E | Security | Autenticação |

---

> **Próximo:** [Voltar à Visão Geral](../00-visao-geral.md) *(ou consultar a Arquitetura na Fase 5 quando disponível)*
