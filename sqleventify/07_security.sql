-----------------------------------------------------------
-- 1. SERVER-NIVÅ: SKAPA LOGIN (Dörrvakten)
-----------------------------------------------------------
USE [master];
GO

-- Skapa inloggningen på servern om den inte redan finns
IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = 'EventifyAppLogin')
BEGIN
    CREATE LOGIN EventifyAppLogin WITH PASSWORD = 'DittLösenord123!', 
    DEFAULT_DATABASE = [EventifyDB], 
    CHECK_EXPIRATION = OFF, 
    CHECK_POLICY = OFF;
    PRINT 'Login "EventifyAppLogin" skapat.';
END
GO

-----------------------------------------------------------
-- 2. DATABAS-NIVÅ: SKAPA ANVÄNDARE OCH ROLLER
-----------------------------------------------------------
USE EventifyDB;
GO

-- Skapa användaren i databasen och koppla den till loginet
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'EventifyAppUser')
BEGIN
    CREATE USER EventifyAppUser FOR LOGIN EventifyAppLogin;
    PRINT 'User "EventifyAppUser" skapat och kopplat till Login.';
END
GO

-- Skapa rollen AppRole
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'AppRole' AND type = 'R')
BEGIN
    CREATE ROLE AppRole;
    PRINT 'Rollen "AppRole" skapad.';
END
GO

-- Lägg till användaren i rollen
ALTER ROLE AppRole ADD MEMBER EventifyAppUser;
GO

-----------------------------------------------------------
-- 3. TILLDELA RÄTTIGHETER (GRANT)
-----------------------------------------------------------

-- Vy-rättigheter (Dessa används av MenuManager för listning och statistik)
GRANT SELECT ON OBJECT::dbo.vw_UpcomingEvents TO AppRole;
GRANT SELECT ON OBJECT::dbo.vw_EventStatistics TO AppRole;

-- Tabell-rättigheter
-- Vi ger SELECT på Events för att .ThenInclude(t => t.Event) ska fungera i C#
GRANT SELECT ON OBJECT::dbo.Events TO AppRole;

-- Rättigheter för att hantera kunder
GRANT INSERT, SELECT, UPDATE ON OBJECT::dbo.Customers TO AppRole;

-- Rättigheter för att hantera biljetter (inklusive radering)
GRANT INSERT, SELECT, DELETE ON OBJECT::dbo.Tickets TO AppRole;

PRINT 'Rättigheter (GRANT) tilldelade till AppRole.';
GO

-----------------------------------------------------------
-- 4. BEGRÄNSA ÅTKOMST (DENY)
-----------------------------------------------------------

-- Vi blockerar direktåtkomst till känsliga tabeller. 
-- Appen MÅSTE använda vyerna för att se denna information.
DENY SELECT ON OBJECT::dbo.Venues TO AppRole;
DENY SELECT ON OBJECT::dbo.Organizers TO AppRole;

PRINT 'Begränsningar (DENY) aktiverade för Venues och Organizers.';
GO

PRINT '--- SÄKERHETSINSTÄLLNINGAR KLARA! ---';