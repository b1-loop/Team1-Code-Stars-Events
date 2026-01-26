USE EventifyDB;
GO
-----------------------------------------------------------
-- 1-4: JOIN QUERIES (Krav: Minst 4 stycken)
-----------------------------------------------------------

-- 1. Lista alla biljetter med Kundnamn och Eventtitel (3-way JOIN)
SELECT T.TicketId, C.FirstName + ' ' + C.LastName AS CustomerName, E.Title AS EventTitle, T.Type
FROM Tickets T
JOIN Customers C ON T.CustomerId = C.CustomerId
JOIN Events E ON T.EventId = E.EventId;

-- 2. Lista alla Events med deras Lokaler och Arrangörer
SELECT E.Title, V.Name AS VenueName, V.City, O.Name AS OrganizerName
FROM Events E
JOIN Venues V ON E.VenueId = V.VenueId
JOIN Organizers O ON E.OrganizerId = O.OrganizerId;

-- 3. Hitta alla kunder som köpt VIP-biljetter
SELECT DISTINCT C.FirstName, C.LastName, C.Email
FROM Customers C
JOIN Tickets T ON C.CustomerId = T.CustomerId
WHERE T.Type = 'VIP';

-- 4. Visa detaljerad info för en specifik biljett (t.ex. för ett kvitto)
SELECT T.TicketId, E.Title, E.StartDate, V.Name AS Venue, T.Price
FROM Tickets T
JOIN Events E ON T.EventId = E.EventId
JOIN Venues V ON E.VenueId = V.VenueId;


-----------------------------------------------------------
-- 5-6: GROUP BY & AGGREGATES (Krav: Minst 2 stycken)
-----------------------------------------------------------

-- 5. RAPPORT 1: Top Entities - Kunder med flest köp
SELECT TOP 5 C.FirstName + ' ' + C.LastName AS Customer, COUNT(T.TicketId) AS TicketsBought
FROM Customers C
JOIN Tickets T ON C.CustomerId = T.CustomerId
GROUP BY C.FirstName, C.LastName
ORDER BY TicketsBought DESC;

-- 6. RAPPORT 2: Summary per Kategori - Antal sålda biljetter och total intäkt per Event
SELECT E.Title, COUNT(T.TicketId) AS TicketsSold, SUM(T.Price) AS TotalRevenue
FROM Events E
LEFT JOIN Tickets T ON E.EventId = T.EventId
GROUP BY E.Title;


-----------------------------------------------------------
-- 7: WHERE FILTRERING + ORDER BY (Krav: Minst 1 styck)
-----------------------------------------------------------

-- 7. Hitta alla dyra biljetter (över 1000 kr) sorterade efter pris
SELECT TicketId, EventId, Price, Type
FROM Tickets
WHERE Price > 1000
ORDER BY Price DESC;


-----------------------------------------------------------
-- 8: LATEST ACTIVITY FEED (Krav: Rapportfråga)
-----------------------------------------------------------

-- 8. Senaste aktivitet: De 20 senaste biljettköpen
SELECT TOP 20 T.PurchaseDate, C.FirstName, C.LastName, E.Title
FROM Tickets T
JOIN Customers C ON T.CustomerId = C.CustomerId
JOIN Events E ON T.EventId = E.EventId
ORDER BY T.PurchaseDate DESC;