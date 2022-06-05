USE CarRental
GO
CREATE TABLE Cars
(
IDCar INT IDENTITY(1,1) NOT NULL,
Make varchar(50) NOT NULL,
Model varchar(50) NOT NULL,
Engine varchar(50) NOT NULL,
Year varchar(5) NOT NULL,
CarAvailable varchar(3) NOT NULL
)
