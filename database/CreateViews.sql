USE StorageSystem

-- Видалити уявлення

IF EXISTS (SELECT name FROM sys.objects WHERE name = 'ProductDetails' AND type_desc = 'VIEW')
    DROP VIEW ProductDetails

IF EXISTS (SELECT name FROM sys.objects WHERE name = 'ShopInventory' AND type_desc = 'VIEW')
    DROP VIEW ShopInventory

IF EXISTS (SELECT name FROM sys.objects WHERE name = 'StorageInventory' AND type_desc = 'VIEW')
    DROP VIEW StorageInventory
GO
-- Створити уявлення

CREATE VIEW ProductDetails AS
SELECT p.product_id, p.name AS product_name, m.name AS manufacturer_name, m.contacts
FROM Product p, Manufacturer m
WHERE p.manufacturer_id = m.manufacturer_id;
GO

CREATE VIEW ShopInventory AS
SELECT sp.shop_id, s.name AS shop_name, p.product_id, p.name AS product_name, sp.price, sps.amount
FROM ShopProduct sp, Product p, Shop s, StoredProduct sps
WHERE sp.product_id = p.product_id
AND sp.shop_id = s.shop_id
AND sp.product_id = sps.product_id
AND sp.shop_id = sps.shop_id;
GO

CREATE VIEW StorageInventory AS
SELECT sp.storage_id, sps.product_id, p.name AS product_name, sps.amount
FROM StoredProduct sps, Storage sp, Product p
WHERE sps.storage_id = sp.storage_id
AND sps.product_id = p.product_id;