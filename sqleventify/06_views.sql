USE EventifyDB;
GO

-----------------------------------------------------------
-- VY 1: Kommande Events (Används i huvudmenyn)
-- Inkluderar EventId för att C#-modellen ska fungera.
-----------------------------------------------------------
CREATE OR ALTER VIEW vw_UpcomingEvents AS
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
JOIN Organizers O ON E.OrganizerId = O.OrganizerId
WHERE E.StartDate >= GETDATE();
GO

-----------------------------------------------------------
-- VY 2: Detaljerad Biljettrapport (Användarvy)
-- Visar köp utan att exponera rådata från tabellerna.
-----------------------------------------------------------
CREATE OR ALTER VIEW vw_DetailedTicketReport AS
SELECT 
    T.TicketId,
    C.FirstName + ' ' + C.LastName AS CustomerName,
    E.Title AS EventName,
    E.StartDate,
    V.Name AS VenueName,
    T.PurchaseDate
FROM Tickets T
JOIN Customers C ON T.CustomerId = C.CustomerId
JOIN Events E ON T.EventId = E.EventId
JOIN Venues V ON E.VenueId = V.VenueId;
GO

-----------------------------------------------------------
-- VY 3: Försäljningsstatistik (Rapportvy)
-- Uppfyller kravet på statistik och sammanställning.
-----------------------------------------------------------
CREATE OR ALTER VIEW vw_EventSalesSummary AS
SELECT 
    E.Title AS EventName,
    COUNT(T.TicketId) AS TicketsSold,
    V.Capacity AS TotalCapacity,
    (V.Capacity - COUNT(T.TicketId)) AS RemainingSeats
FROM Events E
LEFT JOIN Tickets T ON E.EventId = T.EventId
JOIN Venues V ON E.VenueId = V.VenueId
GROUP BY E.Title, V.Capacity;
GO