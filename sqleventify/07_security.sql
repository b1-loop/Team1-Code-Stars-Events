-----------------------------------------------------------
-- 1. SERVER-NIVÅ: SKAPA TVÅ LOGINS (Dörrvakter)
-----------------------------------------------------------
USE [master];
GO

-- 1. Skapa Login för den vanliga applikationen (Begränsad)
IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = 'EventifyAppLogin')
BEGIN
    CREATE LOGIN EventifyAppLogin WITH PASSWORD = 'DittLösenord123!', 
    DEFAULT_DATABASE = [EventifyDB], CHECK_POLICY = OFF;
END

-- 2. Skapa Login för administratören (Full behörighet)
IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = 'EventifyAdminLogin')
BEGIN
    CREATE LOGIN EventifyAdminLogin WITH PASSWORD = 'AdminPassword123!', 
    DEFAULT_DATABASE = [EventifyDB], CHECK_POLICY = OFF;
END
GO

-- Ge båda rättighet att ansluta till SQL Server
GRANT CONNECT SQL TO EventifyAppLogin;
GRANT CONNECT SQL TO EventifyAdminLogin;
GO

-----------------------------------------------------------
-- 2. DATABAS-NIVÅ: SKAPA ROLLER OCH RÄTTIGHETER
-----------------------------------------------------------
USE [EventifyDB];
GO

-- Skapa roller om de inte finns
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'DatabaseAdminRole' AND type = 'R')
    CREATE ROLE DatabaseAdminRole;

IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'AppRole' AND type = 'R')
    CREATE ROLE AppRole;
GO

-- A. ADMIN: Full kontroll över allt i dbo-schemat
GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA::dbo TO DatabaseAdminRole;

-- B. APPUSER: Begränsad åtkomst via vyer och specifika tabeller
GRANT SELECT ON OBJECT::dbo.vw_UpcomingEvents TO AppRole;
GRANT SELECT ON OBJECT::dbo.vw_DetailedTicketReport TO AppRole;
GRANT SELECT ON OBJECT::dbo.vw_EventSalesSummary TO AppRole;

GRANT SELECT ON OBJECT::dbo.Events TO AppRole;
GRANT INSERT, SELECT, UPDATE ON OBJECT::dbo.Customers TO AppRole;
GRANT INSERT, SELECT, DELETE ON OBJECT::dbo.Tickets TO AppRole;

-- C. INTEGRITETSKONTROLL: Explicit NEKA åtkomst till känsliga tabeller
DENY SELECT ON OBJECT::dbo.Venues TO AppRole;
DENY SELECT ON OBJECT::dbo.Organizers TO AppRole;
GO

-----------------------------------------------------------
-- 3. DATABAS-NIVÅ: KOPPLA USERS TILL LOGINS OCH ROLLER
-----------------------------------------------------------

-- Vi rensar gamla users för att undvika mappningsfel vid nykörning
IF EXISTS (SELECT * FROM sys.database_principals WHERE name = 'EventifyAppUser') DROP USER EventifyAppUser;
IF EXISTS (SELECT * FROM sys.database_principals WHERE name = 'EventifyAdminUser') DROP USER EventifyAdminUser;
GO

-- Skapa användare kopplade till loginen
CREATE USER EventifyAppUser FOR LOGIN EventifyAppLogin;
CREATE USER EventifyAdminUser FOR LOGIN EventifyAdminLogin;
GO

-- Lägg till användarna i sina respektive roller
ALTER ROLE AppRole ADD MEMBER EventifyAppUser;
ALTER ROLE DatabaseAdminRole ADD MEMBER EventifyAdminUser;
GO

PRINT '--- SÄKERHETSINSTÄLLNINGAR MED DUBBLA ROLLER KLARA! ---';