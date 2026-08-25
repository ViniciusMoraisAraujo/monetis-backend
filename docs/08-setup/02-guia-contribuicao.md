# 🤝 Guia de Contribuição

> **Propósito:** Padronizar como contribuir com código, docs e fixes no Monetis

---

## 1. Fluxo de Trabalho

```
main ──────────────────────────────────── produção
  │
  └── feature/nome-da-feature ──────── sua feature
        │
        └── commit(s) ── PR ── merge ── delete branch
```

### Passo a passo

```bash
# 1. Crie uma branch a partir de main
git checkout main
git pull
git checkout -b feature/dashboard-financeiro

# 2. Implemente a feature
# ... código ...

# 3. Commit (ver convenções abaixo)
git add .
git commit -m "feat(dashboard): add monthly summary endpoint"

# 4. Push e crie PR
git push -u origin feature/dashboard-financeiro
# Abrir PR no GitHub

# 5. Após review e merge, delete a branch
git checkout main
git pull
git branch -d feature/dashboard-financeiro
```

---

## 2. Convenção de Branches

| Tipo | Formato | Exemplo |
|------|---------|---------|
| Feature | `feature/<descrição>` | `feature/dashboard-financeiro` |
| Bug fix | `fix/<descrição>` | `fix/change-password-bug` |
| Docs | `docs/<descrição>` | `docs/api-reference` |
| Refactor | `refactor/<descrição>` | `refactor/extract-mapping` |
| Hotfix | `hotfix/<descrição>` | `hotfix/security-patch` |

---

## 3. Convenção de Commits

Formato: `<tipo>(<escopo>): <descrição>`

### Tipos

| Tipo | Quando usar | Exemplo |
|------|------------|---------|
| `feat` | Nova funcionalidade | `feat(expenses): add installment payment` |
| `fix` | Correção de bug | `fix(auth): change password lookup by email` |
| `docs` | Apenas documentação | `docs: add setup guide` |
| `refactor` | Refatoração sem mudar comportamento | `refactor: extract MapToResponse` |
| `test` | Adicionar/corrigir testes | `test(account): add deposit edge cases` |
| `chore` | Config, CI, dependências | `chore: update EF Core to 10.0.5` |
| `style` | Formatação (sem lógica) | `style: fix indentation in controllers` |
| `perf` | Melhoria de performance | `perf(expenses): add index on DueDate` |

### Escopos

| Escopo | Camada |
|--------|--------|
| `auth` | Autenticação/Autorização |
| `accounts` | Contas |
| `cards` | Cartões |
| `categories` | Categorias |
| `expenses` | Despesas |
| `incomes` | Receitas |
| `transfers` | Transferências |
| `subscriptions` | Assinaturas |
| `infra` | Infraestrutura/EF Core |

### Exemplos

```
feat(expenses): add overdue expense processing endpoint
fix(auth): use GetByIdAsync instead of GetUserByEmailAsync
docs: add ERD diagram for database schema
refactor: remove duplicate UserContext registration
test: add TransferService unit tests
```

---

## 4. Como Adicionar uma Nova Feature

### Exemplo: Adicionar entidade `Budget` (Orçamento)

#### Passo 1: Domínio
```
src/Monetis.Domain/
├── Entities/
│   └── Budget.cs              ← nova entidade
├── Enums/
│   └── BudgetStatus.cs        ← se precisar
└── Exceptions/
    └── BudgetExceptions.cs    ← exceções específicas
```

#### Passo 2: Application
```
src/Monetis.Application/
├── Abstractions/
│   ├── Persistence/
│   │   └── IBudgetRepository.cs
│   └── Services/
│       └── IBudgetService.cs
├── DTOs/
│   └── BudgetDtos.cs
├── Services/
│   └── BudgetService.cs
└── Validators/
    └── BudgetValidator.cs
```

#### Passo 3: Infrastructure
```
src/Monetis.Infrastructure/
├── Persistence/
│   ├── Configurations/
│   │   └── BudgetConfiguration.cs
│   └── Repositories/
│       └── BudgetRepository.cs
└── DependencyInjection.cs     ← registrar IBudgetRepository
```

#### Passo 4: API
```
src/Monetis.API/
├── Controllers/
│   └── BudgetsController.cs
└── Program.cs                 ← se precisar de algo específico
```

#### Passo 5: Migration
```bash
dotnet ef migrations add AddBudgetEntity \
  --project src/Monetis.Infrastructure \
  --startup-project src/Monetis.API
```

#### Passo 6: Tests
```
tests/
├── Monetis.Domain.Tests/
│   └── Entities/
│       └── BudgetTests.cs
└── Monetis.Application.Tests/
    └── Services/
        └── BudgetServiceTests.cs
```

---

## 5. Regras de Negócio Importantes

### Multi-tenancy
- Toda entidade que herda `UserOwnedEntity` tem filtro automático por `UserId`
- Nunca use `IgnoreQueryFilters` sem documentar o motivo
- O `UserResourceGuard` valida ownership — use nos services

### Validação (dupla camada)
- **FluentValidation** (Application) — valida DTOs de entrada
- **Domínio** (Entities) — valida invariantes de negócio
- Se o domínio limita a 25 chars, o validator não deve permitir 100

### Exceções
- Use exceções de domínio específicas (`ExpenseAlreadyPaidException`)
- NÃO use `throw new Exception("msg")` — use `KeyNotFoundException` ou `DomainException`
- O `ExceptionMiddleware` mapeia automaticamente:
  - `DomainException` → 400
  - `KeyNotFoundException` → 404
  - `UnauthorizedAccessException` → 401

---

## 6. Review Checklist

Antes de abrir PR:

- [ ] Código compila sem warnings
- [ ] Validações no domínio e no validator estão consistentes
- [ ] `UserResourceGuard` é usado para validar ownership
- [ ] `UnitOfWork.CommitAsync()` é chamado para persistir
- [ ] DTOs de request têm validator FluentValidation
- [ ] Exceções são específicas (não genéricas)
- [ ] Testes unitários cobrem os novos cenários
- [ ] Documentação foi atualizada (se aplicável)

---

> **Próximo:** [CHANGELOG](../../CHANGELOG.md)
