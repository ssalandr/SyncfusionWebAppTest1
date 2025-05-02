-- Clear any existing data (optional)
TRUNCATE TABLE "OrdersDetails" RESTART IDENTITY;

-- Insert data from the GetAllRecords() method
INSERT INTO "OrdersDetails" ("CustomerID", "EmployeeID", "Freight", "ShipCity", "Verified", "OrderDate", "ShipName", "ShipCountry", "ShippedDate", "ShipAddress")
VALUES 
-- First batch (i=1)
('ALFKI', 1, 2.3, 'Berlin', false, '1991-05-15 10:40:00', 'Simons bistro', 'Denmark', '1996-07-16 00:00:00', 'Kirchgasse 6'),
('ANATR', 3, 3.3, 'Madrid', true, '1990-04-04 09:30:00', 'Queen Cozinha', 'Brazil', '1996-09-11 00:00:00', 'Avda. Azteca 123'),
('ANTON', 2, 4.3, 'Cholchester', true, '1957-11-30 03:45:00', 'Frankenversand', 'Germany', '1996-10-07 00:00:00', 'Carrera 52 con Ave. Bolívar #65-98 Llano Largo'),
('BLONP', 4, 5.3, 'Marseille', false, '1930-10-22 07:30:00', 'Ernst Handel', 'Austria', '1996-12-30 00:00:00', 'Magazinweg 7'),
('BOLID', 5, 6.3, 'Tsawassen', true, '1953-02-18 05:50:00', 'Hanari Carnes', 'Switzerland', '1997-12-03 00:00:00', '1029 - 12th Ave. S.'),

-- Second batch (i=2)
('ALFKI', 2, 4.6, 'Berlin', false, '1991-05-15 10:40:00', 'Simons bistro', 'Denmark', '1996-07-16 00:00:00', 'Kirchgasse 6'),
('ANATR', 4, 6.6, 'Madrid', true, '1990-04-04 09:30:00', 'Queen Cozinha', 'Brazil', '1996-09-11 00:00:00', 'Avda. Azteca 123'),
('ANTON', 3, 8.6, 'Cholchester', true, '1957-11-30 03:45:00', 'Frankenversand', 'Germany', '1996-10-07 00:00:00', 'Carrera 52 con Ave. Bolívar #65-98 Llano Largo'),
('BLONP', 5, 10.6, 'Marseille', false, '1930-10-22 07:30:00', 'Ernst Handel', 'Austria', '1996-12-30 00:00:00', 'Magazinweg 7'),
('BOLID', 6, 12.6, 'Tsawassen', true, '1953-02-18 05:50:00', 'Hanari Carnes', 'Switzerland', '1997-12-03 00:00:00', '1029 - 12th Ave. S.'),

-- Third batch (i=3)
('ALFKI', 3, 6.9, 'Berlin', false, '1991-05-15 10:40:00', 'Simons bistro', 'Denmark', '1996-07-16 00:00:00', 'Kirchgasse 6'),
('ANATR', 5, 9.9, 'Madrid', true, '1990-04-04 09:30:00', 'Queen Cozinha', 'Brazil', '1996-09-11 00:00:00', 'Avda. Azteca 123'),
('ANTON', 4, 12.9, 'Cholchester', true, '1957-11-30 03:45:00', 'Frankenversand', 'Germany', '1996-10-07 00:00:00', 'Carrera 52 con Ave. Bolívar #65-98 Llano Largo'),
('BLONP', 6, 15.9, 'Marseille', false, '1930-10-22 07:30:00', 'Ernst Handel', 'Austria', '1996-12-30 00:00:00', 'Magazinweg 7'),
('BOLID', 7, 18.9, 'Tsawassen', true, '1953-02-18 05:50:00', 'Hanari Carnes', 'Switzerland', '1997-12-03 00:00:00', '1029 - 12th Ave. S.'),

-- Fourth batch (i=4)
('ALFKI', 4, 9.2, 'Berlin', false, '1991-05-15 10:40:00', 'Simons bistro', 'Denmark', '1996-07-16 00:00:00', 'Kirchgasse 6'),
('ANATR', 6, 13.2, 'Madrid', true, '1990-04-04 09:30:00', 'Queen Cozinha', 'Brazil', '1996-09-11 00:00:00', 'Avda. Azteca 123'),
('ANTON', 5, 17.2, 'Cholchester', true, '1957-11-30 03:45:00', 'Frankenversand', 'Germany', '1996-10-07 00:00:00', 'Carrera 52 con Ave. Bolívar #65-98 Llano Largo'),
('BLONP', 7, 21.2, 'Marseille', false, '1930-10-22 07:30:00', 'Ernst Handel', 'Austria', '1996-12-30 00:00:00', 'Magazinweg 7'),
('BOLID', 8, 25.2, 'Tsawassen', true, '1953-02-18 05:50:00', 'Hanari Carnes', 'Switzerland', '1997-12-03 00:00:00', '1029 - 12th Ave. S.'),

-- Fifth batch (i=5)
('ALFKI', 5, 11.5, 'Berlin', false, '1991-05-15 10:40:00', 'Simons bistro', 'Denmark', '1996-07-16 00:00:00', 'Kirchgasse 6'),
('ANATR', 7, 16.5, 'Madrid', true, '1990-04-04 09:30:00', 'Queen Cozinha', 'Brazil', '1996-09-11 00:00:00', 'Avda. Azteca 123'),
('ANTON', 6, 21.5, 'Cholchester', true, '1957-11-30 03:45:00', 'Frankenversand', 'Germany', '1996-10-07 00:00:00', 'Carrera 52 con Ave. Bolívar #65-98 Llano Largo'),
('BLONP', 8, 26.5, 'Marseille', false, '1930-10-22 07:30:00', 'Ernst Handel', 'Austria', '1996-12-30 00:00:00', 'Magazinweg 7'),
('BOLID', 9, 31.5, 'Tsawassen', true, '1953-02-18 05:50:00', 'Hanari Carnes', 'Switzerland', '1997-12-03 00:00:00', '1029 - 12th Ave. S.'),

-- Sixth batch (i=6)
('ALFKI', 6, 13.8, 'Berlin', false, '1991-05-15 10:40:00', 'Simons bistro', 'Denmark', '1996-07-16 00:00:00', 'Kirchgasse 6'),
('ANATR', 8, 19.8, 'Madrid', true, '1990-04-04 09:30:00', 'Queen Cozinha', 'Brazil', '1996-09-11 00:00:00', 'Avda. Azteca 123'),
('ANTON', 7, 25.8, 'Cholchester', true, '1957-11-30 03:45:00', 'Frankenversand', 'Germany', '1996-10-07 00:00:00', 'Carrera 52 con Ave. Bolívar #65-98 Llano Largo'),
('BLONP', 9, 31.8, 'Marseille', false, '1930-10-22 07:30:00', 'Ernst Handel', 'Austria', '1996-12-30 00:00:00', 'Magazinweg 7'),
('BOLID', 10, 37.8, 'Tsawassen', true, '1953-02-18 05:50:00', 'Hanari Carnes', 'Switzerland', '1997-12-03 00:00:00', '1029 - 12th Ave. S.'),

-- Seventh batch (i=7)
('ALFKI', 7, 16.1, 'Berlin', false, '1991-05-15 10:40:00', 'Simons bistro', 'Denmark', '1996-07-16 00:00:00', 'Kirchgasse 6'),
('ANATR', 9, 23.1, 'Madrid', true, '1990-04-04 09:30:00', 'Queen Cozinha', 'Brazil', '1996-09-11 00:00:00', 'Avda. Azteca 123'),
('ANTON', 8, 30.1, 'Cholchester', true, '1957-11-30 03:45:00', 'Frankenversand', 'Germany', '1996-10-07 00:00:00', 'Carrera 52 con Ave. Bolívar #65-98 Llano Largo'),
('BLONP', 10, 37.1, 'Marseille', false, '1930-10-22 07:30:00', 'Ernst Handel', 'Austria', '1996-12-30 00:00:00', 'Magazinweg 7'),
('BOLID', 11, 44.1, 'Tsawassen', true, '1953-02-18 05:50:00', 'Hanari Carnes', 'Switzerland', '1997-12-03 00:00:00', '1029 - 12th Ave. S.'),

-- Eighth batch (i=8)
('ALFKI', 8, 18.4, 'Berlin', false, '1991-05-15 10:40:00', 'Simons bistro', 'Denmark', '1996-07-16 00:00:00', 'Kirchgasse 6'),
('ANATR', 10, 26.4, 'Madrid', true, '1990-04-04 09:30:00', 'Queen Cozinha', 'Brazil', '1996-09-11 00:00:00', 'Avda. Azteca 123'),
('ANTON', 9, 34.4, 'Cholchester', true, '1957-11-30 03:45:00', 'Frankenversand', 'Germany', '1996-10-07 00:00:00', 'Carrera 52 con Ave. Bolívar #65-98 Llano Largo'),
('BLONP', 11, 42.4, 'Marseille', false, '1930-10-22 07:30:00', 'Ernst Handel', 'Austria', '1996-12-30 00:00:00', 'Magazinweg 7'),
('BOLID', 12, 50.4, 'Tsawassen', true, '1953-02-18 05:50:00', 'Hanari Carnes', 'Switzerland', '1997-12-03 00:00:00', '1029 - 12th Ave. S.'),

-- Ninth batch (i=9)
('ALFKI', 9, 20.7, 'Berlin', false, '1991-05-15 10:40:00', 'Simons bistro', 'Denmark', '1996-07-16 00:00:00', 'Kirchgasse 6'),
('ANATR', 11, 29.7, 'Madrid', true, '1990-04-04 09:30:00', 'Queen Cozinha', 'Brazil', '1996-09-11 00:00:00', 'Avda. Azteca 123'),
('ANTON', 10, 38.7, 'Cholchester', true, '1957-11-30 03:45:00', 'Frankenversand', 'Germany', '1996-10-07 00:00:00', 'Carrera 52 con Ave. Bolívar #65-98 Llano Largo'),
('BLONP', 12, 47.7, 'Marseille', false, '1930-10-22 07:30:00', 'Ernst Handel', 'Austria', '1996-12-30 00:00:00', 'Magazinweg 7'),
('BOLID', 13, 56.7, 'Tsawassen', true, '1953-02-18 05:50:00', 'Hanari Carnes', 'Switzerland', '1997-12-03 00:00:00', '1029 - 12th Ave. S.'); 