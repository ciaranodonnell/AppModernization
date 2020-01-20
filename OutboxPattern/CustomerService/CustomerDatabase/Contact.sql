CREATE TABLE [dbo].[Contact]
(
	[Id] INT NOT NULL PRIMARY KEY,
	CustomerId INT,
	Name VARCHAR(200) NOT NULL,
	Email1 VARCHAR(200),
	Email2 VARCHAR(200),
	PhoneNumber varchar(50),
	Version INT NOT NULL DEFAULT 1, 
    [CreatedDate] DATETIMEOFFSET NOT NULL DEFAULT GETUTCDATE(), 
    [LastUpdatedDate] DATETIMEOFFSET NOT NULL, 
    [LastUpdatedByUser] VARCHAR(50) NULL
)
