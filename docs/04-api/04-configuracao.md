# ⚙️ Configuração da Aplicação

> **Camada:** API (`src/Monetis.API/appsettings.json`)  
> **Propósito:** Centralizar as configurações necessárias para executar a aplicação

---

## Índice

1. [Arquivos de Configuração](#1-arquivos-de-configuração)
2. [Connection Strings](#2-connection-strings)
3. [JWT Settings](#3-jwt-settings)
4. [Logging](#4-logging)
5. [Exemplo Completo](#5-exemplo-completo)

---

## 1. Arquivos de Configuração

A aplicação usa o sistema de configuração do ASP.NET Core, que combina múltiplas fontes:

| Fonte | Arquivo | Propósito |
|-------|---------|-----------|
| JSON | `appsettings.json` | Configurações base (compartilhadas) |
| JSON | `appsettings.Development.json` | Configurações de desenvolvimento (opcional) |
| Variáveis de Ambiente | `CONNECTIONSTRINGS__MONETISCONNECTION`, etc. | Produção / CI/CD |

### Ordem de Precedência

Última fonte vence:
1. `appsettings.json`
2. `appsettings.{Environment}.json`
3. Variáveis de ambiente
4. Command-line arguments

---

## 2. Connection Strings

A connection string do SQL Server é lida de `ConnectionStrings:MonetisConnection`.

### Exemplo (SQL Server Local)

```json
{
  "ConnectionStrings": {
    "MonetisConnection": "Server=localhost;Database=Monetis;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### Exemplo (SQL Server com autenticação SQL)

```json
{
  "ConnectionStrings": {
    "MonetisConnection": "Server=localhost,1433;Database=Monetis;User Id=sa;Password=Su4Senha!;TrustServerCertificate=True;"
  }
}
```

### Via Variável de Ambiente (Produção)

```bash
# Windows
setx ConnectionStrings__MonetisConnection "Server=prod-db;Database=Monetis;User Id=admin;Password=...;"

# Docker / Linux
export ConnectionStrings__MonetisConnection="Server=prod-db;Database=Monetis;..."
```

> ⚠️ No .NET, `:` é usado como separador hierárquico. Em variáveis de ambiente, use `__` (double underscore) como separador.

---

## 3. JWT Settings

Configurações para geração e validação de tokens JWT.

### Estrutura

```json
{
  "Jwt": {
    "Key": "sua-chave-super-segura-com-pelo-menos-32-caracteres!",
    "Issuer": "Monetis",
    "Audience": "MonetisUsers"
  }
}
```

| Chave | Obrigatório | Descrição | Recomendação |
|-------|-------------|-----------|--------------|
| `Jwt:Key` | ✅ | Chave secreta para assinar tokens | Mínimo 32 caracteres, usar string aleatória |
| `Jwt:Issuer` | ✅ | Emissor do token | "Monetis" (qualquer string) |
| `Jwt:Audience` | ✅ | Audiência do token | "MonetisUsers" (qualquer string) |

### Segurança da Chave JWT

| Ambiente | Prática Recomendada |
|----------|-------------------|
| **Desenvolvimento** | `appsettings.Development.json` (valor fixo) |
| **Produção** | Variável de ambiente `Jwt__Key` ou Azure Key Vault |
| **Nunca** | Commit a chave de produção no repositório |

---

## 4. Logging

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

| Namespace | Nível | Propósito |
|-----------|-------|-----------|
| `Default` | `Information` | Logs da aplicação (serviços, controllers) |
| `Microsoft.AspNetCore` | `Warning` | Apenas warnings e erros do ASP.NET Core |

---

## 5. Exemplo Completo

### `appsettings.json`

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### `appsettings.Development.json`

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Information"
    }
  },
  "ConnectionStrings": {
    "MonetisConnection": "Server=localhost;Database=Monetis;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Key": "minha-chave-super-segura-com-32-caracteres!",
    "Issuer": "Monetis",
    "Audience": "MonetisUsers"
  }
}
```

### Variáveis de Ambiente (Produção)

```bash
ConnectionStrings__MonetisConnection="Server=prod-server.database.windows.net;Database=MonetisProd;..."
Jwt__Key="outra-chave-segura-para-producao-32chars"
Jwt__Issuer="Monetis"
Jwt__Audience="MonetisUsers"
```

---

## Mapa de Configurações

| Chave | Onde é Usada | Arquivo |
|-------|-------------|---------|
| `ConnectionStrings:MonetisConnection` | `MonetisDataContextFactory`, `AddDbContext` | Infrastructure |
| `Jwt:Key` | `TokenService.GenerateToken()`, `AddJwtBearer()` | Infrastructure |
| `Jwt:Issuer` | `TokenService.GenerateToken()`, `AddJwtBearer()` | Infrastructure |
| `Jwt:Audience` | `TokenService.GenerateToken()`, `AddJwtBearer()` | Infrastructure |

---

> **Próximo:** [Respostas da API](05-respostas.md)
