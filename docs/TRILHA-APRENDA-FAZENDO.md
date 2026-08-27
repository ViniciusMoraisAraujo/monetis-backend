# 🛠️ Trilha do Aprenda Fazendo — Monetis

> **Objetivo:** planos de aprendizado prático no formato **"faça X e aprenda Y"**, 100% dentro do projeto Monetis.
> **Formato:** 11 missões em ordem de dificuldade, divididas em 3 blocos. Cada missão tem:
> **🎯 Aprender → 📦 Entregar → 🧭 Passos para Fazer → ✅ Critérios de conclusão (DoD) → 📚 Onde estudar**
>
> **Ritmo sugerido:** 1 missão por sessão de prática (~1 dia). Só avance quando o DoD estiver cumprido.

---

## Índice

- [Bloco A — Aquecer](#bloco-a--aquecer-esquentar-o-motor)
  - [A1 · Primeiros testes de domínio](#a1--primeiros-testes-de-domínio)
  - [A2 · Cenários de negócio em `[Theory]`](#a2--cenários-de-negócio-em-theory)
  - [A3 · Feature vertical mínima (`Tag`)](#a3--feature-vertical-mínima-tag)
- [Bloco B — Construir no Monetis](#bloco-b--construir-no-monetis)
  - [B1 · Cobrança em cartão (`Charge`)](#b1--cobrança-em-cartão-charge)
  - [B2 · Orçamento + relatório mensal](#b2--orçamento--relatório-mensal)
  - [B3 · Divisão de despesa (`Split`)](#b3--divisão-de-despesa-split)
- [Bloco C — Refatorar com arquitetura](#bloco-c--refatorar-com-arquitetura)
  - [C1 · Clean Architecture na prática (NetArchTest)](#c1--clean-architecture-na-prática-netarchtest)
  - [C2 · Faça CQRS (MediatR)](#c2--faça-cqrs-mediatr)
  - [C3 · Faça Value Objects](#c3--faça-value-objects)
  - [C4 · Faça um Domain Service](#c4--faça-um-domain-service)
  - [C5 · Faça Domain Events](#c5--faça-domain-events)
- [Mapa de dependências](#mapa-de-dependências)

---

## Bloco A — Aquecer (esquentar o motor)

Missões pequenas para montar a rede de segurança (testes) e o padrão vertical antes de qualquer refatoração.

### A1 · Primeiros testes de domínio

**🎯 Aprender:** framework de testes (xUnit), asserções legíveis (FluentAssertions) e o que testar numa entidade.

**📦 Entregar:** projeto `tests/Monetis.Domain.Tests` cobrindo `Account` e os validators existentes.

**🧭 Passos:**
1. Criar o projeto de testes e referenciar `Monetis.Domain`.
2. Instalar `xunit`, `xunit.runner.visualstudio`, `Microsoft.NET.Test.Sdk`, `FluentAssertions`.
3. Testar `Account`:
   - `Deposit` e `Withdraw` com valor positivo → saldo muda; com valor ≤ 0 → `AccountAmountMustBePositiveException`.
   - `AdjustBalance` com motivo < 5 caracteres → exceção (ver `src/Monetis.Domain/Entities/Account.cs`).
   - `IsNegative` / `GetNegativeAmount()`.
4. Testar 2 validators do `Monetis.Application` (ex.: `CreateAccountValidator`, `CreateExpenseValidator`): dados válidos passam, dados inválidos falham com a mensagem esperada.

**✅ DoD:** `dotnet test` verde; todos os `throw` de `Account` cobertos.

**📚 Estudo:** `docs/06-testes/02-cenarios-principais.md`; [Unit testing C#](https://learn.microsoft.com/en-us/dotnet/core/testing/).

---

### A2 · Cenários de negócio em `[Theory]`

**🎯 Aprender:** testes parametrizados (`[Theory]`/`InlineData`) e casos-limite de regra de negócio.

**📦 Entregar:** suítes parametrizadas para `Expense.CreateInstallment`, `Transfer.Cancel` e `Subscription.Process`.

**🧭 Passos:**
1. `Expense.CreateInstallment` (ver `docs/07-cenarios-de-negocio/04-instalmentos.md`):
   - parcelas válidas (2 e 24), inválidas (1, 25), ajuste de centavos (valor que não divide redondo), sufixo `(i/N)`.
2. `Transfer.Cancel`: cancelar no mesmo dia (ok) e em data posterior (exceção); transferência já cancelada → segunda exceção.
3. `Subscription.Process`: cada frequência (`Menu.md`/enums `02-enums.md`) avança a data corretamente (+1 dia, +1 semana, +1 mês…); `EndDate` atingida → comportamento esperado; gera a despesa vinculada.

**✅ DoD:** cada regra tem pelo menos 1 `[Theory]` com casos válidos e 1 com casos inválidos.

**📚 Estudo:** `docs/07-cenarios-de-negocio/*`; `docs/06-testes/01-estrategia.md` (seção 2.1).

---

### A3 · Feature vertical mínima (`Tag`)

**🎯 Aprender:** como um requisito atravessa todas as camadas — entidade → config → migration → repositório → service → validator → controller.

**📦 Entregar:** CRUD completo de `Tag` (ex.: tags para classificar despesas), seguindo o padrão de `Category`/`Card`.

**🧭 Passos:**
1. **Domain:** `Tag` (name + slug, `UserOwnedEntity`), exceção própria (`TagNameMustBeValidException`).
2. **Infrastructure:** `TagConfiguration` + migration (`Add-Migration AddTag`), `TagRepository : ITagRepository`, registro no DI.
3. **Application:** `ITagService`/`TagService` (SPA: `GetAllAsync`, `GetByIdAsync`, `CreateAsync`, `UpdateAsync`, `DeleteAsync`), DTOs record, `CreateTagValidator`.
4. **API:** `TagsController` herdando `ApiControllerBase`.
5. **Testes:** unitários de serviço (NSubstitute), validação dos validators, integração via banco real (GET/POST/PUT/DELETE).

**✅ DoD:** feature navega no Swagger; `dotnet test` verde incluindo integração; padrão idêntico ao de `Category`.

**📚 Estudo:** copie de perto `Category` (service, controller, configuration) — é o template canônico.

---

## Bloco B — Construir no Monetis

Funcionalidades reais que exigem modelagem de domínio e decisões próprias.

### B1 · Cobrança em cartão (`Charge`)

**🎯 Aprender:** modelagem de domínio rico, invariantes e transições de estado de uma nova entidade.

**📦 Entregar:** conceito de `Charge` — cobrança de um valor em um `Card`, vinculada a uma `Expense` ou `Income`.

**🧭 Passos:**
1. Decidir as regras: valor > 0, `ChargeStatus` (Pendente/Confirmada/Estornada), limite de parcelas, vínculo com `Card` (e crédito disponível).
2. Modelar `Charge` com métodos que carregam as regras (ex.: `Confirm()`, `Refund()`), cada um lançando exceção se a transição for inválida.
3. Persistência (`ChargeConfiguration` + migration), service, validators, controller.
4. Testes de domínio (transições inválidas) + testes de serviço + integração.

**✅ DoD:** nenhuma regra vive no controller; todas as transições de estado têm teste.

**📚 Estudo:** espelhe o padrão de `Expense.Pay()`/`Transfer.Cancel()`; `docs/05-arquitetura/02-fluxo-de-dados.md`.

---

### B2 · Orçamento + relatório mensal

**🎯 Aprender:** camada de **leitura**: projeções direto para DTO, `AsNoTracking()`, agregações no EF.

**📦 Entregar:** `Budget` (limite mensal por categoria) + endpoint `GET /api/reports/monthly?year=&month=` que soma receitas/despesas/saldo por categoria.

**🧭 Passos:**
1. `Budget` como entidade simples (categoria + valor limite + mês/alvo).
2. Para o relatório, **não passe pela entidade**: crie uma query com projeção (`Select(... => new DTO)`) e `AsNoTracking()` (inspire-se no `ExpenseQueryService`).
3. Considerar `GroupBy` para agregação por categoria no EF e comparar com agrupar em memória.
4. Endpoint + testes de integração validando os totais (inclusive usuários diferentes → multi-tenancy).

**✅ DoD:** relatório calcula corretamente receita/despesa/saldo do mês; query de leitura não rastreia entidades (`AsNoTracking`).

**📚 Estudo:** [EF Core — queries](https://learn.microsoft.com/en-us/ef/core/querying/); `docs/01-domain/01-entidades.md`.

---

### B3 · Divisão de despesa (`Split`)

**🎯 Aprender:** transações **multi-entidade** e atomicidade via `UnitOfWork`.

**📦 Entregar:** despesa dividida entre N contas — criar a despesa e debitar o devido de cada conta em **uma** operação atômica.

**🧭 Passos:**
1. Modelar `Split`: despesa mãe + participações (`accountId`, valor, ordem).
2. Regras: soma das partes = total (tolerância de centavos), todas as contas do mesmo usuário (use `UserResourceGuard` para cada conta).
3. No service, dividir o débito: `Transaction` master + N movimentos, tudo dentro do mesmo `CommitAsync`.
4. Teste crítico: **falha no meio não deixa estado parcial** (simular exceção na 3ª conta → nada persistido).

**✅ DoD:** teste comprova atomicidade; `Split` valida soma das partes igual ao total.

**📚 Estudo:** `docs/03-infrastructure/03-security.md` (multi-tenancy), padrao `UnitOfWork`.

---

## Bloco C — Refatorar com arquitetura

As missões que materializam os conceitos que você citou: **Clean Architecture, CQRS, services e value objects**.

### C1 · Clean Architecture na prática (NetArchTest)

**🎯 Aprender:** regras de dependência (Domain sozinho, Application → Domain, Infrastructure/API → tudo) e como **provar** isso em teste.

**📦 Entregar:** projeto de testes de arquitetura com **NetArchTest** que falha se a regra for violada.

**🧭 Passos:**
1. `dotnet add package NetArchTest.Rules` no `tests/Monetis.Architecture.Tests`.
2. Provas:
   - `Domain` não referencia nenhum outro assembly do projeto (`InDomain`/`HaveNoDependencyOn`).
   - `Application` não referencia `Monetis.Infrastructure` nem `Monetis.API`.
   - `Infrastructure` não referencia `Monetis.API`.
   - Nomes de `IService` terminam em `Service`; entidades herdam `BaseEntity`/`UserOwnedEntity` (consistência).
3. Rodar na CI (Missão C1 concluída = `dotnet test` roda as provas).

**✅ DoD:** as 3 regras de dependência passam; qualquer `using` errado quebra o build do teste.

**📚 Estudo:** [Clean Architecture (R. Martin)](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html); `docs/05-arquitetura/01-clean-architecture.md`.

---

### C2 · Faça CQRS (MediatR)

**🎯 Aprender:** comandos × queries, handlers, pipeline de behaviors (validação e logging) e controllers finos.

**📦 Entregar:** `Expense` migrado de `ExpenseService` para handlers MediatR + `ValidationBehavior` + `LoggingBehavior`.

**🧭 Passos:**
1. `dotnet add package MediatR` em `Monetis.Application`; registrar em `AddApplication()`.
2. Criar `Features/Expenses/Commands/*` (CreateExpense, PayExpense, UpdateExpense, DeleteExpense) e `Features/Expenses/Queries/*` (GetExpenses, GetByPeriod, GetOverdue).
3. Implementar `ValidationBehavior<TRequest,TResponse>` que chama o `IValidator<TRequest>` existente (FluentValidation já registrado) e lança `ValidationException`; registrar os behaviors.
4. Migrar um controller por vez, terminando no `ExpensesController`.
5. **Necessário:** registrar ADR-007 em `docs/05-arquitetura/03-decisoes-tecnicas.md` (contexto/decidão/consequências).

**✅ DoD:** `ExpenseService` ficou sem validação manual ou foi removido; todos os endpoints de Expense funcionam com `Mediator.Send(...)`; testes dos handlers com NSubstitute.

**📚 Estudo:** [MediatR](https://github.com/jbogard/MediatR); [nosso roadmap Fase 3](ROADMAP-DESENVOLVEDOR.md#fase-3--cqrs-com-mediatr).

---

### C3 · Faça Value Objects

**🎯 Aprender:** primitive obsession, invariantes em VO, igualdade por valor e suporte do EF Core (value converters).

**📦 Entregar:** `Money`, `Email`, `Description` (e `AccountName`) + migração das entidades + value converters; resolve B-010…B-012 na raiz.

**🧭 Passos:**
1. `money` — `readonly record` (ou `record struct`) com `Amount`, operadores `+ - * /`, `IsPositive`, `Abs`, arredondamento de 2 casas na criação.
2. `Email` — normaliza lowercase + regex validada na criação.
3. `Description`/`AccountName` — valida maxLength (25/50/100/200) na criação; passa a ser a **única fonte de verdade** dos limites (mata a validação duplicada de entidade + validator).
4. Migrar `Account.Balance`, `Amount` das transações, `Transaction.Description`, `Subscription` (ver bugs B-010/B-011/B-012 de `docs/BUGS-FIX-PLAN.md`).
5. **EF Core:** configurar `HasConversion` (ou `OwnsOne`) em cada `Configuration`; validar a migration gerada.

**✅ DoD:** zero `decimal` de dinheiro solto em entidade; VOs imutáveis, criados só por factory; testes de VO na suíte de domínio; B-010…B-012 fecham.

**📚 Estudo:** [EF Core — Value Conversions](https://learn.microsoft.com/en-us/ef/core/modeling/value-conversions); *Domain Modeling Made Functional* (S. Wlaschin); [roadmap Fase 2](ROADMAP-DESENVOLVEDOR.md#fase-2--value-objects-combater-primitive-obsession).

---

### C4 · Faça um Domain Service

**🎯 Aprender:** quando a regra não cabe numa única entidade → **domain service**; modelos ricos × anêmicos.

**📦 Entregar:** extrair do `Subscription.Process()` a regra de "gerar a despesa da próxima parcela e avançar a data" para um domain service (ex.: `SubscriptionBillingService`), deixando a entidade fina e testável.

**🧭 Passos:**
1. Identificar no `Subscription.Process()` o que é regra de **colaboração** (usa repositório/UoW) vs regra **do agregado**.
2. Criar domain service em `src/Monetis.Domain/Services/` (só depende do Domain) que coordena: gerar `Expense`, movimentar `Account`, avançar próxima data.
3. `Subscription.Process()` delega ao service; testes do service isolados (NSubstitute para `IAccountRepository`/`IExpenseRepository`).
4. Refletir sobre a decisão: valeu emular? (pode registrar como ADR de modelagem).

**✅ DoD:** colaboração de cobrança fora da entidade; comportamento preservado (todos os testes A2/B2 de Subscription continuam verdes).

**📚 Estudo:** *Domain-Driven Design: Tackling Complexity* (E. Evans) — cap. "Services"; [roadmap C4](ROADMAP-DESENVOLVEDOR.md).

---

### C5 · Faça Domain Events

**🎯 Aprender:** desacoplamento por eventos e consistência eventual (outbox).

**📦 Entregar:** `ExpenseCreated`/`InvoiceGenerated` publicado quando a cobrança acontece; consumidor simples (ex.: atualizar algum agregado/log); **outbox** gravado junto na mesma transação do `UnitOfWork`.

**🧭 Passos:**
1. Adicionar `DomainEvents` à `BaseEntity` (coleção privada + `AddDomainEvent`/`ClearDomainEvents`).
2. Disparar `ExpenseCreated` no momento certo da regra de negócio (na entidade/service).
3. No `CommitAsync` do `UnitOfWork`, capturar os eventos e **persisti-los na mesma transação** (tabela `Outbox`), depois despachá-los.
4. Handler que consome (ex.: log estruturado ou atualizar contador no relatório).
5. Documentar o padrão escolhido em ADR (consistência imediata vs eventual).

**✅ DoD:** falha após gravar a entidade não perde o evento (outbox); handler roda após commit; estrutura replicável para `InvoiceGenerated`.

**📚 Estudo:** [Domain events no EF Core](https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/domain-events-design-implementation); *Implementing Domain-Driven Design* (V. Vernon) — outbox/eventual consistency.

---

## Mapa de dependências

```
A1 ──► A2 ──► A3 ──► B1
            │        ├──► B2
            │        └──► B3
     (base de testes)
            └─────────────► C1 ──► C2 ──► C3 ──► C4 ──► C5
```

- **A1/A2** criam a rede de segurança que torna C seguro.
- **A3** ensina o padrão vertical que B reutiliza.
- **C1** protege a arquitetura enquanto C2–C5 refatoram.
- Regra de ouro: **toda mudança nasce de um teste que falha primeiro** (red → green → refactor).

---

## ✅ Checklist resumo

- [ ] **A1** · testes de `Account` + validators verdes
- [ ] **A2** · `[Theory]` para parcelas, transferências e assinaturas
- [ ] **A3** · CRUD `Tag` de ponta a ponta
- [ ] **B1** · `Charge` com estados e regras testados
- [ ] **B2** · `Budget` + relatório mensal agregado (`AsNoTracking`)
- [ ] **B3** · `Split` com teste de atomicidade
- [ ] **C1** · NetArchTest provando as regras de dependência
- [ ] **C2** · `Expense` em MediatR + behaviors + ADR-007
- [ ] **C3** · `Money`/`Email`/`Description` + converters; B-010…B-012 fechados
- [ ] **C4** · cobrança da `Subscription` em domain service
- [ ] **C5** · `ExpenseCreated` + outbox no `UnitOfWork`