# 🚀 Guia de Deployment

> **Propósito:** Publicar o Monetis em ambiente de produção

---

## 1. Pré-requisitos

- .NET 10 SDK instalado no servidor de build
- SQL Server acessível (Azure SQL, RDS, ou local)
- Variáveis de ambiente configuradas

---

## 2. Build de Publicação

```bash
# Publish (gera binário otimizado)
dotnet publish src/Monetis.API \
  -c Release \
  -o ./publish \
  --self-contained false
```

### Resultado
```
./publish/
├── Monetis.API.dll
├── Monetis.API.exe (Windows)
├── appsettings.json
├── appsettings.Development.json (NÃO incluir em produção)
└── ... (dependências)
```

> ⚠️ **Nunca** publique com `appsettings.Development.json`. Use variáveis de ambiente em produção.

---

## 3. Variáveis de Ambiente (Produção)

Configure no servidor ou plataforma de hospedagem:

```bash
# Connection String
ConnectionStrings__MonetisConnection="Server=prod-db.database.windows.net;Database=MonetisProd;User Id=admin;Password=...;Encrypt=True;"

# JWT
Jwt__Key="chave-secreta-diferente-da-dev-com-32-caracteres!"
Jwt__Issuer="Monetis"
Jwt__Audience="MonetisUsers"
```

> ⚠️ No .NET, `:` vira `__` (double underscore) em variáveis de ambiente.

---

## 4. Docker (Opcional)

### Dockerfile

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish src/Monetis.API -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 5074
ENTRYPOINT ["dotnet", "Monetis.API.dll"]
```

### Build e Run

```bash
docker build -t monetis-api .
docker run -p 5074:5074 \
  -e ConnectionStrings__MonetisConnection="Server=host.docker.internal;..." \
  -e Jwt__Key="sua-chave-segura" \
  -e Jwt__Issuer="Monetis" \
  -e Jwt__Audience="MonetisUsers" \
  monetis-api
```

---

## 5. Aplicar Migrations em Produção

```bash
# Produção — aplique migrations no startup
dotnet ef database update \
  --project src/Monetis.Infrastructure \
  --startup-project src/Monetis.API
```

> 💡 **Recomendação:** Em CI/CD, execute `dotnet ef database update` antes de deployar a API.

---

## 6. Verificação pós-deploy

A API **não expõe** um endpoint `/health`. Para verificar se o serviço está respondendo, use um endpoint protegido — um retorno `401 Unauthorized` indica que a API está no ar:

```bash
curl -I https://sua-api.com/api/accounts
# Esperado: 401 Unauthorized (API respondendo e exigindo token)
```

---

## 7. CI/CD (GitHub Actions — Exemplo)

```yaml
name: Deploy

on:
  push:
    branches: [main]

jobs:
  deploy:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4

      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '10.0.x'

      - run: dotnet restore
      - run: dotnet build -c Release --no-restore
      - run: dotnet publish src/Monetis.API -c Release -o ./publish

      - name: Deploy
        run: |
          # Seu comando de deploy aqui
          # Ex: scp -r ./publish user@server:/var/monetis/
          # Ou: az webapp deployment ...
```

---

## 8. Checklist de Produção

- [ ] Variáveis de ambiente configuradas (não hardcoded)
- [ ] `Jwt:Key` é diferente da chave de desenvolvimento
- [ ] `AllowedHosts` restrito a domínios legítimos
- [ ] HTTPS habilitado
- [ ] SQL Server com firewall configurado
- [ ] Migrations aplicadas
- [ ] API respondendo (401 em `/api/accounts` sem token)
- [ ] Logs configurados (não console em produção)
- [ ] Rate limiting ativo
- [ ] Swagger/Scalar desabilitado em produção

---

## 9. Rollback

Se a migration causar problemas:

```bash
# Reverter última migration
dotnet ef migrations remove \
  --project src/Monetis.Infrastructure \
  --startup-project src/Monetis.API
```

> ⚠️ **Nunca** delete uma migration que já foi aplicada em produção sem backup do banco.

---

> **Próximo:** [Diagrama ERD](../05-arquitetura/04-diagrama-erd.md)
