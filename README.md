# 🗂️ API de Gestão de Resíduos

Sistema completo para gerenciamento de resíduos e pontos de coleta com Docker, testes unitários e documentação Postman.

## 🚀 **Tecnologias Utilizadas**

- **.NET 8** - Framework principal
- **Entity Framework Core** - ORM para banco de dados
- **SQL Server** - Banco de dados
- **Docker & Docker Compose** - Containerização
- **Swagger** - Documentação da API
- **xUnit, FluentAssertions, Moq** - Testes unitários
- **Postman** - Collection para testes

## 📁 **Estrutura do Projeto**

```
GestaoResiduos.API/
├── src/
│   └── GestaoResiduos.API/          # API principal
│       ├── Controllers/             # Endpoints HTTP
│       ├── Services/               # Lógica de negócio
│       ├── Models/                 # Entidades do banco
│       ├── ViewModels/             # DTOs para transferência
│       ├── Data/                   # DbContext
│       ├── Migrations/             # Migrations do EF
│       ├── Postman/                # Collection para testes
│       └── Dockerfile              # Container da API
├── tests/
│   └── GestaoResiduos.Tests/       # Testes unitários (32 testes)
│       ├── services/               # Testes dos Services
│       └── controllers/            # Testes dos Controllers
├── docker-compose.yml              # Orquestração dos containers
├── README.md                       # Este arquivo
└── Docker-README.md                # Instruções específicas do Docker
```

## ⚡ **Como Executar**

### **Opção 1: Docker Completo (Recomendado)**

```bash
# Clonar repositório
git clone <sua-url-repositorio>
cd GestaoResiduos.API

# Executar com Docker
docker-compose up --build

# Executar migrations (em outro terminal)
docker-compose exec api dotnet ef database update

# Acessar aplicação
# API: http://localhost:8080
# Swagger: http://localhost:8080/swagger
```

### **Opção 2: API Local + SQL Server Docker**

```bash
# Subir apenas banco de dados
docker-compose up sqlserver -d

# Executar API localmente
cd src/GestaoResiduos.API
dotnet run

# Executar migrations
dotnet ef database update

# Acessar aplicação
# API: http://localhost:5031
# Swagger: http://localhost:5031/swagger
```

### **Opção 3: Completamente Local**

```bash
# Executar API (usando SQL Server local)
cd src/GestaoResiduos.API
dotnet run

# Acessar aplicação
# API: http://localhost:5031
# Swagger: http://localhost:5031/swagger
```

## 🧪 **Executar Testes**

```bash
# Executar todos os testes (32 testes)
cd tests/GestaoResiduos.Tests
dotnet test --verbosity normal

# Resultado esperado: 32 testes passando
# - Services: 28 testes de lógica de negócio
# - Controllers: 4 testes de status HTTP
```

## 📋 **Testar com Postman**

1. **Importar Collection**: `src/GestaoResiduos.API/Postman/GestaoResiduos-API.postman_collection.json`
2. **Importar Environment**: `src/GestaoResiduos.API/Postman/GestaoResiduos-Environment.postman_environment.json`
3. **Atualizar baseUrl** no environment para a porta correta
4. **Seguir sequência de testes** no README do Postman

## 🎯 **Funcionalidades Implementadas**

### **Resíduos**
- ✅ CRUD completo
- ✅ Sistema de alertas automáticos
- ✅ Categorização de resíduos
- ✅ Controle de quantidades

### **Pontos de Coleta**
- ✅ CRUD completo
- ✅ Localização geográfica (latitude/longitude)
- ✅ Busca por proximidade
- ✅ Categorias aceitas por ponto

### **Coletas Agendadas**
- ✅ Agendamento de coletas
- ✅ Workflow completo (Pending → Completed)
- ✅ Validação de compatibilidade
- ✅ Atualização automática de quantidades

### **Notificações**
- ✅ Alertas automáticos de coleta
- ✅ Notificações de agendamento
- ✅ Notificações de conclusão
- ✅ Sistema de lidas/não lidas

### **Recursos Técnicos**
- ✅ Paginação em todas as listagens
- ✅ Relacionamentos configurados
- ✅ Validações de entrada
- ✅ Tratamento de erros
- ✅ Documentação Swagger
- ✅ Containerização Docker

## 🗄️ **Banco de Dados**

### **Tabelas Criadas**
- `Residues` - Cadastro de resíduos
- `CollectionPoints` - Pontos de coleta
- `ScheduledCollections` - Coletas agendadas
- `Notifications` - Sistema de notificações

### **Connection Strings**
- **Local**: `Server=localhost\\SQLEXPRESS03;Database=GestaoResiduosDbteste4;...`
- **Docker**: `Server=sqlserver,1433;Database=GestaoResiduosDB;User Id=sa;Password=MinhaSenh@123;...`

## 📚 **Documentação Adicional**

- **Docker**: Ver `Docker-README.md` para instruções específicas
- **Postman**: Ver `src/GestaoResiduos.API/Postman/README.md` para guia de testes
- **API**: Acessar `/swagger` para documentação interativa

## 🏗️ **Arquitetura**

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   Controllers   │───▶│    Services     │───▶│   Repository    │
│   (HTTP Layer)  │    │ (Business Logic)│    │  (Data Layer)   │
└─────────────────┘    └─────────────────┘    └─────────────────┘
         │                       │                       │
         ▼                       ▼                       ▼
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   ViewModels    │    │   Notifications │    │  SQL Server DB  │
│     (DTOs)      │    │   (Alerts)      │    │   (Entities)    │
└─────────────────┘    └─────────────────┘    └─────────────────┘
```

## 🎓 **Para Avaliação Acadêmica**

Este projeto demonstra:
- ✅ **Arquitetura em camadas** bem estruturada
- ✅ **Padrões de desenvolvimento** (Repository, Service, DTO)
- ✅ **Testes unitários** abrangentes (32 testes)
- ✅ **Containerização** com Docker
- ✅ **Documentação** completa
- ✅ **Boas práticas** de desenvolvimento
- ✅ **Tecnologias modernas** (.NET 8, EF Core)

## 👥 **Desenvolvido por**

Lucas Sena - Projeto de Gestão de Resíduos
