# Product Management API - Estudo

> **Nota:** Este é um projeto puramente educacional. Não sou um programador profissional (ainda), sou um estudante explorando o ecossistema .NET. O código aqui reflete meu aprendizado atual e pode conter erros ou abordagens experimentais.

## Sobre o Projeto
Estou construindo esta API para estudar conceitos de desenvolvimento Backend com C#. O objetivo não é criar um produto real, mas sim entender como as peças se encaixam em uma arquitetura mais estruturada.

O foco atual é sair do básico (apenas Controllers) e aplicar padrões de organização e testes.

## O que estou estudando/aplicando aqui:
* **.NET 10 Web API**
* **Arquitetura em Camadas** (tentando separar Domain, Application, Infra)
* **Entity Framework Core** com SQLite
* **Autenticação JWT** e controle de permissões (Admin vs Comum)
* **Paginação de dados** (Skip/Take)
* **Testes de Integração** (usando banco em memória)
* **Validações** (FluentValidation)

## Como rodar (na minha máquina funcionou assim)
1.  Clone o repositório.
2.  Certifique-se de ter o SDK do .NET 8 instalado.
3.  Entre na pasta `src/Api`.
4.  Rode `dotnet run`.
5.  O banco de dados (SQLite) deve ser criado automaticamente na primeira execução.
6.  Acesse `https://localhost:7153/swagger` (ou a porta que aparecer no seu terminal) para ver os endpoints.

---
*Repositório mantido por [Gustavo Meneguelli](https://github.com/gustavo-meneguelli) para fins de estudo.*