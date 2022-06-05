USE CarRental
GO
CREATE TABLE Clients
(
IDClient int IDENTITY (1,1) NOT NULL,
Name varchar(50) NOT NULL,
Surname varchar(50) NOT NULL,
Login varchar(25) NOT NULL,
Password varchar(25) NOT NULL,
UserName varchar(50) NOT NULL
)