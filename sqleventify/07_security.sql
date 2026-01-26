USE EventifyDB;
GO
-----------------------------------------------------------
-- 1. SKAPA ROLL OCH ANVÄNDARE
-----------------------------------------------------------

-- Skapa en databasroll
CREATE ROLE AppRole;

-- Skapa en användare (utan inloggning för detta exempel, 
-- i skarpt läge kopplas denna till en SQL Login)
CREATE USER EventifyAppUser WITHOUT LOGIN;

-- Lägg till användaren i rollen
ALTER ROLE AppRole ADD MEMBER EventifyAppUser;


-----------------------------------------------------------
-- 2. TILLDELA RÄTTIGHETER (GRANT)
-----------------------------------------------------------

-- Ge rollen rättighet att läsa från våra vyer
GRANT SELECT ON OBJECT::dbo.vw_UpcomingEvents TO AppRole;
GRANT SELECT ON OBJECT::dbo.vw_EventStatistics TO AppRole;

-- Om ni vill att appen ska kunna lägga till kunder/biljetter
-- behöver vi även ge INSERT-rättigheter på specifika tabeller:
GRANT INSERT, SELECT, UPDATE ON OBJECT::dbo.Customers TO AppRole;
GRANT INSERT, SELECT ON OBJECT::dbo.Tickets TO AppRole;


-----------------------------------------------------------
-- 3. BEGRÄNSA ÅTKOMST (DENY/RESTRICT)
-----------------------------------------------------------

-- Vi ger INTE rollen SELECT-rättigheter på tabellerna Venues eller Organizers.
-- Detta tvingar appen att använda vw_UpcomingEvents för att se den datan.

-- För att vara extra tydliga i inlämningen kan man explicit neka 
-- åtkomst till känsliga tabeller om man vill:
DENY SELECT ON Organizers TO AppRole;