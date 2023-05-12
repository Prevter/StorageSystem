USE StorageSystem

-- Видалити процедури

DROP PROCEDURE IF EXISTS InsertManufacturer;
DROP PROCEDURE IF EXISTS InsertShop;
DROP PROCEDURE IF EXISTS InsertStorage;
DROP PROCEDURE IF EXISTS InsertProduct;
DROP PROCEDURE IF EXISTS InsertShopProduct;
DROP PROCEDURE IF EXISTS InsertStoredProduct;

-- Створити процедури для вставки даних в таблиці

GO
CREATE PROCEDURE InsertManufacturer
    @manufacturer_id CHAR(10),
    @name VARCHAR(255),
    @contacts VARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Manufacturer (manufacturer_id, name, contacts)
    VALUES (@manufacturer_id, @name, @contacts)
END;

GO
CREATE PROCEDURE InsertShop
    @shop_id CHAR(10),
    @name VARCHAR(255),
    @floor INT,
    @location VARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Shop (shop_id, name, floor, location)
    VALUES (@shop_id, @name, @floor, @location)
END;

GO
CREATE PROCEDURE InsertStorage
    @storage_id CHAR(10),
    @address VARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Storage (storage_id, address)
    VALUES (@storage_id, @address)
END;

GO
CREATE PROCEDURE InsertProduct
    @product_id CHAR(10),
    @name VARCHAR(255),
    @manufacturer_id CHAR(10)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Product (product_id, name, manufacturer_id)
    VALUES (@product_id, @name, @manufacturer_id)
END;

GO
CREATE PROCEDURE InsertShopProduct
    @product_id CHAR(10),
    @shop_id CHAR(10),
    @price DECIMAL(10,2)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO ShopProduct (product_id, shop_id, price)
    VALUES (@product_id, @shop_id, @price)
END;

GO
CREATE PROCEDURE InsertStoredProduct
    @product_id CHAR(10),
    @storage_id CHAR(10),
    @shop_id CHAR(10),
    @amount INT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO StoredProduct (product_id, storage_id, shop_id, amount)
    VALUES (@product_id, @storage_id, @shop_id, @amount)
END;