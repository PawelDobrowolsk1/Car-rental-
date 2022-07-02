USE CarRental
GO
CREATE TABLE CarsRentedInfo
(
IDRentedCar int IDENTITY(1,1) NOT NULL,
IDClient int NOT NULL,
IDCar int NOT NULL,
IsGivenBack varchar(3) NOT NULL
)