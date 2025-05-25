# Gestão de Resíduos API - Postman Collection

Esta pasta contém a collection e environment do Postman para testar a API de Gestão de Resíduos.

## Como usar:

### 1. Importar no Postman
1. Abra o Postman
2. Clique em "Import"
3. Selecione os arquivos:
   - `GestaoResiduos-API.postman_collection.json`
   - `GestaoResiduos-Environment.postman_environment.json`

### 2. Configurar o Environment
1. Selecione o environment "Gestão de Resíduos - Local"
2. Verifique se a variável `baseUrl` está apontando para `https://localhost:5031`
3. Se a API estiver rodando em outra porta, atualize a variável

### 3. Executar a API
```bash
cd c:\Github\GestaoResiduos.API
dotnet run
```

### 4. Sequência de teste recomendada:

#### Passo 1: Criar dados base
1. **Pontos de Coleta > Criar Ponto de Coleta** - Execute 2-3 vezes com dados diferentes
2. **Resíduos > Criar Resíduo** - Execute 3-4 vezes com categorias diferentes

#### Passo 2: Testar funcionalidades
1. **Coletas Agendadas > Agendar Coleta** - Verifique se o ponto aceita a categoria
2. **Coletas Agendadas > Listar Coletas** - Visualize as coletas criadas
3. **Notificações > Listar Notificações** - Verifique notificações automáticas
4. **Coletas Agendadas > Completar Coleta** - Complete uma coleta

#### Passo 3: Testar alertas
1. **Resíduos > Verificar Alertas** - Execute para gerar alertas
2. **Notificações > Contar Não Lidas** - Verifique quantas notificações não lidas

## Exemplos de dados para teste:

### Pontos de Coleta:
```json
{
  "name": "Ecoponto Norte",
  "location": "Avenida Brasil, 456 - Zona Norte",
  "latitude": -23.5200,
  "longitude": -46.6100,
  "responsiblePerson": "Maria Santos",
  "contact": "(11) 88888-8888",
  "acceptedCategories": "Orgânico,Vidro"
}
```

### Resíduos:
```json
{
  "name": "Papel de Escritório",
  "description": "Papéis usados do escritório",
  "category": "Papel",
  "currentQuantity": 200,
  "alertThreshold": 150
}
```

## Troubleshooting:

- **SSL Error**: Desabilite SSL verification no Postman (Settings > General > SSL certificate verification > OFF)
- **Connection Error**: Verifique se a API está rodando e se a porta está correta
- **404 Error**: Verifique se o endpoint está correto na URL
