# 📨 Respostas da API — Formato Padronizado

> **Camada:** API  
> **Propósito:** Padronizar o formato de todas as respostas da API

---

## Índice

1. [Respostas de Sucesso](#1-respostas-de-sucesso)
2. [Respostas de Erro](#2-respostas-de-erro)
3. [Matriz de Status Codes](#3-matriz-de-status-codes)
4. [Exemplos por Controller](#4-exemplos-por-controller)

---

## 1. Respostas de Sucesso

### 200 OK — Recurso encontrado / listado

```json
// GET /api/accounts/{id}
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "Conta Corrente",
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "type": "Checking",
  "balance": 1500.00
}
```

### 200 OK — Lista

```json
// GET /api/accounts
[
  { "id": "guid-1", "name": "Conta 1", ... },
  { "id": "guid-2", "name": "Conta 2", ... }
]
```

### 201 Created — Recurso criado

```json
// POST /api/accounts
// Headers: Location: /api/accounts/{novo-id}
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "Nova Conta",
  "userId": "...",
  "type": "Checking",
  "balance": 0.00
}
```

### 204 No Content — Operação sem retorno

```json
// PUT /api/accounts/{id}
// DELETE /api/accounts/{id}
// (sem corpo na resposta)
```

---

## 2. Respostas de Erro

### Estrutura Padrão

```json
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
| `statusCode` | `int` | Código HTTP |
| `message` | `string` | Mensagem de erro legível |
| `errorCode` | `string` | Código interno para identificação |
| `details` | `string?` | Detalhes (apenas em dev) |
| `stackTrace` | `string?` | Stack trace (apenas em dev) |

### 400 — Erro de Negócio (DomainException)

```json
{
  "statusCode": 400,
  "message": "Account name is required",
  "errorCode": "BUSINESS_ERROR",
  "details": null,
  "stackTrace": null
}
```

### 400 — Argumento Inválido

```json
{
  "statusCode": 400,
  "message": "Bad Request",
  "errorCode": "04X0",
  "details": null,
  "stackTrace": null
}
```

### 404 — Recurso Não Encontrado

```json
{
  "statusCode": 404,
  "message": "Not Found",
  "errorCode": "04X4",
  "details": null,
  "stackTrace": null
}
```

### 500 — Erro Interno

```json
{
  "statusCode": 500,
  "message": "Internal error",
  "errorCode": "07X0",
  "details": null,
  "stackTrace": null
}
```

### 429 — Rate Limit Excedido

```json
"You have exceeded the limit of attempts. Please try again later"
```

> ⚠️ O rate limit retorna texto simples, não JSON padronizado.

---

## 3. Matriz de Status Codes

| Método | Sucesso | Erro de Negócio | Não Encontrado | Erro Interno |
|--------|---------|-----------------|----------------|--------------|
| `GET /{recurso}` | 200 | 400 | 404 | 500 |
| `GET /{recurso}/{id}` | 200 | 400 | 404 | 500 |
| `POST /{recurso}` | 201 | 400 | — | 500 |
| `PUT /{recurso}/{id}` | 204 | 400 | 404 | 500 |
| `DELETE /{recurso}/{id}` | 204 | 400 | 404 | 500 |

### 401 — Não Autenticado

O ASP.NET Core retorna 401 automaticamente quando:
- Token JWT ausente
- Token JWT expirado
- Token JWT inválido (assinatura incorreta)

```json
// Resposta padrão do ASP.NET Core (não passa pelo ExceptionMiddleware)
{
  "type": "https://tools.ietf.org/html/rfc7235#section-3.1",
  "title": "Unauthorized",
  "status": 401
}
```

---

## 4. Exemplos por Controller

### Auth — POST /login

| Situação | Status | Corpo |
|----------|--------|-------|
| Credenciais válidas | 200 | `{ token: "eyJ..." }` |
| Email não encontrado | 401 | `{ statusCode: 401, message: "Invalid credentials." }` |
| Senha incorreta | 401 | `{ statusCode: 401, message: "Invalid credentials." }` |

### Accounts — POST

| Situação | Status | Corpo |
|----------|--------|-------|
| Dados válidos | 201 | `AccountResponse` |
| Nome vazio | 400 | `{ message: "Account name is required", errorCode: "BUSINESS_ERROR" }` |

### Expenses — POST

| Situação | Status | Corpo |
|----------|--------|-------|
| Criada à vista (Cash) | 201 | `ExpenseResponse` com `status: "Paid"` |
| Criada a prazo (Crédito) | 201 | `ExpenseResponse` com `status: "Pending"` |
| Crédito sem cartão | 400 | `{ message: "Credit card is required...", errorCode: "BUSINESS_ERROR" }` |

### Transfers — DELETE

| Situação | Status | Corpo |
|----------|--------|-------|
| Cancelamento no mesmo dia | 204 | (vazio) |
| Cancelamento em dia diferente | 400 | `{ message: "Transfer can only be cancelled...", errorCode: "BUSINESS_ERROR" }` |
| Transferência já cancelada | 400 | `{ message: "Transfer is already cancelled" }` |

---

> **Próximo:** [Clean Architecture](../05-arquitetura/01-clean-architecture.md)
