/*
  SKRIPT: CreateDatabase.sql
  BESKRIVNING: Skapar databasen EventifyDB om den inte finns 
*/

-- Kontrollera om databasen existerar innan vi försöker skapa den
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'EventifyDB')
BEGIN
    CREATE DATABASE EventifyDB;
    PRINT 'Databasen EventifyDB har skapats framgångsrikt.';
END
ELSE
BEGIN
    PRINT 'Databasen EventifyDB finns redan.';
END
GO