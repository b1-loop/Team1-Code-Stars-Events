# Team1-Code-Stars-Events

# 🎟️ Eventify – Team1 Code Stars

> Database First – SQL Server + .NET Console Application
> Grupprojekt inom kursen **Databaser**  
> _Team 1 – Code Stars_
-----------------------------------------------------------------
📌 Projektbeskrivning

**Eventify** är ett  datadrivet system för hantering av:

- Event
- Arrangörer
- Kunder
- Biljetter

Projektet är byggt enligt Database First-principen, där hela databasen:
- modelleras
- skapas
- dokumenteras i SQL  
Innan koppling till .NET sker.

Syftet är att visa att vi kan:
•	designa en relationsdatabas (PK/FK, constraints, normalisering)
•	skapa och köra SQL i rätt ordning (DDL + seed)
•	jobba Database First (scaffolding → genererade modeller)
•	bygga en Console App med menyer, CRUD, och rapporter (JOINs)
Domän (exempel):
•	Venues: lokaler där event hålls
•	Organizers: arrangörer
•	Events: event kopplade till venue + organizer
•	Customers: kunder
•	Tickets: koppling mellan customer ↔ event (many-to-many) + biljettinfo

------------------------------------------------------------------------------
Menyflöden 
Flöde – Lista + detaljvy
1.	“List Events”
2.	välj ett EventId
3.	visa event + venue + organizer (JOIN)

Flöde  – Skapa kund + köp biljett
1.	“Create Customer” (förnamn, efternamn, email)
2.	“Buy Ticket” → välj customer + event
3.	se att ticket skapas och kopplas korrekt
Flöde 3 – Sök/filtrera events
1.	“Search Events” (på titel eller datumintervall)
2.	lista matchningar
3.	visa antal sålda biljetter per event (JOIN + COUNT)
   
Flöde  – Uppdatera event
1.	“Update Event”
2.	ändra datum / venue
3.	verifiera att ändringen syns i listan

Flöde  – Avboka / ta bort ticket
1.	“My Tickets” (för en customer)
2.	välj ticket → Delete
3.	verifiera att ticket uppdateras i rapport
-----------------------------------------------------------------
🎯 Syfte med projektet
- Relationsdatabasdesign (3NF)
- PK / FK & constraints
- CRUD-operationer
- JOIN-frågor & rapporter
- Views
- Databassäkerhet
- Database First med .NET
- Professionell GitHub-struktur

 🧱 Teknikstack
- SQL Server (Docker / Local)
- T-SQL
- .NET 8 – Console Application
- Entity Framework Core (Database First)
- Git & GitHub (grupparbete)

🗄️ Databasmodell

Tabeller
| Tabell | Beskrivning |
|------|------------|
| `Venues` | Lokaler där event hålls |
| `Organizers` | Arrangörer |
| `Events` | Event |
| `Customers` | Kunder |
| `Tickets` | Biljetter (kopplingstabell) |
----------------------------------------------------------------------
Relationer

- Ett Event→ en Venue
- Ett **Event→ en Organizer
- En **Customer → många Tickets
- Ett Event → många Tickets

`Tickets` representerar en **many-to-many-relation** mellan `Customers` och `Events`.
📁 SQL-struktur (filer & uppbyggnad)
/SQL
 ├── 01_create_database.sql
 ├── 02_create_tables.sql
 ├── 03_seed_data.sql
 ├── 04_crud_examples.sql
 ├── 05_queries_joins.sql
 ├── 06_views.sql
 ├── 07_security.sql
 └── 08_cleanup.sql

01_create_database.sql
Syfte: Skapar databasen
Innehåller:
•	CREATE DATABASE EventifyDB
•	USE EventifyDB
•	Grundläggande inställningar
📌 Körs först

02_create_tables.sql
Syfte: Skapar hela databasstrukturen
Innehåller:
•	Alla tabeller (Venues, Organizers, Events, Customers, Tickets)
•	Primärnycklar (PRIMARY KEY)
•	Främmande nycklar (FOREIGN KEY)
•	Constraints:
o	NOT NULL
o	UNIQUE
o	CHECK
o	DEFAULT
•	Tydlig ordning för FK-beroenden
📌 Här sätts dataintegriteten

03_seed_data.sql
Syfte: Testdata för utveckling & test
Innehåller:
•	Insert av venues
•	Insert av organizers
•	Insert av events
•	Insert av customers
•	Insert av tickets
📌 Gör databasen direkt användbar

04_crud_examples.sql
Syfte: Visa CRUD-kunskap
Innehåller exempel på:
•	INSERT
•	SELECT
•	UPDATE
•	DELETE
Mot flera tabeller och realistiska scenarion.

05_queries_joins.sql
Syfte: Rapportering & analys
Innehåller:
•	INNER JOIN
•	LEFT JOIN
•	Sammanställningar som:
o	Events + venues
o	Events + organizers
o	Tickets + customers
•	Sortering & filtrering
📌 Visar förståelse för relationer

06_views.sql
Syfte: Förenkla komplexa frågor
Innehåller:
•	Views för rapportering
•	Exempel:
o	Eventöversikt
o	Biljettförsäljning
o	Kundhistorik
📌 Används som ”färdiga rapporter”
07_security.sql
Syfte: Databassäkerhet
Innehåller exempel på:
•	Skapa databasroller
•	Skapa användare
•	GRANT / DENY
•	Begränsad åtkomst (READ / WRITE)
📌 Visar säkerhetstänk

08_cleanup.sql
Syfte: Rensa databasen
Innehåller:
•	DROP TABLE
•	DROP DATABASE
📌 Används vid ominstallation/test

🖥️ .NET Console Application

Applikationen är byggd med Database First:

Databasen scaffoldas till C#-klasser

DbContext används för åtkomst

Connection string ligger i appsettings.json

Menybaserat console-gränssnitt

Läsning av data via EF Core


