USE StorageSystem

-- Видалити таблиці

IF EXISTS (SELECT name FROM sys.objects WHERE name = 'StoredProduct' AND type_desc = 'USER_TABLE')
    DROP TABLE StoredProduct

IF EXISTS (SELECT name FROM sys.objects WHERE name = 'ShopProduct' AND type_desc = 'USER_TABLE')
    DROP TABLE ShopProduct

IF EXISTS (SELECT name FROM sys.objects WHERE name = 'Product' AND type_desc = 'USER_TABLE')
    DROP TABLE Product

IF EXISTS (SELECT name FROM sys.objects WHERE name = 'Storage' AND type_desc = 'USER_TABLE')
    DROP TABLE Storage

IF EXISTS (SELECT name FROM sys.objects WHERE name = 'Shop' AND type_desc = 'USER_TABLE')
    DROP TABLE Shop

IF EXISTS (SELECT name FROM sys.objects WHERE name = 'Manufacturer' AND type_desc = 'USER_TABLE')
    DROP TABLE Manufacturer

-- Створити таблиці

CREATE TABLE Manufacturer (
    manufacturer_id CHAR(10) PRIMARY KEY,
    name VARCHAR(255),
    contacts VARCHAR(255)
);

CREATE TABLE Shop (
    shop_id CHAR(10) PRIMARY KEY,
    name VARCHAR(255),
    floor INT,
    location VARCHAR(255)
);

CREATE TABLE Storage (
    storage_id CHAR(10) PRIMARY KEY,
    address VARCHAR(255)
);

CREATE TABLE Product (
    product_id CHAR(10) PRIMARY KEY,
    name VARCHAR(255),
    manufacturer_id CHAR(10) REFERENCES Manufacturer(manufacturer_id)
);

CREATE TABLE ShopProduct (
    product_id CHAR(10) REFERENCES Product(product_id),
    shop_id CHAR(10) REFERENCES Shop(shop_id),
    price DECIMAL(10,2),
    PRIMARY KEY (product_id, shop_id)
);

CREATE TABLE StoredProduct (
    product_id CHAR(10) REFERENCES Product(product_id),
    storage_id CHAR(10) REFERENCES Storage(storage_id),
    shop_id CHAR(10) REFERENCES Shop(shop_id),
    amount INT,
    PRIMARY KEY (product_id, storage_id, shop_id)
);