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

CREATE LOGIN viewer WITH PASSWORD = '123456';
CREATE USER viewer FOR LOGIN viewer;
GRANT SELECT ON Manufacturer TO viewer;
GRANT SELECT ON Shop TO viewer;
GRANT SELECT ON Storage TO viewer;
GRANT SELECT ON Product TO viewer;
GRANT SELECT ON ShopProduct TO viewer;
GRANT SELECT ON StoredProduct TO viewer;