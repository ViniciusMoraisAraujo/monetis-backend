# 📐 Decisões Técnicas (ADRs)

> **Camada:** Arquitetura  
> **Propósito:** Registrar as principais decisões arquiteturais e técnicas do projeto

---

## Índice

1. [ADR-001: Clean Architecture](#adr-001-clean-architecture)
2. [ADR-002: Table Per Hierarchy (TPH) para Transações](#adr-002-table-per-hierarchy-tph-para-transações)
3. [ADR-003: Multi-tenancy via Query Filters](#adr-003-multi-tenancy-via-query-filters)
4. [ADR-004: JWT para Autenticação](#adr-004-jwt-para-autenticação)
5. [ADR-005: FluentValidation para Validação](#adr-005-fluentvalidation-para-validação)
6. [ADR-006: Domínio Rico](#adr-006-domínio-rico)

---

## ADR-001: Clean Architecture

| Campo | Valor |
|-------|-------|
| **ID** | ADR-001 |
| **Data** | 2026-05-07 |
| **Status** | ✅ Aceita |
| **Contexto** | Necessidade de separar responsabilidades e permitir testabilidade |

### Decisão
Adotar Clean Architecture com 4 camadas: **Domain**, **Application**, **Infrastructure**, **API**.

### Consequências
- **Positivas:** Testabilidade, independência de frameworks, domínio reutilizável
- **Negativas:** Maior número de projetos, mais arquivos de interface/implementação
- **Riscos:** Over-engineering para features simples; mitigado mantendo o domínio enxuto

### Alternativas Consideradas
- **Monólito simples:** Rejeitado por falta de separação de responsabilidades
- **N-tier tradicional:** Rejeitado por criar dependência direta entre camadas

---

## ADR-002: Table Per Hierarchy (TPH) para Transações

| Campo | Valor |
|-------|-------|
| **ID** | ADR-002 |
| **Data** | 2026-05-07 |
| **Status** | ✅ Aceita |

### Contexto
`Expense`, `Income` e `Transfer` compartilham propriedades comuns (`AccountId`, `Amount`, `Description`) mas têm campos específicos.

### Decisão
Usar **TPH (Table Per Hierarchy)** com uma única tabela `Transactions` e coluna `Discriminator`.

### Consequências
- ✅ **Simplicidade:** Uma única tabela, queries mais simples
- ✅ **Performance:** Sem joins para carregar polimorfismo
- ⚠️ **Espaço:** Colunas nullable para campos específicos (ex: `DestinationAccountId` só é usado por Transfer)
- ⚠️ **Acoplamento:** Alterar uma subclasse pode exigir migração na tabela compartilhada

### Alternativas Consideradas
- **TPT (Table Per Type):** Uma tabela por classe — rejeitado por complexidade de joins
- **TPC (Table Per Concrete):** Tabelas separadas sem tabela base — rejeitado por duplicação

---

## ADR-003: Multi-tenancy via Query Filters

| Campo | Valor |
|-------|-------|
| **ID** | ADR-003 |
| **Data** | 2026-05-07 |
| **Status** | ✅ Aceita |

### Contexto
Cada usuário só pode ver e modificar seus próprios dados. Necessário garantir isolamento em todas as queries.

### Decisão
Aplicar `HasQueryFilter` do EF Core automaticamente em todas as entidades que herdam `UserOwnedEntity`, via reflection no `ModelBuilderExtensions.ApplyMultiTenantFilters()`.

```csharp
// Aplicado automaticamente via reflection
modelBuilder.Entity<T>().HasQueryFilter(e =>
    context.IsUserAuthenticated && e.UserId == context.CurrentUserId);
```

### Consequências
- ✅ **Transparente:** Desenvolvedor não precisa lembrar de adicionar `.Where(u => u.UserId == userId)`
- ✅ **Consistente:** Todas as queries são automaticamente filtradas
- ✅ **Impossível de "esquecer":** O filtro é aplicado no nível do EF Core
- ⚠️ **Categoria tem filtro especial:** `c.UserId == null || (IsUserAuthenticated && c.UserId == CurrentUserId)` — isso porque existem categorias do sistema

---

## ADR-004: JWT para Autenticação

| Campo | Valor |
|-------|-------|
| **ID** | ADR-004 |
| **Data** | 2026-05-07 |
| **Status** | ✅ Aceita |

### Contexto
Necessidade de autenticação stateless para API REST.

### Decisão
Usar **JWT (JSON Web Tokens)** com:
- Algoritmo: HMAC SHA256
- Claims: `sub` (userId), `email`, `jti`
- Expiração: 2 dias
- Chave configurável via `Jwt:Key`

### Consequências
- ✅ **Stateless:** Servidor não precisa armazenar sessões
- ✅ **Padrão da indústria:** Amplamente suportado
- ⚠️ **Token fixo:** Não é possível revogar tokens individualmente (sem blacklist)
- ⚠️ **Expiração longa (2 dias):** Trade-off entre segurança e UX

---

## ADR-005: FluentValidation para Validação

| Campo | Valor |
|-------|-------|
| **ID** | ADR-005 |
| **Data** | 2026-05-07 |
| **Status** | ✅ Aceita |

### Contexto
Necessidade de validar dados de entrada antes de processar nos serviços.

### Decisão
Usar **FluentValidation** com registro automático via `AddValidatorsFromAssembly()`.

```csharp
services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
```

### Consequências
- ✅ **Separação:** Validação fora das entidades e controllers
- ✅ **Testabilidade:** Validators são fáceis de testar unitariamente
- ✅ **Registro automático:** `AddValidatorsFromAssembly` registra todos de uma vez
- ⚠️ **Dupla validação:** Validação no DTO (FluentValidation) + validação na entidade (domínio) — por design, defesa em profundidade

---

## ADR-006: Domínio Rico

| Campo | Valor |
|-------|-------|
| **ID** | ADR-006 |
| **Data** | 2026-05-07 |
| **Status** | ✅ Aceita |

### Contexto
As regras de negócio financeiras são complexas (validação de saldo, parcelamento, estorno de transferências).

### Decisão
Adotar **Domínio Rico** (Rich Domain Model), onde:
- Entidades contêm **comportamento** (métodos), não apenas dados
- Regras de negócio são encapsuladas nas entidades
- Construtores validam invariantes
- Exceções de domínio específicas para cada violação

### Exemplos

```csharp
// Comportamento na entidade, não no service
account.Withdraw(amount);              // Valida amount > 0
expense.Pay(paidAt, accountId);        // Valida status, altera conta
transfer.Cancel(origin, cancellationDate); // Valida data, estorna valores
subscription.Process();                // Gera despesa, calcula próxima data
```

### Consequências
- ✅ **Encapsulamento:** Regras não vazam para os serviços
- ✅ **Testabilidade:** Entidades são testáveis unitariamente sem mocks
- ✅ **Intenção clara:** Código auto-documentado (`account.Withdraw` vs `account.Balance -= amount`)
- ⚠️ **Maior complexidade nas entidades:** Algumas entidades têm muitos métodos (ex: Expense com ~10 métodos públicos)
- ⚠️ **Exceções específicas:** ~60 exceções de domínio — muitas classes pequenas

---

> **Próximo:** [Voltar à Visão Geral](../00-visao-geral.md) *(ou consultar a Finalização na Fase 6 quando disponível)*
