CREATE TABLE [dbo].[CommonOutbox]
(
	[Id] BIGINT NOT NULL PRIMARY KEY,
	GeneratedDate datetime NOT NULL,
	EntityType varchar(200) NOT NULL,
	SentDate datetime NULL

)
