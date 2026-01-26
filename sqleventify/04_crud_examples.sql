USE EventifyDB;
GO

-----------------------------------------------------------
-- 1. CRUD för Customers
-----------------------------------------------------------

-- CREATE: Lägg till en ny kund
INSERT INTO Customers (FirstName, LastName, Email, PhoneNumber)
VALUES ('Kalle', 'Kula', 'kalle.kula@example.com', '073-1234567');

-- READ: Hämta information om en specifik kund via e-post
SELECT * FROM Customers 
WHERE Email = 'kalle.kula@example.com';

-- UPDATE: Uppdatera telefonnumret för en kund
UPDATE Customers 
SET PhoneNumber = '073-9998877' 
WHERE Email = 'kalle.kula@example.com';

-- DELETE: Ta bort en kund (fungerar bara om kunden inte har biljetter)
DELETE FROM Customers 
WHERE Email = 'kalle.kula@example.com';


-----------------------------------------------------------
-- 2. CRUD för Events
-----------------------------------------------------------

-- CREATE: Skapa ett nytt event (Antar att Venue 1 och Organizer 1 finns)
INSERT INTO Events (Title, Description, StartDate, EndDate, VenueId, OrganizerId)
VALUES ('Sommarfest', 'En härlig fest i parken', '2026-08-01 14:00', '2026-08-01 22:00', 1, 1);

-- READ: Lista alla kommande events efter ett visst datum
SELECT Title, StartDate FROM Events 
WHERE StartDate > '2026-01-01'
ORDER BY StartDate ASC;

-- UPDATE: Ändra beskrivningen på ett event
UPDATE Events 
SET Description = 'En ÄNNU härligare fest i parken med mat!' 
WHERE Title = 'Sommarfest';

-- DELETE: Ta bort ett event (fungerar bara om inga biljetter är sålda)
DELETE FROM Events 
WHERE Title = 'Sommarfest';


-----------------------------------------------------------
-- 3. CRUD för Tickets (Kopplingstabellen)
-----------------------------------------------------------

-- CREATE: Köp en biljett (Customer 1 köper till Event 2)
INSERT INTO Tickets (EventId, CustomerId, Price, Type)
VALUES (2, 1, 550.00, 'Regular');

-- READ: Se alla biljetter av en viss typ för ett event
SELECT * FROM Tickets 
WHERE EventId = 2 AND Type = 'Regular';

-- UPDATE: Uppgradera en biljett till VIP
UPDATE Tickets 
SET Type = 'VIP', Price = 950.00 
WHERE TicketId = 1; -- Här använder vi ID:t för den specifika biljetten

-- DELETE: Avboka/Ta bort en specifik biljett
DELETE FROM Tickets 
WHERE TicketId = 1;


-----------------------------------------------------------
-- 4. CRUD för Venues
-----------------------------------------------------------

-- UPDATE: Ändra kapaciteten på en lokal (t.ex. vid renovering)
UPDATE Venues 
SET Capacity = 18000 
WHERE Name = 'Globen';

-- SELECT: Hämta alla lokaler i en viss stad
SELECT Name, Adress, Capacity FROM Venues 
WHERE City = 'Göteborg';