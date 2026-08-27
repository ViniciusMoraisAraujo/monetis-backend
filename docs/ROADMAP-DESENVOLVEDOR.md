# 🛤️ Roadmap de Evolução Técnica — Monetis

> **Objetivo:** trilha de crescimento em engenharia de software usando o próprio Monetis como laboratório.
> **Formato:** fases sequenciais; cada uma traz fundamento teórico → passos práticos → exercício → critérios de conclusão (DoD).
> **Stack a adotar:** xUnit, FluentAssertions, NSubstitute, Testcontainers, MediatR.

---

## 📌 Nota de reconciliação com ``docs/06-testes/01-estrategia.md``

O documento `06-testes/01-estrategia.md` ainda cita **Moq** e **SQLite In-Memory / EF InMemory** como recomendados. Este roadmap **atualiza** essas decisões, conforme alinhado:

| Tópico | `01-estrategia.md` (usar como referência de casos) | Este roadmap (decisão final) |
|---|---|---|
| Mocks | Moq | **NSubstitute** |
| Banco de integração | SQLite In-Memory / InMemory | **SQL Server real via Testcontainers** (fiel ao `nvarchar`, query filters e comportamentos de produção) |
| Asserções | FluentAssertions (recomendado) | **FluentAssertions** ✅ |

A **pirâmide de testes** e os **casos de teste listados** em `06-testes` continuam válidos — apenas as ferramentas mudam.

---

## 0. Pré-requisito — Corrigir os bugs conhecidos

Antes de tudo, resolver `docs/BUGS-FIX-PLAN.md` (B-001→B-012). Cada bug é um exercício de **TDD**: escrever o teste que falha → corrigir → teste passa. É a base que permite refatorar nas fases seguintes sem medo de quebrar comportamento.

---

## Fase 1 — Qualidade: testes e padrões de código

### 1.1 Editorconfig + analyzers

**Fundamento:** análise estática e regras CA (CAxxxx), `dotnet format`.

**Passos:**
- Criar `.editorconfig` na raiz e `Directory.Build.props` com `<AnalysisLevel>latest</AnalysisLevel>`.
- Rodar `dotnet build` e zerar os warnings que aparecerem.
- Meta: compilar com **0 warnings**.

**DoD do item:** build sem warnings (<AnalysisLevel>latest</AnalysisLevel> ativo).

### 1.2 Testes de domínio — `tests/Monetis.Domain.Tests`

**Fundamento:** o domínio rico (entidades com comportamento) é a parte mais valiosa de testar sem mocks.

**Projeto:** `xUnit` + `FluentAssertions`, referência apenas ao `Monetis.Domain`.

**Suites sugeridas:**

| Entidade | Comportamentos a testar |
|---|---|
| `Account` | `Deposit`/`Withdraw` (valida > 0), `AdjustBalance` (motivo ≥ 5, `GetNegativeAmount()`), `Update`, `IsNegative` |
| `Transaction`/`Expense` | `Pay` (já paga → `ExpenseAlreadyPaidException`; move saldo da conta), `MarkAsOverDue`, `Update` (paga/parcelada), `CreateInstallment` (2–24, ajuste de centavos, sufixo `(i/N)`, limites de descrição) |
| `Income` | `CreatePaid` vs `Schedule`, `ConfirmReceipt`, `Cancel`, `Update` |
| `Transfer` | construtor executa débito/crédito (contas diferentes, mesmo usuário, saldo), `Cancel` (mesmo dia, estorno), `Update` (cancelada bloqueia) |
| `Subscription` | `Process` (gera despesa + avança data), frequências (+1 dia/mês, etc.), `EndDate`, `Reactivate`, limites de descrição |
| `User`/`UserOwnedEntity` | `SetUser` (uma única vez), `Update`, `ChangePassword`, validação de email |
| Enums/Exceções | todo `throw new XxxException` coberto |

**Exercício:** parametrizar `CreateInstallment` com `[Theory]`/`InlineData` cobrindo casos-limite (2x, 24x, 25x, valor 0).

**DoD do item:** entidades com comportamento 100% cobertas nas regras declaradas em `docs/01-domain/03-excecoes.md` e `06-testes/02-cenarios-principais.md`.

### 1.3 Testes de aplicação — `tests/Monetis.Application.Tests`

**Fundamento:** serviços orquestram repositórios + `UserResourceGuard` + `IUnitOfWork`.

**Ferramentas:** NSubstitute (fakes), FluentAssertions.

**Passos:** testar cada `Service` em `src/Monetis.Application/Services` (Account, Card, Category, Expense, Income, Transfer, Subscription, UserAuth, UserService):
- fluxo feliz
- recurso não encontrado
- ownership violado (`UserResourceGuard`)
- validação via `IValidator<T>` (validators isolados com `ValidateAsync` e com dados válidos/inválidos)

**Tema de estudo:** verificação de interações com mocks (`Received()`) — ex.: `DeleteAsync` chama `DeleteAsync` do repositório **e** `CommitAsync`.

**DoD do item:** todos os services + validators cobertos; mocks verificam contratos com `UnitOfWork`.

### 1.4 Testes de integração — `tests/Monetis.API.IntegrationTests`

**Fundamento:** testar a API de ponta a ponta em **SQL Server real**.

**Ferramentas:** `Microsoft.AspNetCore.Mvc.Testing` (`WebApplicationFactory<Program>`) + **Testcontainers** (`dotnet add package Testcontainers.MsSql`).

**Passos:**
- Fixture sobe container SQL + aplica migrations via `MonetisDataContextFactory`/`MigrateAsync`.
- Fluxos a cobrir (da tabela de E2E de `06-testes/01-estrategia.md`):
  - registrar usuário → login (JWT) → CRUD de conta
  - categoria do sistema vs categoria do usuário
  - despesa à vista movimentando saldo; despesa parcelada gerando N registros
  - transferência cria estorno correto; cancelamento estorna
  - assinatura gera a 1ª despesa
  - processamento `process-overdue` (hosted service)
  - request sem token → 401; `rate limiting` → 429
  - **multi-tenancy** (usuário A não vê dados de B) — o teste de segurança mais importante do projeto

**DoD do item:** a suíte SÓ usa o banco real do container (nada de InMemory, que ignora query filters e comportamento de `nvarchar`).

### 1.5 Cobertura + CI

- `coverlet.collector` + `coverlet.msbuild`; rodar com `dotnet test --collect:"XPlat Code Coverage"`.
- `ReportGenerator` (HTML) com gate sugerido de **≥ 70%** nas camadas Domain/Application.

**DoD da Fase 1:** `dotnet test` verde com cobertura publicada; 0 warnings no build.

---

## Fase 2 — Value Objects (combater "primitive obsession")

**Fundamento:** valores com regras próprias não devem ser `decimal`/`string` crus. Um VO é **imutável**, compara-se por **valor** e carrega suas **invariantes**.

### 2.1 `Money` — `src/Monetis.Domain/ValueObjects/Money.cs`

- Encapsula arredondamento (2 casas), operadores `+`, `-`, `*`, `/`, `!=`, `IsPositive`, `Abs`, formatação.
- Migrar `Account.Balance`, `Account.GetNegativeAmount()`, `Amount` das transações, `AdjustBalance`.
- **Ponto-chave com EF Core:** usar **value converters** (`HasConversion(v => v.Amount, v => Money.From(...))`) (ver `docs/03-infrastructure/01-persistence.md`).
- **Exercício:** garantir que `Money` nunca aceite arredondamento implícito; `CreateInstallment` passa a depender de `Money` para o ajuste de centavos.

### 2.2 `Email` — `src/Monetis.Domain/ValueObjects/Email.cs`

- Normaliza lowercase, valida regex e tamanho — hoje espalhado no `User` e no validator de usuário.

### 2.3 `Description` / `AccountName`

- Absorve as regras de tamanho (25/50/100/200) que hoje vivem duplicadas (entidade + validator + coluna).
- **Liga com o BUGS-FIX-PLAN:** resolve B-010/B-011/B-012 pela raiz — o limite passa a ter **uma única fonte de verdade** no VO.
- Considerar `Currency`/`Percentage` apenas se um novo requisito pedir.

**DoD:** nenhum `decimal` de dinheiro solto em entidade; `Email`/`Description` obrigam a criação via factory; testes de VO na suíte de domínio.

---

## Fase 3 — CQRS com MediatR

**Fundamento:** separar **Comandos** (escrevem, mudam estado) de **Queries** (leem, retornam DTO). Controllers ficam finos (1-3 linhas), serviços viram *handlers*.

### 3.1 Setup — `src/Monetis.Application`

- `dotnet add package MediatR`
- `AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))` em `DependencyInjection.cs`.
- Estrutura por agregado: `Features/Expenses/Commands/CreateExpenseCommand{,.Handler}` e `Features/Expenses/Queries/GetExpensesQuery{,.Handler}`.

### 3.2 Behaviors (pipeline) — o ganho real

| Behavior | Responsabilidade |
|---|---|
| `ValidationBehavior<TRequest,TResponse>` | Invoca o `IValidator<TRequest>` já registrado (FluentValidation) e lança `ValidationException`; **remove a validação manual dos services** |
| `LoggingBehavior` | Log de request/handler/duração (ex.: `Stopwatch`) |
| `TransactionBehavior` | Coordena o `IUnitOfWork.CommitAsync()` (cuidado: queries não devem commitar) |

**Exercício:** usar o `LoggingBehavior` com `Stopwatch` para identificar a query mais cara do `ExpenseQueryService`.

### 3.3 Migração por agregado — começar por `Expense`

1. Mover `ExpenseService` → handlers de comando; validators já chamados pelo `ValidationBehavior`.
2. `GetExpensesQuery` com projeção direta para DTO (`AsNoTracking()`, sem materializar entidade).
3. Testes: handlers com NSubstitute (a suíte da Fase 1.3 evolui para "testes de handlers").
4. Controller: `[HttpPost] → Mediator.Send(new CreateExpenseCommand(...))`.
- Depois, na mesma ordem: `Account`, `Transfer`, `Subscription`.

**DoD:** `ExpenseService`/`IncomeService` ficam vazios ou são removidos; cada handler tem testes; `ValidationBehavior` desliga a validação inline duplicada.

### ⚠️ Trade-off para registrar no ADR

Adicionar **ADR-007: CQRS com MediatR** em `docs/05-arquitetura/03-decisoes-tecnicas.md` (contexto/decidão/consequências) — documentar *por que* a indireção vale e o custo (mais arquivos, rastreabilidade).

---

## Fase 4 — Engenharia de Produção (diferencial de mercado)

| Item | Ação concreta no projeto |
|---|---|
| **ProblemDetails** | `AddProblemDetails()`; `ExceptionMiddleware` devolve `application/problem+json` (RFC 7807) — hoje é body custom (ver `docs/04-api/02-middlewares.md`) |
| **Serilog** | substituir logging de console por structured logs (Sink.Console + arquivo; enriquecer com `UserId`/`correlationId`) |
| **Health Checks** | `MapHealthChecks` + probe do DbContext (era FEAT-004 do `docs/ANALISE-MELHORIAS-PROJETO.md`) |
| **Paginação** | `PageRequest`/`PageResult<T>` nos repositórios de leitura (skip/take + `CountAsync`) |
| **CI** | `.github/workflows/build-test.yml`: NuGet restore → build → test com Testcontainers → coverage |
| **Domain Events + Outbox** (avançado/opcional) | desacoplar `Subscription.Process()` → geração de despesa |
| **OpenTelemetry** (opcional) | traces via `AddOpenTelemetry().WithTracing(...)` |

**DoD:** pipeline de CI verde; `docker compose up` sobe API + SQL + testes; API retorna `problem+json`; logs estruturados buscáveis.

---

## 🗺️ Mapa para o código atual

| Fase | Caminhos principais no código |
|---|---|
| 1.2 | `src/Monetis.Domain/Entities/*` (Account, Expense, Income, Transfer, Subscription, User) e `Domain/Exceptions/*` |
| 1.3 | `src/Monetis.Application/Services/*`, `Services/UserServices/*`, `Abstractions.Services/*`, Validators |
| 1.4 | `src/Monetis.API/Program.cs`, `Migrations`, `Infrastructure/Persistence/MonetisDataContext` |
| 2 | `src/Monetis.Domain/Entities/Account.cs` (Balance), `Transaction.cs`, `Expense.cs`, `Subscription.cs`, configurações em `Infrastructure/Persistence/Configurations/*` |
| 3 | `src/Monetis.Application/DependencyInjection.cs`, `Services/ExpenseService.cs`, `Services/ExpenseQueryService.cs`, `Controllers/ExpensesController.cs` |
| 4 | `src/Monetis.API/Middlewares/ExceptionMiddleware.cs`, `appsettings.json`, `Program.cs` |

---

## 📚 Proposta de ordem de leitura (para estudo)

1. *Clean Architecture* (Robert C. Martin) — a base que o projeto já segue.
2. *Domain Modeling Made Functional* (Scott Wlaschin) — value objects e tipos.
3. Docs oficiais:
   - [Unit testing C#](https://learn.microsoft.com/en-us/dotnet/core/testing/)
   - [Testcontainers for .NET](https://dotnet.testcontainers.org/)
   - [MediatR](https://github.com/jbogard/MediatR)
   - [EF Core Value Conversions](https://learn.microsoft.com/en-us/ef/core/modeling/value-conversions)
   - [ProblemDetails (RFC 7807)](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/openapi#problem-details)

---

## ✅ Checklist resumo

- [ ] Fase 1: analyzers + Domain/Application/Integration tests + cobertura ≥ 70%
- [ ] Fase 2: `Money`, `Email`, `Description` com value converters
- [ ] Fase 3: MediatR + Validation/Logging/Transaction behaviors + ADR-007
- [ ] Fase 4: ProblemDetails, Serilog, health checks, paginação, CI
- [ ] Sempre: cada mudança nasce de um teste que falha