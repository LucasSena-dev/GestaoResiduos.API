# 🎓 Instruções para Avaliação - API Gestão de Resíduos

## ⚡ **Execução Rápida (2 comandos)**

```bash
# 1. Subir aplicação completa
docker-compose up --build

# 2. Executar migrations (em outro terminal)
docker-compose exec api dotnet ef database update
```

**Pronto!** API rodando em: http://localhost:8080/swagger

## 🔧 **Alternativa: API Local**

```bash
# 1. Executar API localmente
cd src/GestaoResiduos.API
dotnet run

# 2. Executar migrations
dotnet ef database update
```

**Pronto!** API rodando em: http://localhost:5031/swagger

## 🧪 **Executar Testes (1 comando)**

```bash
cd tests/GestaoResiduos.Tests
dotnet test --verbosity normal
```

**Resultado:** 32 testes passando

## 📋 **Testar Funcionalidades**

1. **Swagger**: http://localhost:5031/swagger
2. **Postman**: Importar `src/GestaoResiduos.API/Postman/*.json`
3. **Sequência sugerida**:
   - Criar Ponto de Coleta
   - Criar Resíduo  
   - Agendar Coleta
   - Completar Coleta
   - Verificar Notificações

## 📊 **Validar Implementações**

✅ **CRUD Completo** - Todos os endpoints funcionando  
✅ **Relacionamentos** - Foreign keys configuradas  
✅ **Validações** - ModelState e business rules  
✅ **Testes** - 32 testes cobrindo Services e Controllers  
✅ **Docker** - Aplicação containerizada  
✅ **Documentação** - Swagger + Postman + READMEs  

## 🚑 **Se houver problemas**

- **Docker não inicia**: Verificar se Docker Desktop está rodando
- **Migrations falham**: Aguardar SQL Server inicializar (30-60s)
- **Porta ocupada**: Alterar portas no docker-compose.yml
