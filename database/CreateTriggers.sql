USE StorageSystem

-- Видалити тригери

DROP TRIGGER IF EXISTS DeleteManufacturerTrigger;
DROP TRIGGER IF EXISTS DeleteShopTrigger;
DROP TRIGGER IF EXISTS DeleteStorageTrigger;
DROP TRIGGER IF EXISTS DeleteProductTrigger;

-- Створити тригери

GO
CREATE TRIGGER DeleteManufacturerTrigger
ON Manufacturer
INSTEAD OF DELETE
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM Product WHERE manufacturer_id IN (SELECT manufacturer_id FROM deleted);
    DELETE FROM Manufacturer WHERE manufacturer_id IN (SELECT manufacturer_id FROM deleted);
END;

GO
CREATE TRIGGER DeleteShopTrigger
ON Shop
INSTEAD OF DELETE
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM ShopProduct WHERE shop_id IN (SELECT shop_id FROM deleted);
    DELETE FROM StoredProduct WHERE shop_id IN (SELECT shop_id FROM deleted);
    DELETE FROM Shop WHERE shop_id IN (SELECT shop_id FROM deleted);
END;

GO
CREATE TRIGGER DeleteStorageTrigger
ON Storage
INSTEAD OF DELETE
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM StoredProduct WHERE storage_id IN (SELECT storage_id FROM deleted);
    DELETE FROM Storage WHERE storage_id IN (SELECT storage_id FROM deleted);
END;

GO
CREATE TRIGGER DeleteProductTrigger
ON Product
INSTEAD OF DELETE
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM ShopProduct WHERE product_id IN (SELECT product_id FROM deleted);
    DELETE FROM StoredProduct WHERE product_id IN (SELECT product_id FROM deleted);
    DELETE FROM Product WHERE product_id IN (SELECT product_id FROM deleted);
END;