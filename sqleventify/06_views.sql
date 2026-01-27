USE EventifyDB;
GO
-- 1. PUBLIC VIEW: vw_UpcomingEvents
-- Syfte: Visar information om kommande events för en besökare.
-- Döljer interna ID-nummer, skapandedatum och arrangörers kontaktinfo.
-----------------------------------------------------------
CREATE VIEW vw_UpcomingEvents AS
SELECT 
    E.EventId,
    E.Title AS [Event],
    E.Description,
    E.StartDate,
    V.Name AS [Venue],
    V.City,
    O.Name AS [Organizer]
FROM Events E
JOIN Venues V ON E.VenueId = V.VenueId
JOIN Organizers O ON E.OrganizerId = O.OrganizerId;
GO

-----------------------------------------------------------
-- 2. REPORT VIEW: vw_EventStatistics
-- Syfte: En vy som Console Appen kan använda för att visa försäljning.
-- Kombinerar data från Events och Tickets för en snabb överblick.
-----------------------------------------------------------
CREATE VIEW vw_EventStatistics AS
SELECT 
    E.EventId,
    E.Title AS [EventTitle],
    COUNT(T.TicketId) AS [TicketsSold],
    SUM(T.Price) AS [TotalRevenue],
    MAX(V.Capacity) AS [MaxCapacity]
FROM Events E
LEFT JOIN Tickets T ON E.EventId = T.EventId
JOIN Venues V ON E.VenueId = V.VenueId
GROUP BY E.EventId, E.Title;
GO

-- Exempel på hur man testar vyerna:
 SELECT * FROM vw_UpcomingEvents;
 SELECT * FROM vw_EventStatistics WHERE TicketsSold > 0;