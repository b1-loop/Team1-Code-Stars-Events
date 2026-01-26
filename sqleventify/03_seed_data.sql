USE EventifyDB;
GO

-- 1. Fyll på Venues (Lokaler)
INSERT INTO Venues (Name, Adress, City, Capacity, Date) VALUES
('Globen', 'Arenatorget 1', 'Stockholm', 16000, '1989-02-19'),
('Ullevi', 'Skånegatan', 'Göteborg', 75000, '1958-05-29'),
('Malmö Arena', 'Hyllie Stationstorg 2', 'Malmö', 13000, '2008-11-06'),
('Scandinavium', 'Valhallagatan 1', 'Göteborg', 12000, '1971-05-18');

-- 2. Fyll på Organizers (Arrangörer)
INSERT INTO Organizers (Name, Email, PhoneNumber) VALUES
('Live Nation', 'info@livenation.se', '08-6670000'),
('Luger', 'hello@luger.se', '08-57867900'),
('Eventify AB', 'contact@eventify.com', '031-123456');

-- 3. Fyll på Events (Minst 6 stycken enligt krav)
-- Antar att VenueId 1-4 och OrganizerId 1-3 skapats ovan
INSERT INTO Events (Title, Description, StartDate, EndDate, VenueId, OrganizerId) VALUES
('Rock Night 2026', 'En kväll med tung rock.', '2026-05-10 19:00', '2026-05-10 23:30', 1, 1),
('Jazz under stjärnorna', 'Lugn jazz i sommarnatten.', '2026-07-15 20:00', '2026-07-15 23:00', 2, 2),
('Tech Conf 2026', 'Framtidens kodning och AI.', '2026-09-20 09:00', '2026-09-21 17:00', 3, 3),
('Standup Comedy', 'Sveriges roligaste komiker.', '2026-03-12 19:30', '2026-03-12 21:30', 4, 1),
('Pop Extravaganza', 'Den ultimata poppartyt.', '2026-06-05 18:00', '2026-06-05 23:59', 1, 2),
('Classical Masterpieces', 'Symfoniorkestern spelar Bach.', '2026-11-01 15:00', '2026-11-01 18:00', 2, 3);

-- 4. Fyll på Customers (Minst 10 stycken enligt krav)
INSERT INTO Customers (FirstName, LastName, Email, PhoneNumber) VALUES
('Anna', 'Andersson', 'anna.a@mail.com', '070-1112233'),
('Bertil', 'Bengtsson', 'berras@post.se', '070-2223344'),
('Cecilia', 'Ceder', 'cecci@live.com', '070-3334455'),
('David', 'Dahl', 'david.d@gmail.com', '070-4445566'),
('Erik', 'Eriksson', 'erik.e@outlook.com', '070-5556677'),
('Frida', 'Forsberg', 'frida.f@skola.se', '070-6667788'),
('Gustav', 'Gunnarsson', 'gustav@firma.se', '070-7778899'),
('Hanna', 'Holm', 'hanna.h@webb.se', '070-8889900'),
('Isak', 'Isaksson', 'isak.i@home.com', '070-9990011'),
('Johanna', 'Jansson', 'johanna.j@test.com', '070-0001122');

-- 5. Fyll på Tickets (Minst 25 stycken i kopplingstabellen enligt krav)
-- Blandar VIP, Regular, Student och Backstage för att testa CHECK-constrainten
INSERT INTO Tickets (EventId, CustomerId, Price, Type) VALUES
(1, 1, 850.00, 'VIP'), (1, 2, 450.00, 'Regular'), (1, 3, 350.00, 'Student'), (1, 4, 1200.00, 'Backstage'), (1, 5, 450.00, 'Regular'),
(2, 6, 500.00, 'Regular'), (2, 7, 500.00, 'Regular'), (2, 8, 400.00, 'Student'), (2, 1, 950.00, 'VIP'),
(3, 2, 2500.00, 'Regular'), (3, 3, 2500.00, 'Regular'), (3, 4, 1500.00, 'Student'), (3, 9, 3500.00, 'VIP'),
(4, 5, 350.00, 'Regular'), (4, 6, 350.00, 'Regular'), (4, 10, 350.00, 'Regular'), (4, 7, 250.00, 'Student'),
(5, 8, 600.00, 'Regular'), (5, 9, 1000.00, 'VIP'), (5, 1, 600.00, 'Regular'), (5, 2, 600.00, 'Regular'),
(6, 3, 400.00, 'Regular'), (6, 4, 400.00, 'Regular'), (6, 5, 300.00, 'Student'), (6, 10, 800.00, 'VIP');