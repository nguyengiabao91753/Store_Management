USE master
GO

DROP DATABASE IF EXISTS StoreManagement
GO
CREATE DATABASE StoreManagement;
GO

USE StoreManagement;
GO

-- ===========================
-- USERS
-- ===========================
CREATE TABLE Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    Username VARCHAR(50) UNIQUE NOT NULL,
    [Password] VARCHAR(255) NOT NULL,
    FullName NVARCHAR(100),
    [Role] VARCHAR(20) DEFAULT 'staff' CHECK ([Role] IN ('admin', 'staff')),
    CreatedAt DATETIME DEFAULT GETDATE()
);
GO

-- ===========================
-- CUSTOMERS
-- ===========================
CREATE TABLE Customers (
    CustomerId INT IDENTITY(1,1) PRIMARY KEY,
    [Name] NVARCHAR(100) NOT NULL,
    Phone VARCHAR(20),
    Email VARCHAR(100),
    [Address] NVARCHAR(255),
    CreatedAt DATETIME DEFAULT GETDATE()
);
GO

-- ===========================
-- CATEGORIES
-- ===========================
CREATE TABLE Categories (
    CategoryId INT IDENTITY(1,1) PRIMARY KEY,
    CategoryName NVARCHAR(100) NOT NULL
);
GO

-- ===========================
-- SUPPLIERS
-- ===========================
CREATE TABLE Suppliers (
    SupplierId INT IDENTITY(1,1) PRIMARY KEY,
    [Name] NVARCHAR(100) NOT NULL,
    Phone VARCHAR(20),
    Email VARCHAR(100),
    [Address] NVARCHAR(255)
);
GO

-- ===========================
-- PRODUCTS
-- ===========================
CREATE TABLE Products (
    ProductId INT IDENTITY(1,1) PRIMARY KEY,
    CategoryId INT NULL,
    SupplierId INT NULL,
    ProductName NVARCHAR(100) NOT NULL,
	ProductImage VARCHAR(MAX) NOT NULL,
    Barcode VARCHAR(50) UNIQUE,
    Price DECIMAL(10,2) NOT NULL,
    [Unit] VARCHAR(20) DEFAULT 'pcs',
    CreatedAt DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Products_Categories FOREIGN KEY (CategoryId) REFERENCES Categories(CategoryId),
    CONSTRAINT FK_Products_Suppliers FOREIGN KEY (SupplierId) REFERENCES Suppliers(SupplierId)
);
GO

-- ===========================
-- INVENTORY
-- ===========================
CREATE TABLE Inventory (
    InventoryId INT IDENTITY(1,1) PRIMARY KEY,
    ProductId INT NOT NULL,
    Quantity INT DEFAULT 0,
    UpdatedAt DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Inventory_Products FOREIGN KEY (ProductId) REFERENCES Products(ProductId)
);
GO

-- ===========================
-- PROMOTIONS
-- ===========================
CREATE TABLE Promotions (
    PromoId INT IDENTITY(1,1) PRIMARY KEY,
    PromoCode VARCHAR(50) UNIQUE NOT NULL,
    [Description] NVARCHAR(255),
    DiscountType VARCHAR(10) CHECK (DiscountType IN ('percent', 'fixed')),
    DiscountValue DECIMAL(10,2) NOT NULL,
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,
    MinOrderAmount DECIMAL(10,2) DEFAULT 0,
    UsageLimit INT DEFAULT 0,
    UsedCount INT DEFAULT 0,
    [Status] VARCHAR(10) DEFAULT 'active' CHECK ([Status] IN ('active', 'inactive'))
);
GO

-- ===========================
-- ORDERS
-- ===========================
CREATE TABLE Orders (
    OrderId INT IDENTITY(1,1) PRIMARY KEY,
    CustomerId INT NULL,
    UserId INT NULL,
    PromoId INT NULL,
    OrderDate DATETIME DEFAULT GETDATE(),
    [Status] VARCHAR(20) DEFAULT 'pending' CHECK ([Status] IN ('pending', 'paid', 'canceled')),
    TotalAmount DECIMAL(10,2),
    DiscountAmount DECIMAL(10,2) DEFAULT 0,
    CONSTRAINT FK_Orders_Customers FOREIGN KEY (CustomerId) REFERENCES Customers(CustomerId),
    CONSTRAINT FK_Orders_Users FOREIGN KEY (UserId) REFERENCES Users(UserId),
    CONSTRAINT FK_Orders_Promotions FOREIGN KEY (PromoId) REFERENCES Promotions(PromoId)
);
GO

-- ===========================
-- ORDER ITEMS
-- ===========================
CREATE TABLE OrderItems (
    OrderItemId INT IDENTITY(1,1) PRIMARY KEY,
    OrderId INT NOT NULL,
    ProductId INT NOT NULL,
    Quantity INT NOT NULL,
    Price DECIMAL(10,2) NOT NULL,
    Subtotal AS (Quantity * Price) PERSISTED,
    CONSTRAINT FK_OrderItems_Orders FOREIGN KEY (OrderId) REFERENCES Orders(OrderId),
    CONSTRAINT FK_OrderItems_Products FOREIGN KEY (ProductId) REFERENCES Products(ProductId)
);
GO

-- ===========================
-- PAYMENTS
-- ===========================
CREATE TABLE Payments (
    PaymentId INT IDENTITY(1,1) PRIMARY KEY,
    OrderId INT NOT NULL,
    Amount DECIMAL(10,2) NOT NULL,
    PaymentMethod VARCHAR(20) DEFAULT 'cash'
        CHECK (PaymentMethod IN ('cash', 'card', 'bank_transfer', 'e-wallet')),
    PaymentDate DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Payments_Orders FOREIGN KEY (OrderId) REFERENCES Orders(OrderId)
);
GO


-- ===========================
-- INSERT DATA: USERS
-- ===========================
INSERT INTO Users (Username, [Password], FullName, [Role])
VALUES
    ('admin', '123456', N'Administrator', 'admin'),
    ('staff01', '123456', N'Nguyễn Văn A', 'staff'),
    ('staff02', '123456', N'Lê Thị B', 'staff');
GO

-- ===========================
-- INSERT DATA: CUSTOMERS
-- ===========================
INSERT INTO Customers ([Name], Phone, Email, [Address])
VALUES
    (N'Khách hàng 1', '0909000001', 'kh1@mail.com', N'Địa chỉ 1'),
    (N'Khách hàng 2', '0909000002', 'kh2@mail.com', N'Địa chỉ 2'),
    (N'Khách hàng 3', '0909000003', 'kh3@mail.com', N'Địa chỉ 3'),
    (N'Khách hàng 4', '0909000004', 'kh4@mail.com', N'Địa chỉ 4'),
    (N'Khách hàng 5', '0909000005', 'kh5@mail.com', N'Địa chỉ 5'),
    (N'Khách hàng 6', '0909000006', 'kh6@mail.com', N'Địa chỉ 6'),
    (N'Khách hàng 7', '0909000007', 'kh7@mail.com', N'Địa chỉ 7'),
    (N'Khách hàng 8', '0909000008', 'kh8@mail.com', N'Địa chỉ 8'),
    (N'Khách hàng 9', '0909000009', 'kh9@mail.com', N'Địa chỉ 9'),
    (N'Khách hàng 10', '0909000010', 'kh10@mail.com', N'Địa chỉ 10');
GO

-- ===========================
-- INSERT DATA: CATEGORIES
-- ===========================
INSERT INTO Categories (CategoryName)
VALUES
    (N'Đồ uống'),
    (N'Bánh kẹo'),
    (N'Gia vị'),
    (N'Đồ gia dụng'),
    (N'Mỹ phẩm');
GO

-- ===========================
-- INSERT DATA: SUPPLIERS
-- ===========================
INSERT INTO Suppliers ([Name], Phone, Email, [Address])
VALUES
    (N'Công ty ABC', '0909123456', 'abc@gmail.com', N'Hà Nội'),
    (N'Công ty XYZ', '0912123456', 'xyz@gmail.com', N'TP HCM'),
    (N'Công ty 123', '0933123456', '123@gmail.com', N'Đà Nẵng');
GO

-- ===========================
-- INSERT DATA: PRODUCTS
-- ===========================
INSERT INTO Products (CategoryId, SupplierId, ProductName, ProductImage, Barcode, Price, [Unit])
VALUES
    (1, 1, N'Trà Ô Long', 'oolong_tea.png', 'SP0001', 12000, 'bottle'),
    (1, 2, N'Nước suối Lavie', 'lavie_water.png', 'SP0002', 8000, 'bottle'),
    (2, 2, N'Bánh ChocoPie', 'chocopie.png', 'SP0003', 35000, 'box'),
    (3, 3, N'Nước mắm Nam Ngư', 'nam_ngu_fish_sauce.png', 'SP0004', 42000, 'bottle'),
    (4, 1, N'Nồi cơm điện Sunhouse', 'sunhouse_ricecooker.png', 'SP0005', 890000, 'unit'),
    (5, 3, N'Sữa rửa mặt Senka', 'senka_facewash.png', 'SP0006', 120000, 'tube');
GO


-- ===========================
-- INSERT DATA: INVENTORY
-- ===========================
INSERT INTO Inventory (ProductId, Quantity)
VALUES
    (1, 100),
    (2, 200),
    (3, 150),
    (4, 80),
    (5, 40),
    (6, 90);
GO

-- ===========================
-- INSERT DATA: PROMOTIONS
-- ===========================
INSERT INTO Promotions (PromoCode, [Description], DiscountType, DiscountValue, StartDate, EndDate, MinOrderAmount, UsageLimit, [Status])
VALUES
    ('SALE10', N'Giảm 10% cho đơn hàng từ 200k', 'percent', 10, '2025-10-01', '2025-12-31', 200000, 100, 'active'),
    ('FIX50', N'Giảm 50,000 cho đơn hàng từ 300k', 'fixed', 50000, '2025-10-01', '2025-11-30', 300000, 50, 'active');
GO

-- ===========================
-- INSERT DATA: ORDERS
-- ===========================
INSERT INTO Orders (CustomerId, UserId, PromoId, [Status], TotalAmount, DiscountAmount)
VALUES
    (1, 1, 1, 'paid', 250000, 25000),
    (2, 2, NULL, 'pending', 180000, 0),
    (3, 1, 2, 'paid', 350000, 50000);
GO

-- ===========================
-- INSERT DATA: ORDER ITEMS
-- ===========================
INSERT INTO OrderItems (OrderId, ProductId, Quantity, Price)
VALUES
    (1, 1, 2, 12000),
    (1, 3, 3, 35000),
    (2, 2, 5, 8000),
    (3, 5, 1, 890000),
    (3, 6, 2, 120000);
GO

-- ===========================
-- INSERT DATA: PAYMENTS
-- ===========================
INSERT INTO Payments (OrderId, Amount, PaymentMethod)
VALUES
    (1, 225000, 'card'),
    (2, 0, 'cash'),
    (3, 300000, 'bank_transfer');
GO

-- Lệnh SQL để thêm cột Active
ALTER TABLE Users
ADD Active BIT DEFAULT 1 NOT NULL;
