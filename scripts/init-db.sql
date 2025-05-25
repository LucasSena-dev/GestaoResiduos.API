-- Script de inicialização do banco de dados
USE master;
GO

-- Criar database se não existir
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'GestaoResiduosDB')
BEGIN
    CREATE DATABASE GestaoResiduosDB;
END
GO

USE GestaoResiduosDB;
GO

-- O Entity Framework criará as tabelas via migrations
PRINT 'Database GestaoResiduosDB inicializado com sucesso!';
