# Docker Setup - Gestão de Resíduos API

## Pré-requisitos

- Docker Desktop instalado
- Docker Compose disponível

## Como executar

### 1. Build e execução completa:
```bash
# Na pasta raiz do projeto
docker-compose up --build

# Ou em background
docker-compose up --build -d
```

### 2. Primeira execução (Migrations):
```bash
# Aguardar containers subirem, depois executar migrations
docker-compose exec api dotnet ef database update

# Ou rodar migrations localmente apontando para container
dotnet ef database update --connection "Server=localhost,1433;Database=GestaoResiduosDBteste4;User Id=sa;Password=MinhaSenh@123;TrustServerCertificate=true;"
```

### 3. Parar containers:
```bash
docker-compose down

# Para remover volumes também
docker-compose down -v
```

## Acessos

- **API**: http://localhost:5031
- **Swagger**: http://localhost:5031/swagger
- **SQL Server**: localhost:1433
  - User: sa
  - Password: MinhaSenh@123

## Comandos úteis

```bash
# Ver logs da API
docker-compose logs api

# Ver logs do banco
docker-compose logs sqlserver

# Executar comandos na API
docker-compose exec api dotnet --version

# Acessar bash do container da API
docker-compose exec api bash

# Rebuild apenas um serviço
docker-compose build api
docker-compose up api
```

## Volumes

- `sqlserver_data`: Persiste dados do SQL Server

## Rede

- `gestao-residuos-network`: Rede interna para comunicação entre containers

## Troubleshooting

### Erro de conexão com banco:
1. Verificar se container do SQL Server está healthy: `docker-compose ps`
2. Aguardar inicialização completa (pode levar alguns minutos)
3. Verificar logs: `docker-compose logs sqlserver`

### Erro de migrations:
1. Executar migrations manualmente após containers subirem
2. Verificar connection string no appsettings.Production.json

### Porta já em uso:
1. Parar containers: `docker-compose down`
2. Verificar se algum processo está usando as portas 5031 ou sua porta
3. Alterar portas no docker-compose.yml se necessário
