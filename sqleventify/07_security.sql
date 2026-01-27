-----------------------------------------------------------
-- STEG 1: SERVER-NIVÅ (Skapa dörrvakter/Logins i master)
-----------------------------------------------------------
USE [master];
GO

-- 1. Skapa Login för den vanliga applikationen (Begränsad)
IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = 'EventifyAppLogin')
BEGIN
    CREATE LOGIN EventifyAppLogin WITH PASSWORD = 'DittLösenord123!', 
    DEFAULT_DATABASE = [EventifyDB], 
    CHECK_POLICY = OFF;
END
GO

-- 2. Skapa Login för administratören (Full behörighet)
IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = 'EventifyAdminLogin')
BEGIN
    CREATE LOGIN EventifyAdminLogin WITH PASSWORD = 'AdminPassword123!', 
    DEFAULT_DATABASE = [EventifyDB], 
    CHECK_POLICY = OFF;
END
GO

-- Ge båda rättighet att överhuvudtaget ansluta till servern
GRANT CONNECT SQL TO EventifyAppLogin;
GRANT CONNECT SQL TO EventifyAdminLogin;
GO

-----------------------------------------------------------
-- STEG 2: DATABAS-NIVÅ (Roller, Users & Rättigheter)
-----------------------------------------------------------
USE [EventifyDB];
GO

-- 1. Skapa databasroller
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'DatabaseAdminRole' AND type = 'R')
    CREATE ROLE DatabaseAdminRole;

IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'AppRole' AND type = 'R')
    CREATE ROLE AppRole;
GO

-- 2. Tilldela rättigheter till DatabaseAdminRole (Full CRUD)
-- Denna roll får göra allt i dbo-schemat
GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA::dbo TO DatabaseAdminRole;

-- 3. Tilldela rättigheter till AppRole (Begränsad/Säker)
-- Ge läsrättigheter till alla vyer
GRANT SELECT ON OBJECT::dbo.vw_UpcomingEvents TO AppRole;
GRANT SELECT ON OBJECT::dbo.vw_DetailedTicketReport TO AppRole;
GRANT SELECT ON OBJECT::dbo.vw_EventSalesSummary TO AppRole;

-- Tillåt hantering av kunder, biljetter och läsning av events
GRANT INSERT, SELECT, UPDATE ON OBJECT::dbo.Customers TO AppRole;
GRANT INSERT, SELECT ON OBJECT::dbo.Tickets TO AppRole;
GRANT SELECT ON OBJECT::dbo.Events TO AppRole;

-- 4. INTEGRITETSKONTROLL: Explicit NEKA åtkomst
-- AppRole får absolut inte läsa direkt från dessa tabeller
DENY SELECT ON Organizers TO AppRole;
DENY SELECT ON Venues TO AppRole;
GO

-- 5. Skapa Users och koppla dem till Logins och Roller
-- Vi rensar gamla users först för att undvika mappningsfel
IF EXISTS (SELECT * FROM sys.database_principals WHERE name = 'EventifyAppUser') DROP USER EventifyAppUser;
IF EXISTS (SELECT * FROM sys.database_principals WHERE name = 'EventifyAdminUser') DROP USER EventifyAdminUser;
GO

CREATE USER EventifyAppUser FOR LOGIN EventifyAppLogin;
CREATE USER EventifyAdminUser FOR LOGIN EventifyAdminLogin;
GO

-- Lägg till användarna i sina respektive roller
ALTER ROLE AppRole ADD MEMBER EventifyAppUser;
ALTER ROLE DatabaseAdminRole ADD MEMBER EventifyAdminUser;
GO

PRINT 'Säkerhetskonfigurationen är nu helt slutförd!';