-- Создание базы данных
CREATE DATABASE EmployeeRegistrationDB;

-- Таблица компаний
CREATE TABLE Companies (
                           Id INT IDENTITY(1,1) PRIMARY KEY,
                           Name NVARCHAR(200) NOT NULL,
                           LegalForm NVARCHAR(50) NOT NULL
);

-- Таблица работников
CREATE TABLE Employees (
                           Id INT IDENTITY(1,1) PRIMARY KEY,
                           LastName NVARCHAR(100) NOT NULL,
                           FirstName NVARCHAR(100) NOT NULL,
                           MiddleName NVARCHAR(100),
                           HireDate DATE NOT NULL,
                           Position NVARCHAR(50) NOT NULL,
                           CompanyId INT NOT NULL,
                           CONSTRAINT FK_Employee_Company FOREIGN KEY (CompanyId) REFERENCES Companies(Id)
);