CREATE TABLE [dbo].[Applicant] (
    [ApplicantId] INT           IDENTITY (1, 1) NOT NULL,
    [Name]        VARCHAR (100) NOT NULL,
    [CreditScore] INT NULL, 
    [DateOfBirth] DATE NOT NULL, 
    PRIMARY KEY CLUSTERED ([ApplicantId] ASC)
);

