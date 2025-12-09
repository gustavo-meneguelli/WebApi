# Darklyn Tech Store API

API REST para e-commerce de produtos tecnológicos, desenvolvida com .NET 10 e PostgreSQL.

**Demo:** https://darklyn-api.onrender.com/swagger

---

## 🎯 Sobre

Projeto pessoal focado em aplicar boas práticas de desenvolvimento:
- Clean Architecture
- Autenticação JWT
- Validações centralizadas
- Testes automatizados

---

## 🛠️ Tech Stack

- **.NET 10** - Framework
- **PostgreSQL** - Banco de dados
- **Entity Framework Core** - ORM
- **FluentValidation** - Validações
- **AutoMapper** - Mapeamento de objetos
- **JWT** - Autenticação
- **Swagger** - Documentação
- **xUnit** - Testes

---

## 📁 Estrutura do Projeto

```
src/
├── Domain/          # Entidades e regras de negócio
├── Application/     # Casos de uso e interfaces
├── Infrastructure/  # Implementações (banco, repos)
└── Api/            # Controllers e configuração
```

Clean Architecture: dependências apontam sempre para o Domain (núcleo da aplicação).

---

## ✨ Funcionalidades

**Produtos**
- Listar com paginação
- Criar, atualizar (parcial), deletar
- Busca por ID
- Soft delete (recuperável)

**Categorias**
- CRUD completo com paginação
- Proteção: não permite deletar se houver produtos vinculados

**Autenticação**
- Registro de usuários
- Login com JWT
- Autorização por roles (Admin/Common)

---

## 🔧 Decisões Técnicas

**Soft Delete**  
Registros não são removidos do banco, apenas marcados como deletados. Permite recuperação e auditoria.

**Paginação**  
Implementada em todas as listagens para escalabilidade e performance.

**Validações Centralizadas**  
FluentValidation com validações assíncronas (verificação de unicidade, existência de relacionamentos).

**Update Parcial (PATCH)**  
Campos vazios/nulos são ignorados, permite atualizar apenas o necessário.

---

## 🚀 Como Rodar

**Pré-requisitos:**
- .NET 10 SDK
- PostgreSQL (ou Docker)

**Setup:**
```bash
# Clone
git clone https://github.com/gustavo-meneguelli/darklyn-tech-store-api.git

# Configure connection string
# Edite appsettings.json ou use variável DATABASE_URL

# Aplique migrations
dotnet ef database update

# Execute
dotnet run --project src/Api

# Acesse Swagger
http://localhost:5000/swagger
```

**Credenciais padrão:** admin / (senha em appsettings)

---

## 🧪 Testes

```bash
dotnet test
# 17 testes (unitários + integração)
```

Cobertura: Validators, Services, Endpoints

---

## 🔐 Variáveis de Ambiente

Para produção:
```
DATABASE_URL=postgresql://...
JwtSettings__SecretKey=sua-chave-256-bits
AdminSettings__Password=senha-admin
```

---

## 📚 Conceitos Aplicados

- Clean Architecture (isolamento de camadas)
- Repository Pattern (abstração de dados)
- Unit of Work (transações)
- Result Pattern (tratamento de erros)
- Dependency Injection (inversão de controle)
- JWT Bearer Authentication
- Soft Delete Pattern
- Eager Loading (otimização N+1)

---

## 🚧 Em Desenvolvimento

Próximas features planejadas:
- [ ] Cache com Redis
- [ ] Rate limiting
- [ ] Upload de imagens de produtos
- [ ] Filtros avançados (preço, categoria)
- [ ] Webhooks para integrações

---

## 📖 Documentação

Swagger/OpenAPI disponível em `/swagger` com:
- Descrição de todos endpoints
- Modelos de request/response
- Códigos de status HTTP
- Testes interativos

---

## 📄 Licença

MIT License - Livre para uso educacional e referência.

---

**Desenvolvido por Gustavo Meneguelli**  
[LinkedIn](#) | [Portfolio](#)