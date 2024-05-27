-- Check if KhumaloCraft database exists before creating
IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'KhumaloCraft')
BEGIN
    CREATE DATABASE KhumaloCraft;
    PRINT 'KhumaloCraft database created.';
END;
ELSE
BEGIN
    PRINT 'KhumaloCraft database already exists.';
END;
GO

-- Use KhumaloCraft database
USE KhumaloCraft;
GO

-- Check if Users table exists before creating
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Users')
BEGIN
    CREATE TABLE Users (
        UserID INT PRIMARY KEY,
        Username VARCHAR(50),
        Email VARCHAR(100),
        Password VARCHAR(100),
        UserType VARCHAR(20)
    );
    PRINT 'Users table created.';
END;
ELSE
BEGIN
    PRINT 'Users table already exists.';
END;

-- Check if Categories table exists before creating
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Categories')
BEGIN
    CREATE TABLE Categories (
        CategoryID INT PRIMARY KEY,
        CategoryName VARCHAR(50)
    );
    PRINT 'Categories table created.';
END;
ELSE
BEGIN
    PRINT 'Categories table already exists.';
END;

-- Check if Products table exists before creating
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Products')
BEGIN
    CREATE TABLE Products (
        ProductID INT IDENTITY(1,1) PRIMARY KEY,
        ProductName VARCHAR(100),
        Description TEXT,
        Price DECIMAL(10, 2),
        CategoryID INT,
        Availability BIT,
        FOREIGN KEY (CategoryID) REFERENCES Categories(CategoryID)
    );
    PRINT 'Products table created.';
END;
ELSE
BEGIN
    PRINT 'Products table already exists.';
END;

-- Check if Orders table exists before creating
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Orders')
BEGIN
    CREATE TABLE Orders (
        OrderID INT PRIMARY KEY,
        UserID INT,
        OrderDate DATETIME,
        TotalAmount DECIMAL(10, 2),
        FOREIGN KEY (UserID) REFERENCES Users(UserID)
    );
    PRINT 'Orders table created.';
END;
ELSE
BEGIN
    PRINT 'Orders table already exists.';
END;

-- Check if OrderDetails table exists before creating
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'OrderDetails')
BEGIN
    CREATE TABLE OrderDetails (
        OrderDetailID INT PRIMARY KEY,
        OrderID INT,
        ProductID INT,
        Quantity INT,
        UnitPrice DECIMAL(10, 2),
        FOREIGN KEY (OrderID) REFERENCES Orders(OrderID),
        FOREIGN KEY (ProductID) REFERENCES Products(ProductID)
    );
    PRINT 'OrderDetails table created.';
END;
ELSE
BEGIN
    PRINT 'OrderDetails table already exists.';
END;


INSERT INTO Categories (CategoryID, CategoryName) VALUES (1, 'Handmade Jewelry');
INSERT INTO Categories (CategoryID, CategoryName) VALUES (2, 'Home Decor');
INSERT INTO Categories (CategoryID, CategoryName) VALUES (3, 'Art & Sculptures');
INSERT INTO Categories (CategoryID, CategoryName) VALUES (4, 'Clothing & Accessories');
INSERT INTO Categories (CategoryID, CategoryName) VALUES (5, 'Wood Crafts');

ALTER TABLE Products
ADD ImageURL VARCHAR(255);

ALTER TABLE Orders
ALTER COLUMN UserId NVARCHAR(256);

-- Drop OrderDetails table first
IF OBJECT_ID('dbo.OrderDetails', 'U') IS NOT NULL
    DROP TABLE dbo.OrderDetails;

-- Drop Orders table next
IF OBJECT_ID('dbo.Orders', 'U') IS NOT NULL
    DROP TABLE dbo.Orders;

-- Drop Products table
IF OBJECT_ID('dbo.Products', 'U') IS NOT NULL
    DROP TABLE dbo.Products;

-- Drop Categories table
IF OBJECT_ID('dbo.Categories', 'U') IS NOT NULL
    DROP TABLE dbo.Categories;