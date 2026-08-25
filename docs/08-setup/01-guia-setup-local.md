# 🛠️ Guia de Setup Local

> **Propósito:** Configurar o ambiente de desenvolvimento do Monetis
> **Tempo estimado:** 10-15 minutos

---

## 1. Pré-requisitos

| Ferramenta | Versão Mínima | Como verificar | Onde baixar |
|-----------|:------------:|---------------|-------------|
| .NET SDK | 10.0+ | `dotnet --version` | [dotnet.microsoft.com](https://dotnet.microsoft.com/download) |
| SQL Server | 2022+ | SQL Server Management Studio | [microsoft.com/pt-br/sql-server](https://www.microsoft.com/pt-br/sql-server/sql-server-downloads) |
| Git | 2.0+ | `git --version` | [git-scm.com](https://git-scm.com/) |

> 💡 **Alternativa:** Se não tiver SQL Server local, use o Docker (ver seção 4).

---

## 2. Clone e Instalação

```bash
# 1. Clone o repositório
git clone https://github.com/ViniciusMoraisAraujo/monetis.git
cd monetis

# 2. Restaure as dependências
dotnet restore
```

---

## 3. Variáveis de Ambiente

Crie o arquivo `src/Monetis.API/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "MonetisConnection": "Server=localhost,1433;Database=MonetisDb;User Id=sa;Password=SuaSenha@123;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Key": "sua-chave-secreta-com-pelo-menos-32-caracteres!",
    "Issuer": "Monetis",
    "Audience": "MonetisUsers"
  }
}
```

### Variáveis obrigatórias

| Variável | Descrição | Exemplo |
|----------|-----------|---------|
| `ConnectionStrings:MonetisConnection` | Connection string do SQL Server | `Server=localhost,1433;Database=MonetisDb;...` |
| `Jwt:Key` | Chave secreta para assinar JWT (mín. 32 chars) | `minha-chave-super-segura-com-32-caracteres!` |
| `Jwt:Issuer` | Emissor do token | `Monetis` |
| `Jwt:Audience` | Audiência do token | `MonetisUsers` |

> ⚠️ **Nunca** commite o `appsettings.Development.json` — ele já está no `.gitignore`.

---

## 4. SQL Server (Docker — Opcional)

Se não tiver SQL Server instalado localmente:

```bash
# Execute o SQL Server em container
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=SuaSenha@123" \
  -p 1433:1433 --name monetis-sql \
  -d mcr.microsoft.com/mssql/server:2022-latest
```

Ajuste a connection string em `appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "MonetisConnection": "Server=localhost,1433;Database=MonetisDb;User Id=sa;Password=SuaSenha@123;TrustServerCertificate=True;"
  }
}
```

---

## 5. Rodar Migrations

```bash
# Aplique as migrations no banco de dados
dotnet ef database update \
  --project src/Monetis.Infrastructure \
  --startup-project src/Monetis.API
```

> Isso cria o banco `MonetisDb` com todas as tabelas e categorias do sistema (Alimentação, Transporte, Salário).

---

## 6. Executar a API

```bash
dotnet run --project src/Monetis.API
```

A API estará disponível em:

| URL | Descrição |
|-----|-----------|
| `https://localhost:5074` | API REST |
| `https://localhost:5074/swagger` | Swagger UI |
| `https://localhost:5074/scalar/v1` | Scalar API Reference |
| `http://localhost:5074/health` | Health Check |

---

## 7. Testar a API

### Criar usuário
```bash
curl -X POST https://localhost:5074/api/users \
  -H "Content-Type: application/json" \
  -d '{"firstName":"João","lastName":"Silva","email":"joao@test.com","password":"Senha@123"}'
```

### Login
```bash
curl -X POST https://localhost:5074/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"joao@test.com","password":"Senha@123"}'
```

### Listar categorias (com token)
```bash
TOKEN="eyJhbGciOiJIUzI1NiIs..."
curl https://localhost:5074/api/categories \
  -H "Authorization: Bearer $TOKEN"
```

---

## 8. Estrutura de Pastas

```
monetis/
├── src/
│   ├── Monetis.API/              # Controllers, Middlewares, Program.cs
│   ├── Monetis.Application/      # Services, DTOs, Validators, Interfaces
│   ├── Monetis.Infrastructure/   # EF Core, Repositories, JWT, Migrations
│   └── Monetis.Domain/           # Entities, Enums, Exceptions
├── docs/                         # Documentação
├── Monetis.slnx                  # Solution file
└── README.md
```

---

## 9. Problemas Comuns

### Erro: "A network-related or instance-specific error occurred"
- Verifique se o SQL Server está rodando
- Verifique a connection string em `appsettings.Development.json`

### Erro: "The JWT key is not configured"
- Adicione a chave `Jwt:Key` no `appsettings.Development.json`

### Erro: "No service for type 'IExpenseQueryService'"
- O `ExpenseQueryService` não está registrado no DI — isso é um bug conhecido (ver `docs/ANALISE-MELHORIAS-PROJETO.md`)

### Erro: "The name 'app' does not exist in the current context"
- Verifique se está usando .NET 10 (top-level statements)

---

> **Próximo:** [Guia de Contribuição](02-guia-contribuicao.md)
