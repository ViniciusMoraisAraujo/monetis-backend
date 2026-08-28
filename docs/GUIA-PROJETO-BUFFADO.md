# 💪 Guia do Projeto Buffado — Monetis

> **Objetivo:** Mapa completo de tudo que falta para transformar o Monetis de um projeto de estudos em um repositório com maturidade de time sênior. Dividido em 10 categorias, cada uma com o **porquê**, **como fazer** e **nível de prioridade**.
>
> **Legenda de prioridade:**
> 🔴 Essencial (faça primeiro) · 🟡 Importante (destaque em portfólio) · 🟢 Diferencial (impressiona em entrevistas)

---

## 🗺️ Índice

1. [Governança de Código (`.editorconfig`, `Directory.Build.props`, `global.json`)](#1-governança-de-código)
2. [Gestão de Configurações e Segredos](#2-gestão-de-configurações-e-segredos)
3. [Observabilidade (Logging Estruturado, Health Checks, Métricas)](#3-observabilidade)
4. [Resiliência e Robustez do Banco de Dados](#4-resiliência-e-robustez-do-banco-de-dados)
5. [Segurança HTTP (Headers, CORS, HSTS)](#5-segurança-http)
6. [Testes de Integração (WebApplicationFactory & Testcontainers)](#6-testes-de-integração)
7. [Cobertura de Código (Code Coverage)](#7-cobertura-de-código)
8. [API Profissional (Versionamento, Compressão, ProblemDetails)](#8-api-profissional)
9. [README & Presença no GitHub (Badges, LICENSE, Templates)](#9-readme--presença-no-github)
10. [Makefile / Task Runner (Atalhos de Desenvolvimento)](#10-makefile--task-runner)

---

## 1. Governança de Código

**Prioridade:** 🔴 Essencial

> Detalhes completos em [`PADROES-E-GOVERNANCA-PROJETO.md`](PADROES-E-GOVERNANCA-PROJETO.md).

### O que o Monetis NÃO tem hoje:

| Arquivo | O que faz | Status |
| :--- | :--- | :---: |
| `.editorconfig` | Padroniza indentação, encoding, convenções de nomes C# entre IDEs | ❌ Falta |
| `Directory.Build.props` | Centraliza `TargetFramework`, `Nullable`, `TreatWarningsAsErrors` para todos os `.csproj` | ❌ Falta |
| `Directory.Packages.props` | Centraliza versões de NuGet (CPM) em um único arquivo | ❌ Falta |
| `global.json` | Trava a versão exata do .NET SDK usada no projeto | ❌ Falta |
| Roslyn Analyzers | Análise estática de código embutida no `dotnet build` | ❌ Falta |

### Por que isso importa?
Sem esses arquivos, cada desenvolvedor (ou cada IDE) pode usar regras diferentes de formatação, versões diferentes de pacotes NuGet e versões diferentes do SDK. Em um time real, isso gera PRs cheios de "diff fantasma" (mudanças de espaço/tab) e bugs sutis por incompatibilidade de versão.

---

## 2. Gestão de Configurações e Segredos

**Prioridade:** 🔴 Essencial

> Detalhes completos em [`PADROES-E-GOVERNANCA-PROJETO.md`](PADROES-E-GOVERNANCA-PROJETO.md) (Seção 1).

### A Hierarquia de Configuração do ASP.NET Core

O .NET **NÃO usa `.env`** nativamente como o Node.js. Ele tem seu próprio sistema de configuração hierárquica, onde cada camada sobrescreve a anterior:

```
appsettings.json          → Base (valores padrão, versionados no Git)
appsettings.{Env}.json    → Por ambiente (Development, Staging, Production)
dotnet user-secrets        → Segredos locais do dev (FORA do repositório)
Variáveis de Ambiente      → Produção / Docker / CI (maior prioridade)
```

### O que o Monetis precisa ajustar:

#### a) Criar `.env.example` como documentação
Mesmo que o .NET não leia `.env` nativamente, ter um `.env.example` na raiz ajuda qualquer dev (inclusive quem vem do Node.js/Python) a entender rapidamente quais variáveis existem:

```env
# .env.example — NÃO contém valores reais, apenas nomes
ConnectionStrings__MonetisConnection="Server=localhost;Database=MonetisDb;User Id=sa;Password=TROQUE_AQUI;TrustServerCertificate=True;"
Jwt__Key="TROQUE_AQUI_MinimoDe32Caracteres!!!!"
Jwt__Issuer="Monetis"
Jwt__Audience="MonetisUsers"
```

#### b) Usar `dotnet user-secrets` para desenvolvimento local
```bash
# Inicializar no projeto da API (cria um UserSecretsId no .csproj)
dotnet user-secrets init --project src/Monetis.API

# Salvar segredos FORA do repositório
dotnet user-secrets set "ConnectionStrings:MonetisConnection" "Server=localhost;Database=MonetisDb;..." --project src/Monetis.API
dotnet user-secrets set "Jwt:Key" "MinhaChaveLocalSegura32Caracteres!" --project src/Monetis.API
```

#### c) Para Docker: carregar via `docker-compose.yml` com arquivo `.env`
```yaml
services:
  monetis-api:
    build: .
    env_file:
      - .env   # Docker Compose lê este arquivo e injeta como variáveis de ambiente
    ports:
      - "5074:8080"
```

#### d) Fail-Fast: Validar configurações obrigatórias no startup
Adicionar validação no `Program.cs` para que a API **não suba** se faltar uma configuração crítica:
```csharp
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Jwt:Key is required. Set it via user-secrets or environment variable.");

if (jwtKey.Length < 32)
    throw new InvalidOperationException("Jwt:Key must be at least 32 characters.");
```

---

## 3. Observabilidade

**Prioridade:** 🟡 Importante

### 3.1 Logging Estruturado com Serilog

Hoje o Monetis usa o `ILogger` padrão do ASP.NET Core com output de texto simples no console. Em produção, logs de texto são difíceis de filtrar e buscar.

**Serilog** gera logs em **JSON estruturado**, permitindo buscas por campo (ex: "me mostre todos os logs com `UserId = abc` e `StatusCode = 500`").

**Pacotes:**
```bash
dotnet add src/Monetis.API package Serilog.AspNetCore
dotnet add src/Monetis.API package Serilog.Sinks.Console
dotnet add src/Monetis.API package Serilog.Sinks.File
```

**Configuração no `Program.cs`:**
```csharp
builder.Host.UseSerilog((context, config) => config
    .ReadFrom.Configuration(context.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/monetis-.log", rollingInterval: RollingInterval.Day));
```

### 3.2 Health Checks

Health checks permitem que ferramentas de monitoramento (Kubernetes, Docker, Azure) saibam se a API está saudável.

**No `Program.cs`:**
```csharp
builder.Services.AddHealthChecks()
    .AddDbContextCheck<MonetisDataContext>();

// Após app.Build()
app.MapHealthChecks("/health");
```

### 3.3 OpenTelemetry (Nível Avançado)

Para rastreamento distribuído (traces) e métricas de performance. Permite ver quanto tempo cada query SQL, cada middleware e cada service levou para executar.

```bash
dotnet add src/Monetis.API package OpenTelemetry.Extensions.Hosting
dotnet add src/Monetis.API package OpenTelemetry.Instrumentation.AspNetCore
dotnet add src/Monetis.API package OpenTelemetry.Instrumentation.EntityFrameworkCore
```

---

## 4. Resiliência e Robustez do Banco de Dados

**Prioridade:** 🟡 Importante

### 4.1 Concorrência Otimista (`RowVersion`)

**O problema atual:** Se dois requests sacarem da mesma conta no mesmo milissegundo, ambos leem o mesmo saldo, ambos subtraem e ambos salvam — resultando em saldo incorreto (lost update).

**A solução:** Adicionar uma coluna `RowVersion` (ou `ConcurrencyToken`) nas entidades críticas:
```csharp
// Na entidade
public class Account : UserOwnedEntity
{
    // ... propriedades existentes
    [Timestamp]
    public byte[] RowVersion { get; private set; }
}
```

O EF Core automaticamente verifica se o `RowVersion` mudou entre o SELECT e o UPDATE. Se mudou, lança `DbUpdateConcurrencyException`.

### 4.2 Transactions Explícitas para Operações Multi-Entidade

Hoje o `TransferService.CreateAsync` faz `Withdraw` + `Deposit` + `Create` sem transaction explícita. Se o `SaveChanges` falhar no meio, o estado pode ficar inconsistente.

```csharp
await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
try
{
    sourceAccount.Withdraw(amount);
    destinationAccount.Deposit(amount);
    transferRepository.Create(transfer);
    await unitOfWork.CommitAsync(cancellationToken);
    await transaction.CommitAsync(cancellationToken);
}
catch
{
    await transaction.RollbackAsync(cancellationToken);
    throw;
}
```

### 4.3 Retry Policy (Resiliência de Conexão)

Conexões SQL podem cair momentaneamente. O EF Core tem retry nativo:
```csharp
services.AddDbContext<MonetisDataContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions =>
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(5),
            errorNumbersToAdd: null)));
```

### 4.4 Índices de Performance

Adicionar índices compostos nas queries mais frequentes:
```csharp
// No EntityConfiguration
builder.HasIndex(e => new { e.AccountId, e.DueDate })
    .HasDatabaseName("IX_Expenses_AccountId_DueDate");

builder.HasIndex(e => new { e.Status, e.DueDate })
    .HasDatabaseName("IX_Expenses_Status_DueDate")
    .HasFilter("[Status] = 0"); // Só Pending
```

---

## 5. Segurança HTTP

**Prioridade:** 🟡 Importante

### 5.1 Security Headers

Headers HTTP que protegem contra ataques comuns (XSS, clickjacking, MIME sniffing):

```csharp
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    context.Response.Headers.Append("X-XSS-Protection", "0");
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
    context.Response.Headers.Append("Content-Security-Policy", "default-src 'self'");
    await next();
});
```

### 5.2 CORS (Cross-Origin Resource Sharing)

Hoje o Monetis tem `AllowedHosts: "*"`, que aceita qualquer origem. Quando o frontend existir, configure CORS restritivo:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("MonetisPolicy", policy =>
    {
        policy.WithOrigins("https://monetis.app", "http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

app.UseCors("MonetisPolicy");
```

### 5.3 HSTS (HTTP Strict Transport Security)

Força o navegador a usar apenas HTTPS após o primeiro acesso:
```csharp
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}
```

---

## 6. Testes de Integração

**Prioridade:** 🟡 Importante

### 6.1 `WebApplicationFactory` (API em Memória)

Testa o pipeline HTTP completo (middleware + controller + service + banco) sem precisar de um servidor rodando:

```csharp
public class ExpensesApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ExpensesApiTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Substitui SQL Server por banco in-memory
                services.AddDbContext<MonetisDataContext>(options =>
                    options.UseInMemoryDatabase("TestDb"));
            });
        }).CreateClient();
    }

    [Fact]
    public async Task CreateExpense_WithValidData_Returns201()
    {
        // Arrange - login, obter token...
        // Act
        var response = await _client.PostAsJsonAsync("/api/expenses", request);
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}
```

### 6.2 Testcontainers (Banco Real em Docker)

Para testes que precisam de SQL Server real (ex: testar migrations, query filters, TPC):

```bash
dotnet add tests/Monetis.Integration.Tests package Testcontainers.MsSql
```

```csharp
var container = new MsSqlBuilder()
    .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
    .Build();

await container.StartAsync();
var connectionString = container.GetConnectionString();
```

---

## 7. Cobertura de Código

**Prioridade:** 🟢 Diferencial

### 7.1 Gerar Relatório de Cobertura

```bash
dotnet test --collect:"XPlat Code Coverage"
dotnet tool install -g dotnet-reportgenerator-globaltool
reportgenerator -reports:**/coverage.cobertura.xml -targetdir:coverage-report -reporttypes:Html
```

### 7.2 Integrar no CI/CD

Adicionar no GitHub Actions:
```yaml
- name: Run tests with coverage
  run: dotnet test --collect:"XPlat Code Coverage" --results-directory ./coverage

- name: Upload coverage to Codecov
  uses: codecov/codecov-action@v4
  with:
    files: ./coverage/**/coverage.cobertura.xml
```

### 7.3 Badge de Cobertura no README

Após integrar com Codecov ou Coveralls:
```markdown
![Coverage](https://codecov.io/gh/ViniciusMoraisAraujo/monetis-backend/branch/main/graph/badge.svg)
```

---

## 8. API Profissional

**Prioridade:** 🟡 Importante

### 8.1 ProblemDetails (RFC 7807)

Padrão da indústria para respostas de erro em APIs REST. O ASP.NET Core tem suporte nativo:

```csharp
builder.Services.AddProblemDetails();
```

Resposta padronizada:
```json
{
  "type": "https://tools.ietf.org/html/rfc7807",
  "title": "Business Rule Violation",
  "status": 400,
  "detail": "Account balance is insufficient for this withdrawal.",
  "instance": "/api/expenses",
  "errorCode": "BUSINESS_ERROR"
}
```

### 8.2 Response Compression

Comprime as respostas JSON automaticamente (reduz até 70% do tráfego):

```csharp
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});

app.UseResponseCompression(); // Antes dos middlewares de conteúdo
```

### 8.3 API Versioning

Quando o frontend estiver consumindo v1, você pode criar v2 sem quebrar:

```bash
dotnet add src/Monetis.API package Asp.Versioning.Mvc
dotnet add src/Monetis.API package Asp.Versioning.Mvc.ApiExplorer
```

```csharp
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ExpensesController : ApiControllerBase { }
```

### 8.4 Paginação

Todos os endpoints `GetAll` hoje retornam a lista completa. Criar um padrão de paginação:

```csharp
public record PagedResult<T>(
    IReadOnlyList<T> Items,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages);
```

---

## 9. README & Presença no GitHub

**Prioridade:** 🟢 Diferencial

### 9.1 Badges Dinâmicas

Adicionar no topo do `README.md` quando o CI/CD estiver pronto:
```markdown
![Build](https://github.com/ViniciusMoraisAraujo/monetis-backend/actions/workflows/ci.yml/badge.svg)
![Coverage](https://codecov.io/gh/ViniciusMoraisAraujo/monetis-backend/branch/main/graph/badge.svg)
![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)
![License](https://img.shields.io/badge/license-MIT-blue)
```

### 9.2 Templates de Issue e Pull Request

Criar templates padronizados no GitHub:

**`.github/ISSUE_TEMPLATE/bug_report.md`:**
```markdown
---
name: Bug Report
about: Reportar um problema encontrado
---

## Descrição
<!-- Descreva o bug -->

## Passos para Reproduzir
1.
2.
3.

## Comportamento Esperado

## Comportamento Atual

## Ambiente
- .NET Version:
- OS:
```

**`.github/PULL_REQUEST_TEMPLATE.md`:**
```markdown
## O que mudou?
<!-- Descreva as alterações -->

## Tipo de mudança
- [ ] Bug fix
- [ ] Nova feature
- [ ] Refatoração
- [ ] Documentação

## Checklist
- [ ] Testes adicionados/atualizados
- [ ] Documentação atualizada
- [ ] `dotnet build` sem warnings
- [ ] `dotnet test` verde
```

### 9.3 Arquivo `LICENSE`

O README menciona MIT, mas o arquivo `LICENSE` na raiz não existe. Criar para formalizar.

---

## 10. Makefile / Task Runner

**Prioridade:** 🟢 Diferencial

Em vez de lembrar comandos longos, criar um arquivo de atalhos na raiz. No Windows, usar um script PowerShell (`make.ps1`) ou um `Makefile` (se tiver Make instalado):

### Opção A: `make.ps1` (PowerShell)
```powershell
param([string]$Target = "help")

switch ($Target) {
    "build"    { dotnet build Monetis.slnx --configuration Release }
    "test"     { dotnet test Monetis.slnx --configuration Release --verbosity normal }
    "format"   { dotnet format Monetis.slnx }
    "coverage" {
        dotnet test --collect:"XPlat Code Coverage"
        reportgenerator -reports:**/coverage.cobertura.xml -targetdir:coverage-report -reporttypes:Html
        Start-Process coverage-report/index.html
    }
    "migrate"  {
        param([string]$Name)
        dotnet ef migrations add $Name --project src/Monetis.Infrastructure --startup-project src/Monetis.API
    }
    "db-update" { dotnet ef database update --project src/Monetis.Infrastructure --startup-project src/Monetis.API }
    "run"      { dotnet run --project src/Monetis.API }
    "clean"    { dotnet clean Monetis.slnx }
    "help"     {
        Write-Host ""
        Write-Host "Comandos disponíveis:" -ForegroundColor Cyan
        Write-Host "  .\make.ps1 build      — Compila a solução"
        Write-Host "  .\make.ps1 test       — Roda todos os testes"
        Write-Host "  .\make.ps1 format     — Formata o código (.editorconfig)"
        Write-Host "  .\make.ps1 coverage   — Gera relatório de cobertura HTML"
        Write-Host "  .\make.ps1 run        — Sobe a API"
        Write-Host "  .\make.ps1 db-update  — Aplica migrations pendentes"
        Write-Host "  .\make.ps1 clean      — Limpa artefatos de build"
        Write-Host ""
    }
}
```

**Uso:**
```bash
.\make.ps1 build
.\make.ps1 test
.\make.ps1 coverage
```

---

## 🏆 Checklist Geral do Projeto Buffado

### 🔴 Essenciais (Fazer Primeiro)
- [ ] `.editorconfig` na raiz
- [ ] `Directory.Build.props` com `TreatWarningsAsErrors`
- [ ] `global.json` travando SDK
- [ ] `.env.example` na raiz
- [ ] `dotnet user-secrets` configurado para dev local
- [ ] Fail-Fast no `Program.cs` (validar `Jwt:Key` e connection string no startup)

### 🟡 Importantes (Destaque em Portfólio)
- [ ] Serilog com logging estruturado (JSON)
- [ ] Health Check (`/health`) com verificação do banco
- [ ] Concorrência otimista (`RowVersion`) em `Account`
- [ ] Transactions explícitas em `TransferService`
- [ ] Retry policy no EF Core (`EnableRetryOnFailure`)
- [ ] Índices compostos nas queries frequentes
- [ ] Security Headers (X-Content-Type-Options, X-Frame-Options)
- [ ] CORS configurado corretamente
- [ ] `ProblemDetails` (RFC 7807) no ExceptionMiddleware
- [ ] Response Compression
- [ ] Paginação nos endpoints de lista
- [ ] Testes de integração com `WebApplicationFactory`
- [ ] `Directory.Packages.props` (CPM)
- [ ] Roslyn Analyzers (`SonarAnalyzer.CSharp`)

### 🟢 Diferenciais (Impressiona em Entrevistas)
- [ ] OpenTelemetry (traces + métricas)
- [ ] Testcontainers para testes com SQL Server real
- [ ] Code Coverage com relatório HTML e badge no README
- [ ] API Versioning (`/api/v1/expenses`)
- [ ] Templates de Issue e PR no GitHub
- [ ] Arquivo `LICENSE` (MIT) na raiz
- [ ] `make.ps1` com atalhos de desenvolvimento
- [ ] Conventional Commits em todo o histórico
- [ ] Badge de Build do GitHub Actions no README
