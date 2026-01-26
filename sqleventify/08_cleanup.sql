USE EventifyDB;
GO 

-----------------------------------------------------------
-- 1. TA BORT VYER
-----------------------------------------------------------
DROP VIEW IF EXISTS vw_EventStatistics;
DROP VIEW IF EXISTS vw_UpcomingEvents;

-----------------------------------------------------------
-- 2. TA BORT TABELLER (VIKTIG ORDNING!)
-----------------------------------------------------------
-- Vi måste börja med "barnen" (de med Foreign Keys) 
-- och sluta med "föräldrarna".

DROP TABLE IF EXISTS Tickets;    -- Refererar till Events och Customers
DROP TABLE IF EXISTS Events;     -- Refererar till Venues och Organizers
DROP TABLE IF EXISTS Customers;
DROP TABLE IF EXISTS Organizers;
DROP TABLE IF EXISTS Venues;

-----------------------------------------------------------
-- 3. TA BORT SÄKERHETSOBJEKT
-----------------------------------------------------------
-- Ta bort medlemmen från rollen först (valfritt vid drop, men snyggt)
IF EXISTS (SELECT * FROM sys.database_principals WHERE name = 'EventifyAppUser')
BEGIN
    DROP USER EventifyAppUser;
END

IF EXISTS (SELECT * FROM sys.database_principals WHERE name = 'AppRole')
BEGIN
    DROP ROLE AppRole;
END

PRINT 'Databasen är nu rensad och redo för en ny körning!';