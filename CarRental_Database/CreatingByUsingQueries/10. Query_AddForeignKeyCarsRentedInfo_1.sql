USE CarRental
GO
ALTER TABLE CarsRentedInfo
ADD CONSTRAINT FK_Clients_CarsRentedInfo
FOREIGN KEY (IDClient)
REFERENCES Clients (IDClient)
GO