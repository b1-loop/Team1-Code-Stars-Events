USE master;
GO

-- Tvinga bort alla anslutningar och radera databasen
IF EXISTS (SELECT * FROM sys.databases WHERE name = 'EventifyDB')
BEGIN
    ALTER DATABASE EventifyDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE EventifyDB;
    PRINT 'Databasen EventifyDB har raderats helt.';
END
ELSE
BEGIN
    PRINT 'Databasen EventifyDB hittades inte.';
END