# Changelog

Todas as mudanças notáveis neste projeto serão documentadas neste arquivo.

O formato é baseado em [Keep a Changelog](https://keepachangelog.com/pt-BR/1.0.0/),
e este projeto adere ao [Semantic Versioning](https://semver.org/lang/pt-BR/).

---

## [Unreleased]

### Added
- Documentação completa do projeto (docs/)
- Guia de consumo da API para front-end (API-CONSUMPTION-GUIDE.md)
- Análise de melhorias com 54 itens (ANALISE-MELHORIAS-PROJETO.md)
- Plano de correção de 8 bugs (BUGS-FIX-PLAN.md)
- Auditoria de 17 vulnerabilidades de segurança (SECURITY-VULNERABILITIES.md)
- Guia de setup local (docs/08-setup/01-guia-setup-local.md)
- Guia de contribuição (docs/08-setup/02-guia-contribuicao.md)
- CHANGELOG.md

### Fixed
- README.md: corrigido erro na descrição do README original (TPC está correto)

---

## [1.0.0] - 2026-05-07

### Added
- Estrutura Clean Architecture (4 camadas)
- Autenticação JWT com multi-tenancy por query filters
- CRUD completo: Users, Accounts, Cards, Categories
- Despesas simples e parceladas (2-24x no cartão de crédito)
- Receitas com depósito automático na conta
- Transferências entre contas com cancelamento no mesmo dia
- Assinaturas recorrentes com geração automática de despesas
- Background service para processamento de despesas vencidas
- Rate limiting nativo do ASP.NET Core (100 req/min)
- Exception middleware global com respostas padronizadas
- UserContext middleware para extração de userId do JWT
- Resource Guard para validação de ownership
- Unit of Work com atribuição automática de UserId
- FluentValidation para todos os DTOs de entrada
- Swagger + Scalar para documentação da API
- Seed data com 3 categorias do sistema (Alimentação, Transporte, Salário)
- Entity Framework Core com SQL Server
- ~60 exceções de domínio tipadas
- ~18 validators FluentValidation
