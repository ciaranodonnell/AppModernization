CREATE TABLE [dbo].[Property] (
    [PropertyId] INT           IDENTITY (1, 1) NOT NULL,
    [Address]    VARCHAR (500) NULL,
    [Value]      MONEY         NULL,
    PRIMARY KEY CLUSTERED ([PropertyId] ASC)
);

