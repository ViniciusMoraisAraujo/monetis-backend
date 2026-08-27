# 🌐 Monetis — Guia de Consumo para Front-End

> **Guia prático** para desenvolvedores front-end consumirem a API Monetis.  
> **Base URL (dev):** `https://localhost:7260`  
> **Formato:** JSON  
> **Autenticação:** JWT Bearer Token  
> **Rate Limit:** 100 requisições/minuto por IP

---

## Sumário

1. [Autenticação (JWT)](#1-autenticação-jwt)
2. [Formato das Respostas](#2-formato-das-respostas)
3. [Erros e Status Codes](#3-erros-e-status-codes)
4. [Enums (constantes do front-end)](#4-enums-constantes-do-front-end)
5. [API de Usuários](#5-api-de-usuários)
6. [API de Contas](#6-api-de-contas)
7. [API de Cartões](#7-api-de-cartões)
8. [API de Categorias](#8-api-de-categorias)
9. [API de Despesas](#9-api-de-despesas)
10. [API de Receitas](#10-api-de-receitas)
11. [API de Transferências](#11-api-de-transferências)
12. [API de Assinaturas](#12-api-de-assinaturas)
13. [Regras de Negócio Importantes](#13-regras-de-negócio-importantes)
14. [Fluxos Completos (Exemplos)](#14-fluxos-completos-exemplos)

---

## 1. Autenticação (JWT)

### 1.1 Registro de Usuário

```http
POST /api/users
Content-Type: application/json

{
  "firstName": "João",
  "lastName": "Silva",
  "email": "joao@email.com",
  "password": "Senha@123"
}
```

**Resposta (201 Created):**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "firstName": "João",
  "lastName": "Silva",
  "email": "joao@email.com"
}
```

### 1.2 Login

```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "joao@email.com",
  "password": "Senha@123"
}
```

**Resposta (200 OK):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

### 1.3 Usando o Token

Envie o token em **todas** as requisições autenticadas:

```http
GET /api/accounts
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

> ⚠️ **O token expira em 2 dias.** Após expirar, o servidor retorna `401 Unauthorized`. O front-end deve redirecionar para a tela de login.

### 1.4 Regras de Senha

| Regra | Detalhe |
|-------|---------|
| Mínimo | 8 caracteres |
| Máximo | 128 caracteres |
| Maiúscula | Pelo menos 1 (`A-Z`) |
| Minúscula | Pelo menos 1 (`a-z`) |
| Número | Pelo menos 1 (`0-9`) |
| Especial | Pelo menos 1 (`!@#$%^&*()_+-=[]{}|;':\",./<>?~`) |

> 💡 **Dica:** Valide a senha no front-end antes de enviar para evitar `400 Bad Request`.

---

## 2. Formato das Respostas

### 2.1 Sucesso

| Status | Quando | Formato |
|--------|--------|---------|
| `200 OK` | GET, POST (com retorno) | Objeto JSON ou array |
| `201 Created` | POST (recurso criado) | Objeto JSON + header `Location` |
| `204 No Content` | PUT, DELETE | Corpo vazio |

### 2.2 Erro Padronizado

```json
// 400 Bad Request
{
  "statusCode": 400,
  "message": "Account name is required",
  "errorCode": "BUSINESS_ERROR",
  "details": null,
  "stackTrace": null
}
```

| Campo | Tipo | Descrição |
|-------|------|-----------|
| `statusCode` | `number` | Código HTTP |
| `message` | `string` | Mensagem legível (use esta para exibir ao usuário) |
| `errorCode` | `string` | Código interno para identificação |
| `details` | `string?` | Detalhes (apenas em desenvolvimento) |
| `stackTrace` | `string?` | Stack trace (apenas em desenvolvimento) |

---

## 3. Erros e Status Codes

### 3.1 Matriz Rápida

| Código | Significado | Ação do Front-End |
|--------|-------------|-------------------|
| `200` | OK | Sucesso |
| `201` | Created | Recurso criado com sucesso |
| `204` | No Content | Operação sem retorno |
| `400` | Bad Request | Erro de negócio ou validação. Exibir `message` ao usuário |
| `401` | Unauthorized | Token ausente, inválido ou expirado. Redirecionar para login |
| `403` | Forbidden | Sem permissão |
| `404` | Not Found | Recurso não existe (pode ocorrer ao navegar para detalhes) |
| `429` | Too Many Requests | Excedeu rate limit. Aguardar e tentar novamente |
| `500` | Internal Error | Erro no servidor. Mostrar mensagem genérica |

### 3.2 Tratamento no Front-End

```typescript
// Exemplo de função fetch com tratamento de erros
async function apiFetch<T>(url: string, options?: RequestInit): Promise<T> {
  const response = await fetch(url, {
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${getToken()}`,
      ...options?.headers,
    },
    ...options,
  });

  if (!response.ok) {
    if (response.status === 401) {
      // Token expirado — redirecionar para login
      redirectToLogin();
      throw new Error('Sessão expirada');
    }
    
    if (response.status === 429) {
      // Rate limit — aguardar e tentar novamente
      throw new Error('Muitas requisições. Tente novamente em instantes.');
    }
    
    // Tenta parsear o erro padronizado
    const errorBody = await response.json().catch(() => null);
    throw new Error(
      errorBody?.message || `Erro ${response.status}: ${response.statusText}`
    );
  }

  // 204 No Content — sem corpo
  if (response.status === 204) return undefined as T;

  return response.json();
}
```

---

## 4. Enums (constantes do front-end)

### 4.1 AccountType (Tipo de Conta)

```typescript
enum AccountType {
  Checking = 'Checking',   // Conta Corrente
  Saving = 'Saving',       // Poupança
  CreditCard = 'CreditCard' // Cartão de Crédito
}
```

### 4.2 TransactionStatus (Status da Transação)

```typescript
enum TransactionStatus {
  Pending = 0,    // Pendente
  Paid = 1,       // Paga/Recebida
  Cancelled = 2,  // Cancelada
  Overdue = 3     // Vencida (apenas despesas)
}
```

> 💡 **Dica:** O número `0` (Pending) é o valor default em C#. Ao construir o JSON no front-end, lembre-se de que `Pending` corresponde a `0`.

### 4.3 PaymentMethod (Forma de Pagamento)

```typescript
enum PaymentMethod {
  Cash = 0,        // Dinheiro (débito automático)
  Debit = 1,       // Débito (débito automático)
  CreditCard = 2,  // Cartão de Crédito (sem débito automático)
  Pix = 3,         // Pix (débito automático)
  Transfer = 4     // Transferência
}
```

> ⚠️ **Importante:** `Cash`, `Debit` e `Pix` são **pagamentos à vista** — o valor é debitado automaticamente da conta ao criar a despesa. `CreditCard` é **a prazo** — o saldo NÃO é debitado.

### 4.4 Frequency (Frequência de Assinatura)

```typescript
enum Frequency {
  Weekly = 0,      // Semanal (+7 dias)
  Biweekly = 1,    // Quinzenal (+15 dias)
  Monthly = 2,     // Mensal (+1 mês)
  Bimonthly = 3,   // Bimestral (+2 meses)
  Quarterly = 4,   // Trimestral (+3 meses)
  Semiannual = 5,  // Semestral (+6 meses)
  Yearly = 6       // Anual (+1 ano)
}
```

> 💡 Os valores numéricos são os `enum` do C#. Prefira usar os números em vez de strings para garantir compatibilidade.

---

## 5. API de Usuários

**Base:** `/api/users`

### 5.1 Criar Usuário (Público)

```http
POST /api/users
Content-Type: application/json
```

```json
{
  "firstName": "João",
  "lastName": "Silva",
  "email": "joao@email.com",
  "password": "Senha@123"
}
```

| Campo | Tipo | Regras |
|-------|------|--------|
| `firstName` | `string` | 2-50 caracteres, apenas letras/espaços/hífens |
| `lastName` | `string` | 2-50 caracteres, apenas letras/espaços/hífens |
| `email` | `string` | Formato email, max 100 caracteres |
| `password` | `string` | 8-128 caracteres, com maiúscula, minúscula, número e especial |

**Retorna:** `201 Created` + [`UserResponse`](#userresponse)

### 5.2 Listar Usuários (Autenticado)

```http
GET /api/users
Authorization: Bearer <token>
```

**Retorna:** `200 OK` + `UserResponse[]`

### 5.3 Buscar Usuário por ID (Autenticado)

```http
GET /api/users/{id}
Authorization: Bearer <token>
```

**Retorna:** `200 OK` + `UserResponse` | `404 Not Found`

### 5.4 Atualizar Usuário (Autenticado)

```http
PUT /api/users/{id}
Authorization: Bearer <token>
Content-Type: application/json
```

```json
{
  "firstName": "João",
  "lastName": "Silva",
  "email": "joao.novo@email.com"
}
```

**Retorna:** `204 No Content`

### 5.5 Deletar Usuário (Autenticado)

```http
DELETE /api/users/{id}
Authorization: Bearer <token>
```

**Retorna:** `204 No Content`

### 📦 Tipos

```typescript
// UserResponse
interface UserResponse {
  id: string;          // UUID
  firstName: string;
  lastName: string;
  email: string;       // Sempre em lowercase
}
```

---

## 6. API de Contas

**Base:** `/api/accounts`

### 6.1 Listar Contas

```http
GET /api/accounts
Authorization: Bearer <token>
```

**Retorna:** `200 OK` + `AccountResponse[]`

### 6.2 Buscar Conta por ID

```http
GET /api/accounts/{id}
Authorization: Bearer <token>
```

**Retorna:** `200 OK` + `AccountResponse` | `404 Not Found`

### 6.3 Criar Conta

```http
POST /api/accounts
Authorization: Bearer <token>
Content-Type: application/json
```

```json
{
  "name": "Conta Corrente",
  "type": "Checking"
}
```

| Campo | Tipo | Valores |
|-------|------|---------|
| `name` | `string` | Obrigatório, **max 25 caracteres** |
| `type` | `string` | `Checking`, `Saving`, ou `CreditCard` |

**Retorna:** `201 Created` + [`AccountResponse`](#accountresponse) (saldo inicial = 0)

### 6.4 Atualizar Conta

```http
PUT /api/accounts/{id}
Authorization: Bearer <token>
Content-Type: application/json
```

```json
{
  "name": "Conta Atualizada"
}
```

**Retorna:** `204 No Content` | `404 Not Found`

### 6.5 Deletar Conta

```http
DELETE /api/accounts/{id}
Authorization: Bearer <token>
```

**Retorna:** `204 No Content`

### 📦 Tipos

```typescript
// AccountResponse
interface AccountResponse {
  id: string;           // UUID
  name: string;
  userId: string;       // UUID do dono
  type: 'Checking' | 'Saving' | 'CreditCard';
  balance: number;      // Saldo atual (pode ser negativo)
}

// CreateAccountRequest
interface CreateAccountRequest {
  name: string;         // Max 25 caracteres
  type: 'Checking' | 'Saving' | 'CreditCard';
}

// UpdateAccountRequest
interface UpdateAccountRequest {
  name: string;
}
```

> ⚠️ O nome da conta tem limite de **25 caracteres** no backend. Valide no front-end!

---

## 7. API de Cartões

**Base:** `/api/cards` (rota herdada de `ApiControllerBase` como `/api/Cards`, mas ASP.NET Core ignora maiúsculas/minúsculas em rotas)

### 7.1 Listar Cartões

```http
GET /api/cards
Authorization: Bearer <token>
```

**Retorna:** `200 OK` + `CardResponse[]`

### 7.2 Buscar Cartão por ID

```http
GET /api/cards/{id}
Authorization: Bearer <token>
```

**Retorna:** `200 OK` + `CardResponse` | `404 Not Found`

### 7.3 Criar Cartão

```http
POST /api/cards
Authorization: Bearer <token>
Content-Type: application/json
```

```json
{
  "name": "Cartão Nubank"
}
```

**Retorna:** `201 Created` + `CardResponse`

### 7.4 Atualizar Cartão

```http
PUT /api/cards/{id}
Authorization: Bearer <token>
Content-Type: application/json
```

```json
{
  "name": "Cartão Atualizado"
}
```

**Retorna:** `204 No Content` | `404 Not Found`

### 7.5 Deletar Cartão

```http
DELETE /api/cards/{id}
Authorization: Bearer <token>
```

**Retorna:** `204 No Content`

### 📦 Tipos

```typescript
interface CardResponse {
  id: string;
  name: string;
  userId: string;
}

interface CreateCardRequest {
  name: string;
}
```

---

## 8. API de Categorias

**Base:** `/api/categories`

> 💡 Existem **categorias do sistema** (compartilhadas, `userId = 0000...`) e **categorias do usuário** (privadas).

### 8.1 Listar Categorias

```http
GET /api/categories
Authorization: Bearer <token>
```

**Retorna:** `200 OK` + `CategoryResponse[]` (sistema + do usuário)

### 8.2 Buscar Categoria por ID

```http
GET /api/categories/{id}
Authorization: Bearer <token>
```

**Retorna:** `200 OK` + `CategoryResponse` | `404 Not Found`

### 8.3 Criar Categoria Personalizada

```http
POST /api/categories
Authorization: Bearer <token>
Content-Type: application/json
```

```json
{
  "name": "Assinaturas",
  "icon": "📺"
}
```

**Retorna:** `201 Created` + `CategoryResponse`

### 8.4 Atualizar Categoria

```http
PUT /api/categories/{id}
Authorization: Bearer <token>
Content-Type: application/json
```

```json
{
  "name": "Streaming",
  "icon": "🎬"
}
```

**Retorna:** `204 No Content` | `404 Not Found`

### 8.5 Deletar Categoria

```http
DELETE /api/categories/{id}
Authorization: Bearer <token>
```

**Retorna:** `204 No Content`

> ⚠️ **Categorias do sistema** (`userId = "00000000-0000-0000-0000-000000000000"`) **não podem ser removidas**. O front-end deve ocultar o botão de deletar para essas categorias.

### 📦 Tipos

```typescript
interface CategoryResponse {
  id: string;
  name: string;
  userId: string;    // "0000..." = categoria do sistema
  icon: string;      // Emoji (🍽️, 🚗, 💰, etc.)
}

interface CreateCategoryRequest {
  name: string;
  icon: string;
}
```

### Categorias do Sistema (IDs fixos)

| Nome | Ícone | ID |
|------|-------|----|
| Alimentação | 🍽️ | `28e61ce8-8149-4c81-a570-c0085eefa121` |
| Transporte | 🚗 | `e29e79d5-7844-491f-a9bd-9e744890555b` |
| Salário | 💰 | `96d53840-9752-4f2b-a522-c7357cdc4986` |

---

## 9. API de Despesas

**Base:** `/api/expenses`

### 9.1 Listar Despesas

```http
GET /api/expenses
Authorization: Bearer <token>
```

**Retorna:** `200 OK` + `ExpenseResponse[]`

### 9.2 Buscar Despesa por ID

```http
GET /api/expenses/{id}
Authorization: Bearer <token>
```

**Retorna:** `200 OK` + `ExpenseResponse` | `404 Not Found`

### 9.3 Criar Despesa Simples

```http
POST /api/expenses
Authorization: Bearer <token>
Content-Type: application/json
```

```json
{
  "accountId": "3fa85f64-...",
  "categoryId": "28e61ce8-...",
  "amount": 150.00,
  "description": "Supermercado",
  "dueDate": "2026-07-20T00:00:00Z",
  "paymentMethod": 0,
  "creditCardId": null
}
```

| Campo | Tipo | Obrigatório | Observação |
|-------|------|:-----------:|-----------|
| `accountId` | `string` (UUID) | ✅ | Conta de débito |
| `categoryId` | `string` (UUID) | ✅ | Categoria |
| `amount` | `number` | ✅ | > 0 |
| `description` | `string` | ✅ | **Max 100 caracteres** |
| `dueDate` | `string` (ISO date) | ✅ | Não pode ser anterior a 1 ano |
| `paymentMethod` | `number` | ✅ | 0=Cash, 1=Debit, 2=CreditCard, 3=Pix, 4=Transfer |
| `creditCardId` | `string` (UUID) | 🔶 Se `paymentMethod = 2` | **Obrigatório** se for crédito |

**Comportamento especial:**
- `paymentMethod = 0 (Cash)`, `1 (Debit)` ou `3 (Pix)` → **débito automático** na conta + status `Paid (1)`
- `paymentMethod = 2 (CreditCard)` → **sem débito**, status `Pending (0)`

**Retorna:** `201 Created` + `ExpenseResponse`

### 9.4 Criar Despesa Parcelada

```http
POST /api/expenses/installments
Authorization: Bearer <token>
Content-Type: application/json
```

```json
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

| Campo | Tipo | Regra |
|-------|------|-------|
| `totalAmount` | `number` | > 0 |
| `numberOfInstallments` | `number` | **2 a 24** |
| `creditCardId` | `string` (UUID) | **Obrigatório** |

**Comportamento:**
- Gera N despesas (uma por parcela) com mesmo `installmentGroupId`
- `installmentNumber` varia de 1 a N
- `paymentMethod` é sempre `CreditCard (2)`
- Nenhuma parcela debita da conta (status `Pending`)

**Retorna:** `200 OK` + `ExpenseResponse[]`

> 💡 **Exemplo:** 12 parcelas de R$ 1.200 → 1ª parcela R$ 100,08 (ajuste de centavos), demais R$ 100,00 cada.

### 9.5 Pagar Despesa

```http
POST /api/expenses/{id}/pay
Authorization: Bearer <token>
Content-Type: application/json
```

```json
{
  "paidAt": "2026-07-15T10:00:00Z",
  "accountId": "3fa85f64-..."
}
```

| Campo | Tipo | Observação |
|-------|------|-----------|
| `paidAt` | `string` (ISO datetime) | Data/hora do pagamento |
| `accountId` | `string` (UUID) | Opcional. Se diferente do original, **move a despesa** para outra conta |

**Retorna:** `200 OK` + `ExpenseResponse`

> ⚠️ O `accountId` é opcional. Se não enviado, usa a conta original da despesa.

### 9.6 Atualizar Despesa

```http
PUT /api/expenses/{id}
Authorization: Bearer <token>
Content-Type: application/json
```

```json
{
  "categoryId": "guid",
  "amount": 200.00,
  "description": "Supermercado",
  "dueDate": "2026-07-25T00:00:00Z"
}
```

**Campo adicional (opcional):**

| Campo | Tipo | Descrição |
|-------|------|-----------|
| `isUpdatingGroup` | `boolean` | Se `true`, atualiza **todo o grupo de parcelas** de uma vez |

> ⚠️ **Não pode atualizar** despesas já pagas. Parcelas individuais não podem ser editadas — use `isUpdatingGroup: true` para alterar o grupo inteiro.

**Retorna:** `200 OK` + `ExpenseResponse`

### 9.7 Processar Vencidas (Manual)

```http
POST /api/expenses/process-overdue
Authorization: Bearer <token>
```

Marca todas as despesas `Pending` com `DueDate < hoje` como `Overdue`.

**Retorna:** `204 No Content`

> 💡 Também executado automaticamente todos os dias às 00:01 UTC pelo background service.

### 📦 Tipos

```typescript
interface ExpenseResponse {
  id: string;
  accountId: string;
  categoryId: string;
  status: number;           // 0=Pending, 1=Paid, 2=Cancelled, 3=Overdue
  paidAt: string | null;    // ISO datetime ou null
  amount: number;
  description: string;
  dueDate: string;          // ISO date
  paymentMethod: number;    // 0=Cash, 1=Debit, 2=CreditCard, 3=Pix, 4=Transfer
  installmentNumber: number | null;
  totalInstallments: number | null;
  installmentGroupId: string | null;
  creditCardId: string | null;
}

interface CreateExpenseRequest {
  accountId: string;
  categoryId: string;
  amount: number;
  description: string;        // Max 100 caracteres!
  dueDate: string;            // ISO date
  paymentMethod: number;      // Enum PaymentMethod
  creditCardId?: string | null;
}

interface CreateInstallmentRequest {
  accountId: string;
  categoryId: string;
  totalAmount: number;
  description: string;
  firstDueDate: string;
  numberOfInstallments: number;  // 2 a 24
  creditCardId: string;          // Obrigatório!
}

interface PayExpenseRequest {
  paidAt: string;           // ISO datetime
  accountId?: string | null;  // Opcional
}
```

---

## 10. API de Receitas

**Base:** `/api/incomes`

### 10.1 Listar Receitas

```http
GET /api/incomes
Authorization: Bearer <token>
```

**Retorna:** `200 OK` + `IncomeResponse[]`

### 10.2 Buscar Receita por ID

```http
GET /api/incomes/{id}
Authorization: Bearer <token>
```

**Retorna:** `200 OK` + `IncomeResponse` | `404 Not Found`

### 10.3 Criar Receita

```http
POST /api/incomes
Authorization: Bearer <token>
Content-Type: application/json
```

```json
{
  "accountId": "3fa85f64-...",
  "categoryId": "96d53840-...",
  "amount": 5000.00,
  "description": "Salário",
  "receivedAt": "2026-07-05T00:00:00Z"
}
```

> ⚠️ **`receivedAt` não pode ser futura.** O valor é **automaticamente depositado** na conta.

**Retorna:** `201 Created` + `IncomeResponse`

### 10.4 Atualizar Receita

```http
PUT /api/incomes/{id}
Authorization: Bearer <token>
Content-Type: application/json
```

```json
{
  "categoryId": "guid",
  "amount": 5500.00,
  "description": "Salário + Bônus",
  "receivedAt": "2026-07-05T00:00:00Z"
}
```

> ⚠️ Se o valor mudar, o saldo da conta é **ajustado automaticamente** (depósito/retirada da diferença).

**Retorna:** `204 No Content`

### 10.5 Deletar Receita

```http
DELETE /api/incomes/{id}
Authorization: Bearer <token>
```

> ⚠️ O valor é **retirado da conta** (estorno).

**Retorna:** `204 No Content`

### 📦 Tipos

```typescript
interface IncomeResponse {
  id: string;
  accountId: string;
  categoryId: string;
  amount: number;
  description: string;
  receivedAt: string;     // ISO datetime
}

interface CreateIncomeRequest {
  accountId: string;
  categoryId: string;
  amount: number;
  description: string;
  receivedAt: string;     // NÃO pode ser futura!
}
```

---

## 11. API de Transferências

**Base:** `/api/transfers`

### 11.1 Listar Transferências

```http
GET /api/transfers
Authorization: Bearer <token>
```

**Retorna:** `200 OK` + `TransferResponse[]`

### 11.2 Buscar Transferência por ID

```http
GET /api/transfers/{id}
Authorization: Bearer <token>
```

**Retorna:** `200 OK` + `TransferResponse` | `404 Not Found`

### 11.3 Criar Transferência

```http
POST /api/transfers
Authorization: Bearer <token>
Content-Type: application/json
```

```json
{
  "accountId": "guid-conta-origem",
  "destinationAccountId": "guid-conta-destino",
  "amount": 500.00,
  "description": "Transferência para poupança",
  "transferredAt": "2026-07-10T00:00:00Z"
}
```

**Regras:**
- `accountId` ≠ `destinationAccountId`
- Ambas contas devem pertencer ao **mesmo usuário**
- Saldo da origem deve ser suficiente
- Data não pode ser futura
- A transferência é **executada imediatamente**

**Retorna:** `201 Created` + `TransferResponse`

### 11.4 Atualizar Transferência

```http
PUT /api/transfers/{id}
Authorization: Bearer <token>
Content-Type: application/json
```

```json
{
  "amount": 600.00,
  "description": "Transferência atualizada"
}
```

> ⚠️ Se o valor mudar, os saldos de origem e destino são **ajustados pela diferença**.

**Retorna:** `204 No Content` | `404 Not Found`

### 11.5 Cancelar Transferência (mesmo dia)

```http
DELETE /api/transfers/{id}
Authorization: Bearer <token>
```

> ⚠️ **Só pode cancelar no mesmo dia da transferência.** Os valores são estornados (origem recebe de volta, destino perde o valor).

**Retorna:** `204 No Content`

### 📦 Tipos

```typescript
interface TransferResponse {
  id: string;
  accountId: string;              // Conta origem
  destinationAccountId: string;   // Conta destino
  amount: number;
  description: string;
  transferredAt: string;          // ISO datetime
}

interface CreateTransferRequest {
  accountId: string;
  destinationAccountId: string;   // Diferente de accountId!
  amount: number;
  description: string;
  transferredAt: string;          // Não futura
}
```

---

## 12. API de Assinaturas

**Base:** `/api/subscriptions`

### 12.1 Listar Assinaturas

```http
GET /api/subscriptions
Authorization: Bearer <token>
```

**Retorna:** `200 OK` + `SubscriptionResponse[]`

### 12.2 Buscar Assinatura por ID

```http
GET /api/subscriptions/{id}
Authorization: Bearer <token>
```

**Retorna:** `200 OK` + `SubscriptionResponse` | `404 Not Found`

### 12.3 Criar Assinatura

```http
POST /api/subscriptions
Authorization: Bearer <token>
Content-Type: application/json
```

```json
{
  "accountId": "guid",
  "categoryId": "guid",
  "amount": 29.90,
  "description": "Netflix",
  "frequency": 2,
  "nextDueDate": "2026-08-15T00:00:00Z",
  "paymentMethod": 2
}
```

| Campo | Tipo | Valores |
|-------|------|---------|
| `frequency` | `number` | 0=Weekly, 1=Biweekly, 2=Monthly, 3=Bimonthly, 4=Quarterly, 5=Semiannual, 6=Yearly |
| `paymentMethod` | `number` | 0=Cash, 1=Debit, 2=CreditCard, 3=Pix, 4=Transfer |

> ⚠️ Ao criar, **já gera a primeira despesa** automaticamente com `dueDate = nextDueDate`.

**Retorna:** `201 Created` + `SubscriptionResponse`

### 12.4 Atualizar Assinatura

```http
PUT /api/subscriptions/{id}
Authorization: Bearer <token>
Content-Type: application/json
```

```json
{
  "amount": 39.90,
  "description": "Netflix Premium",
  "frequency": 2,
  "nextDueDate": "2026-08-15T00:00:00Z",
  "isActive": true
}
```

> ⚠️ Atualmente **não é possível alterar** `accountId`, `paymentMethod` ou `cardId` pelo update (campos não estão no DTO).

**Retorna:** `204 No Content` | `404 Not Found`

### 12.5 Cancelar (Desativar) Assinatura

```http
DELETE /api/subscriptions/{id}
Authorization: Bearer <token>
```

> ⚠️ A assinatura **não é removida** do banco — apenas marcada como `isActive = false`. As despesas já geradas continuam existindo.

**Retorna:** `204 No Content`

### 📦 Tipos

```typescript
interface SubscriptionResponse {
  id: string;
  amount: number;
  description: string;
  frequency: number;       // 0-6 (Enum Frequency)
  nextDueDate: string;     // ISO date
  isActive: boolean;
  // ⚠️ AccountId, CategoryId, PaymentMethod NÃO são retornados
  // atualmente (bug conhecido T-008)
}

interface CreateSubscriptionRequest {
  accountId: string;
  categoryId: string;
  amount: number;
  description: string;
  frequency: number;       // 0-6
  nextDueDate: string;     // ISO date
  paymentMethod: number;   // 0-4 (Enum PaymentMethod)
}
```

---

## 13. Regras de Negócio Importantes

### 13.1 Efeitos Colaterais (Side Effects)

| Ação | Efeito no Saldo |
|------|----------------|
| Criar despesa à vista (`Cash`, `Debit`, `Pix`) | ❌ **Debita** o valor da conta |
| Criar despesa no crédito (`CreditCard`) | ✅ **Não altera** saldo |
| Criar despesa parcelada | ✅ **Não altera** saldo (nenhuma parcela) |
| Pagar despesa pendente | ❌ **Debita** o valor da conta |
| Criar receita | ⬆️ **Credita** o valor na conta |
| Atualizar receita (mudou valor) | ⬆️/❌ **Ajusta** diferença no saldo |
| Deletar receita | ❌ **Retira** o valor da conta (estorno) |
| Criar transferência | 🔄 **Debita** origem, **credita** destino |
| Cancelar transferência (mesmo dia) | 🔄 **Estorna**: destino perde, origem recebe |
| Criar assinatura | ✅ **Gera 1ª despesa** (mas não debita automaticamente) |

### 13.2 Validações de Tamanho (Importantes para o UI)

| Campo | Limite | Onde Validar |
|-------|:------:|-------------|
| `Account.name` | **25** caracteres | Formulário de conta |
| `Expense.description` | **100** caracteres | Formulário de despesa |
| `Income.description` | **200** caracteres | Formulário de receita |
| `Transfer.description` | **200** caracteres | Formulário de transferência |
| `Subscription.description` | **100** caracteres | Formulário de assinatura |
| `User.firstName` / `lastName` | 2-50 caracteres | Cadastro/edição |
| `User.email` | Max 100 | Cadastro/edição |

### 13.3 Status de Despesa (para UI)

| Status | Cor Sugerida | Ícone Sugerido | Ações Disponíveis |
|--------|-------------|----------------|-------------------|
| `0` Pending | 🟡 Amarelo | ⏳ | Pagar, Editar, Excluir |
| `1` Paid | 🟢 Verde | ✅ | Visualizar |
| `2` Cancelled | ⚪ Cinza | ❌ | Visualizar |
| `3` Overdue | 🔴 Vermelho | ⚠️ | Pagar |

### 13.4 Comportamento do Cartão de Crédito

- Ao criar despesa com `paymentMethod = 2 (CreditCard)`, o campo `creditCardId` é **obrigatório**
- O saldo da conta **não é alterado** no momento da criação
- A despesa nasce com `status = 0 (Pending)`
- O pagamento efetivo ocorre quando o usuário chama `POST /api/expenses/{id}/pay`

### 13.5 Parcelamento (Regras)

| Regra | Valor |
|-------|-------|
| Mínimo de parcelas | 2 |
| Máximo de parcelas | 24 |
| Cartão de crédito | **Obrigatório** |
| Ajuste de centavos | 1ª parcela recebe o ajuste |

---

## 14. Fluxos Completos (Exemplos)

### 14.1 Fluxo: Novo Usuário

```mermaid
sequenceDiagram
    participant F as Front-End
    participant API as Monetis API

    F->>API: POST /api/users (criar conta)
    API-->>F: 201 + UserResponse

    F->>API: POST /api/auth/login
    API-->>F: 200 + { token }

    Note over F: Salva token no localStorage/secure storage

    F->>API: GET /api/categories (carregar categorias sistema)
    API-->>F: 200 + CategoryResponse[]

    F->>API: POST /api/accounts (criar conta corrente)
    API-->>F: 201 + AccountResponse

    F->>API: POST /api/accounts (criar cartão de crédito)
    API-->>F: 201 + AccountResponse
```

### 14.2 Fluxo: Adicionar Despesa e Ver Saldo

```mermaid
sequenceDiagram
    participant F as Front-End
    participant API as Monetis API

    F->>API: POST /api/expenses (Cash, R$ 150)
    API->>API: Debita R$ 150 da conta
    API-->>F: 201 + ExpenseResponse (status: Paid)

    F->>API: GET /api/accounts/{id}
    API-->>F: 200 + AccountResponse (balance reduzido em 150)
```

### 14.3 Fluxo: Criar Parcelamento

```mermaid
sequenceDiagram
    participant F as Front-End
    participant API as Monetis API

    F->>API: POST /api/expenses/installments (12x R$ 1200)
    API-->>F: 200 + ExpenseResponse[12]
    
    Note over F: Mostra tabela com 12 parcelas
    
    F->>API: GET /api/expenses (filtrar por installmentGroupId)
    API-->>F: 200 + ExpenseResponse[] (12 itens)
```

### 14.4 Fluxo: Transferir entre Contas e Cancelar

```mermaid
sequenceDiagram
    participant F as Front-End
    participant API as Monetis API

    F->>API: POST /api/transfers (R$ 500, contaA → contaB)
    API-->>F: 201 + TransferResponse

    Note over F: Usuário decide cancelar (mesmo dia)

    F->>API: DELETE /api/transfers/{id}
    API->>API: Estorna valores
    API-->>F: 204 No Content
```

### 14.5 Fluxo: Assinatura Gerando Despesas

```mermaid
sequenceDiagram
    participant F as Front-End
    participant API as Monetis API

    F->>API: POST /api/subscriptions (Netflix, Monthly, R$ 29,90)
    API->>API: Gera 1ª despesa + Avança nextDueDate
    API-->>F: 201 + SubscriptionResponse

    Note over F: Mostra assinatura ativa + 1ª despesa já criada

    F->>API: GET /api/expenses (filtrar subscriptionId)
    API-->>F: 200 + ExpenseResponse[] (inclui a despesa gerada)
```

---

## 💡 Dicas para o Front-End

1. **Cache de categorias**: As categorias do sistema têm IDs fixos. Podem ser cacheadas no front-end.
2. **Formatação de valores**: O backend trabalha com `decimal` (ponto flutuante). Envie e receba valores com 2 casas decimais.
3. **Datas em UTC**: Todas as datas são em UTC (`2026-07-20T00:00:00Z`). Converta para o fuso local do usuário no front-end.
4. **UUIDs**: Todos os IDs são UUIDs (Guid). O backend retorna como strings.
5. **Balance negativo**: Contas podem ficar com saldo negativo! O front-end deve exibir `account.balance` que pode ser menor que zero.
6. **Limpeza de formulários**: `description` da despesa tem limite de **100 caracteres** (diferente dos outros recursos que usam 200).

---

| Versão | Data | Descrição |
|--------|------|-----------|
| 1.0 | Julho 2026 | Primeira versão do guia de consumo |
