# 💰 Monetis — API

> API back-end da plataforma de finanças pessoais Monetis. Controle de receitas, despesas, categorias e relatórios mensais.

![Status](https://img.shields.io/badge/status-em%20desenvolvimento-yellow)
![License](https://img.shields.io/badge/license-MIT-blue)

---

## 📋 Sobre o Projeto

O **Monetis** é uma plataforma full stack para gerenciamento de finanças pessoais. Este repositório contém o **back-end** da aplicação, responsável por toda a lógica de negócio, persistência de dados e exposição da API.

Com ele, será possível registrar lançamentos de receitas e despesas, organizar por categorias, visualizar um dashboard com gráficos e acompanhar relatórios mensais detalhados.

> 🚧 Projeto em desenvolvimento ativo.

---

## ✨ Funcionalidades

- 📥 Lançamento de receitas e despesas
- 🏷️ Categorização de transações
- 📊 Dashboard com gráficos e resumos
- 📅 Relatório mensal detalhado
- 🔐 Autenticação de usuários

---

## 🛠️ Tecnologias

| Tecnologia | Descrição |
|---|---|
| ![.NET](https://img.shields.io/badge/.NET-512BD4?style=flat&logo=dotnet&logoColor=white) | Framework back-end |
| ![C#](https://img.shields.io/badge/C%23-239120?style=flat&logo=csharp&logoColor=white) | Linguagem principal |
| ![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?style=flat&logo=microsoftsqlserver&logoColor=white) | Banco de dados relacional |

---

## 🚀 Como executar

### Pré-requisitos

- [.NET SDK](https://dotnet.microsoft.com/download) 8+
- [SQL Server](https://www.microsoft.com/pt-br/sql-server/sql-server-downloads)

### Instalação

```bash
# Clone o repositório
git clone https://github.com/ViniciusMoraisAraujo/monetis.git

# Entre na pasta do projeto
cd monetis
```

### Variáveis de ambiente

Crie um arquivo `appsettings.Development.json` com:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=monetis;User Id=sa;Password=sua_senha;"
  },
  "JwtSettings": {
    "Secret": "sua_chave_secreta"
  }
}
```

### Rodando a aplicação

```bash
# Restaurar dependências
dotnet restore

# Aplicar migrations
dotnet ef database update

# Iniciar o servidor
dotnet run
```

A documentação da API estará disponível em `https://localhost:5001/swagger` após iniciar o servidor.

---

## 📡 Endpoints da API

> Documentação completa disponível via Swagger após iniciar o servidor.
> *(Rotas em desenvolvimento — esta seção será atualizada em breve.)*

---

## 🗺️ Roadmap

- [x] Estrutura inicial do back-end
- [ ] CRUD de transações
- [ ] CRUD de categorias
- [ ] Autenticação JWT
- [ ] Relatório mensal
- [ ] Deploy

---

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.

---

<p align="center">Feito com ❤️ por <a href="https://github.com/ViniciusMoraisAraujo">ViniciusMoraisAraujo</a></p>