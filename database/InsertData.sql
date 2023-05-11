USE StorageSystem

-- Очистити таблиці

DELETE FROM Manufacturer
DELETE FROM Shop
DELETE FROM Storage
DELETE FROM Product

-- Додати дані

INSERT INTO Manufacturer (manufacturer_id, name, contacts)
VALUES ('M001', 'ПрАТ "Оболонь"', '04212, м. Київ, Богатирська вулиця, буд. 3'),
       ('M002', 'ТОВ "ЧІПСИ ЛЮКС"', 'вулиця Польова, 17, Старі Петрівці, Київська обл., 07353'),
       ('M003', 'КОКА-КОЛА-УКРАЇНА ЛІМІТЕД', '51 км Санкт-Петербурзького шосе, Велика Димерка, Київська обл.'),
       ('M004', 'ДП "КК "РОШЕН"', 'проспект Науки, 1, Київ, 02000');

INSERT INTO Product (product_id, name, manufacturer_id)
VALUES ('P001', 'Напій слабогазований "Живчик" 2л.', 'M001'),
       ('P002', 'Вода питна Оболонська Артезіанська 6л.', 'M001'),
       ('P003', 'Пиво світле Жигулівське', 'M001'),
       ('P004', 'Чіпси "Люкс" Сметана та цибуля 133г.', 'M002'),
       ('P005', 'Чіпси картопляні Люкс Хвилясті зі смаком краба 125г.', 'M002'),
       ('P006', 'Чіпси картопляні Люкс Бекон 71г.', 'M002'),
       ('P007', 'Напій Coca-Cola сильногазований 2 л', 'M003'),
       ('P008', 'Напій Sprite сильногазований 0.5л.', 'M003'),
       ('P009', 'Чай FuzeTea Лимон чорний 0.5л.', 'M003'),
       ('P010', 'Торт Київський, 850 г', 'M004'),
       ('P011', 'Медаль шоколадна Рошен', 'M004'),
       ('P012', 'Цукерки Київ Вечірній Святкова колекція', 'M004');

INSERT INTO Storage (storage_id, address)
VALUES ('S001', 'м. Київ, вул. Михайла Грушевського, 20'),
       ('S002', 'м. Київ, пров. Волкова, 96'),
       ('S003', 'м. Київ, пров. Лесі Українки, 48');

INSERT INTO Shop (shop_id, name, floor, location)
VALUES ('SH001', 'Ашан', 1, 'Весь поверх'),
       ('SH002', 'Рошен', 2, 'Секція 1');