-- Använd databasen
USE EventifyDB;
GO

-- 1. Lokaler (Venues)
CREATE TABLE Venues (
    VenueId INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Adress NVARCHAR(200),
    City NVARCHAR(100),
    Capacity INT CHECK (Capacity > 0), 
    Date DATETIME, 
    CreatedAt DATETIME DEFAULT GETDATE()
);

-- 2. Arrangörer (Organizers)
CREATE TABLE Organizers (
    OrganizerId INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    -- CHECK: Ser till att e-posten har ett giltigt format
    Email NVARCHAR(100) UNIQUE CHECK (Email LIKE '%_@_%._%'),
    PhoneNumber NVARCHAR(20),
    CreatedAt DATETIME DEFAULT GETDATE()
);

-- 3. Evenemang (Events)
CREATE TABLE Events (
    EventId INT PRIMARY KEY IDENTITY(1,1),
    Title NVARCHAR(150) NOT NULL,
    Description NVARCHAR(MAX),
    StartDate DATETIME NOT NULL,
    EndDate DATETIME NOT NULL,
    VenueId INT NOT NULL,
    OrganizerId INT NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),

    CONSTRAINT FK_Events_Venues FOREIGN KEY (VenueId) REFERENCES Venues(VenueId),
    CONSTRAINT FK_Events_Organizers FOREIGN KEY (OrganizerId) REFERENCES Organizers(OrganizerId),
    CONSTRAINT CHK_Dates CHECK (EndDate >= StartDate)
);

-- 4. Kunder (Customers)
CREATE TABLE Customers (
    CustomerId INT PRIMARY KEY IDENTITY(1,1),
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    -- CHECK: Samma validering här för kundernas e-post
    Email NVARCHAR(100) UNIQUE NOT NULL CHECK (Email LIKE '%_@_%._%'),
    PhoneNumber NVARCHAR(20),
    CreatedAt DATETIME DEFAULT GETDATE()
);

-- 5. Biljetter (Tickets)
CREATE TABLE Tickets (
    TicketId INT PRIMARY KEY IDENTITY(1,1),
    EventId INT NOT NULL,
    CustomerId INT NOT NULL,
    PurchaseDate DATETIME DEFAULT GETDATE(),
    Price DECIMAL(10, 2) NOT NULL CHECK (Price >= 0),
    
    -- CHECK: Krav 6 - Begränsar biljettyp
    Type NVARCHAR(20) DEFAULT 'Regular' 
        CHECK (Type IN ('VIP', 'Regular', 'Student', 'Backstage')),

    CONSTRAINT FK_Tickets_Events FOREIGN KEY (EventId) REFERENCES Events(EventId),
    CONSTRAINT FK_Tickets_Customers FOREIGN KEY (CustomerId) REFERENCES Customers(CustomerId)
);
