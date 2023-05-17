USE StorageSystem

-- Видалити користувачів

IF EXISTS (SELECT * FROM sys.database_principals WHERE name = 'manager')
BEGIN
    REVOKE SELECT, INSERT, UPDATE, DELETE ON Manufacturer TO manager;
    REVOKE SELECT, INSERT, UPDATE, DELETE ON Shop TO manager;
    REVOKE SELECT, INSERT, UPDATE, DELETE ON Storage TO manager;
    REVOKE SELECT, INSERT, UPDATE, DELETE ON Product TO manager;
    REVOKE SELECT, INSERT, UPDATE, DELETE ON ShopProduct TO manager;
    REVOKE SELECT, INSERT, UPDATE, DELETE ON StoredProduct TO manager;
	REVOKE SELECT ON ProductDetails TO manager;
	REVOKE SELECT ON ShopInventory TO manager;
	REVOKE SELECT ON StorageInventory TO manager;
	REVOKE EXECUTE ON InsertManufacturer TO manager;
	REVOKE EXECUTE ON InsertShop TO manager;
	REVOKE EXECUTE ON InsertStorage TO manager;
	REVOKE EXECUTE ON InsertProduct TO manager;
	REVOKE EXECUTE ON InsertShopProduct TO manager;
	REVOKE EXECUTE ON InsertStoredProduct TO manager;
    DROP USER manager;
END

IF EXISTS (SELECT * FROM sys.server_principals WHERE name = 'manager')
    DROP LOGIN manager;

IF EXISTS (SELECT * FROM sys.database_principals WHERE name = 'viewer')
BEGIN
    REVOKE SELECT ON Manufacturer TO viewer;
    REVOKE SELECT ON Shop TO viewer;
    REVOKE SELECT ON Storage TO viewer;
    REVOKE SELECT ON Product TO viewer;
    REVOKE SELECT ON ShopProduct TO viewer;
    REVOKE SELECT ON StoredProduct TO viewer;
	REVOKE SELECT ON ProductDetails TO viewer;
	REVOKE SELECT ON ShopInventory TO viewer;
	REVOKE SELECT ON StorageInventory TO viewer;
    DROP USER viewer;
END

IF EXISTS (SELECT * FROM sys.server_principals WHERE name = 'viewer')
    DROP LOGIN viewer;

-- Створити користувачів

CREATE LOGIN manager WITH PASSWORD = '12345678';
CREATE USER manager FOR LOGIN manager;
GRANT SELECT, INSERT, UPDATE, DELETE ON Manufacturer TO manager;
GRANT SELECT, INSERT, UPDATE, DELETE ON Shop TO manager;
GRANT SELECT, INSERT, UPDATE, DELETE ON Storage TO manager;
GRANT SELECT, INSERT, UPDATE, DELETE ON Product TO manager;
GRANT SELECT, INSERT, UPDATE, DELETE ON ShopProduct TO manager;
GRANT SELECT, INSERT, UPDATE, DELETE ON StoredProduct TO manager;
GRANT SELECT ON ProductDetails TO manager;
GRANT SELECT ON ShopInventory TO manager;
GRANT SELECT ON StorageInventory TO manager;
GRANT EXECUTE ON InsertManufacturer TO manager;
GRANT EXECUTE ON InsertShop TO manager;
GRANT EXECUTE ON InsertStorage TO manager;
GRANT EXECUTE ON InsertProduct TO manager;
GRANT EXECUTE ON InsertShopProduct TO manager;
GRANT EXECUTE ON InsertStoredProduct TO manager;

CREATE LOGIN viewer WITH PASSWORD = '123456';
CREATE USER viewer FOR LOGIN viewer;
GRANT SELECT ON Manufacturer TO viewer;
GRANT SELECT ON Shop TO viewer;
GRANT SELECT ON Storage TO viewer;
GRANT SELECT ON Product TO viewer;
GRANT SELECT ON ShopProduct TO viewer;
GRANT SELECT ON StoredProduct TO viewer;
GRANT SELECT ON ProductDetails TO viewer;
GRANT SELECT ON ShopInventory TO viewer;
GRANT SELECT ON StorageInventory TO viewer;