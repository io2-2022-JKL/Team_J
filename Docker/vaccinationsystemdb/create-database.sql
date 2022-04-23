CREATE DATABASE [vaccinationSystem] COLLATE Polish_CS_AS;
GO

USE [vaccinationSystem];
GO

CREATE TABLE [dbo].[Admin] (
	Id uniqueidentifier NOT NULL PRIMARY KEY,
	PESEL varchar(11) NOT NULL,
	FirstName varchar(50) NOT NULL,
	LastName varchar(50) NOT NULL,
	DateOfBirth datetime2(7) NOT NULL,
	Mail varchar(100) NOT NULL,
	Password nvarchar(MAX) NOT NULL,
	PhoneNumber varchar(15) NOT NULL
);
GO

CREATE TABLE [dbo].[Vaccine] (
	Id uniqueidentifier NOT NULL PRIMARY KEY,
	Company varchar(50) NOT NULL,
	Name varchar(50) NOT NULL,
	NumberOfDoses int NOT NULL,
	MinDaysBetweenDoses int NOT NULL,
	MaxDaysBetweenDoses int NOT NULL,
	Virus int NOT NULL,
	MinPatientAge int NOT NULL,
	MaxPatientAge int NOT NULL,
	Active bit NOT NULL
);
GO

CREATE TABLE [dbo].[VaccinationCenter] (
	Id uniqueidentifier NOT NULL PRIMARY KEY,
	Name varchar(100) NOT NULL,
	City varchar(40) NOT NULL,
	Address varchar(100) NOT NULL,
	Active bit NOT NULL
);
GO

CREATE TABLE [dbo].[Patient] (
	Id uniqueidentifier NOT NULL PRIMARY KEY,
	PESEL varchar(11) NOT NULL,
	FirstName varchar(50) NOT NULL,
	LastName varchar(50) NOT NULL,
	DateOfBirth datetime2(7) NOT NULL,
	Mail varchar(100) NOT NULL,
	Password nvarchar(MAX) NOT NULL,
	PhoneNumber varchar(15) NOT NULL,
	Active bit NOT NULL
);
GO

CREATE TABLE [dbo].[VaccineeInVaccinationCenter] (
	Id uniqueidentifier NOT NULL PRIMARY KEY,
	VaccinationCenterId uniqueidentifier FOREIGN KEY REFERENCES [dbo].[VaccinationCenter](Id),
	VaccineId uniqueidentifier FOREIGN KEY REFERENCES [dbo].[Vaccine](Id)
);
GO

CREATE TABLE [dbo].[Doctor] (
	Id uniqueidentifier NOT NULL PRIMARY KEY,
	VaccinationCenterId uniqueidentifier FOREIGN KEY REFERENCES [dbo].[VaccinationCenter](Id),
	PatientId uniqueidentifier FOREIGN KEY REFERENCES [dbo].[Patient](Id),
	Active bit
);
GO

CREATE TABLE [dbo].[OpeningHours] (
	Id uniqueidentifier NOT NULL PRIMARY KEY,
	[From] time(7) NOT NULL,
	[To] time(7) NOT NULL,
	WeekDay int NOT NULL,
	VaccinationCenterId uniqueidentifier FOREIGN KEY REFERENCES [dbo].[VaccinationCenter](Id)
);
GO

CREATE TABLE [dbo].[TimeSlot] (
	Id uniqueidentifier NOT NULL PRIMARY KEY,
	[From] datetime2(7) NOT NULL,
	[To] datetime2(7) NOT NULL,
	DoctorId uniqueidentifier FOREIGN KEY REFERENCES [dbo].[Doctor](Id),
	IsFree bit NOT NULL,
	Active bit NOT NULL
);
GO

CREATE TABLE [dbo].[Appointment] (
	Id uniqueidentifier NOT NULL PRIMARY KEY,
	WhichDose int NOT NULL,
	TimeSlotId uniqueidentifier FOREIGN KEY REFERENCES [dbo].[TimeSlot](Id),
	PatientId uniqueidentifier FOREIGN KEY REFERENCES [dbo].[Patient](Id),
	VaccineId uniqueidentifier FOREIGN KEY REFERENCES [dbo].[Vaccine](Id),
	State int NOT NULL,
	VaccineBatchNumber varchar(100)
);
GO

CREATE TABLE [dbo].[Certificate] (
	Id uniqueidentifier NOT NULL PRIMARY KEY,
	Url varchar(250) NOT NULL,
	PatientId uniqueidentifier FOREIGN KEY REFERENCES [dbo].[Patient](Id),
	VaccineId uniqueidentifier FOREIGN KEY REFERENCES [dbo].[Vaccine](Id)
);
GO

BEGIN TRANSACTION
	  INSERT INTO [dbo].[Admin] ([Id]
      ,[PESEL]
      ,[FirstName]
      ,[LastName]
      ,[DateOfBirth]
      ,[Mail]
      ,[Password]
      ,[PhoneNumber])
	VALUES ('f72a1dda-b5fa-4fc9-ba56-1924f93d6634', '00000000000', 'Super', 'Admin', '2022-04-17 00:00:00.0000000', 'admin@systemszczepien.org.pl', '1234', '+48987654321');
	  INSERT INTO [dbo].[Vaccine] ([Id]
      ,[Company]
      ,[Name]
      ,[NumberOfDoses]
      ,[MinDaysBetweenDoses]
      ,[MaxDaysBetweenDoses]
      ,[Virus]
      ,[MinPatientAge]
      ,[MaxPatientAge]
      ,[Active])
	VALUES ('2dba46e3-a040-4dab-9ec4-600e44dbaf8e', 'BioNTech', 'Pfizer', 2, 14, 30, 0, 18, 150, 1),
    ('aa82a7a5-d92c-45d2-b47f-f2a3a5e0b3f7', 'Janssen Pharmaceutical', 'Johnson & Johnson', 1, -1, -1, 0, 10, 100, 1),
    ('31d9b4bf-5c1c-4f2d-b997-f6096758eac9', 'Moderna', 'Moderna', 2, 30, -1, 0, 18, 75, 1),
    ('1fe2941e-f2f3-4edf-9ae3-712460e88ec7', 'Oxford', 'AstraZeneca', 2, -1, 30, 0, -1, -1, 1),
    ('90a43a67-34d3-40b7-bbbb-f5a4c7f4576f', 'SputnikV', 'SputnikV', 3, 15, 45, 0, 15, -1, 0);
	  INSERT INTO [dbo].[VaccinationCenter] ([Id]
      ,[Name]
      ,[City]
      ,[Address]
      ,[Active])
	VALUES ('837c1d09-8664-4480-beff-45fbd914c87e', 'Szpital przy Banacha', 'Warszawa', 'Stefana Banacha 1a', 1),
    ('420755a9-4cbc-48a6-99a4-bda4a231f011', 'Apteka na Chruściela', 'Warszawa', 'Aleja Sztandarów 1', 1),
    ('a0130e3c-bab9-4dcb-acca-15139a1e49c4', 'Punkt szczepień przy rynku', 'Gdańsk', 'Rynek 1', 0),
    ('873a1836-6e7c-4043-83ac-1ee09a1b0aa3', 'Stadion Legii Warszawa', 'Warszawa', 'Łazienkowska 3', 0),
    ('31ea1617-1bf9-48c1-8b40-0e2939bb6302', 'Szpital Uniwersytecki w Krakowie', 'Kraków', 'Macieja Jakubowskiego 2', 1),
    ('250b86b0-28bf-4ca2-9322-0ff57953be8f', 'Szpital Kliniczny im. Heliodora Święcickiego Uniwersytetu Medycznego', 'Poznań', 'Przybyszewskiego 49', 1);
	  INSERT INTO [dbo].[Patient] ([Id]
      ,[PESEL]
      ,[FirstName]
      ,[LastName]
      ,[DateOfBirth]
      ,[Mail]
      ,[Password]
      ,[PhoneNumber]
      ,[Active])
	VALUES ('802c5ceb-22b8-422a-a963-a976a26efdc8', '93040518422', 'Hans', 'Zimmer', '1993-05-04 00:00:00.0000000', 'hZimm@gmail.com', 'Password', '453628905', 0),
    ('f969ffd0-6dbc-4900-8eb8-b4fe25906a74', '88030851737', 'Peter', 'Paker', '1988-03-08 00:00:00.0000000', 'pParker@gmail.com', 'Password', '489657215', 1),
    ('1448be96-c2de-4fdb-93c5-3caf1de2f8a0', '42102712753', 'Janusz', 'Mikke', '1942-10-27 00:00:00.0000000', 'korwinKrul@wp.pl', '5Procent', '445445445', 1),
    ('acd9fa16-404e-4305-b57f-93659054be7e', '68041377873', 'James', 'Bond', '1968-04-13 00:00:00.0000000', 'james.bond@mi6.gov.uk', 'AllH4ilTheQu33n', '+44007' , 1),
    ('31f90897-4455-4d26-aa61-d3c3adcd8f70', '50102844947', 'Cecylia Magdalena', 'Piątek-Sobota', '1950-10-28 00:00:00.0000000', 'cecyliaPS@wp.pl', 'w?&duNBLE2j_>Wwb', '+48924110125', 0),
    ('3d0d9e6b-6db7-45c9-ad96-786ce0c6e447', '70013131688', 'Aaeesh''a', 'Abd Al-Rashid', '1970-01-31 00:00:00.0000000', 'aaeeshaAAR@doktor.org.pl', '?f$#Ybe72qnAu>7*', '863928017', 1),
    ('690fb24f-1116-4a80-a2d4-479207be3706', '90050739775', 'Sylwester', 'Stefański', '1990-05-07 00:00:00.0000000', 'sylwesterS@doktor.org.pl', '-EV92QbHF$!8keH=', '+48964937619', 1),
    ('815a0a02-d036-41c6-89c2-20c614214047', '02281258837', 'Adam', 'Nowak', '2002-08-12 00:00:00.0000000', 'adi222@wp.pl', 'haslohaslo', '+48982938179', 1),
    ('f982aaa8-4be7-4115-a4f9-6cbab37ae726', '00262872986', 'Monika', 'Górecka', '2000-06-28 00:00:00.0000000', '01168691@pw.edu.pl', '3b6L\"[>k4?V?x?#', '+48967123729', 1);
	  INSERT INTO [dbo].[VaccineeInVaccinationCenter] ([Id]
      ,[VaccinationCenterId],
	  [VaccineId])
	VALUES ('d124c85f-9f24-48ef-a625-1ae972c84b61', '837c1d09-8664-4480-beff-45fbd914c87e', '2dba46e3-a040-4dab-9ec4-600e44dbaf8e'),
	('774d3c33-e3d3-4aac-806e-d47a738f62ef', '837c1d09-8664-4480-beff-45fbd914c87e', '1fe2941e-f2f3-4edf-9ae3-712460e88ec7'),
	('39c3d0dd-02ee-46f8-8832-a458d80d76bb', '420755a9-4cbc-48a6-99a4-bda4a231f011', '2dba46e3-a040-4dab-9ec4-600e44dbaf8e'),
	('e6c7562b-7a52-4355-aedf-ba55c45a8920', 'a0130e3c-bab9-4dcb-acca-15139a1e49c4', 'aa82a7a5-d92c-45d2-b47f-f2a3a5e0b3f7'),
	('4852dde8-532c-41f0-8836-823d7ea59ddf', 'a0130e3c-bab9-4dcb-acca-15139a1e49c4', '90a43a67-34d3-40b7-bbbb-f5a4c7f4576f'),
	('f46022b3-9402-489a-85ba-83eac0758c10', 'a0130e3c-bab9-4dcb-acca-15139a1e49c4', '2dba46e3-a040-4dab-9ec4-600e44dbaf8e'),
	('2dc7c1dc-a4e7-4536-8a68-9840cc666003', '873a1836-6e7c-4043-83ac-1ee09a1b0aa3', '31d9b4bf-5c1c-4f2d-b997-f6096758eac9'),
	('4b338bea-e55d-4c9b-841e-c52930728307', '31ea1617-1bf9-48c1-8b40-0e2939bb6302', '1fe2941e-f2f3-4edf-9ae3-712460e88ec7'),
	('99ee0246-c4da-4c95-b26a-4ad0c92a3ee0', '31ea1617-1bf9-48c1-8b40-0e2939bb6302', 'aa82a7a5-d92c-45d2-b47f-f2a3a5e0b3f7'),
	('ede0d113-da20-4177-a2d8-47914e37e3e6', '250b86b0-28bf-4ca2-9322-0ff57953be8f', '90a43a67-34d3-40b7-bbbb-f5a4c7f4576f');
	  INSERT INTO [dbo].[Doctor] ([Id]
      ,[VaccinationCenterId]
      ,[PatientId]
      ,[Active])
	VALUES ('9d77b5e9-2823-4274-b326-d371e5582274', '420755a9-4cbc-48a6-99a4-bda4a231f011', '690fb24f-1116-4a80-a2d4-479207be3706', 1),
    ('003a7da9-f6e3-4342-85ea-0d3296f99d41', 'a0130e3c-bab9-4dcb-acca-15139a1e49c4', '690fb24f-1116-4a80-a2d4-479207be3706', 0),
    ('2dffdf02-aef5-4974-a286-85b506884d75', 'a0130e3c-bab9-4dcb-acca-15139a1e49c4', '1448be96-c2de-4fdb-93c5-3caf1de2f8a0', 0),
    ('34d2abf7-cc9e-4ad0-bc7f-decfd98fbdc0', '31ea1617-1bf9-48c1-8b40-0e2939bb6302', '31f90897-4455-4d26-aa61-d3c3adcd8f70', 0),
    ('89a11879-4edf-4a67-a6f7-23c76763a418', '837c1d09-8664-4480-beff-45fbd914c87e', '3d0d9e6b-6db7-45c9-ad96-786ce0c6e447', 1),
    ('cf7cbcec-7250-41be-98e9-474f1409dd32', '250b86b0-28bf-4ca2-9322-0ff57953be8f', '1448be96-c2de-4fdb-93c5-3caf1de2f8a0', 0);
	  INSERT INTO [dbo].[OpeningHours] ([Id]
      ,[From]
      ,[To]
      ,[WeekDay]
      ,[VaccinationCenterId])
	VALUES ('4e994d2a-81ad-4f5f-8d26-69ca40524129', '07:00:00.0000000', '19:00:00.0000000', 0,'837c1d09-8664-4480-beff-45fbd914c87e'),
    ('9a0f1d66-6b29-4aaf-912a-56b4efcbf4ea', '07:00:00.0000000', '19:00:00.0000000', 1,'837c1d09-8664-4480-beff-45fbd914c87e'),
    ('b256c309-4828-45ac-966f-958a4bdbf167', '07:00:00.0000000', '19:00:00.0000000', 2,'837c1d09-8664-4480-beff-45fbd914c87e'),
    ('09936f73-c102-466d-a92e-890520860670', '07:00:00.0000000', '19:00:00.0000000', 3,'837c1d09-8664-4480-beff-45fbd914c87e'),
    ('d1ff9a4b-1681-4817-afe7-9c425450262b', '07:00:00.0000000', '21:00:00.0000000', 4,'837c1d09-8664-4480-beff-45fbd914c87e'),
    ('3cc64799-132f-463c-81bd-8040fa22bb67', '09:00:00.0000000', '16:00:00.0000000', 5,'837c1d09-8664-4480-beff-45fbd914c87e'),
    ('83e9eeab-66ae-4885-bc02-d26fe0a86163', '00:00:00.0000000', '00:00:00.0000000', 6,'837c1d09-8664-4480-beff-45fbd914c87e'),

    ('60049231-b166-444b-80b9-dd08a312805b', '07:00:00.0000000', '19:00:00.0000000', 0,'420755a9-4cbc-48a6-99a4-bda4a231f011'),
    ('d8dbe2a2-a6df-4dcc-8a77-e9381cdf2206', '07:00:00.0000000', '19:00:00.0000000', 1,'420755a9-4cbc-48a6-99a4-bda4a231f011'),
    ('ef15aafc-28ed-454f-bcf9-c1a82d46a986', '07:30:00.0000000', '19:30:00.0000000', 2,'420755a9-4cbc-48a6-99a4-bda4a231f011'),
    ('2a7bf9ed-158c-4358-9e41-bf14d6c83d63', '09:00:00.0000000', '22:00:00.0000000', 3,'420755a9-4cbc-48a6-99a4-bda4a231f011'),
    ('ba58651a-f8c1-4d1c-bbdc-ff68be15eac9', '00:00:00.0000000', '00:00:00.0000000', 4,'420755a9-4cbc-48a6-99a4-bda4a231f011'),
    ('c0a94c2c-35e1-4e0c-b9ca-73809a1ce1ff', '07:00:00.0000000', '19:00:00.0000000', 5,'420755a9-4cbc-48a6-99a4-bda4a231f011'),
    ('f1d56ff5-252a-4077-8cae-3b99c93072db', '00:00:00.0000000', '00:00:00.0000000', 6,'420755a9-4cbc-48a6-99a4-bda4a231f011'),

    ('91f0b078-c10d-41cd-8e3f-a37e9c547148', '00:00:00.0000000', '23:59:59.9999999', 0,'a0130e3c-bab9-4dcb-acca-15139a1e49c4'),
    ('d6f2da64-92b2-4ca6-a39b-d27c6e91f7bd', '00:00:00.0000000', '23:59:59.9999999', 1,'a0130e3c-bab9-4dcb-acca-15139a1e49c4'),
    ('0bd9cef1-4399-4f34-9e82-364099317a81', '00:00:00.0000000', '23:59:59.9999999', 2,'a0130e3c-bab9-4dcb-acca-15139a1e49c4'),
    ('e23213b5-059a-4eeb-abe3-885422668dec', '00:00:00.0000000', '19:00:00.0000000', 3,'a0130e3c-bab9-4dcb-acca-15139a1e49c4'),
    ('924372c5-9fe6-49b5-9c01-05fb2b02b9ae', '06:15:00.0000000', '23:59:59.9999999', 4,'a0130e3c-bab9-4dcb-acca-15139a1e49c4'),
    ('cd2d5144-b494-4e47-b858-e3ac9c582365', '00:00:00.0000000', '23:59:59.9999999', 5,'a0130e3c-bab9-4dcb-acca-15139a1e49c4'),
    ('22c20e4e-0f3b-4397-9ae9-3b4df4c0288a', '00:00:00.0000000', '00:00:00.0000000', 6,'a0130e3c-bab9-4dcb-acca-15139a1e49c4'),

    ('cc458301-53fc-460f-8e92-79d9732855d1', '05:45:00.0000000', '16:45:00.0000000', 0,'873a1836-6e7c-4043-83ac-1ee09a1b0aa3'),
    ('d3f5e42d-d9f4-462c-ac52-d16a12c1a670', '05:45:00.0000000', '16:45:00.0000000', 1,'873a1836-6e7c-4043-83ac-1ee09a1b0aa3'),
    ('f047bc8f-671e-4765-b68b-728d983a232b', '05:45:00.0000000', '16:45:00.0000000', 2,'873a1836-6e7c-4043-83ac-1ee09a1b0aa3'),
    ('249a1110-ad9f-4594-be6d-d0340fbc873c', '07:00:00.0000000', '19:00:00.0000000', 3,'873a1836-6e7c-4043-83ac-1ee09a1b0aa3'),
    ('dfe81eb3-aef0-4730-bed8-da5aa74e8ff8', '05:45:00.0000000', '19:45:00.0000000', 4,'873a1836-6e7c-4043-83ac-1ee09a1b0aa3'),
    ('8d4e15e8-5aef-4b45-b971-7bf1738f0926', '09:00:00.0000000', '16:00:00.0000000', 5,'873a1836-6e7c-4043-83ac-1ee09a1b0aa3'),
    ('816c6eeb-874f-400f-a10b-ed8cff0045c5', '00:00:00.0000000', '00:00:00.0000000', 6,'873a1836-6e7c-4043-83ac-1ee09a1b0aa3'),

    ('378525a6-9756-43ab-93fd-480812d9f696', '00:00:00.0000000', '12:00:00.0000000', 0,'31ea1617-1bf9-48c1-8b40-0e2939bb6302'),
    ('a3637faf-bdc5-48ae-9d7e-4018e7f2590e', '00:00:00.0000000', '12:00:00.0000000', 1,'31ea1617-1bf9-48c1-8b40-0e2939bb6302'),
    ('56ad69ac-eced-463d-9e09-48561f373855', '00:00:00.0000000', '12:00:00.0000000', 2,'31ea1617-1bf9-48c1-8b40-0e2939bb6302'),
    ('e863f053-e419-4dd7-b3d3-175d5b07f412', '00:00:00.0000000', '12:00:00.0000000', 3,'31ea1617-1bf9-48c1-8b40-0e2939bb6302'),
    ('37d6f322-48f6-420f-8334-13a8972e3bd8', '06:00:00.0000000', '15:00:00.0000000', 4,'31ea1617-1bf9-48c1-8b40-0e2939bb6302'),
    ('86fbc9ae-ea81-4f3b-922e-e1fe62d72a50', '07:00:00.0000000', '14:00:00.0000000', 5,'31ea1617-1bf9-48c1-8b40-0e2939bb6302'),
    ('726ada2d-f775-4a20-a23b-5cdf9f5b7c40', '09:00:00.0000000', '14:00:00.0000000', 6,'31ea1617-1bf9-48c1-8b40-0e2939bb6302'),

    ('0b2f995a-6be1-471d-bb7c-229bb8e4d4ea', '07:00:00.0000000', '19:00:00.0000000', 0,'250b86b0-28bf-4ca2-9322-0ff57953be8f'),
    ('1f0b43e5-c39b-49be-bf4c-6ba53b862894', '07:00:00.0000000', '19:00:00.0000000', 1,'250b86b0-28bf-4ca2-9322-0ff57953be8f'),
    ('0067d41a-bb34-417d-a88f-6fcc3c57cfe5', '07:00:00.0000000', '19:00:00.0000000', 2,'250b86b0-28bf-4ca2-9322-0ff57953be8f'),
    ('5b91c826-a8b5-49af-9633-a388bc1ecb95', '07:00:00.0000000', '19:00:00.0000000', 3,'250b86b0-28bf-4ca2-9322-0ff57953be8f'),
    ('04e97fc0-22c2-4f13-a7e7-8b2cea5ab0c0', '07:00:00.0000000', '21:00:00.0000000', 4,'250b86b0-28bf-4ca2-9322-0ff57953be8f'),
    ('1d873800-6fff-499d-93e1-4cba9cf9fa01', '09:00:00.0000000', '18:00:00.0000000', 5,'250b86b0-28bf-4ca2-9322-0ff57953be8f'),
    ('89edb219-f569-433c-bc7b-6604934296e4', '09:00:00.0000000', '14:00:00.0000000', 6,'250b86b0-28bf-4ca2-9322-0ff57953be8f');
	  INSERT INTO [dbo].[TimeSlot] ([Id]
      ,[From]
      ,[To]
      ,[DoctorId]
      ,[IsFree]
      ,[Active])
	VALUES ('50c12fe6-a321-4775-b6e3-d901dfda2616', '2022-03-01 07:00:00.0000000', '2022-03-01 08:00:00.0000000', '9d77b5e9-2823-4274-b326-d371e5582274', 0, 0),
    ('2e9009fd-d824-41b1-9a02-992cccb04d43', '2022-03-01 08:00:00.0000000', '2022-03-01 09:00:00.0000000', '9d77b5e9-2823-4274-b326-d371e5582274', 0, 0),
    ('f492deb3-02ca-41aa-a54c-98f83f915234', '2022-03-01 09:00:00.0000000', '2022-03-01 10:00:00.0000000', '9d77b5e9-2823-4274-b326-d371e5582274', 0, 0),
    ('43c89e51-b74f-48c3-80d9-f1304331d03d', '2022-03-01 10:00:00.0000000', '2022-03-01 11:00:00.0000000', '9d77b5e9-2823-4274-b326-d371e5582274', 1, 0),
    ('74c0ac81-a643-4846-ba42-9a310faf70f0', '2022-03-01 11:00:00.0000000', '2022-03-01 12:00:00.0000000', '9d77b5e9-2823-4274-b326-d371e5582274', 1, 0),
    ('3a6a6b44-5443-4760-9ce5-da1cc4644cc8', '2022-03-01 13:00:00.0000000', '2022-03-01 14:00:00.0000000', '9d77b5e9-2823-4274-b326-d371e5582274', 1, 0),
    ('5ab7b870-5bad-4b7f-8ee9-95773434842a', '2022-03-01 14:00:00.0000000', '2022-03-01 15:00:00.0000000', '9d77b5e9-2823-4274-b326-d371e5582274', 1, 0),

    ('731e953c-6d7a-4aef-a40d-0ded3ec3cfd3', '2022-03-01 00:00:00.0000000', '2022-03-01 01:00:00.0000000', '34d2abf7-cc9e-4ad0-bc7f-decfd98fbdc0', 0, 0),
    ('386791e1-966c-430b-8492-61badceb0c09', '2022-03-01 01:00:00.0000000', '2022-03-01 02:00:00.0000000', '34d2abf7-cc9e-4ad0-bc7f-decfd98fbdc0', 1, 0),
    ('e2fd8ec7-eeb0-47c8-95a8-bd51d513db49', '2022-03-01 02:00:00.0000000', '2022-03-01 03:00:00.0000000', '34d2abf7-cc9e-4ad0-bc7f-decfd98fbdc0', 1, 0),
    ('9e358a18-2b97-47ac-81f1-b82c204a80b6', '2022-03-01 03:00:00.0000000', '2022-03-01 04:00:00.0000000', '34d2abf7-cc9e-4ad0-bc7f-decfd98fbdc0', 1, 0),
    ('456cdf7d-315d-4e69-96c7-2479d6cc7400', '2022-03-01 05:00:00.0000000', '2022-03-01 06:00:00.0000000', '34d2abf7-cc9e-4ad0-bc7f-decfd98fbdc0', 1, 0),
    ('6526d366-b00d-4bb2-9da1-8c2c907be31c', '2022-03-01 06:00:00.0000000', '2022-03-01 07:00:00.0000000', '34d2abf7-cc9e-4ad0-bc7f-decfd98fbdc0', 1, 0),
    ('39d798ca-7543-4117-a32e-3767f5d44964', '2022-03-01 07:00:00.0000000', '2022-03-01 08:00:00.0000000', '34d2abf7-cc9e-4ad0-bc7f-decfd98fbdc0', 1, 0),

    ('76e8c080-2c19-4f57-a629-6f4b721f2ae3', '2022-03-01 18:00:00.0000000', '2022-03-01 19:00:00.0000000', '2dffdf02-aef5-4974-a286-85b506884d75', 0, 0),
    ('f6292edd-9317-4c85-bd0d-56eca8716628', '2022-03-01 19:00:00.0000000', '2022-03-01 20:00:00.0000000', '2dffdf02-aef5-4974-a286-85b506884d75', 0, 0),
    ('53a6e33d-05ac-45f5-89ec-bc53b27f47e2', '2022-03-01 21:00:00.0000000', '2022-03-01 22:00:00.0000000', '2dffdf02-aef5-4974-a286-85b506884d75', 1, 0),
    ('4df7ce5b-9ae3-4de7-bbbf-14a69eae8a82', '2022-03-01 23:00:00.0000000', '2022-03-01 23:59:59.9999999', '2dffdf02-aef5-4974-a286-85b506884d75', 1, 0),

    ('e230f822-47bc-4c5a-be50-4d68ebed57a8', '2022-04-01 15:45:00.0000000', '2022-04-01 16:45:00.0000000', '2dffdf02-aef5-4974-a286-85b506884d75', 1, 0),
    ('22c30fab-0333-4494-8636-a5ed79b146c3', '2022-04-01 16:45:00.0000000', '2022-04-01 17:45:00.0000000', '2dffdf02-aef5-4974-a286-85b506884d75', 1, 0),
    ('a72b8191-f6dc-4466-b194-8071bc6dc0e4', '2022-04-01 17:45:00.0000000', '2022-04-01 18:45:00.0000000', '2dffdf02-aef5-4974-a286-85b506884d75', 1, 0),
    ('c4c0b592-93a8-41ee-9628-02db3c3336fe', '2022-04-01 18:45:00.0000000', '2022-04-01 19:45:00.0000000', '2dffdf02-aef5-4974-a286-85b506884d75', 1, 0),

    ('7f4eb90a-a10f-4b40-9d4f-21863382f1a5', '2022-04-01 07:00:00.0000000', '2022-04-01 08:00:00.0000000', '89a11879-4edf-4a67-a6f7-23c76763a418', 0, 0),
    ('6318ad19-5a9c-40e1-84bf-68a39606218b', '2022-04-01 08:00:00.0000000', '2022-04-01 09:00:00.0000000', '89a11879-4edf-4a67-a6f7-23c76763a418', 1, 0),
    ('aba53e18-14db-4813-8841-00175633dac5', '2022-04-01 09:00:00.0000000', '2022-04-01 10:00:00.0000000', '89a11879-4edf-4a67-a6f7-23c76763a418', 1, 0),
    ('f7618b63-36c3-48d4-8e57-33b95a414dac', '2022-04-01 10:00:00.0000000', '2022-04-01 11:00:00.0000000', '89a11879-4edf-4a67-a6f7-23c76763a418', 1, 0),
    ('06fffdae-a68f-4deb-b923-4a4c643f160e', '2022-04-01 11:00:00.0000000', '2022-04-01 12:00:00.0000000', '89a11879-4edf-4a67-a6f7-23c76763a418', 1, 0),
    ('e7655406-4158-4eb7-9cad-34cf4e6b7b5d', '2022-04-01 13:00:00.0000000', '2022-04-01 14:00:00.0000000', '89a11879-4edf-4a67-a6f7-23c76763a418', 1, 0),
    ('4f971ff0-f56f-4de9-9c7f-21d84ea8f985', '2022-04-01 14:00:00.0000000', '2022-04-01 15:00:00.0000000', '89a11879-4edf-4a67-a6f7-23c76763a418', 0, 0),

    ('5c333c05-f200-4ebf-9513-2bd2922b5127', '2022-04-02 07:00:00.0000000', '2022-04-02 08:00:00.0000000', '9d77b5e9-2823-4274-b326-d371e5582274', 0, 0),
    ('1e85720c-c7cf-441c-b6f2-be748f03223e', '2022-04-02 08:00:00.0000000', '2022-04-02 09:00:00.0000000', '9d77b5e9-2823-4274-b326-d371e5582274', 1, 0),
    ('102f9a50-2659-47ae-a3ba-e8d80a6a4975', '2022-04-02 09:00:00.0000000', '2022-04-02 10:00:00.0000000', '9d77b5e9-2823-4274-b326-d371e5582274', 1, 0),
    ('46dd4c51-ff26-4545-8fae-e88da21cb2a4', '2022-04-02 10:00:00.0000000', '2022-04-02 11:00:00.0000000', '9d77b5e9-2823-4274-b326-d371e5582274', 1, 0),
    ('73b87a42-6da3-45ea-82b2-5eea5eac60d2', '2022-04-02 11:00:00.0000000', '2022-04-02 12:00:00.0000000', '9d77b5e9-2823-4274-b326-d371e5582274', 1, 0),
    ('bddb5e11-32e5-4984-b362-038c3103c9ca', '2022-04-02 13:00:00.0000000', '2022-04-02 14:00:00.0000000', '9d77b5e9-2823-4274-b326-d371e5582274', 1, 0),
    ('39e87cd9-4ccc-4d11-8183-37ea53aaf214', '2022-04-02 14:00:00.0000000', '2022-04-02 15:00:00.0000000', '9d77b5e9-2823-4274-b326-d371e5582274', 1, 0),

    ('9b329d2c-5c85-431a-b457-07c21a883555', '2022-05-02 07:00:00.0000000', '2022-05-02 08:00:00.0000000', '89a11879-4edf-4a67-a6f7-23c76763a418', 0, 1),
    ('87143ee0-1e83-42fb-9d8a-cfeeb9327bde', '2022-05-02 08:00:00.0000000', '2022-05-02 09:00:00.0000000', '89a11879-4edf-4a67-a6f7-23c76763a418', 1, 1),
    ('2c624033-4865-4e8c-95a8-87b00d26745d', '2022-05-02 09:00:00.0000000', '2022-05-02 10:00:00.0000000', '89a11879-4edf-4a67-a6f7-23c76763a418', 1, 1),
    ('02f2b999-d0ba-4f33-9a9a-69e1cae2952b', '2022-05-02 10:00:00.0000000', '2022-05-02 11:00:00.0000000', '89a11879-4edf-4a67-a6f7-23c76763a418', 1, 1),
    ('27a55496-956c-47ae-bdff-fc782b4c14a9', '2022-05-02 11:00:00.0000000', '2022-05-02 12:00:00.0000000', '89a11879-4edf-4a67-a6f7-23c76763a418', 1, 1),
    ('5843d71b-574d-451e-be40-093fdb13983a', '2022-05-02 13:00:00.0000000', '2022-05-02 14:00:00.0000000', '89a11879-4edf-4a67-a6f7-23c76763a418', 1, 1),
    ('2ecfbc3f-23db-4eb9-b32f-6b96b878506b', '2022-05-02 14:00:00.0000000', '2022-05-02 15:00:00.0000000', '89a11879-4edf-4a67-a6f7-23c76763a418', 0, 1),

    ('6c152ae1-3f25-4e22-bfbe-6534ce22beed', '2022-05-02 07:00:00.0000000', '2022-05-02 08:00:00.0000000', '9d77b5e9-2823-4274-b326-d371e5582274', 0, 1),
    ('60cfaff7-3c6c-4331-b89a-d04331c197f3', '2022-05-02 08:00:00.0000000', '2022-05-02 09:00:00.0000000', '9d77b5e9-2823-4274-b326-d371e5582274', 1, 1),
    ('a52f6beb-8abb-4344-91ac-835aaf0f52f6', '2022-05-02 09:00:00.0000000', '2022-05-02 10:00:00.0000000', '9d77b5e9-2823-4274-b326-d371e5582274', 1, 1),
    ('5a0c8d97-6da0-42f9-903e-286f928f0594', '2022-05-02 10:00:00.0000000', '2022-05-02 11:00:00.0000000', '9d77b5e9-2823-4274-b326-d371e5582274', 0, 0),
    ('5719246e-14b7-4979-a652-7aea66d99fcf', '2022-05-02 11:00:00.0000000', '2022-05-02 12:00:00.0000000', '9d77b5e9-2823-4274-b326-d371e5582274', 1, 0),
    ('7ad80bec-cdc2-4890-98db-e760b4d76493', '2022-05-02 13:00:00.0000000', '2022-05-02 14:00:00.0000000', '9d77b5e9-2823-4274-b326-d371e5582274', 1, 0),
    ('0dc9363e-267e-44b5-8af2-0f3d44d88921', '2022-05-02 14:00:00.0000000', '2022-05-02 15:00:00.0000000', '9d77b5e9-2823-4274-b326-d371e5582274', 1, 0);
	  INSERT INTO [dbo].[Appointment] ([Id]
      ,[WhichDose]
      ,[TimeSlotId]
      ,[PatientId]
      ,[VaccineId]
      ,[State]
      ,[VaccineBatchNumber])
	VALUES ('04f3e06e-d098-4366-871d-a2a9c4020cbc', 1, '50c12fe6-a321-4775-b6e3-d901dfda2616', '802c5ceb-22b8-422a-a963-a976a26efdc8', '2dba46e3-a040-4dab-9ec4-600e44dbaf8e', 0, null),
    ('d7e25c56-27d4-4c79-b615-38bb5dc224f3', 1, '2e9009fd-d824-41b1-9a02-992cccb04d43', '802c5ceb-22b8-422a-a963-a976a26efdc8', '2dba46e3-a040-4dab-9ec4-600e44dbaf8e', 2, 'a5e372d8-baaf-46fe-a5b0-4c779216c9c1'),

    ('ced082ea-974c-4e62-bb20-49461e515bb1', 1, 'f492deb3-02ca-41aa-a54c-98f83f915234', 'f969ffd0-6dbc-4900-8eb8-b4fe25906a74', '2dba46e3-a040-4dab-9ec4-600e44dbaf8e', 2, '7c1d4164-7191-44d6-b41f-8d30a6a7ab02'),

    ('0d88ec97-a35a-497b-a561-6363280cb4c8', 1, '50c12fe6-a321-4775-b6e3-d901dfda2616', '1448be96-c2de-4fdb-93c5-3caf1de2f8a0', '2dba46e3-a040-4dab-9ec4-600e44dbaf8e', 2, '8ad26c4c-10f1-4f7d-bdff-782642b2030d'),

    ('2282885a-7323-4fcc-b1d8-f3429b1dfd33', 1, '731e953c-6d7a-4aef-a40d-0ded3ec3cfd3', 'acd9fa16-404e-4305-b57f-93659054be7e', '1fe2941e-f2f3-4edf-9ae3-712460e88ec7', 0, null),
    ('340eff40-cee8-4408-946a-23b6be94f3f8', 1, '731e953c-6d7a-4aef-a40d-0ded3ec3cfd3', 'acd9fa16-404e-4305-b57f-93659054be7e', 'aa82a7a5-d92c-45d2-b47f-f2a3a5e0b3f7', 2, '69b113c0-9508-48c1-8a08-267d0443af6a'),

    ('ecaaf8ec-7d04-4341-a4b8-34e6a0d087d3', 1, '76e8c080-2c19-4f57-a629-6f4b721f2ae3', '31f90897-4455-4d26-aa61-d3c3adcd8f70', '90a43a67-34d3-40b7-bbbb-f5a4c7f4576f', 2, '5afc65c7-4d82-4ff0-9333-de30cc0a0b3e'),

    ('86d41932-8098-472d-8d83-64d90820989a', 1, 'f6292edd-9317-4c85-bd0d-56eca8716628', '3d0d9e6b-6db7-45c9-ad96-786ce0c6e447', 'aa82a7a5-d92c-45d2-b47f-f2a3a5e0b3f7', 2, 'd2dfdf33-474d-434e-9abc-e4b3025e8f43'),

    ('b48f7ae9-3b7c-4b0d-ade0-91ad8885329f', 2, 'e230f822-47bc-4c5a-be50-4d68ebed57a8', '31f90897-4455-4d26-aa61-d3c3adcd8f70', '90a43a67-34d3-40b7-bbbb-f5a4c7f4576f', 0, null),

    ('4f971ff0-f56f-4de9-9c7f-21d84ea8f985', 2, '5c333c05-f200-4ebf-9513-2bd2922b5127', '1448be96-c2de-4fdb-93c5-3caf1de2f8a0', '2dba46e3-a040-4dab-9ec4-600e44dbaf8e', 2, '170c81d7-450b-43c4-99c6-ad21807e7880'),
    ('324db83d-3343-49f3-ba77-b6de3eb83815', 2, '1e85720c-c7cf-441c-b6f2-be748f03223e', 'f969ffd0-6dbc-4900-8eb8-b4fe25906a74', '2dba46e3-a040-4dab-9ec4-600e44dbaf8e', 0, null),

    ('df5c9d5c-06e4-4b65-a0e3-e7b4d78ba0f7', 2, '7f4eb90a-a10f-4b40-9d4f-21863382f1a5', 'f969ffd0-6dbc-4900-8eb8-b4fe25906a74', '2dba46e3-a040-4dab-9ec4-600e44dbaf8e', 2, '120730c2-d382-4528-b676-3879b25bb54c'),
    ('4987bb16-df31-4c55-a71c-19c0ca2c2076', 1, '4f971ff0-f56f-4de9-9c7f-21d84ea8f985', '690fb24f-1116-4a80-a2d4-479207be3706', '1fe2941e-f2f3-4edf-9ae3-712460e88ec7', 2, 'b83b7e23-a3f7-41d9-8785-92488d8c829c'),

    ('03470d45-3406-4e14-9f47-c6fbf5799c04', 1, '5a0c8d97-6da0-42f9-903e-286f928f0594', '815a0a02-d036-41c6-89c2-20c614214047', '2dba46e3-a040-4dab-9ec4-600e44dbaf8e', 0, null),
    ('ebaa5881-f5d7-47b9-97e5-8ae354c6fc82', 1, '6c152ae1-3f25-4e22-bfbe-6534ce22beed', '815a0a02-d036-41c6-89c2-20c614214047', '2dba46e3-a040-4dab-9ec4-600e44dbaf8e', 1, null),

    ('20d65e7b-7017-4c4e-b44b-b5266f8ac2a5', 2, '2ecfbc3f-23db-4eb9-b32f-6b96b878506b', '690fb24f-1116-4a80-a2d4-479207be3706', '1fe2941e-f2f3-4edf-9ae3-712460e88ec7', 1, null),

    ('e9ee54a5-9809-4cac-bd8d-9092638e6d4e', 1, '87143ee0-1e83-42fb-9d8a-cfeeb9327bde', 'f982aaa8-4be7-4115-a4f9-6cbab37ae726', '1fe2941e-f2f3-4edf-9ae3-712460e88ec7', 0, null),
    ('a7059950-8f4d-4dac-b482-eef413c9f717', 1, '9b329d2c-5c85-431a-b457-07c21a883555', 'f982aaa8-4be7-4115-a4f9-6cbab37ae726', '1fe2941e-f2f3-4edf-9ae3-712460e88ec7', 0, null),
    ('e72a0381-297f-4409-9d7a-395d8135061d', 1, '9b329d2c-5c85-431a-b457-07c21a883555', 'f982aaa8-4be7-4115-a4f9-6cbab37ae726', '2dba46e3-a040-4dab-9ec4-600e44dbaf8e', 1, null);
	  INSERT INTO [dbo].[Certificate] ([Id]
      ,[Url]
      ,[PatientId]
      ,[VaccineId])
	VALUES ('4f341846-1436-4c2e-870f-bc9a9966e090', 'https://vaccinationsystem.blob.core.windows.net/certificates/James_Bond/53cf7004-db63-4d53-a6c7-313168a71af5.pdf', 'acd9fa16-404e-4305-b57f-93659054be7e', 'aa82a7a5-d92c-45d2-b47f-f2a3a5e0b3f7'),
    ('4682ca4c-be71-43e8-a7b1-9053d1a25533', 'https://vaccinationsystem.blob.core.windows.net/certificates/Aaeesh%27a_Abd%20Al-Rashid/0958024f-31e6-42be-a27d-3934aeb2d0f4.pdf', '3d0d9e6b-6db7-45c9-ad96-786ce0c6e447', 'aa82a7a5-d92c-45d2-b47f-f2a3a5e0b3f7'),
    ('ce030cef-04c0-484e-ac2a-1bcc90a28b56', 'https://vaccinationsystem.blob.core.windows.net/certificates/Janusz_Mikke/327ac066-cd35-4be4-aa10-3cb2bba248eb.pdf', '1448be96-c2de-4fdb-93c5-3caf1de2f8a0', '2dba46e3-a040-4dab-9ec4-600e44dbaf8e'),
    ('19432e6f-62f6-4c43-9cfd-7b127859ba5a', 'https://vaccinationsystem.blob.core.windows.net/certificates/Peter_Parker/0c771072-84ae-44f9-960c-5c8b9aa32c10.pdf', 'f969ffd0-6dbc-4900-8eb8-b4fe25906a74', '2dba46e3-a040-4dab-9ec4-600e44dbaf8e');
COMMIT
